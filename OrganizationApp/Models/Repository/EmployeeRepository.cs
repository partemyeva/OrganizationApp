using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using LinqKit;

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
        

        public async Task<Employee> GetByIDAsync(int id)
        {
            return await Context.Employees.FindAsync(id);
        }


        public async Task SaveAsync(Employee employee)
        {
            Context.Employees.Add(employee);

            await Context.SaveChangesAsync();
        }


        public async Task RemoveAsync(Employee employee)
        {
            Context.Employees.Remove(employee);

            await Context.SaveChangesAsync();
        }


        public async Task<bool> ModifyAsync(int id, Employee employee)
        {   
            try
            {
                // Пытаемся получить сотрудника по идентификатору
                var emp = await GetByIDAsync(id);
                
                if (emp == null)
                    return false;

                // Копируем значения полей
                emp.CloneValues(employee);

                Context.Entry(emp).State = EntityState.Modified;

                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var employeeExists = await EmployeeExistsAsync(id);

                if (!employeeExists)
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


        private async Task<bool> EmployeeExistsAsync(int id)
        {
            return await Context.Employees.CountAsync(e => e.ID == id) > 0;
        }


        public async Task<IEnumerable<Employee>> GetEmployeesAsync(EmployeeFilterParams fp)
        {

            var predicate = PredicateBuilder.New<Employee>(true);

            //
            // Задан фильтр по должности
            if (!string.IsNullOrEmpty(fp.Position))
            {
                predicate.And(x => x.Position.Equals(fp.Position, StringComparison.CurrentCultureIgnoreCase));
            }

            //
            // Задан фильтр по возрасту
            // Сотрудники старше чем minAge и младше чем maxAge, то есть minAge и больше лет и строго меньше maxAge
            if (fp.MinAge.HasValue && fp.MaxAge.HasValue)
            {
                predicate.And(x => x.Age >= fp.MinAge.Value && x.Age < fp.MaxAge.Value);
            }
            // Сотрудники старше чем minAge, то есть minAge и больше лет 
            else if (fp.MinAge.HasValue)
            {
                predicate.And(x => x.Age >= fp.MinAge.Value);
            }
            // Сотрудники младше чем maxAge, то есть строго меньше чем maxAge
            else if (fp.MaxAge.HasValue)
            {
                predicate.And(x => x.Age < fp.MaxAge.Value);
            }

            //
            // Задан фильтр по стажу работы в компании
            // Сотрудники со стажем работы в компании более minExperience и менее maxExperience лет
            if (fp.MinExperience.HasValue && fp.MaxExperience.HasValue)
            {
                predicate.And(x => x.Experience >= fp.MinExperience.Value && x.Experience < fp.MaxExperience.Value);
            }
            // Сотрудники со стажем работы в компании более minExperience лет
            else if (fp.MinExperience.HasValue)
            {
                predicate.And(x => x.Experience >= fp.MinExperience.Value);
            }
            // Сотрудники со стажем работы в компании менее maxExperience лет
            else if (fp.MaxExperience.HasValue)
            {
                predicate.And(x => x.Experience < fp.MaxExperience.Value);
            }

            return await Context.Employees.Where(predicate).ToListAsync();
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Context != null)
                {
                    Context.Dispose();
                    Context = null;
                }
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public bool VerifyEmployeeData(Employee employee)
        {
            return employee.Age > 14 && 
                employee.DateOfEmployment < DateTime.Now && 
                LettersOnly(employee.FirstName) &&
                LettersOnly(employee.LastName) &&
                LettersOnly(employee.Patronymic);
        }


        private bool LettersOnly(string str)
        {
            if (string.IsNullOrEmpty(str))
                return true;

            return str.All(c => char.IsLetter(c) || c == '-' || c == ' ');
        }
        
    }
}