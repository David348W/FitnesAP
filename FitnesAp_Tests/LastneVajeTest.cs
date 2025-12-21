using FitnesAP.Data;
using FitnesAP.Models;
using FitnesAP.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FitnesAp_Tests
{
    [TestClass]
    public class CustomExerciseTests
    {
        private string _testFilePath;
        private ExerciseService _service;

        [TestInitialize]
        public void Setup()
        {
            _testFilePath = $"test_custom_exercises_{Guid.NewGuid()}.json";
            _service = new ExerciseService(_testFilePath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testFilePath))
            {
                try { File.Delete(_testFilePath); } catch { }
            }
        }

        [TestMethod]
        public void DodajLastnoVajo_PravilnoZapiseCreatedBy()
        {
            string testUser = "Test";
            var novaVaja = new Exercise
            {
                Ime = "Moja Posebna Vaja",
                CreatedBy = testUser
            };
            _service.AddExercise(novaVaja);
            var vseVaje = _service.GetExercises();
            var shranjenaVaja = vseVaje.First();

            Assert.AreEqual(testUser, shranjenaVaja.CreatedBy, "CreatedBy se mora ujemati z ID-jem uporabnika, ki je vajo ustvaril.");
        }
    }
}
        
