using System;
using NUnit.Framework;
using Microsoft.Playwright;
using System.Threading.Tasks;
using PlaywrightTests.Utilities;
using static Microsoft.Playwright.Assertions;  

namespace PlaywrightTests.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;

        public LoginPage(IPage page)
        {
            _page = page;
        }

        public async Task LoginWithValidOrInvalidCred(string username, string password)
        {
            string loginButtonLoc = "//button[text()='Login']";
            await _page.GotoAsync(ConfigManager.Settings.Urls?.BaseUrl!);
            await _page.Locator("#userEmail").FillAsync(username);
            await _page.Locator("#userPassword").FillAsync(password);
            await _page.Locator(loginButtonLoc).ClickAsync();

            var dashboardLocator = _page.Locator("(//p[text()='Dashboard'])[2]");
            var invalidCredLocator = _page.Locator("//p[text()='Invalid credentials']");

            // Wait for either success or failure condition
            await _page.WaitForTimeoutAsync(1000); // Optional small delay if needed

            if (await dashboardLocator.IsVisibleAsync())
            {
                await Expect(dashboardLocator).ToBeVisibleAsync(new() { Timeout = 5000 });
                TestContext.WriteLine("Login successful. Dashboard is visible.");
            }
            else if (await invalidCredLocator.IsVisibleAsync())
            {
                await Expect(invalidCredLocator).ToBeVisibleAsync(new() { Timeout = 5000 });
                TestContext.WriteLine("Login failed. Invalid credentials message is visible.");
            }
            else
            {
                Assert.Fail("Login result could not be determined — neither Dashboard nor Invalid Credentials message appeared.");
            }
        }


          public  async Task PrintHolidayCountAsync()
        {
            await _page.Locator("ul.events-list div.holiday-display-card").First.WaitForAsync(new() { Timeout = 5000 });
            int count = await _page.Locator("ul.events-list div.holiday-display-card").CountAsync();
            Console.WriteLine($"Total holidays displayed: {count}");
        }
    }
}
