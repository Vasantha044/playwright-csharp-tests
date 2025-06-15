using System.Globalization;
using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using static Microsoft.Playwright.Assertions;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Pages
{
    public class ApplyLeavePage
    {
        private readonly IPage _page;
        private readonly ILocator leaveManagementSideBar;
        private readonly ILocator applyLeaveButton;
        private readonly ILocator fromDateInput;
        private readonly ILocator toDateInput;
        private readonly ILocator leadDropDown;
        private readonly ILocator leaveTypeCheckbox;
        private readonly ILocator subjectInput;
        private readonly ILocator reasonInput;
        private readonly ILocator submitButton;
        private readonly ILocator leaveAppliedsuccessMessage;
        private readonly ILocator lopOkButton;
        private readonly ILocator leaveAlreadyExists;

        private readonly ILocator approveLeaveButton;

        private readonly ILocator rejectLeaveButton;
        private readonly ILocator requestTabButton;

        private readonly ILocator reimbursementSideBar;
        private readonly ILocator searchEmployee;

        private ActionHelper actionHelper;

        private readonly string scrollRight;


        public ApplyLeavePage(IPage page)
        {
            _page = page;
            leaveManagementSideBar = page.Locator("text=Leave Management");
            applyLeaveButton = page.Locator("text=Apply Leave");
            fromDateInput = page.Locator("input#fromDate");
            toDateInput = page.Locator("input#toDate");
            leadDropDown = page.Locator("[name='lead']");
            leaveTypeCheckbox = page.Locator("#leave");
            subjectInput = page.Locator("[name='subject']");
            reasonInput = page.Locator("[name='reason']");
            submitButton = page.Locator("button:has-text(\"Submit\")");
            leaveAppliedsuccessMessage = page.Locator("text=Leave Applied Successfully");
            lopOkButton = page.Locator("//*[text()='Ok']");
            leaveAlreadyExists = page.Locator("//*[text()='Leave request already exists for the same date(s).']");
            approveLeaveButton = page.Locator("(//button[text()='Approve'])[1]");
            rejectLeaveButton = page.Locator("//button[text()='Reject']");
            requestTabButton = page.Locator("//button[text()='Requests']");
            reimbursementSideBar = page.Locator("text=Reimbursement");
            searchEmployee = page.Locator("//input[@aria-label='EMP ID Filter Input']");
            scrollRight = "div.ag-body-horizontal-scroll-viewport";
            //scrollRight = "//div[contains(@class, 'horizontal') and @ref='eViewport']";
            actionHelper = new ActionHelper(_page);

        }


        private static DateTime GetNextWeekday(DateTime date)
        {
            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }
            return date;
        }

        public async Task ClickLeaveManagementSidebarAsync()
        {
            await Expect(leaveManagementSideBar).ToBeVisibleAsync(new() { Timeout = 5000 });
            await leaveManagementSideBar.ClickAsync();
        }

        public async Task ClickApplyLeaveButtonAsync()
        {
            await Expect(applyLeaveButton).ToBeVisibleAsync(new() { Timeout = 5000 });
            await applyLeaveButton.ClickAsync();

            if (await lopOkButton.Nth(0).IsVisibleAsync())
            {
                await lopOkButton.Nth(1).ClickAsync();
            }
        }
        public async Task EnterDatesAsync()
        {

            string fromDate = GetNextWeekday(DateTime.Now.AddDays(1)).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string toDate = GetNextWeekday(DateTime.Parse(fromDate).AddDays(1)).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            await Expect(fromDateInput).ToBeVisibleAsync();
            await Expect(toDateInput).ToBeVisibleAsync();

            await fromDateInput.FillAsync(fromDate);
            await toDateInput.FillAsync(toDate);
        }

        public async Task SelectLeadAsync(string lead)
        {
            await Expect(leadDropDown).ToBeVisibleAsync();
            await leadDropDown.SelectOptionAsync(new SelectOptionValue { Label = lead });
        }

        public async Task CheckLeaveTypeAsync()
        {
            await Expect(leaveTypeCheckbox).ToBeVisibleAsync();
            await leaveTypeCheckbox.CheckAsync();
        }

        public async Task EnterSubjectAndReasonAsync(string subject, string reason)
        {
            await Expect(subjectInput).ToBeVisibleAsync();
            await Expect(reasonInput).ToBeVisibleAsync();

            await subjectInput.FillAsync(subject);
            await reasonInput.FillAsync(reason);
        }

        public async Task SubmitFormAsync()
        {
            int retryCount = 0;
            int maxRetries = 4;

            while (retryCount < maxRetries)
            {
                await Expect(submitButton).ToBeVisibleAsync();
                await submitButton.ClickAsync();

                await _page.WaitForTimeoutAsync(1000);

                if (await leaveAppliedsuccessMessage.IsVisibleAsync())
                {
                    await Expect(leaveAppliedsuccessMessage).ToBeVisibleAsync();
                    Console.WriteLine("Leave applied successfully.");
                    return;
                }

                if (await leaveAlreadyExists.IsVisibleAsync())
                {
                    await Expect(leaveAlreadyExists).ToBeVisibleAsync();
                    Console.WriteLine("Leave request already exists.");
                    return;
                }

                retryCount++;
                Console.WriteLine($"Retry attempt: {retryCount}");
            }

            Assert.Fail("Submission failed: no success or error message displayed.");
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
