using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
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

        //
        // GET: api/Employees
        public async Task<IEnumerable<Employee>> GetEmployees([FromUri]EmployeeFilterParams filterParams)
        {
            return await Repository.GetEmployeesAsync(filterParams);
        }

        //      
        // PUT: api/Employees/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutEmployee(int id, Employee employee)
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

            // Изменяем поля сотрудника с идентификатором id
            var result = await Repository.ModifyAsync(id, employee);

            if (!result)
            {
                return NotFound();
            }
            
            return StatusCode(HttpStatusCode.NoContent);
        }

        //
        // POST: api/Employees
        [ResponseType(typeof(Employee))]
        public async Task<IHttpActionResult> PostEmployee(Employee employee)
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

            await Repository.SaveAsync(employee);
            
            return CreatedAtRoute("DefaultApi", new { id = employee.ID }, employee);
        }

        //
        // DELETE: api/Employees/5
        [ResponseType(typeof(Employee))]
        public async Task<IHttpActionResult> DeleteEmployee(int id)
        {
            var employee = await Repository.GetByIDAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            await Repository.RemoveAsync(employee);

            return Ok(employee);
        }
    }
}