using NUnit.Framework;
using System;
using System.IO;
using Allure.Commons;

namespace PlaywrightTests.Utilities
{
    [SetUpFixture]
    public class AllureResultsManager
    {
        private readonly string allureResultsPath;

        public AllureResultsManager()
        {
            allureResultsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../allure-results");
        }

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            DeleteOldResults();
            CreateResultsDirectory();
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
            else
            {
                Console.WriteLine($"No existing Allure results directory at: {allureResultsPath}");
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

        public static string GetAllureResultsPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../allure-results");
        }

        // **Add this TearDown method here inside the same class**
        [TearDown]
        public void InjectDefaultLabels()
        {
            var lifecycle = AllureLifecycle.Instance;
            var testUuid = TestContext.CurrentContext.Test.ID;

            Console.WriteLine($"Injecting Allure labels for: {TestContext.CurrentContext.Test.Name}");

            lifecycle.UpdateTestCase(testUuid, testResult =>
            {
                if (!testResult.labels.Exists(l => l.name == "severity"))
                    testResult.labels.Add(Label.Severity(SeverityLevel.normal));
                if (!testResult.labels.Exists(l => l.name == "owner"))
                    testResult.labels.Add(Label.Owner("vasantha"));
                if (!testResult.labels.Exists(l => l.name == "tag"))
                    testResult.labels.Add(Label.Tag("Smoke"));
                if (!testResult.labels.Exists(l => l.name == "suite"))
                    testResult.labels.Add(Label.Suite("Login Suite"));

                testResult.labels.Add(Label.TestClass(TestContext.CurrentContext.Test.ClassName));
                testResult.labels.Add(Label.TestMethod(TestContext.CurrentContext.Test.MethodName));
            });
        }
    }
}
