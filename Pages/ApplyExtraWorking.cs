using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using static Microsoft.Playwright.Assertions;
using PlaywrightTests.Utilities;


namespace PlaywrightTests.Pages
{
    public class ApplyExtraWorkingPage
    {
        private readonly IPage _page;
        private readonly ILocator reimbursementSideBar;
        private readonly ILocator applyExtraWorkButton;
        private readonly ILocator applyReimbursementDialogBox;
        private readonly ILocator dateInput;
        private readonly ILocator enterExtraHoursInput;
        private readonly ILocator selectLeadDropDn;
        private readonly ILocator submitButton;
        private readonly ILocator extraWorkingHoursAppliedsuccessMessage;
        private readonly ILocator searchEmployee;

        private ActionHelper? actionHelper;

        private readonly ILocator leaveManagementSideBar;

        private readonly ILocator requestTabButton;

        private readonly string scrollRight;

        private readonly ILocator approveLeaveButton;

        public ApplyExtraWorkingPage(IPage page)
        {
            _page = page;
            leaveManagementSideBar = page.Locator("text=Leave Management");
            reimbursementSideBar = page.Locator("text=Reimbursement");
            applyExtraWorkButton = page.Locator("text=Apply Extra Work");
            applyReimbursementDialogBox = page.Locator("text=Apply Reimbursement");
            dateInput = page.Locator("//input[@name='date']");
            selectLeadDropDn = page.Locator("select[name='lead']");
            enterExtraHoursInput = page.Locator("[name='hours']");
            submitButton = page.Locator("button:has-text(\"Submit\")");
            extraWorkingHoursAppliedsuccessMessage = page.Locator("text=Extra work Applied Successfully");
            requestTabButton = page.Locator("//button[text()='Requests']");
            searchEmployee = page.Locator("//input[@aria-label='EMP ID Filter Input']");
            scrollRight = "div.ag-body-horizontal-scroll-viewport";
            approveLeaveButton =page.Locator("(//button[text()='Approve'])[1]");
        }

        public async Task ClickReimbursementSideBarAsync()
        {
            await reimbursementSideBar.ClickAsync();
            Assert.That(await applyExtraWorkButton.IsVisibleAsync(), Is.True,
                "Apply Extra Work button is not visible after clicking Reimbursement sidebar.");
        }

        public async Task ClickApplyExtraWorkButtonAsync()
        {
            await applyExtraWorkButton.ClickAsync();
            Assert.That(await applyReimbursementDialogBox.IsVisibleAsync(), Is.True,
                "Apply Reimbursement dialog is not visible after clicking Apply Extra Work button.");
        }

        public async Task EnterRandomDateAndHoursAsync()
        {
            var random = new Random();
            int daysAgo = random.Next(0, 30);
            DateTime randomDate = DateTime.Today.AddDays(-daysAgo);
            string formattedDate = randomDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string hours = random.Next(1, 9).ToString();

            await dateInput.FillAsync(formattedDate);
            await enterExtraHoursInput.FillAsync(hours);

            string actualDate = await dateInput.InputValueAsync();
            string actualHours = await enterExtraHoursInput.InputValueAsync();

            Assert.That(actualDate, Is.EqualTo(formattedDate), "Date input value mismatch.");
            Assert.That(actualHours, Is.EqualTo(hours), "Hours input value mismatch.");
        }

        public async Task SelectLeadAsync(string lead)
        {
            await selectLeadDropDn.ClickAsync();
            await selectLeadDropDn.SelectOptionAsync(new SelectOptionValue { Label = lead });
            var selectedValue = await selectLeadDropDn.InputValueAsync();

            Assert.That(string.IsNullOrEmpty(selectedValue), Is.False, "No lead selected.");
        }

        public async Task SubmitFormAsync()
        {
            await submitButton.ClickAsync();
        }

        public async Task AssertExtraWorkAppliedAsync()
        {
            await extraWorkingHoursAppliedsuccessMessage.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });

            bool isSuccessMessageVisible = await extraWorkingHoursAppliedsuccessMessage.IsVisibleAsync();
            Assert.That(isSuccessMessageVisible, Is.True, "Success message not visible after submitting form.");
        }
        
          public async Task ClickApproveLeaveButtonAsync(string Email)
        {
            await Expect(reimbursementSideBar).ToBeVisibleAsync(new() { Timeout = 5000 });
            await leaveManagementSideBar.ClickAsync();
            await requestTabButton.ClickAsync();
            await searchEmployee.IsVisibleAsync();
            await searchEmployee.ClearAsync();
            await searchEmployee.FillAsync(Email);
            await actionHelper.ScrollRightAsync(scrollRight);
            await approveLeaveButton.ClickAsync();
        }
        
    }
}
