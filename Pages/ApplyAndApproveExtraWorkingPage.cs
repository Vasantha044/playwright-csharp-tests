using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Playwright;
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
        private ActionHelper actionHelper;

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
            approveLeaveButton = page.Locator("(//button[text()='Approve'])[1]");
             actionHelper = new ActionHelper(_page); 
        }

        public async Task ClickReimbursementSideBarAsync()
        {
            await reimbursementSideBar.ClickAsync();
            await Expect(applyExtraWorkButton).ToBeVisibleAsync();
        }

        public async Task ClickApplyExtraWorkButtonAsync()
        {
            await applyExtraWorkButton.ClickAsync();
            await Expect(applyReimbursementDialogBox).ToBeVisibleAsync();
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

            await Expect(dateInput).ToHaveValueAsync(formattedDate);
            await Expect(enterExtraHoursInput).ToHaveValueAsync(hours);
        }

        public async Task SelectLeadAsync(string lead)
        {
            await selectLeadDropDn.SelectOptionAsync(new SelectOptionValue { Label = lead });
            await Expect(selectLeadDropDn).Not.ToHaveValueAsync(string.Empty);
        }

        public async Task SubmitFormAsync()
        {
            await submitButton.ClickAsync();
        }

        public async Task AssertExtraWorkAppliedAsync()
        {
            await Expect(extraWorkingHoursAppliedsuccessMessage).ToBeVisibleAsync();
            
        }

        public async Task ClickApproveLeaveButtonAsync(string email)
        {
            await Expect(reimbursementSideBar).ToBeVisibleAsync();

            await reimbursementSideBar.ClickAsync();
            await requestTabButton.ClickAsync();

            await Expect(searchEmployee).ToBeVisibleAsync();
            await searchEmployee.FillAsync(email);

            await actionHelper!.ScrollRightAsync(scrollRight);
            await approveLeaveButton.ClickAsync();
        }
    }
}
