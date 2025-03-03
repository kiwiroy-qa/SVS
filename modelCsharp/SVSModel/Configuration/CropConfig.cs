﻿using System;
using System.Collections.Generic;

namespace Helper
{
    /// <summary>
    /// Class that stores the configuration information in the correct type for a specific crop .  
    /// I.e constructor takes all config settings as objects and converts them to appropriates types
    /// </summary>
    public class CropConfig
    {
        public string CropNameFull { get; private set; }
        public string EstablishStage { get; private set; }
        public string HarvestStage { get; private set; }
        public double SaleableYield { get; private set; }
        public string Units { get; private set; }
        public double ToKGperHA { get { return Constants.UnitConversions[Units]; } }
        public double FieldLoss { get; private set; }
        public double DressingLoss { get; private set; }
        public double MoistureContent { get; private set; }
        public DateTime EstablishDate { get; private set; }
        public DateTime HarvestDate { get; private set; }
        public double ResidueFactRetained { get; private set; }
        public double ResidueFactIncorporated { get; private set; }
        public double ResRoot { get; set; }
        public double ResStover { get; set; }
        public double ResFieldLoss { get; set; }
        public double NUptake { get; set; }
        public CropConfig(Dictionary<string, object> c, string pos)
        {
            CropNameFull = c[pos + "CropNameFull"].ToString();
            EstablishStage = c[pos + "EstablishStage"].ToString();
            HarvestStage = c[pos + "HarvestStage"].ToString();
            SaleableYield = Functions.Num(c[pos + "SaleableYield"])*1000; //UI sends yield in t/ha but model works in kg/ha so convert here
            FieldLoss = Functions.Num(c[pos + "FieldLoss"]);
            DressingLoss = Functions.Num(c[pos + "DressingLoss"]);
            MoistureContent = Functions.Num(c[pos + "MoistureContent"]);
            EstablishDate = Functions.Date(c[pos + "EstablishDate"]);
            HarvestDate = Functions.Date(c[pos + "HarvestDate"]);
            ResidueFactRetained = Constants.ResidueFactRetained[c[pos + "ResidueRemoval"].ToString()];
            ResidueFactIncorporated = Constants.ResidueIncorporation[c[pos + "ResidueIncorporation"].ToString()];
        }
    }
}
