using FitnesAP.data;
using FitnesAP.Data;
using FitnesAP.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FitnesAp_Tests
{
    [TestClass]
    public class WorkoutServiceTests
    {
        private string _testFilePath;
        private WorkoutService _service;

        [TestInitialize]
        public void Setup()
        {
            _testFilePath = $"test_workouts_{Guid.NewGuid()}.json";
            _service = new WorkoutService(_testFilePath);
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
        public void DodajTrening_PravilnoShrani()
        {
            var novTrening = new Workout
            {
                UserId = 1,
                Name = "Leg Day",
            };

            _service.AddWorkout(novTrening);
            var treningi = _service.GetWorkoutsForUser(1);

            Assert.AreEqual(1, treningi.Count, "Uporabnik bi moral imeti 1 trening.");
            Assert.AreEqual("Leg Day", treningi.First().Name);
            Assert.IsTrue(treningi.First().Id > 0, "Trening mora dobiti ID.");
        }

        [TestMethod]
        public void GetWorkoutsForUser_VrneSamoZaDoločenegaUporabnika()
        {
            _service.AddWorkout(new Workout { UserId = 1, Name = "User 1 Workout" });

            _service.AddWorkout(new Workout { UserId = 2, Name = "User 2 Workout" });

            var result = _service.GetWorkoutsForUser(1);

            Assert.AreEqual(1, result.Count, "Metoda mora vrniti samo treninge uporabnika 1.");
            Assert.AreEqual("User 1 Workout", result[0].Name);
        }

        [TestMethod]
        public void IzbrisiTrening_OdstraniIzSeznama()
        {
            var trening = new Workout { UserId = 1, Name = "Za Brisat" };
            _service.AddWorkout(trening);

            var shranjenTrening = _service.GetWorkoutsForUser(1).First();
            int idZaBrisanje = shranjenTrening.Id;

            _service.DeleteWorkout(idZaBrisanje);

            var rezultat = _service.GetWorkoutsForUser(1);
            Assert.AreEqual(0, rezultat.Count, "Seznam bi moral biti prazen po brisanju.");
        }

        [TestMethod]
        public void PosodobiTrening_ShraniSpremembe()
        {
            _service.AddWorkout(new Workout { UserId = 1, Name = "Stari Ime" });
            var original = _service.GetWorkoutsForUser(1).First();

            original.Name = "Novo Ime";
            original.EndTime = DateTime.Now;

            original.Exercises.Add(new WorkoutExercise
            {
                Name = "Bench",
                Sets = new List<WorkoutSet> { new WorkoutSet { Weight = 100, Reps = 5 } }
            });

            _service.UpdateWorkout(original);

            var posodobljen = _service.GetWorkoutById(original.Id);

            Assert.IsNotNull(posodobljen);
            Assert.AreEqual("Novo Ime", posodobljen.Name);
            Assert.IsNotNull(posodobljen.EndTime, "EndTime bi moral biti shranjen.");
            Assert.AreEqual(1, posodobljen.Exercises.Count, "Vaja bi morala biti shranjena.");
            Assert.AreEqual(100, posodobljen.Exercises[0].Sets[0].Weight);
        }

        [TestMethod]
        public void GetWorkoutById_VrneNullCeNeObstaja()
        {
            var result = _service.GetWorkoutById(999);
            Assert.IsNull(result, "Če ID ne obstaja, mora vrniti null.");
        }
    }
}