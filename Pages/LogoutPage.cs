using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    public class LogoutPage
    {
        private readonly IPage page;
        private readonly ILocator logoutButton;
        private readonly ILocator yesButton;
        private readonly ILocator loginHomePage;

        public LogoutPage(IPage page)
        {
            this.page = page;
            logoutButton = page.Locator("//*[text()='Logout']");
            yesButton = page.Locator("//button[text()='Yes']");
            loginHomePage = page.Locator("//*[@class='welcomeMessage']");
        }

        public async Task LogoutAsync()
        {
            await logoutButton.ClickAsync();
            await yesButton.ClickAsync();
        }

        public async Task VerifyLoggedOutAsync()
        {
            await loginHomePage.WaitForAsync(
                new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 }
            );
        }
    }
}