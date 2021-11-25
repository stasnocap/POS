﻿using NUnit.Framework;
using POSCore.EstimateLogic;
using POSCore.EstimateLogic.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace POSCoreTests.EstimateLogic.EstimateConnetorTests
{
    public class ConnectShould
    {
        private IEstimateConnector _estimateConnector;

        private string[] _defaultEstimateWorksNames = new string[]
        {
            "ЭЛЕКТРОХИМИЧЕСКАЯ ЗАЩИТА",
            "БЛАГОУСТРОЙСТВО ТЕРРИТОРИИ",
            "ОРГАНИЗАЦИЯ ДОРОЖНОГО ДВИЖЕНИЯ НА ПЕРИОД СТРОИТЕЛЬСТВА",
            "ВРЕМЕННЫЕ ЗДАНИЯ И СООРУЖЕНИЯ 8,56Х0,93 - 7,961%"
        };

        [SetUp]
        public void SetUp()
        {
            _estimateConnector = new EstimateConnector();
        }

        private List<EstimateWork> CreateDefaultEstimateWorks()
        {
            return _defaultEstimateWorksNames.Select(x => new EstimateWork(x, 0, 0, 0, 0)).ToList();
        }

        [Test]
        public void BeInOrder()
        {
            var estimateWorksVatFree = new List<EstimateWork>
            {
                new EstimateWork("ЭЛЕКТРОХИМИЧЕСКАЯ ЗАЩИТА", 0, 0, 0, 0),
                new EstimateWork("БЛАГОУСТРОЙСТВО ТЕРРИТОРИИ", 0, 0, 0, 0),
                new EstimateWork("ОРГАНИЗАЦИЯ ДОРОЖНОГО ДВИЖЕНИЯ НА ПЕРИОД СТРОИТЕЛЬСТВА", 0, 0, 0, 0),
                new EstimateWork("ВРЕМЕННЫЕ ЗДАНИЯ И СООРУЖЕНИЯ 8,56Х0,93 - 7,961%", 0, 0, 0, 0),
            };
            var estimateVatFree = new Estimate(estimateWorksVatFree);

            var estimateWorksVat = new List<EstimateWork>
            {
                new EstimateWork("ЭЛЕКТРОХИМИЧЕСКАЯ ЗАЩИТА", 0, 0, 0, 0),
                new EstimateWork("ШРП", 0, 0, 0, 0),
                new EstimateWork("БЛАГОУСТРОЙСТВО ТЕРРИТОРИИ", 0, 0, 0, 0),
                new EstimateWork("ГРП", 0, 0, 0, 0),
                new EstimateWork("ОРГАНИЗАЦИЯ ДОРОЖНОГО ДВИЖЕНИЯ НА ПЕРИОД СТРОИТЕЛЬСТВА", 0, 0, 0, 0),
                new EstimateWork("ВРЕМЕННЫЕ ЗДАНИЯ И СООРУЖЕНИЯ 8,56Х0,93 - 7,961%", 0, 0, 0, 0),
            };
            var estimateVat = new Estimate(estimateWorksVat);

            var expectedOrder = new List<EstimateWork>
            {
                new EstimateWork("ЭЛЕКТРОХИМИЧЕСКАЯ ЗАЩИТА", 0, 0, 0, 0),
                new EstimateWork("БЛАГОУСТРОЙСТВО ТЕРРИТОРИИ", 0, 0, 0, 0),
                new EstimateWork("ШРП", 0, 0, 0, 0),
                new EstimateWork("ОРГАНИЗАЦИЯ ДОРОЖНОГО ДВИЖЕНИЯ НА ПЕРИОД СТРОИТЕЛЬСТВА", 0, 0, 0, 0),
                new EstimateWork("ГРП", 0, 0, 0, 0),
                new EstimateWork("ВРЕМЕННЫЕ ЗДАНИЯ И СООРУЖЕНИЯ 8,56Х0,93 - 7,961%", 0, 0, 0, 0),
            };

            var estimates = new Estimate[]
            {
                estimateVatFree,
                estimateVat
            };

            var estimate = _estimateConnector.Connect(estimates);

            var estimateWorks = estimate.EstimateWorks;
            for (int i = 0; i < expectedOrder.Count; i++)
            {
                Assert.AreEqual(expectedOrder[i].WorkName, estimateWorks[i].WorkName);
            }
        }

        [Test]
        public void NotReturnNull()
        {
            var estimateWorks = CreateDefaultEstimateWorks();

            var estimates = new Estimate[]
            {
                new Estimate(estimateWorks),
                new Estimate(estimateWorks)
            };

            var estimate = _estimateConnector.Connect(estimates);

            Assert.NotNull(estimate);
        }

        [Test]
        public void ReturnEstimateWithNotNullEstimateWorks()
        {
            var estimateWorks = CreateDefaultEstimateWorks();

            var estimates = new Estimate[]
            {
                new Estimate(estimateWorks),
                new Estimate(estimateWorks)
            };

            var estimate = _estimateConnector.Connect(estimates);

            foreach (var estimateWork in estimate.EstimateWorks)
            {
                Assert.NotNull(estimateWork);
            }
        }

        [Test]
        public void NotContainRepetitiveWorks()
        {
            var estimateWorks = CreateDefaultEstimateWorks();

            var estimates = new Estimate[]
            {
                new Estimate(estimateWorks),
                new Estimate(estimateWorks)
            };

            var estimate = _estimateConnector.Connect(estimates);

            foreach (var estimateWork in estimate.EstimateWorks)
            {
                Assert.True(estimate.EstimateWorks.Count(x => x.WorkName == estimateWork.WorkName) == 1);
            }
        }

        private Estimate CreateEstimateWithOneEstimateWork(string workName, decimal equipmentCost, decimal otherProductsCost, decimal totalCost, int chapter)
        {
            var estimateWork = new EstimateWork(workName, equipmentCost, otherProductsCost, totalCost, chapter);
            var estimateWorks = new List<EstimateWork> { estimateWork };
            return new Estimate(estimateWorks);
        }

        [Test]
        public void SumEstimateWorkEquipmentCosts()
        {
            var workName = "ЭЛЕКТРОХИМИЧЕСКАЯ ЗАЩИТА";

            var estimateWorkVatFreeEquipmentCost = (decimal)0.021;
            var estimateVatFree = CreateEstimateWithOneEstimateWork(workName, estimateWorkVatFreeEquipmentCost, 0, 0, 0);

            var estimateWorkVatEquipmentCost = (decimal)0.023;
            var estimateVat = CreateEstimateWithOneEstimateWork(workName, estimateWorkVatEquipmentCost, 0, 0, 0);

            var estimates = new Estimate[]
            {
                estimateVatFree,
                estimateVat,
            };

            var estimate = _estimateConnector.Connect(estimates);

            var estimateWorkEquipmentCost = estimate.EstimateWorks.Find(x => x.WorkName == workName).EquipmentCost;

            Assert.AreEqual(estimateWorkVatFreeEquipmentCost + estimateWorkVatEquipmentCost, estimateWorkEquipmentCost);
        }

        [Test]
        public void SumEstimateWorkOtherProductsCosts()
        {
            var workName = "ЭЛЕКТРОХИМИЧЕСКАЯ ЗАЩИТА";

            var estimateWorkVatFreeOtherProductsCost = (decimal)0.022;
            var estimateVatFree = CreateEstimateWithOneEstimateWork(workName, 0, estimateWorkVatFreeOtherProductsCost, 0, 0);

            var estimateWorkVatOtherProductsCost = (decimal)0.024;
            var estimateVat = CreateEstimateWithOneEstimateWork(workName, 0, estimateWorkVatOtherProductsCost, 0, 0);

            var estimates = new Estimate[]
            {
                estimateVatFree,
                estimateVat,
            };

            var estimate = _estimateConnector.Connect(estimates);

            var estimateWorkOtherPoductsCost = estimate.EstimateWorks.Find(x => x.WorkName == workName).OtherProductsCost;

            Assert.AreEqual(estimateWorkVatFreeOtherProductsCost + estimateWorkVatOtherProductsCost, estimateWorkOtherPoductsCost);
        }

        [Test]
        public void SumEstimateWorkTotalCosts()
        {
            var workName = "ЭЛЕКТРОХИМИЧЕСКАЯ ЗАЩИТА";

            var estimateWorkVatFreeTotalCost = (decimal)55.297;
            var estimateVatFree = CreateEstimateWithOneEstimateWork(workName, 0, 0, estimateWorkVatFreeTotalCost, 0);

            var estimateWorkVatTotalCost = (decimal)21.316;
            var estimateVat = CreateEstimateWithOneEstimateWork(workName, 0, 0, estimateWorkVatTotalCost, 0);

            var estimates = new Estimate[]
            {
                estimateVatFree,
                estimateVat,
            };

            var estimate = _estimateConnector.Connect(estimates);

            var estimateWorkTotalCost = estimate.EstimateWorks.Find(x => x.WorkName == workName).TotalCost;

            Assert.AreEqual(estimateWorkVatFreeTotalCost + estimateWorkVatTotalCost, estimateWorkTotalCost);
        }

        [Test]
        public void SetEstimateWorkChapterFromFirstPassedEstimate()
        {
            var workName = "ЭЛЕКТРОХИМИЧЕСКАЯ ЗАЩИТА";

            var chapter = 1;
            var estimateVatFree = CreateEstimateWithOneEstimateWork(workName, 0, 0, 0, chapter);

            var estimateVat = CreateEstimateWithOneEstimateWork(workName, 0, 0, 0, chapter);

            var estimates = new Estimate[]
            {
                estimateVatFree,
                estimateVat,
            };

            var estimate = _estimateConnector.Connect(estimates);

            var estimateWorkChapter = estimate.EstimateWorks.Find(x => x.WorkName == workName).Chapter;

            Assert.AreEqual(chapter, estimateWorkChapter);
        }

        [Test]
        public void ThreeEstimates_CorrectConnectWork()
        {
            var workName = "ЭЛЕКТРОХИМИЧЕСКАЯ ЗАЩИТА";
            var chapter = 1;

            var estimates = new Estimate[]
            {
                CreateEstimateWithOneEstimateWork(workName, (decimal)1.111, (decimal)2.222, (decimal)3.333, chapter),
                CreateEstimateWithOneEstimateWork(workName, (decimal)1.111, (decimal)2.222, (decimal)3.333, chapter),
                CreateEstimateWithOneEstimateWork(workName, (decimal)1.111, (decimal)2.222, (decimal)3.333, chapter),
            };

            var estimate = _estimateConnector.Connect(estimates);

            var connectedWork = estimate.EstimateWorks.Find(x => x.WorkName == workName);

            Assert.AreEqual(workName, connectedWork.WorkName);
            Assert.AreEqual((decimal)3.333, connectedWork.EquipmentCost);
            Assert.AreEqual((decimal)6.666, connectedWork.OtherProductsCost);
            Assert.AreEqual((decimal)9.999, connectedWork.TotalCost);
        }
    }
}
