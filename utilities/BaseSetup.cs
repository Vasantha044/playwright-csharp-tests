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

        private bool headless;
        private int slowMo;
        private int timeout;
        private int navigationTimeout;

        [SetUp]
        public async Task TestSetupAsync()
        {
            ReadRunSettingsParameters();

            PlaywrightInstance = await Playwright.CreateAsync();

            Browser = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless,
                SlowMo = slowMo
            });

            Context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = allureResultsPath
            });

            Context.SetDefaultTimeout(timeout);
            Context.SetDefaultNavigationTimeout(navigationTimeout);

            Page = await Context.NewPageAsync();
        }

        private void ReadRunSettingsParameters()
        {
            try
            {
                // Required: Ensure all values are set in .runsettings file
                string headlessParam = TestContext.Parameters["Headless"];
                string slowMoParam = TestContext.Parameters["SlowMo"];
                string timeoutParam = TestContext.Parameters["Timeout"];
                string navigationTimeoutParam = TestContext.Parameters["NavigationTimeout"];

                if (headlessParam == null || slowMoParam == null || timeoutParam == null || navigationTimeoutParam == null)
                    throw new ArgumentException("Missing one or more required parameters in .runsettings file.");

                headless = headlessParam.ToLower() == "true";
                slowMo = int.Parse(slowMoParam);
                timeout = int.Parse(timeoutParam);
                navigationTimeout = int.Parse(navigationTimeoutParam);

                Console.WriteLine("---- ✅ Loaded Parameters from RunSettings ----");
                Console.WriteLine($"Headless           = {headless}");
                Console.WriteLine($"SlowMo             = {slowMo}");
                Console.WriteLine($"Timeout            = {timeout}");
                Console.WriteLine($"NavigationTimeout  = {navigationTimeout}");
                Console.WriteLine("------------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading test parameters: {ex.Message}");
                throw;
            }
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

                        Console.WriteLine($"Screenshot taken: {screenshotPath}");
                        allure.AddAttachment("Failure Screenshot", "image/png", screenshotPath);
                    }
                    else
                    {
                        Console.WriteLine("Page already closed. Screenshot not taken.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Screenshot capture failed: {ex.Message}");
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
                Console.WriteLine($"Cleanup failed: {ex.Message}");
            }
        }
    }
}
