using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace OrganizationApp.Models.Repository
{
    public class EmployeeDbInitializer : DropCreateDatabaseAlways<EmployeeContext>
    {
        protected override void Seed(EmployeeContext db)
        {
            db.Employees.Add(new Employee()
            {
                FirstName = "Алексей",
                Patronymic = "Петрович",
                LastName = "Беребердин",
                DateOfBirth = new DateTime(1987, 1, 27),
                Position = "Технический директор",
                DateOfEmployment = new DateTime(2010, 12, 1)
            });

            db.Employees.Add(new Employee()
            {
                FirstName = "Лев",
                Patronymic = "Алексеевич",
                LastName = "Назаров",
                DateOfBirth = new DateTime(1982, 5, 17),
                Position = "Ведущий разработчик",
                DateOfEmployment = new DateTime(2011, 1, 10)
            });

            db.Employees.Add(new Employee()
            {
                FirstName = "Тимофей",
                Patronymic = "Павлович",
                LastName = "Статенин",
                DateOfBirth = new DateTime(1984, 8, 11),
                Position = "Программист",
                DateOfEmployment = new DateTime(2011, 4, 1)
            });

            db.Employees.Add(new Employee()
            {
                FirstName = "Анастасия",
                Patronymic = "Дмитриевна",
                LastName = "Сурина",
                DateOfBirth = new DateTime(1985, 6, 4),
                Position = "Аналитик",
                DateOfEmployment = new DateTime(2013, 9, 30)
            });

            db.Employees.Add(new Employee()
            {
                FirstName = "Ирина",
                Patronymic = "Игоревна",
                LastName = "Бондаренко",
                DateOfBirth = new DateTime(1984, 2, 4),
                Position = "Тестировщик ПО",
                DateOfEmployment = new DateTime(2014, 3, 30)
            });

            db.Employees.Add(new Employee()
            {
                FirstName = "Макар",
                Patronymic = "Станиславович",
                LastName = "Романов",
                DateOfBirth = new DateTime(1986, 12, 30),
                Position = "Программист",
                DateOfEmployment = new DateTime(2015, 10, 30)
            });

            db.Employees.Add(new Employee()
            {
                FirstName = "Кира",
                Patronymic = "Станиславович",
                LastName = "Владиславовна",
                DateOfBirth = new DateTime(1988, 2, 4),
                Position = "Технический писатель",
                DateOfEmployment = new DateTime(2016, 9, 10)
            });

            base.Seed(db);
        }
    }
}