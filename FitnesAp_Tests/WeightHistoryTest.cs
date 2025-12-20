using FitnesAP.Data;
using FitnesAP.Models;
using FitnesAP.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using FitnesAP.data;
namespace FitnesAp_Tests
{
    [TestClass]
    public class WeightHistoryTests
    {
        private string _testPath;
        private WeightHistoryService _service;

        [TestInitialize]
        public void Setup()
        {
            _testPath = $"test_history_{Guid.NewGuid()}.json";
            _service = new WeightHistoryService(_testPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testPath))
            {
                try { File.Delete(_testPath); } catch { }
            }
        }

        [TestMethod]
        public void AddEntry_DodaNovVnos_InOhraniStare()
        {
            _service.AddEntry(1, 80.0);
            _service.AddEntry(1, 79.5);
            var zgodovina = _service.GetHistoryForUsers(1);

            Assert.AreEqual(2, zgodovina.Count, "Biti morata 2 vnosa (stari se ne sme izbrisati).");
            Assert.AreEqual(80.0, zgodovina[0].Weight);
            Assert.AreEqual(79.5, zgodovina[1].Weight);
        }

        [TestMethod]
        public void GetHistory_VrneSamoZaDolocenegaUporabnika()
        {
            _service.AddEntry(1, 80.0);
            _service.AddEntry(2, 60.0);

            var davidZgodovina = _service.GetHistoryForUsers(1);

            Assert.AreEqual(1, davidZgodovina.Count);
            Assert.AreEqual(80.0, davidZgodovina[0].Weight);
        }

        [TestMethod]
        public void GetHistory_SortiranoPoDatumu()
        {
            var vnosi = new List<WeightEntry>
            {
                new WeightEntry { UserId = 1, Weight = 85, Date = new DateTime(2025, 12, 05) }, // SREDINA
                new WeightEntry { UserId = 1, Weight = 80, Date = new DateTime(2025, 12, 01) }, // NAJSTAREJŠI
                new WeightEntry { UserId = 1, Weight = 90, Date = new DateTime(2025, 12, 10) }  // NAJNOVEJŠI
            };
            File.WriteAllText(_testPath, JsonSerializer.Serialize(vnosi));

            var rezultat = _service.GetHistoryForUsers(1);           
            Assert.AreEqual(80, rezultat[0].Weight); 
            Assert.AreEqual(85, rezultat[1].Weight);
            Assert.AreEqual(90, rezultat[2].Weight);
        }
    }
}