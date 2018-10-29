using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using OrganizationApp.Models;
using OrganizationApp.Models.Repository;


namespace OrganizationApp.Controllers
{
    public class EmployeesController : ApiController
    {
        public IRepository Repository
        { get; private set; }

        public EmployeesController(IRepository r)
        {
            Repository = r;
        }

        // GET: api/Employees
        public IQueryable<Employee> GetEmployees([FromUri]EmployeeFilterParams filterParams)
        {
            var employees = Repository.GetEmployees();

            // Фильтруем по должности
            employees = Repository.FilterByPosition(filterParams.Position, employees);

            // Фильтруем по возрасту
            employees = Repository.FilterByAge(filterParams.MinAge, filterParams.MaxAge, employees);

            // Фильтруем по опыту работы
            employees = Repository.FilterByExperience(filterParams.MinExperience, filterParams.MaxExperience, employees);

            return employees;
        }

              
        // PUT: api/Employees/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmployee(int id, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.ID)
            {
                return BadRequest();
            }

            // Проверяем, что для сотрудника пришли валидные данные
            if (!Repository.VerifyEmployeeData(employee))
            {
                return BadRequest();
            }

            var result = Repository.Modify(id, employee);

            if (!result)
            {
                return NotFound();
            }
            
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Employees
        [ResponseType(typeof(Employee))]
        public IHttpActionResult PostEmployee(Employee employee)
        {
            if (!ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }

            // Проверяем, что для сотрудника пришли валидные данные
            if (!Repository.VerifyEmployeeData(employee))
            {
                return BadRequest();
            }

            Repository.Save(employee);
            
            return CreatedAtRoute("DefaultApi", new { id = employee.ID }, employee);
        }

        // DELETE: api/Employees/5
        [ResponseType(typeof(Employee))]
        public IHttpActionResult DeleteEmployee(int id)
        {
            var employee = Repository.GetByID(id);
            if (employee == null)
            {
                return NotFound();
            }

            Repository.Remove(employee);

            return Ok(employee);
        }
    }
}