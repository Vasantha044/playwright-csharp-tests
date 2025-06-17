using NUnit.Framework;
using Microsoft.Playwright;
using System.Threading.Tasks;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using PlaywrightTests.Pages;
using PlaywrightTests.Utilities;
using System;

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
        private LogoutPage _logOutPage;

        [SetUp]
        public async Task Setup()
        {
            _loginPage = new LoginPage(Page);
            extraWorkPage = new ApplyExtraWorkingPage(Page);
            _addEmployeeHelper = new AddEmployeeHelper(Page);
            _logOutPage = new LogoutPage(Page);

            await _loginPage.LoginWithValidOrInvalidCred(
                ConfigManager.Settings.Credentials?.Username!,
                ConfigManager.Settings.Credentials?.Password!);
        }

        [Test]
        [Category("ExtraWork")]
        [AllureStep("Apply and Approve Extra Working Hours")]
        public async Task TestApplyAndApproveExtraWorkingHours()
        {
            // Step 1: Create Lead and Employee
            var lead = await _addEmployeeHelper.CreateOrAddEmployeeInUrbuddi("Admin");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var employee = await _addEmployeeHelper.CreateOrAddEmployeeInUrbuddi("Employee");

            Console.WriteLine($"Lead Email: {lead.Email}");
            Console.WriteLine($"Employee Email: {employee.Email}");

            // Step 2: Logout as Admin, Login as Employee
            await _logOutPage.LogoutAsync();
            await _loginPage.LoginWithValidOrInvalidCred(employee.Email!, employee.Password!);

            // Step 3: Apply for Extra Working Hours
            await extraWorkPage.ClickReimbursementSideBarAsync();
            await extraWorkPage.ClickApplyExtraWorkButtonAsync();
            await extraWorkPage.EnterRandomDateAndHoursAsync();
            await extraWorkPage.SelectLeadAsync(lead.Email!.ToLower());
            await extraWorkPage.SubmitFormAsync();

            // Step 4: Logout as Employee, Login back as Admin
            await _logOutPage.LogoutAsync();
            await _loginPage.LoginWithValidOrInvalidCred(
                ConfigManager.Settings.Credentials?.Username!,
                ConfigManager.Settings.Credentials?.Password!);

            // Step 5: Approve the Extra Working Hours
            await extraWorkPage.ClickApproveLeaveButtonAsync(employee.EmployeeID);
        }
    }
}
