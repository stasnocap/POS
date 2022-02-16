﻿using System;
using System.Collections.Generic;
using System.Linq;
using BGTG.POS.CalendarPlanTool.Base;
using BGTG.POS.EstimateTool;

namespace BGTG.POS.CalendarPlanTool
{
    public class CalendarWorkCreator : ICalendarWorkCreator
    {
        private readonly IConstructionMonthsCreator _constructionMonthsCreator;

        public CalendarWorkCreator(IConstructionMonthsCreator constructionMonthsCreator)
        {
            _constructionMonthsCreator = constructionMonthsCreator;
        }

        private CalendarWork Create(string workName, decimal totalCost, decimal totalCostIncludingCAIW, int estimateChapter, IEnumerable<ConstructionMonth> constructionMonths)
        {
            return new CalendarWork(workName, totalCost, totalCostIncludingCAIW, constructionMonths, estimateChapter);
        }

        private CalendarWork Create(string workName, decimal totalCost, decimal totalCostIncludingCAIW, int estimateChapter,
            DateTime constructionStartDate, List<decimal> percentages)
        {
            var constructionMonths = _constructionMonthsCreator.Create(constructionStartDate, totalCost, totalCostIncludingCAIW, percentages);
            return Create(workName, totalCost, totalCostIncludingCAIW, estimateChapter, constructionMonths);
        }

        public CalendarWork Create(EstimateWork estimateWork, DateTime constructionStartDate)
        {
            var totalCostIncludingCAIW = estimateWork.TotalCost - estimateWork.EquipmentCost - estimateWork.OtherProductsCost;
            return Create(estimateWork.WorkName, estimateWork.TotalCost, totalCostIncludingCAIW, estimateWork.Chapter, constructionStartDate, estimateWork.Percentages);
        }

        public CalendarWork CreateAnyPreparatoryWork(string workName, List<CalendarWork> preparatoryCalendarWorks, int estimateChapter,
            DateTime constructionStartDate, List<decimal> percentages)
        {
            var totalCost = preparatoryCalendarWorks.Sum(x => x.TotalCost);
            var totalCostIncludingCAIW = preparatoryCalendarWorks.Sum(x => x.TotalCostIncludingCAIW);
            var constructionMonths = _constructionMonthsCreator.Create(constructionStartDate, totalCost, totalCostIncludingCAIW, percentages);
            return Create(workName, totalCost, totalCostIncludingCAIW, estimateChapter, constructionMonths);
        }

        public CalendarWork CreateMainTotalWork(List<CalendarWork> mainCalendarWorks, CalendarWork initialTotalMainWork, DateTime constructionStartDate, int constructionDurationCeiling)
        {
            var constructionMonths = Enumerable.Range(0, constructionDurationCeiling).Select(i =>
                    new ConstructionMonth(
                        constructionStartDate.AddMonths(i),
                        mainCalendarWorks.Sum(calendarWork => calendarWork.ConstructionMonths.FirstOrDefault(constructionMonth => constructionMonth.Date == constructionStartDate.AddMonths(i))?.InvestmentVolume)!.Value,
                        mainCalendarWorks.Sum(calendarWork => calendarWork.ConstructionMonths.FirstOrDefault(constructionMonth => constructionMonth.Date == constructionStartDate.AddMonths(i))?.VolumeCAIW)!.Value,
                        (mainCalendarWorks.Sum(calendarWork => calendarWork.ConstructionMonths.FirstOrDefault(constructionMonth => constructionMonth.Date == constructionStartDate.AddMonths(i))?.InvestmentVolume) / initialTotalMainWork.TotalCost)!.Value,
                        i
                )).ToList();

            return Create(CalendarPlanInfo.TotalWorkName, initialTotalMainWork.TotalCost, initialTotalMainWork.TotalCostIncludingCAIW,
                initialTotalMainWork.EstimateChapter, constructionMonths);
        }

        public CalendarWork CreateOtherExpensesWork(List<CalendarWork> mainCalendarWorks, CalendarWork initialMainTotalWork,
            DateTime constructionStartDate, List<decimal> percentages)
        {
            var totalCost = initialMainTotalWork.TotalCost - mainCalendarWorks.Sum(x => x.TotalCost);
            var totalCostIncludingCAIW = initialMainTotalWork.TotalCostIncludingCAIW - mainCalendarWorks.Sum(x => x.TotalCostIncludingCAIW);
            return Create(CalendarPlanInfo.MainOtherExpensesWorkName, totalCost, totalCostIncludingCAIW, CalendarPlanInfo.MainOtherExpensesWorkChapter, constructionStartDate, percentages);
        }
    }
}
