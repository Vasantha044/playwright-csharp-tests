using System;
using System.Threading.Tasks;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using PlaywrightTests.Utilities;
using PlaywrightTests.Pages;
using Microsoft.Playwright;

namespace PlaywrightTests.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Apply and Approve Leave Test")]
    public class ApplyLeaveTest : BaseSetup
    {
        private ApplyLeavePage applyLeavePage;
        private LoginPage _loginPage;
        private AddEmployeeHelper _addEmployeeHelper;
        private LogoutPage _logOutPage;

        [SetUp]
        public async Task Init()
        {
            _loginPage = new LoginPage(Page);
            applyLeavePage = new ApplyLeavePage(Page);
            _addEmployeeHelper = new AddEmployeeHelper(Page);
            _logOutPage = new LogoutPage(Page);

            await _loginPage.LoginWithValidOrInvalidCred(
                ConfigManager.Settings.Credentials?.Username!,
                ConfigManager.Settings.Credentials?.Password!);
        }

        [Test]
        [AllureStep("Apply and Approve Leave")]
        public async Task TestApplyAndApproveLeave()
        {
            // Step 1: Create Lead and Employee
            var lead = await _addEmployeeHelper.CreateOrAddEmployeeInUrbuddi("Admin");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var employee = await _addEmployeeHelper.CreateOrAddEmployeeInUrbuddi("Employee");

            TestContext.WriteLine($"Created Employee: {employee.Email}, Lead: {lead.Email}");

            // Step 2: Logout as Admin and login as Employee
            await _logOutPage.LogoutAsync();
            await _loginPage.LoginWithValidOrInvalidCred(employee.Email!, employee.Password!);

            // Step 3: Apply Leave
            await applyLeavePage.ClickLeaveManagementSidebarAsync();
            await applyLeavePage.ClickApplyLeaveButtonAsync();
            await applyLeavePage.EnterDatesAsync();
            await applyLeavePage.CheckLeaveTypeAsync();
            await applyLeavePage.EnterSubjectAndReasonAsync("Leave request", "Personal reason");
            await applyLeavePage.SubmitFormAsync();

            // Step 4: Logout as Employee and login back as Admin to approve
            await _logOutPage.LogoutAsync();
            await _loginPage.LoginWithValidOrInvalidCred(
                ConfigManager.Settings.Credentials?.Username!,
                ConfigManager.Settings.Credentials?.Password!);

            // Step 5: Approve Leave
            await applyLeavePage.ClickApproveLeaveButtonAsync(employee.EmployeeID);
        }
    }
}
