using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrganizationApp.Models.Repository
{
    public interface IRepository
    {
        Employee GetByID(int id);

        void Save(Employee employee);

        void Remove(Employee employee);

        bool Modify(int id, Employee employee);

        IQueryable<Employee> FilterByPosition(string position, IQueryable<Employee> employees);

        IQueryable<Employee> FilterByAge(int? minAge, int? maxAge, IQueryable<Employee> employees);

        IQueryable<Employee> FilterByExperience(int? minExperience, int? maxExperience, IQueryable<Employee> employees);

        IQueryable<Employee> GetEmployees();

        bool VerifyEmployeeData(Employee employee);

    }
}
