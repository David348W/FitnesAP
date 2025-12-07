using Microsoft.VisualStudio.TestTools.UnitTesting;
using FitnesAP.Data;
using FitnesAP.Models;
using System.IO;
using System.Linq;

namespace FitnesAp_Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private string _testFilePath;
        private UserService _service;

        [TestInitialize]
        public void Setup()
        {          
            _testFilePath = $"test_users_{Guid.NewGuid()}.json";         
            _service = new UserService(_testFilePath);
        }

        [TestCleanup]
        public void Cleanup()    //Ker se prehitro izvajajo
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
        public void RegistrirajNovegaUporabnikaTrue()
        {
           
            bool result = _service.Register("TestUser", "password123");            
            Assert.IsTrue(result, "Registracija bi morala uspeti.");
        }

        [TestMethod]
        public void RegistrirajObstoječegaUporabnikaFalse()
        {           
            _service.Register("David", "pass");     
            bool result = _service.Register("David", "novogeslo");           
            Assert.IsFalse(result, "Registracija obstoječega uporabnika ne sme uspeti.");
        }

        [TestMethod]
        public void PravilniLoginTrue()
        {        
            _service.Register("FitnesMojster", "skrivnost");         
            bool loginSuccess = _service.CheckLogin("FitnesMojster", "skrivnost");  
            Assert.IsTrue(loginSuccess, "Prijava s pravimi podatki bi morala uspeti.");
        }

        [TestMethod]
        public void PosodobiImeUporabnikaTrue()
        {           
            _service.Register("Marko", "123");
            var user = _service.GetUserByUsername("Marko");
            user.Ime = "SpremenjenoIme";
            _service.UpdateUser(user);
            var updatedUser = _service.GetUserByUsername("Marko");            
            Assert.AreEqual("SpremenjenoIme", updatedUser.Ime);
        }
    }
}