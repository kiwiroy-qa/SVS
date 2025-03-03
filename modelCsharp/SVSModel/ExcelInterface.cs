﻿using SVSModel;
using System;
using System.Collections.Generic;

namespace Helper
{
    public class MyFunctions
    {
        /// <summary>
        /// Function that takes input data in 2D array format and calculates a N balance for a 3 crops rotation and returns N balance variables in 2D array format
        /// </summary>
        /// <param name="met">2D Array with dates in first column and daily meterological data over the duration of the rotation in the second column</param>
        /// <param name="config">2D array with parameter names and values for crop field configuration parameters</param>
        /// <param name="testResults">2D array with dates in teh first column and soil N test results in the second</param>
        /// <returns>Dictionary with parameter names as keys and parameter values as values</returns>
        public static object[,] GetDailyNBalance(object[,] met, object[,] config, object[,] testResults, object[,] nApplied)
        {
            List<string> configErrors = Functions.ValidateConfig(config);

            if (configErrors.Count == 0)
            {
                Dictionary<DateTime, double> _tt = Functions.dictMaker(met, "MeanT");
                Dictionary<DateTime, double> _rain = Functions.dictMaker(met, "Rain");
                Dictionary<DateTime, double> _pet = Functions.dictMaker(met, "MeanPET");
                Dictionary<DateTime, double> _testResults = Functions.dictMaker(testResults, "Value");
                Dictionary<DateTime, double> _nApplied = Functions.dictMaker(nApplied, "Amount");
                Dictionary<string, object> _config = Functions.dictMaker(config);
                return Simulation.SimulateField(_tt, _rain, _pet, _testResults, _nApplied, _config);
            }
            else
            {
                object[,] listOfComplaints = new object[configErrors.Count,3];
                int c = 0;
                foreach (string e in configErrors)
                { 
                    listOfComplaints[c, 2] = e;
                    c++;
                }
                return listOfComplaints;
            }
        }

        /// <summary>
        /// Takes daily mean temperature 2D array format with date in the first column, calculates variables for a single crop and returns them in a 2D array)
        /// </summary>
        /// <param name="Tt">Array of daily thermal time over the duration of the crop</param>
        /// <param name="Config">2D aray with parameter names and values for crop configuration parameters</param>
        /// <returns>Dictionary with parameter names as keys and parameter values as values</returns>
        public static object[,] GetDailyCropData(double[] Tt, object[,] Config)
        {
            Dictionary<string, object> c = Functions.dictMaker(Config);
            CropConfig config = new CropConfig(c, "Current");
            DateTime[] cropDates = Functions.DateSeries(config.EstablishDate, config.HarvestDate);
            Dictionary<DateTime, double> tt = Functions.dictMaker(cropDates, Tt);
            Dictionary<DateTime, double> AccTt = Functions.AccumulateTt(cropDates, tt);
            return Crop.Grow(AccTt, config);
        }

        public static object[,] GetCropCoefficients()
        {
            return Functions.packDataFrame(SVSModel.Crop.LoadCropCoefficients());
        }
    }
}