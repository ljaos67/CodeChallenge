using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;
//<summary>
// An employee controller class to Create, Get, and update employees. Includes API endpoints for retrieving employee reporting structure,
// as well creating or retrieving their compensation details. 
//<summary>
namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;
        // summary: EmployeeController constructor, initializes the logger and employee service.
        // inputs: logger -- logger object for logging activities,employeeService -- service layer object for managing employee data
        // returns: none
        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        // summary: Creates a new employee.
        // inputs: employee -- the employee object to be created
        // returns: A CreatedAtRouteResult indicating the employee was successfully created
        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        // summary: Retrieves an employee by their ID.
        // inputs: id -- the ID of the employee to retrieve
        // returns: An OkObjectResult containing the employee data if found, otherwise a NotFoundResult
        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }
        // summary: Updates an existing employee with new data.
        // inputs: id -- the ID of the employee to update, newEmployee -- the new employee data to replace the existing data
        // returns: An OkObjectResult containing the updated employee data if successful, otherwise a NotFoundResult
        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        // summary: Retrieves the reporting structure for a given employee.
        // inputs: id -- the ID of the employee to retrieve the reporting structure for
        // returns: An OkObjectResult containing the reporting structure data if found, otherwise a NotFoundResult
        [HttpGet("{id}/reportingStructure", Name = "getReportingStructure")]
        public IActionResult GetReportingStructure(string id)
        {
            _logger.LogDebug($"Received reporting structure get request for employee '{id}'");

            var reportingStructure = _employeeService.GetReportingStructure(id);

            if (reportingStructure == null)
                return NotFound();

            return Ok(reportingStructure);
        }
        // summary: Creates a new compensation entry for an employee.
        // inputs: compensation -- the compensation object to be created
        // returns: A CreatedAtRouteResult indicating the compensation was successfully created, or a BadRequestResult if the input data is invalid
        [HttpPost("compensation")]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            if (compensation == null || compensation.Employee == null)
            {
                return BadRequest("Compensation and Employee information must be provided.");
            }

            try
            {
                _employeeService.CreateCompensation(compensation);
                return CreatedAtRoute("getCompensationByEmployeeId", new { id = compensation.Employee.EmployeeId }, compensation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateCompensation: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the compensation.");
            }
        }

        // summary: Retrieves the compensation details for a given employee.
        // inputs: id -- the ID of the employee to retrieve the compensation for
        // returns: An OkObjectResult containing the compensation data if found, otherwise a NotFoundResult
        [HttpGet("{id}/compensation", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(string id)
        {
            _logger.LogDebug($"Received compensation get request for employee '{id}'");

            var compensation = _employeeService.GetCompensationByEmployeeId(id);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }


    }
}
