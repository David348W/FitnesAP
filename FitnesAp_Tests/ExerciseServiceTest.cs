using Microsoft.VisualStudio.TestTools.UnitTesting;
using FitnesAP.Data;
using FitnesAP.Models;
using System.IO;
using System.Linq;
using System;

namespace FitnesAp_Tests
{
    [TestClass]
    public class ExerciseServiceTests
    {
        private string _testFilePath;
        private ExerciseService _service;

        [TestInitialize]
        public void Setup()
        {           
            _testFilePath = $"test_exercises_{Guid.NewGuid()}.json";        
            _service = new ExerciseService(_testFilePath);
        }

        [TestCleanup]
        public void Cleanup()
        {         
            if (File.Exists(_testFilePath))
            {
                try
                {
                    File.Delete(_testFilePath);
                }
                catch
                {                   
                }
            }
        }

        [TestMethod]
        public void DodajVajo()
        {          
            var vaja = new Exercise
            {
                Ime = "Push Ups",
                VideoUrl = "https://youtube.com/example",
                SlikaUrl = "/slike/pushups.jpg"
            };          
            _service.AddExercise(vaja);
            var vseVaje = _service.GetExercises();          
            Assert.AreEqual(1, vseVaje.Count, "Seznam bi moral vsebovati točno 1 vajo.");
            Assert.AreEqual("Push Ups", vseVaje.First().Ime, "Ime vaje se mora ujemati.");
        }

        [TestMethod]
        public void AutoIncrementNaDodanihVajah()
        {       
            var vaja1 = new Exercise { Ime = "Vaja 1" };
            var vaja2 = new Exercise { Ime = "Vaja 2" };
            _service.AddExercise(vaja1);
            _service.AddExercise(vaja2);
            var vseVaje = _service.GetExercises();           
            var zadnjaVaja = vseVaje.Last();
            Assert.IsTrue(zadnjaVaja.Id > 1, "ID bi se moral avtomatsko povečati.");
        }

        [TestMethod]
        public void NiDatotekeZaVaj()
        {         
            var rezultat = _service.GetExercises();          
            Assert.IsNotNull(rezultat);
            Assert.AreEqual(0, rezultat.Count, "Če datoteke ni, mora vrniti prazen seznam, ne null.");
        }
    }
}