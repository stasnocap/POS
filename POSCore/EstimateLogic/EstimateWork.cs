﻿namespace POSCore.EstimateLogic
{
    public class EstimateWork
    {
        public string WorkName { get; }
        public int Chapter { get; }
        public double EquipmentCost { get; }
        public double OtherProductsCost { get; }
        public double TotalCost { get; }

        public EstimateWork(string workName, double equipmentCost, double otherProductsCost, double totalCost, int chapter)
        {
            WorkName = workName;
            EquipmentCost = equipmentCost;
            OtherProductsCost = otherProductsCost;
            TotalCost = totalCost;
            Chapter = chapter;
        }
    }
}
