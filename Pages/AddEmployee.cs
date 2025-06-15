using System.Threading.Tasks;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;

namespace PlaywrightTests.Pages
{
    public class AddEmployee
    {
        private readonly IPage _page;

        public AddEmployee(IPage page)
        {
            _page = page;
        }

        private string employeeSec => "(//div[@class='nav-item-icon']/following::p[text()='Employees'])[1]";
        private string addEmployeebtn => "//button[text()='Add Employee']";

        private ILocator RoleDropdown => _page.Locator("select[name='role']");
        private ILocator QualificationDropdown => _page.Locator("select[name='qualifications']");
        private ILocator ReportingDropdown => _page.Locator("select[name='reportingTo']");
        private ILocator GenderDropdown => _page.Locator("select[name='gender']");
        private ILocator BloodGroupDropdown => _page.Locator("select[name='bloodGroup']");
        private ILocator AddButton => _page.Locator("//button[text()='Add']");

        private async Task EnterFieldAsync(string label, string value)
        {
            var locator = $"//label[text()='{label}']/preceding-sibling::input";
            await _page.Locator(locator).FillAsync(value);
        }

        public async Task EnterFirstNameLastNameAsync(string firstName, string lastName, string employeeId, string email)
        {
            await _page.Locator(employeeSec).ClickAsync();
            await _page.Locator(addEmployeebtn).ClickAsync();

            await EnterFieldAsync("First Name*", firstName);
            await EnterFieldAsync("Last Name*", lastName);
            await EnterFieldAsync("Employee ID*", employeeId);
            await EnterFieldAsync("Email*", email);
        }


        public async Task SelectDropdownValuesAsync(string role, string qualification, string reportingTo, string gender, string bloodGroup)
        {
            await RoleDropdown.SelectOptionAsync(new SelectOptionValue { Label = role });
            await QualificationDropdown.SelectOptionAsync(new SelectOptionValue { Label = qualification });
            await ReportingDropdown.SelectOptionAsync(new SelectOptionValue { Label = reportingTo });
            await GenderDropdown.SelectOptionAsync(new SelectOptionValue { Label = gender });
            await BloodGroupDropdown.SelectOptionAsync(new SelectOptionValue { Label = bloodGroup });
        }

        public async Task SelectDOBAndDateOfJoiningAsync(string dob, string doj)
        {
            await _page.Locator("input[name='dob']").FillAsync(dob);
            await _page.Locator("input[name='joiningDate']").FillAsync(doj);
        }

        public async Task EnterMobileAndDesiginationAndLocationAndDepartmentValuesAsync(string department, string password, string mobile, string location, string designation)
        {
            await EnterFieldAsync("Department*", department);
            await EnterFieldAsync("Mobile No*", mobile);
            await EnterFieldAsync("Location*", location);
            await EnterFieldAsync("Designation*", designation);
            await EnterFieldAsync("Password*", password);
            await EnterFieldAsync("Salary*", "12345");
        }


        public async Task ClickAddButtonAsync()
        {
            await AddButton.ClickAsync();
        }
        
      
    }
}
