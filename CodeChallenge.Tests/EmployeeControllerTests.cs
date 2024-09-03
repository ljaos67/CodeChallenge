using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using CodeChallenge.Data;
using CodeChallenge.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using CodeCodeChallenge.Tests.Integration.Helpers;
using System.Text;
using System.Net;
using CodeCodeChallenge.Tests.Integration.Extensions;

//<summary>
// Employee controller tests, not including compensation
//<summary>
namespace CodeChallenge.Tests.Integration
{
    
    [TestClass]
    public class EmployeeControllerTests
    {
        //configure app for database seeding, configure client for api usage
        private static WebApplicationFactory<CodeChallenge.Config.App> _factory;
        private static HttpClient _httpClient;

        // Initialize Testing class
        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _factory = new WebApplicationFactory<CodeChallenge.Config.App>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddDbContext<EmployeeContext>(options =>
                            options.UseInMemoryDatabase("EmployeeDB"));
                    });
                });

            _httpClient = _factory.CreateClient();
        }
        // seed test data 
        [TestInitialize]
        public void SeedDatabase()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<EmployeeContext>();

                // Ensure the database is created
                db.Database.EnsureCreated();

                // Clear existing data
                db.Employees.RemoveRange(db.Employees);
                db.SaveChanges();

                // Seed data
                var john = new Employee
                {
                    EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                    FirstName = "John",
                    LastName = "Lennon",
                    Position = "Development Manager",
                    Department = "Engineering",
                    DirectReports = new List<Employee>
                    {
                        new Employee
                        {
                            EmployeeId = "b7839309-3348-463b-a7e3-5de1c168beb3",
                            FirstName = "Paul",
                            LastName = "McCartney",
                            Position = "Developer I",
                            Department = "Engineering"
                        },
                        new Employee
                        {
                            EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                            FirstName = "Ringo",
                            LastName = "Starr",
                            Position = "Developer V",
                            Department = "Engineering",
                            DirectReports = new List<Employee>
                            {
                                new Employee
                                {
                                    EmployeeId = "62c1084e-6e34-4630-93fd-9153afb65309",
                                    FirstName = "Pete",
                                    LastName = "Best",
                                    Position = "Developer II",
                                    Department = "Engineering"
                                },
                                new Employee
                                {
                                    EmployeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c",
                                    FirstName = "George",
                                    LastName = "Harrison",
                                    Position = "Developer III",
                                    Department = "Engineering"
                                }
                            }
                        }
                    }
                };

                db.Employees.Add(john);
                db.SaveChanges();
            }
        }
        // test case: Retrieve reporting number for employee with multi-level reports, should return 4.  Tests reporting structure endpoint.
        [TestMethod]
        public async Task GetReportingStructure_ForJohnLennon_ReturnsCorrectNumberOfReports()
        {
            // Act
            var response = await _httpClient.GetAsync("api/employee/16a596ae-edd3-4847-99fe-c4518e82c86f/reportingStructure");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var reportingStructure = JsonConvert.DeserializeObject<ReportingStructure>(content);
            Assert.IsNotNull(reportingStructure);
            Assert.AreEqual(4, reportingStructure.NumberOfReports);
            Assert.AreEqual("16a596ae-edd3-4847-99fe-c4518e82c86f", reportingStructure.Employee.EmployeeId);
        }

        // cleans up client and seed data functions
        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _factory.Dispose();
        }

        // Employee create
        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }
        // testing employee reporting number for leaf, employee with no reports. Should return 0. Tests reporting structure endpoint.
        [TestMethod]
        public void GetReportingStructure_ForPaulMcCartney_ReturnsCorrectNumberOfReports()
        {
            // Act
            var response = _httpClient.GetAsync("api/employee/b7839309-3348-463b-a7e3-5de1c168beb3/reportingStructure").Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var reportingStructure = JsonConvert.DeserializeObject<ReportingStructure>(response.Content.ReadAsStringAsync().Result);
            Assert.IsNotNull(reportingStructure);
            Assert.AreEqual(0, reportingStructure.NumberOfReports);
            Assert.AreEqual("b7839309-3348-463b-a7e3-5de1c168beb3", reportingStructure.Employee.EmployeeId);
        }
        // testing employee controller retrieve employee by id endpoint
        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
            Assert.AreNotEqual(null, employee.DirectReports);
        }

        // testing employee controller update employee endpoint
        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }
        // testing employee controller retrieve employee by id endpoint when employee not found
        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        // testing employee reporting number for employee that has upper and lower level reports. Should return 2. Tests reporting structure endpoint.
        [TestMethod]
        public void GetReportingStructure_ForRingoStar_ReturnsCorrectNumberOfReports()
        {
            // Act
            var response = _httpClient.GetAsync("api/employee/03aa1462-ffa9-4978-901b-7c001562cf6f/reportingStructure").Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var reportingStructure = JsonConvert.DeserializeObject<ReportingStructure>(response.Content.ReadAsStringAsync().Result);
            Assert.IsNotNull(reportingStructure);
            Assert.AreEqual(2, reportingStructure.NumberOfReports);

        }
    }
}
