using Bogus;
using PlaywrightTests.Utilities;
using System.Text.RegularExpressions;

public static class EmployeeFaker
{
    private static string RemoveSpecialCharacters(string input)
    {
        return Regex.Replace(input, "[^a-zA-Z]", ""); // Removes anything that's not a-z or A-Z
    }

    public static Employee GenerateTestEmployee()
    {
        var faker = new Faker<Employee>()
            .RuleFor(e => e.FirstName, f => "Test" + RemoveSpecialCharacters(f.Name.FirstName()))
            .RuleFor(e => e.LastName, f => RemoveSpecialCharacters(f.Name.LastName()))
            .RuleFor(e => e.Email, (f, e) => f.Internet.Email(e.FirstName, e.LastName))
            .RuleFor(e => e.Department, f => RemoveSpecialCharacters(f.Random.Word()))
            .RuleFor(e => e.Mobile, f => f.Phone.PhoneNumber("##########"))
            .RuleFor(e => e.Designation, f => RemoveSpecialCharacters(f.Random.Word()))
            .RuleFor(e => e.Location, f => RemoveSpecialCharacters(f.Address.City()))
            .RuleFor(e => e.EmployeeID, f => f.Random.Number(1000, 9999).ToString())
            .RuleFor(e => e.Password, f => f.Internet.Password());

        return faker.Generate();
    }
}
