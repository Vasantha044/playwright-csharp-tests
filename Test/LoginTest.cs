using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using System;
using System.Threading.Tasks;
using PlaywrightTests.Utilities;       // For BaseSetup
using PlaywrightTests.Pages;

namespace PlaywrightTests.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Login Tests")]
    public class LoginTest : BaseSetup
    {
        private LoginPage _loginPage;

        string username = ConfigManager.Settings.SystemCredentials?.Username ?? "";
        string password = ConfigManager.Settings.SystemCredentials?.Password ?? "";
        string email = ConfigManager.Settings.Credentials?.InvalidUsername ?? "";

        [SetUp]
        public void Init()
        {
            _loginPage = new LoginPage(Page);

            // ✅ Log runsettings parameters to verify they are loaded
            TestContext.WriteLine("---- Loaded Test Parameters from RunSettings ----");
            foreach (string param in TestContext.Parameters.Names)
            {
                var value = TestContext.Parameters[param];
                TestContext.WriteLine($"{param} = {value}");
            }
            TestContext.WriteLine("--------------------------------------------------");
        }

        [Test]
        [AllureStep]
        public async Task TC_01_LoginWithValidCredentials()
        {
            Console.WriteLine("Secret User: " + ConfigManager.Settings.SystemCredentials?.Username);
            await _loginPage.LoginWithValidOrInvalidCred(username, password);
            await _loginPage.PrintHolidayCountAsync();
        }

        [Test]
        [AllureStep]
        public async Task TC_02_LoginWithInvalidEmail()
        {
            await _loginPage.LoginWithValidOrInvalidCred(email, password);
        }
    }
}
