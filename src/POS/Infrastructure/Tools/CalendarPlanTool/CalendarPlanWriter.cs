﻿using System.Globalization;
using POS.Infrastructure.Tools.CalendarPlanTool.Base;
using POS.Infrastructure.Tools.CalendarPlanTool.Models;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace POS.Infrastructure.Tools.CalendarPlanTool;

public class CalendarPlanWriter : ICalendarPlanWriter
{
    private const string WorkNamePattern = "%WN%";
    private const string TotalCostPattern = "%TC%";
    private const string TotalCostIncludingCAIWPattern = "%TIC%";
    private const string DateAcceptancePattern = "%DA%";

    private const int TopPatternRowIndex = 2;
    private const int BottomPatternRowIndex = 3;

    private const string AcceptanceTimeCellStr = "Приемка объекта в эксплуатацию";

    public MemoryStream Write(CalendarPlan calendarPlan, string preparatoryTemplatePath, string mainTemplatePath)
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");

        using var preparatoryDocument = DocX.Load(preparatoryTemplatePath);

        var constructionMonths = calendarPlan.MainCalendarWorks.First(x => x.WorkName == AppData.TotalWorkName).ConstructionMonths.ToArray();

        var preparatoryTable = preparatoryDocument.Tables[0];
        ModifyCalendarPlanTable(preparatoryTable, calendarPlan.PreparatoryCalendarWorks, constructionMonths);

        using var mainDocument = DocX.Load(mainTemplatePath);
        var mainTable = mainDocument.Tables[0];

        ModifyCalendarPlanTable(mainTable, calendarPlan.MainCalendarWorks, constructionMonths);

        if (calendarPlan.ConstructionDurationCeiling > 1)
        {
            mainTable.MergeCellsInColumn(mainTable.ColumnCount - 1, TopPatternRowIndex,
                mainTable.RowCount - 2);
            mainTable.Rows[TopPatternRowIndex].Cells[mainTable.ColumnCount - 1].Paragraphs[0]
                .Append(AcceptanceTimeCellStr).FontSize(12);
        }

        preparatoryDocument.InsertDocument(mainDocument);

        var memoryStream = new MemoryStream();
        preparatoryDocument.SaveAs(memoryStream);

        return memoryStream;
    }

    private void ModifyCalendarPlanTable(Table table, IEnumerable<CalendarWork> calendarWorks, ConstructionMonth[] constructionMonths)
    {
        ReplaceDatePatternWithActualDate(table, constructionMonths);

        var topPatternRow = table.Rows[TopPatternRowIndex];
        var bottomPatternRow = table.Rows[BottomPatternRowIndex];
        foreach (var calendarWork in calendarWorks)
        {
            AddRowToTable(table, topPatternRow, bottomPatternRow, calendarWork);
        }
        topPatternRow.Remove();
        bottomPatternRow.Remove();

        ReplacePercentPartsWithActualPercentages(table, constructionMonths);

        MergeExtraConstructionMonthIntoDash(table);
    }

    private void ReplaceDatePatternWithActualDate(Table calendarPlanPatternTable, ConstructionMonth[] constructionMonths)
    {
        var monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

        for (int i = 0; i < constructionMonths.Length; i++)
        {
            var monthName = monthNames[constructionMonths[i].Date.Month - 1];
            calendarPlanPatternTable.Rows[1].ReplaceText($"%D{i}%", char.ToUpper(monthName[0]) + monthName.Substring(1) + " " + constructionMonths[i].Date.Year);
        }

        if (constructionMonths.Length > 1)
        {
            var acceptanceDate = constructionMonths[^1].Date.AddMonths(1);
            var acceptanceMonthName = monthNames[acceptanceDate.Month - 1];
            calendarPlanPatternTable.Rows[1].ReplaceText(DateAcceptancePattern, acceptanceMonthName + " " + acceptanceDate.Year);
        }
    }

    private void AddRowToTable(Table table, Row topPatternRow, Row bottomPatternRow, CalendarWork calendarWork)
    {
        var newTopRow = table.InsertRow(topPatternRow, table.RowCount - 2);
        var newBottomRow = table.InsertRow(bottomPatternRow, table.RowCount - 2);

        newTopRow.ReplaceText(WorkNamePattern, calendarWork.WorkName);
        newTopRow.ReplaceText(TotalCostPattern, calendarWork.TotalCost.ToString(AppData.DecimalThreePlacesFormat));
        newTopRow.ReplaceText(TotalCostIncludingCAIWPattern, calendarWork.TotalCostIncludingCAIW.ToString(AppData.DecimalThreePlacesFormat));
        var constructionMonths = calendarWork.ConstructionMonths.ToArray();
        foreach (var constructionMonth in constructionMonths)
        {
            var creationIndex = constructionMonth.CreationIndex;
            newTopRow.ReplaceText($"%IV{creationIndex}%", constructionMonth.InvestmentVolume.ToString(AppData.DecimalThreePlacesFormat));
            newBottomRow.ReplaceText($"%IW{creationIndex}%", constructionMonth.VolumeCAIW.ToString(AppData.DecimalThreePlacesFormat));
        }
    }

    private void ReplacePercentPartsWithActualPercentages(Table calendarPlanPatternTable, ConstructionMonth[] constructionMonths)
    {
        var lastRow = calendarPlanPatternTable.Rows[^1];
        if (lastRow.Paragraphs.Count > 1)
        {
            for (int i = 0; i < constructionMonths.Length; i++)
            {
                lastRow.ReplaceText($"%P{i}%", constructionMonths[i].PercentPart.ToString(AppData.PercentFormat));
            }
        }
    }

    private void MergeExtraConstructionMonthIntoDash(Table calendarPlanPatternTable)
    {
        for (int rowIndex = 2; rowIndex < calendarPlanPatternTable.RowCount - 1; rowIndex++)
        {
            for (int columnIndex = 3; columnIndex < calendarPlanPatternTable.Rows[rowIndex].Paragraphs.Count; columnIndex++)
            {
                var paragraph = calendarPlanPatternTable.Rows[rowIndex].Paragraphs[columnIndex];
                if (paragraph.Text.StartsWith("%IV"))
                {
                    var nextRowParagraph = calendarPlanPatternTable.Rows[rowIndex + 1].Paragraphs[columnIndex];
                    paragraph.RemoveText(0, paragraph.Text.Length);
                    nextRowParagraph.RemoveText(0, nextRowParagraph.Text.Length);
                    calendarPlanPatternTable.MergeCellsInColumn(columnIndex, rowIndex, rowIndex + 1);
                    paragraph.Append("-").FontSize(12);
                }
            }
        }
    }
}