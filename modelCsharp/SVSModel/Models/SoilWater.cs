﻿using Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SVSModel
{
    class SoilWater
    {
        /// <summary>
        /// Calculates the soil water content and leaching risk daily leaching risk
        /// </summary>
        /// <param name="rswc">A date indexed dictionary of daily releative soil water content</param>
        /// <param name="drainage">a date indexed dictionary of daily water losses from the root zone</param>
        /// <param name="irrigation">A date indexed dictionary of assumed irrigation applied</param>
        /// <param name="meanRain"> A date indexed dictionary of long term mean daily rainfall</param>
        /// <param name="meanPET">A date indexed dictionary of long term meand daily potential evapo transpiration</param>
        /// <param name="cover">A date indexed dictionary of crop cover</param>
        /// <returns>Date indexed series of daily N mineralised from residues</returns>
        public static void Balance(ref Dictionary<DateTime, double> rswc,
                                   ref Dictionary<DateTime, double> drainage,
                                   ref Dictionary<DateTime, double> irrigation,
                                   Dictionary<DateTime, double> meanRain, 
                                   Dictionary<DateTime, double> meanPET, 
                                   Dictionary<DateTime, double> cover)
        {
            DateTime[] simDates = rswc.Keys.ToArray();
            Dictionary<DateTime, double> SWC = Functions.dictMaker(simDates, new double[simDates.Length]);
            double dul = Config.Field.AWC;
            foreach (DateTime d in simDates)
            {
                if (d == simDates[0])
                {
                    SWC[simDates[0]] = dul * Config.Field.PrePlantRainFactor;
                    rswc[simDates[0]] = SWC[simDates[0]] / dul;
                }
                else
                {
                    DateTime yest = d.AddDays(-1);
                    double T = Math.Min(SWC[yest] * 0.1, meanPET[d] * cover[d]);
                    double E = meanPET[d] * (1 - cover[d]) * rswc[yest];
                    SWC[d] = SWC[yest] + meanRain[d] - T - E;
                    if (SWC[d]>dul)
                    {
                        drainage[d] = SWC[d] - dul;
                        SWC[d] = dul;
                    }
                    else
                    {
                        drainage[d] = 0.0;
                        if (SWC[d]/dul < Config.Field.IrrigationTrigger)
                        {
                            double apply = dul * (Config.Field.IrrigationRefill - Config.Field.IrrigationTrigger);
                            SWC[d] += apply;
                            irrigation[d] = apply;
                        }
                    }
                }
                rswc[d] = SWC[d] / dul;
            }
        }
    }
}
