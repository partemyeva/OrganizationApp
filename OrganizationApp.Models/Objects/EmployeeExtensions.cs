using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrganizationApp.Models
{
    public static class EmployeeExtensions
    {
        /// <summary>
        /// Копирует значения свойств объекта <paramref name="employee2"/> в свойства объекта <paramref name="employee1"/>
        /// </summary>
        /// <param name="employee1"></param>
        /// <param name="employee2"></param>
        public static void CloneValues(this Employee employee1, Employee employee2)
        {
            employee1.ID = employee2.ID;
            employee1.FirstName = employee2.FirstName;
            employee1.LastName = employee2.LastName;
            employee1.Patronymic = employee2.Patronymic;
            employee1.DateOfBirth = employee2.DateOfBirth;
            employee1.Position = employee2.Position;
            employee1.DateOfEmployment = employee2.DateOfEmployment;
        }
    }
}