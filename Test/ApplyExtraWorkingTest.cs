using NUnit.Framework;
using Microsoft.Playwright;
using System.Threading.Tasks;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using PlaywrightTests.Pages;
using PlaywrightTests.Utilities;
using System;
using NUnit.Framework.Constraints;  // Import the helper namespace

namespace PlaywrightTests.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("ApplyExtraWorking Test")]
    public class ApplyExtraWorkingHoursTest : BaseSetup
    {
        private ApplyExtraWorkingPage extraWorkPage;
        private LoginPage _loginPage;
        private AddEmployeeHelper _addEmployeeHelper;
        private LogoutPage _logOut;


        private Employee? createLead, createEmployee;

        [SetUp]
        public async Task Setup()
        {
            _loginPage = new LoginPage(Page);
            await _loginPage.LoginWithValidOrInvalidCred(
                ConfigManager.Settings.Credentials?.Username!,
                ConfigManager.Settings.Credentials?.Password!);

            extraWorkPage = new ApplyExtraWorkingPage(Page);
            _addEmployeeHelper = new AddEmployeeHelper(Page);  // Initialize helper with Page
            _logOut = new LogoutPage(Page);
        }

        [Test]
        [Category("ExtraWork")]
        [AllureStep]
        public async Task TestApplyExtraWorkingHours()
        {
            createLead = await _addEmployeeHelper.CreateOrAddEmployeeInUrbuddi("Admin");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            createEmployee = await _addEmployeeHelper.CreateOrAddEmployeeInUrbuddi("Employee");
            Console.Write(createEmployee.Email);
            Console.Write(createLead.Email);
            await _logOut.LogoutAsync();
            await _loginPage.LoginWithValidOrInvalidCred(createEmployee!.Email!, createEmployee!.Password!);
            await extraWorkPage.ClickReimbursementSideBarAsync();
            await extraWorkPage.ClickApplyExtraWorkButtonAsync();
            await extraWorkPage.EnterRandomDateAndHoursAsync();
            await extraWorkPage.SelectLeadAsync(createLead!.Email!.ToLower());
            await extraWorkPage.SubmitFormAsync();
            await _logOut.LogoutAsync();
            await _logOut.VerifyLoggedOutAsync();
        }
       
        [Test]
        public async Task TestApproveExtraWorkingHours()
        {
              await extraWorkPage.ClickApproveLeaveButtonAsync(createEmployee.EmployeeID);
        }

    }
    
}
