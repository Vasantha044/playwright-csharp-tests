using System.Threading.Tasks;
using PlaywrightTests.Pages;
using PlaywrightTests.Utilities;
using Microsoft.Playwright;

namespace PlaywrightTests.Tests
{
    public class AddEmployeeHelper
    {
        private readonly AddEmployee _addEmployeePage;

        public AddEmployeeHelper(IPage page)
        {
            _addEmployeePage = new AddEmployee(page);
        }

        public async Task<Employee> CreateOrAddEmployeeInUrbuddi(string role)
        {
            var employee = EmployeeFaker.GenerateTestEmployee();

            await _addEmployeePage.EnterFirstNameLastNameAsync(
                employee.FirstName,
                employee.LastName,
                employee.EmployeeID,
                employee.Email
            );

            await _addEmployeePage.SelectDropdownValuesAsync(
                role,                        // Role
                "PG",                        // Qualification
                "twl4admin@gmail.com",// ReportingTo
                "Male",                      // Gender
                "O+"                         // Blood Group
            );

            await _addEmployeePage.SelectDOBAndDateOfJoiningAsync(
                "2000-12-13", "2023-08-16"
            );

            await _addEmployeePage.EnterMobileAndDesiginationAndLocationAndDepartmentValuesAsync(
                employee.Department,
                employee.Password,
                employee.Mobile,
                employee.Location,
                employee.Designation
            );

            await _addEmployeePage.ClickAddButtonAsync();

            return employee;
        }
    }
}
