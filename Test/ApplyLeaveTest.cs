using System;
using System.Globalization;
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
    [AllureSuite("ApplyLeave Test")]
    public class ApplyLeaveTest : BaseSetup
    {

        private ApplyLeavePage applyLeavePage;
        private LoginPage _loginPage;
        private AddEmployeeHelper _addEmployeeHelper;

        private LogoutPage _logOutPage;

        private Employee? createLead, createEmployee;
        [SetUp]
        public async Task Init()
        {
            _loginPage = new LoginPage(Page);
            await _loginPage.LoginWithValidOrInvalidCred(
                ConfigManager.Settings.Credentials?.Username!,
                ConfigManager.Settings.Credentials?.Password!);
            _logOutPage = new LogoutPage(Page);
            applyLeavePage = new ApplyLeavePage(Page);
             _addEmployeeHelper = new AddEmployeeHelper(Page);
        }

        [Test]
      [AllureStep]
public async Task TestApplyLeave()
{
    createLead = await _addEmployeeHelper.CreateOrAddEmployeeInUrbuddi("Admin");
    await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    createEmployee = await _addEmployeeHelper.CreateOrAddEmployeeInUrbuddi("Employee");
    
    Console.Write(createEmployee.Email);
    Console.Write(createLead.Email);

    await _logOutPage.LogoutAsync();
    await _loginPage.LoginWithValidOrInvalidCred(createEmployee!.Email!, createEmployee!.Password!);
    
    await applyLeavePage.ClickLeaveManagementSidebarAsync();
    await applyLeavePage.ClickApplyLeaveButtonAsync();
    await applyLeavePage.EnterDatesAsync();
    await applyLeavePage.CheckLeaveTypeAsync();
    await applyLeavePage.EnterSubjectAndReasonAsync("Leave request", "Personal reason");
    await applyLeavePage.SubmitFormAsync();
}



        [Test]
        [AllureStep]
        public async Task TestApproveLeave()
        {
        //    await _logOutPage.LogoutAsync();
        //     await _loginPage.LoginWithValidOrInvalidCred(ConfigManager.Settings.Credentials?.Username!,
        //         ConfigManager.Settings.Credentials?.Password!);

        //     // await applyLeavePage.ClickLeaveManagementSidebarAsync();
            await applyLeavePage.ClickApproveLeaveButtonAsync(createEmployee.EmployeeID);
        } 
    }
}