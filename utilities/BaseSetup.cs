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

        private string GetRunSetting(string key, string defaultValue)
        {
            var val = TestContext.Parameters.Get(key);
            return string.IsNullOrEmpty(val) ? defaultValue : val;
        }

        private int GetRetryCount()
        {
            var retryStr = TestContext.Parameters.Get("Retries");
            return int.TryParse(retryStr, out int retry) ? retry : 0;
        }

        protected async Task RetryAsync(Func<Task> testLogic)
        {
            int retries = GetRetryCount();
            int attempt = 0;
            Exception? lastException = null;

            while (attempt <= retries)
            {
                try
                {
                    await testLogic();
                    return; // ✅ success
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    attempt++;
                    if (attempt > retries)
                    {
                        throw lastException ?? new Exception("Test failed after all retries.");
                    }
                }
            }
        }

        [OneTimeSetUp]
        public void GlobalSetup()
        {
          //  DeleteOldResults();
            //CreateResultsDirectory();
        }

        [SetUp]
        public async Task TestSetupAsync()
        {
            PlaywrightInstance = await Playwright.CreateAsync();

            bool headless = bool.Parse(GetRunSetting("Headless", "false"));
            int slowMo = int.Parse(GetRunSetting("SlowMo", "1000"));
            int defaultTimeoutMs = int.Parse(GetRunSetting("Timeout", "45000"));
            int navigationTimeoutMs = int.Parse(GetRunSetting("NavigationTimeout", "60000"));

            Browser = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless,
                SlowMo = slowMo
            });

            Context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = allureResultsPath
            });

            Context.SetDefaultTimeout(defaultTimeoutMs);
            Context.SetDefaultNavigationTimeout(navigationTimeoutMs);

            Page = await Context.NewPageAsync();
            if (Page == null)
                throw new Exception("Failed to create a new page.");
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;

            if (testStatus == TestStatus.Failed)
            {
                try
                {
                    if (Page != null)
                    {
                        // Capture screenshot
                        try
                        {
                            var screenshotPath = Path.Combine(allureResultsPath, $"{TestContext.CurrentContext.Test.MethodName}.png");
                            await Page.ScreenshotAsync(new PageScreenshotOptions
                            {
                                Path = screenshotPath,
                                FullPage = true
                            });
                            allure.AddAttachment("Failure Screenshot", "image/png", screenshotPath);
                        }
                        catch (PlaywrightException e)
                        {
                            Console.WriteLine($"Could not take screenshot: {e.Message}");
                        }

                        // Capture video
                        try
                        {
                            var videoPath = await Page.Video?.PathAsync();
                            if (!string.IsNullOrEmpty(videoPath) && File.Exists(videoPath))
                            {
                                var destPath = Path.Combine(allureResultsPath, $"{TestContext.CurrentContext.Test.MethodName}.webm");
                                File.Move(videoPath, destPath, overwrite: true);
                                allure.AddAttachment("Test Video", "video/webm", destPath);
                            }
                        }
                        catch (PlaywrightException e)
                        {
                            Console.WriteLine($"Could not access video: {e.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to capture failure artifacts: " + ex.Message);
                }
            }

            try
            {
                if (Browser != null)
                    await Browser.CloseAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error closing browser: {e.Message}");
            }

            PlaywrightInstance?.Dispose();
        }

        private void DeleteOldResults()
        {
            if (Directory.Exists(allureResultsPath))
            {
                try
                {
                    Directory.Delete(allureResultsPath, recursive: true);
                    Console.WriteLine($"Deleted old Allure results at: {allureResultsPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete Allure results directory: {ex.Message}");
                }
            }
        }

        private void CreateResultsDirectory()
        {
            if (!Directory.Exists(allureResultsPath))
            {
                Directory.CreateDirectory(allureResultsPath);
                Console.WriteLine($"Created fresh Allure results directory at: {allureResultsPath}");
            }
        }
    }
}
