﻿using System.IO;
using NUnit.Framework;
using POS.DomainModels.EstimateDomainModels;
using POS.Infrastructure.Readers;

namespace POS.Tests.Infrastructure.Readers;

public class EstimateReaderTests
{
    private EstimateReader _estimateReader = null!;

    private const string EstimateSourceFilesDirectory = @"..\..\..\Infrastructure\SourceFiles\EstimateSourceFiles";

    [SetUp]
    public void SetUp()
    {
        _estimateReader = new EstimateReader();
    }

    [Test]
    public void Read_EstimateSourceFile548_CorrectEstimate()
    {
        var expectedEstimate = EstimateSource.Estimate548VAT;

        var estimatePath = Path.Combine(EstimateSourceFilesDirectory, "5.5-20.548VAT.xlsx");

        Estimate actualEstimate;
        using (var fileStream = new FileStream(estimatePath, FileMode.Open))
        {
            actualEstimate = _estimateReader.Read(fileStream, TotalWorkChapter.TotalWork1To12Chapter);
        }

        Assert.AreEqual(expectedEstimate, actualEstimate);
    }

    [Test]
    public void Read_EstimateSourceFile158VAT_CorrectEstimate()
    {
        var expectedEstimate = EstimateSource.Estimate158VAT;

        var estimatePath = Path.Combine(EstimateSourceFilesDirectory, "5.4-18.158VAT.xlsx");

        Estimate actualEstimate;
        using (var fileStream = new FileStream(estimatePath, FileMode.Open))
        {
            actualEstimate = _estimateReader.Read(fileStream, TotalWorkChapter.TotalWork1To12Chapter);
        }

        Assert.AreEqual(expectedEstimate, actualEstimate);
    }
}