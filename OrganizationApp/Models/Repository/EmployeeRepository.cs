using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


namespace OrganizationApp.Models.Repository
{
    public class EmployeeRepository : IRepository, IDisposable
    {
        public EmployeeContext Context
        { get; private set; }

        public EmployeeRepository(EmployeeContext context)
        {
            Context = context;
        }


        public Employee GetByID(int id)
        {
            return Context.Employees.Find(id);
        }


        public void Save(Employee employee)
        {
            Context.Employees.Add(employee);
            Context.SaveChanges();
        }


        public void Remove(Employee employee)
        {
            Context.Employees.Remove(employee);
            Context.SaveChanges();
        }


        public bool Modify(int id, Employee employee)
        {   
            try
            {
                var emp = GetByID(id);

                if (emp == null)
                    return false;

                emp.CloneValues(employee);

                Context.Entry(emp).State = EntityState.Modified;

                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }


        private bool EmployeeExists(int id)
        {
            return Context.Employees.Count(e => e.ID == id) > 0;
        }


        public IQueryable<Employee> GetEmployees()
        {
            return Context.Employees;
        }


        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
                Context = null;
            }
        }


        public IQueryable<Employee> FilterByPosition(string position, IQueryable<Employee> employees)
        {
            if (string.IsNullOrEmpty(position))
                return employees;

            return employees.Where(x => x.Position.Equals(position, StringComparison.CurrentCultureIgnoreCase));
        }


        public IQueryable<Employee> FilterByAge(int? minAge, int? maxAge, IQueryable<Employee> employees)
        {
            // Сотрудники старше чем minAge и младше чем maxAge, то есть minAge и больше лет и строго меньше maxAge
            if (minAge.HasValue && maxAge.HasValue)
            {
                return employees.Where(x => x.Age >= minAge.Value && x.Age < maxAge.Value);
            }

            // Сотрудники старше чем minAge, то есть minAge и больше лет 
            if (minAge.HasValue)
            {
                return employees.Where(x => x.Age >= minAge.Value);
            }

            // Сотрудники младше чем maxAge, то есть строго меньше чем maxAge
            if (maxAge.HasValue)
            {
                return employees.Where(x => x.Age < maxAge.Value);
            }

            // Если фильтр по возрасту не задан, возвращаем всех сотрудников
            return employees;
        }


        public IQueryable<Employee> FilterByExperience(int? minExperience, int? maxExperience, IQueryable<Employee> employees)
        {
            // Сотрудники со стажем работы в компании более minExperience и менее maxExperience лет
            if (minExperience.HasValue && maxExperience.HasValue)
            {
                return employees.Where(x => x.Experience >= minExperience.Value && x.Experience < maxExperience.Value);
            }

            // Сотрудники со стажем работы в компании более minExperience лет
            if (minExperience.HasValue)
            {
                return employees.Where(x => x.Experience >= minExperience.Value);
            }

            // Сотрудники со стажем работы в компании менее maxExperience лет
            if (maxExperience.HasValue)
            {
                return employees.Where(x => x.Experience < maxExperience.Value);
            }

            return employees;
        }


        public bool VerifyEmployeeData(Employee employee)
        {
            return employee.Age > 14 && 
                employee.DateOfEmployment < DateTime.Now && 
                OnlyLetters(employee.FirstName) &&
                OnlyLetters(employee.LastName) &&
                OnlyLetters(employee.Patronymic);
        }


        private bool OnlyLetters(string str)
        {
            if (string.IsNullOrEmpty(str))
                return true;

            return str.All(c => char.IsLetter(c) || c == '-' || c == ' ');
        }
        
    }
}