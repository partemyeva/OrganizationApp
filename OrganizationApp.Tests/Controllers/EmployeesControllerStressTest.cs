using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using NUnit.Framework;
using OrganizationApp.Models;
using System.Net.Http;

namespace OrganizationApp.Tests.Controllers
{
    public class EmployeesControllerStressTest
    {
        string baseUrl;

        public EmployeesControllerStressTest()
        {
            baseUrl = $"{ConfigurationManager.AppSettings["BaseUrl"]}employees/";
        }

        [Test]
        public async Task MultipleAddEmployee()
        {
            var count = 1000;

            using (var client = new HttpClient())
            {

                for (int i = 0; i < count; i++)
                {
                    var employee = new Employee()
                    {
                        FirstName = "Оксана",
                        Patronymic = "Сергеевна",
                        LastName = "Харитонова",
                        DateOfBirth = new DateTime(1990, 5, 29),
                        Position = "Бухгалтер",
                        DateOfEmployment = new DateTime(2015, 11, 30)
                    };


                    // Отправляем запрос на добавление нового сотрудника
                    var response = await client.PostAsXmlAsync(baseUrl, employee);

                    Assert.IsTrue(response.IsSuccessStatusCode);
                }
            }
        }

        [Test]
        public async Task MultipleDeleteEmployee()
        {
            using (var client = new HttpClient())
            {
                // Получаем список всех сотрудников
                var getResponse = await client.GetAsync(baseUrl);
                var employees = await getResponse.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(getResponse.IsSuccessStatusCode);
                Assert.IsNotEmpty(employees);

                // Сотрудник, которого будем удалять
                var lastName = "Харитонова";
                var employeesToDelete = employees.Where(x => x.LastName == lastName);

                foreach (var employee in employeesToDelete)
                {
                    var url = $"{baseUrl}/{employee.ID}";

                    // Отправляем запрос на удаление сотрудника
                    var response = await client.DeleteAsync(url);

                    Assert.IsTrue(response.IsSuccessStatusCode);
                }

                // Получаем список всех сотрудников
                getResponse = await client.GetAsync(baseUrl);
                employees = await getResponse.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(getResponse.IsSuccessStatusCode);

                // Проверяем, чем удаленный сотрудник действительно отсутствует
                Assert.That(employees.Count(x => x.LastName == lastName) == 0);
            }
        }

        [Test]
        public async Task MultipleModifyEmployee()
        {
            using (var client = new HttpClient())
            {
                // Получаем список всех сотрудников
                var response = await client.GetAsync(baseUrl);
                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.IsNotEmpty(employees);

                var i = 0;
                var count = 1000;
                do
                {
                    foreach (var employee in employees)
                    {
                        // Поменяем дату приема на работу
                        employee.DateOfEmployment = employee.DateOfEmployment.AddDays(-i);

                        var url = $"{baseUrl}/{employee.ID}";

                        // Запрос на изменение информации о сотруднике
                        response = await client.PutAsJsonAsync(url, employee);

                        Assert.IsTrue(response.IsSuccessStatusCode);

                        i++;
                    }
                } while (i < count);

            }
        }
    }
}
