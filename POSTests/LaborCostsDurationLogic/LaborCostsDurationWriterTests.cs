﻿using System.IO;
using NUnit.Framework;
using POS.LaborCostsDurationLogic;
using Xceed.Words.NET;

namespace POSTests.LaborCostsDurationLogic
{
    public class LaborCostsDurationWriterTests
    {
        private LaborCostsDurationWriter _laborCostsDurationWriter;

        private const string _laborCostsDurationFileName = "LaborCostsDuration.docx";
        private const string _laborCostsDurationTemplatesDirectory = @"..\..\..\LaborCostsDurationLogic\LaborCostsDurationTemplates";

        private LaborCostsDuration CreateDefaultLaborCostsDuration()
        {
            return new LaborCostsDuration(1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, true, true, 0);
        }

        [SetUp]
        public void SetUp()
        {
            _laborCostsDurationWriter = new LaborCostsDurationWriter();
        }

        [Test]
        public void Write_RoundingPlusAcceptancePlus_SaveCorrectLaborCostsDuration()
        {
            var laborCostsDuration = CreateDefaultLaborCostsDuration();

            var savePath = Path.Combine(Directory.GetCurrentDirectory(), _laborCostsDurationFileName);
            var templatePath = Path.Combine(_laborCostsDurationTemplatesDirectory, "Rounding+Acceptance+Template.docx");

            _laborCostsDurationWriter.Write(laborCostsDuration, templatePath, savePath);

            using (var document = DocX.Load(savePath))
            {
                StringAssert.Contains(laborCostsDuration.AcceptanceTime.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.Duration.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.LaborCosts.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.NumberOfEmployees.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.NumberOfWorkingDays.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.PreparatoryPeriod.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.RoundedDuration.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.Shift.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.TotalDuration.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.WorkingDayDuration.ToString(), document.Text);
            }
        }

        [Test]
        public void Write_RoundingPlusAcceptanceMinus_SaveCorrectLaborCostsDuration()
        {
            var laborCostsDuration = CreateDefaultLaborCostsDuration();

            var savePath = Path.Combine(Directory.GetCurrentDirectory(), _laborCostsDurationFileName);
            var templatePath = Path.Combine(_laborCostsDurationTemplatesDirectory, "Rounding+Acceptance-Template.docx");

            _laborCostsDurationWriter.Write(laborCostsDuration, templatePath, savePath);

            using (var document = DocX.Load(savePath))
            {
                StringAssert.DoesNotContain(laborCostsDuration.AcceptanceTime.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.TotalDuration.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.Duration.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.LaborCosts.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.NumberOfEmployees.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.NumberOfWorkingDays.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.PreparatoryPeriod.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.RoundedDuration.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.Shift.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.WorkingDayDuration.ToString(), document.Text);
            }
        }

        [Test]
        public void Write_RoundingMinusAcceptanceMinus_SaveCorrectLaborCostsDuration()
        {
            var laborCostsDuration = CreateDefaultLaborCostsDuration();

            var savePath = Path.Combine(Directory.GetCurrentDirectory(), _laborCostsDurationFileName);
            var templatePath = Path.Combine(_laborCostsDurationTemplatesDirectory, "Rounding-Acceptance-Template.docx");

            _laborCostsDurationWriter.Write(laborCostsDuration, templatePath, savePath);

            using (var document = DocX.Load(savePath))
            {
                StringAssert.DoesNotContain(laborCostsDuration.AcceptanceTime.ToString(), document.Text);
                StringAssert.DoesNotContain(laborCostsDuration.TotalDuration.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.RoundedDuration.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.Duration.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.LaborCosts.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.NumberOfEmployees.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.NumberOfWorkingDays.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.PreparatoryPeriod.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.Shift.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.WorkingDayDuration.ToString(), document.Text);
            }
        }

        [Test]
        public void Write_RoundingMinusAcceptancePlus_SaveCorrectLaborCostsDuration()
        {
            var laborCostsDuration = CreateDefaultLaborCostsDuration();

            var savePath = Path.Combine(Directory.GetCurrentDirectory(), _laborCostsDurationFileName);
            var templatePath = Path.Combine(_laborCostsDurationTemplatesDirectory, "Rounding-Acceptance+Template.docx");

            _laborCostsDurationWriter.Write(laborCostsDuration, templatePath, savePath);

            using (var document = DocX.Load(savePath))
            {
                StringAssert.Contains(laborCostsDuration.TotalDuration.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.AcceptanceTime.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.RoundedDuration.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.Duration.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.LaborCosts.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.NumberOfEmployees.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.NumberOfWorkingDays.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.PreparatoryPeriod.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.Shift.ToString(), document.Text);
                StringAssert.Contains(laborCostsDuration.WorkingDayDuration.ToString(), document.Text);
            }
        }
    }
}
