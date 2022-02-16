﻿using BGTG.POS.DurationTools.DurationByTCPTool.TCP;
using NUnit.Framework;

namespace BGTG.POS.Tests.DurationTools.DurationByTCPTool.TCP
{
    public class DiameterRangeTests
    {
        private DiameterRange _diameterRange = null!;

        [TestCase(0, true)]
        [TestCase(200, true)]
        [TestCase(499, true)]
        [TestCase(500, false)]
        public void IsInRange_VariousValues_CorrectAnswers(int value, bool expectedIsInRange)
        {
            _diameterRange = new DiameterRange(0, 500, "до 500");

            var actualIsInRange = _diameterRange.IsInRange(value);

            Assert.AreEqual(expectedIsInRange, actualIsInRange);
        }
    }
}
