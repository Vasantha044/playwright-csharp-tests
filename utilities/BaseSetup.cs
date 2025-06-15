using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;
using Allure.Commons;
using NUnit.Framework.Interfaces;

namespace PlaywrightTests
{
    public class BaseSetup
    {
        protected IBrowser Browser;
        protected IBrowserContext Context;
        protected IPage Page;
        protected IPlaywright PlaywrightInstance;

        private readonly string allureResultsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../allure-results");
        private readonly AllureLifecycle allure = AllureLifecycle.Instance;

        [SetUp]
        public async Task TestSetupAsync()
        {
            PlaywrightInstance = await Playwright.CreateAsync();

            Browser = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 1000
            });

            Context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = allureResultsPath
            });

            Context.SetDefaultTimeout(45000);
            Context.SetDefaultNavigationTimeout(60000);

            Page = await Context.NewPageAsync();
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
            var testName = TestContext.CurrentContext.Test.MethodName;

            if (testStatus == TestStatus.Failed && Page != null)
            {
                try
                {
                    Directory.CreateDirectory(allureResultsPath);
                    string screenshotPath = Path.Combine(allureResultsPath, $"{testName}.png");

                    if (!Page.IsClosed)
                    {
                        await Page.ScreenshotAsync(new PageScreenshotOptions
                        {
                            Path = screenshotPath,
                            FullPage = true
                        });

                        Console.WriteLine($"✅ Screenshot taken: {screenshotPath}");

                        // ✅ Attach screenshot using the valid overload
                        allure.AddAttachment("Failure Screenshot", "image/png", screenshotPath);
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Page already closed. Screenshot not taken.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Screenshot capture failed: {ex.Message}");
                }
            }

            try
            {
                if (Context != null)
                    await Context.CloseAsync();

                if (Browser != null)
                    await Browser.CloseAsync();

                PlaywrightInstance?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Cleanup failed: {ex.Message}");
            }
        }
    }
}
