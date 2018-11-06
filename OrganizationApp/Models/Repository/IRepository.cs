using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationApp.Models.Repository
{
    public interface IRepository
    {
        Task<Employee> GetByIDAsync(int id);

        Task<IEnumerable<Employee>> GetEmployeesAsync(EmployeeFilterParams fp);

        Task SaveAsync(Employee employee);

        Task RemoveAsync(Employee employee);

        Task<bool> ModifyAsync(int id, Employee employee);

        bool VerifyEmployeeData(Employee employee);

    }
}
