﻿using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using POS.DomainModels.CalendarPlanDomainModels;
using POS.DomainModels.EstimateDomainModels;
using POS.Infrastructure.Constants;
using POS.Infrastructure.Creators;
using POS.Infrastructure.Creators.Base;

namespace POS.Tests.Infrastructure.Creators;

public class CalendarPlanCreatorTests
{
    private CalendarPlanCreator _calendarPlanCreator = null!;
    private Mock<ICalendarWorksCreator> _calendarWorksProvider = null!;

    [SetUp]
    public void SetUp()
    {
        _calendarWorksProvider = new Mock<ICalendarWorksCreator>();
        _calendarPlanCreator = new CalendarPlanCreator(_calendarWorksProvider.Object);
    }

    [Test]
    public void Create_Estimate548VAT_CorrectCalendarPlan()
    {
        var estimate = EstimateSource.Estimate548VAT;
        var otherExpensesPercentages = new List<decimal>();
        var totalPreparatoryCalendarWork = new CalendarWork(AppConstants.TotalWorkName, 0, 0, new List<ConstructionMonth>(), 0);

        var preparatoryCalendarWorks = new List<CalendarWork> { totalPreparatoryCalendarWork };
        _calendarWorksProvider
            .Setup(x => x.CreatePreparatoryCalendarWorks(estimate.PreparatoryEstimateWorks, estimate.ConstructionStartDate))
            .Returns(preparatoryCalendarWorks);

        var mainCalendarWorks = new List<CalendarWork>();
        _calendarWorksProvider
            .Setup(x => x.CreateMainCalendarWorks(estimate.MainEstimateWorks, totalPreparatoryCalendarWork,
                estimate.ConstructionStartDate, estimate.ConstructionDurationCeiling, otherExpensesPercentages, TotalWorkChapter.TotalWork1To12Chapter))
            .Returns(mainCalendarWorks);

        var expectedCalendarPlan = new CalendarPlan(preparatoryCalendarWorks, mainCalendarWorks, estimate.ConstructionStartDate, estimate.ConstructionDuration, estimate.ConstructionDurationCeiling);

        var actualCalendarPlan = _calendarPlanCreator.Create( estimate, otherExpensesPercentages, TotalWorkChapter.TotalWork1To12Chapter);

        Assert.AreEqual(expectedCalendarPlan, actualCalendarPlan);
        _calendarWorksProvider.Verify(x => x.CreatePreparatoryCalendarWorks(estimate.PreparatoryEstimateWorks, estimate.ConstructionStartDate), Times.Once);
        _calendarWorksProvider.Verify(x => x.CreateMainCalendarWorks(estimate.MainEstimateWorks, totalPreparatoryCalendarWork,
            estimate.ConstructionStartDate, estimate.ConstructionDurationCeiling, otherExpensesPercentages, TotalWorkChapter.TotalWork1To12Chapter), Times.Once);
    }
}