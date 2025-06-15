using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace PlaywrightTests.Utilities
{
    public static class ConfigManager
    {
        private static IConfigurationRoot Configuration = null!;

        public static AppSettings Settings { get; private set; } = null!;

        static ConfigManager()
        {
            LoadConfiguration();
        }



        private static void LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Safe for test execution paths
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<SecretsMarker>();

            Configuration = builder.Build();

            Settings = new AppSettings();
            Configuration.Bind(Settings);
        }

        public static void Reload()
        {
            LoadConfiguration();
        }

        public static T GetSection<T>(string sectionName) where T : new()
        {
            var section = Configuration.GetSection(sectionName);
            if (section.Exists())
            {
                return section.Get<T>() ?? new T();
            }
            return new T();
        }

        public static string GetValue(string key, string defaultValue = "")
        {
            var val = Configuration[key];
            return string.IsNullOrEmpty(val) ? defaultValue : val;
        }
    }

    public class AppSettings
    {
        public UrlsConfig? Urls { get; set; }
        public Credentials? Credentials { get; set; }
        public PlaywrightSettings? PlaywrightSettings { get; set; }
        public SystemCredentials? SystemCredentials { get; set; }
    }

    public class UrlsConfig
    {
        public string? BaseUrl { get; set; }
        public string? LoginUrl { get; set; }
        public string? DashboardUrl { get; set; }
    }

    public class Credentials
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? InvalidUsername { get; set; }
        public string? InvalidPassword { get; set; }
    }

    public class PlaywrightSettings
    {
        public bool Headless { get; set; } = true;
        public int DefaultTimeoutMs { get; set; } = 15000;
        public int NavigationTimeoutMs { get; set; } = 30000;
    }

    public class Employee
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string Mobile { get; set; } = null!;
        public string Designation { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string EmployeeID { get; set; } = null!;
        public string Password { get; set; } = null!;

    }

    public class SystemCredentials // These are secreat 
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}
}
