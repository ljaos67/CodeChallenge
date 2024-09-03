using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Data;
using CodeChallenge.Models;
using CodeChallenge.Repositories;
using CodeChallenge.Services;
using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

//<summary>
// Employee controller tests for compensation endpoints
//<summary>

namespace CodeChallenge.Tests.Integration
{
   
    [TestClass]
    public class CompensationControllerTests
    {
        // initializes httpClient for requests, WebApplicationFactory object for seeding database (for testing purposes)
        private static WebApplicationFactory<CodeChallenge.Config.App> _factory;
        private static HttpClient _httpClient;

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

                        // Ensure CompensationRepository and any other necessary services are registered
                        services.AddScoped<ICompensationRepository, CompensationRepository>();
                        services.AddScoped<IEmployeeRepository, EmployeeRespository>();
                        services.AddScoped<IEmployeeService, EmployeeService>();
                    });
                });

            _httpClient = _factory.CreateClient();
        }
        // Seed database for test
        [TestInitialize]
        public void SeedDatabase()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<EmployeeContext>();

                db.Database.EnsureDeleted();  // Clear the in-memory database
                db.Database.EnsureCreated();  // Recreate the database

                var employee = new Employee
                {
                    EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                    FirstName = "John",
                    LastName = "Lennon",
                    Position = "Development Manager",
                    Department = "Engineering"
                };

                db.Employees.Add(employee);
                db.SaveChanges();

                var compensation = new Compensation
                {
                    CompensationId = Guid.NewGuid().ToString(), // Generate a unique ID
                    Employee = employee, // Link to the employee
                    Salary = 120000,
                    EffectiveDate = DateTime.Now
                };

                db.Compensations.Add(compensation);
                db.SaveChanges();
            }
        }
        // test case: Use create compensation endopint to create an employee compensation entry
        [TestMethod]
        public void CreateCompensation_ReturnsCreated()
        {
            // Arrange
            var employee = new Employee
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                FirstName = "John",
                LastName = "Lennon",
                Position = "Development Manager",
                Department = "Engineering"
            };

            // Seed the database with the employee first to ensure it's tracked by the context
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<EmployeeContext>();

                if (!db.Employees.Any(e => e.EmployeeId == employee.EmployeeId))
                {
                    db.Employees.Add(employee);
                    db.SaveChanges();
                }
            }

            var compensation = new Compensation
            {
                Employee = employee,
                Salary = 120000,
                EffectiveDate = DateTime.Now
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate.ToString("yyyy-MM-dd"), newCompensation.EffectiveDate.ToString("yyyy-MM-dd"));
            Assert.AreEqual(compensation.Employee.EmployeeId, newCompensation.Employee.EmployeeId);
        }
        
        // test case: use get compensation endpoint to test compensation retrieval by id
        [TestMethod]
        public void GetCompensationById_ReturnsOk()
        {
            // Act
            var response = _httpClient.GetAsync("api/employee/16a596ae-edd3-4847-99fe-c4518e82c86f/compensation").Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var compensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(compensation);
            Assert.AreEqual(120000, compensation.Salary);
            Assert.AreEqual("16a596ae-edd3-4847-99fe-c4518e82c86f", compensation.Employee.EmployeeId);
        }



        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _factory.Dispose();
        }
    }
}
