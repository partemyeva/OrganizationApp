using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using NUnit.Framework;
using OrganizationApp.Models;
using OrganizationApp.Tests.Utils;


namespace OrganizationApp.Tests.Controllers
{
    [TestFixture]
    public class EmployeesControllerStressTest
    {
        readonly string baseUrl;

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


                var stopwatch = Stopwatch.StartNew();

                for (int i = 0; i < count; i++)
                {
                    employee = new Employee()
                    {
                        FirstName = "Оксана",
                        Patronymic = "Сергеевна",
                        LastName = "Харитонова",
                        DateOfBirth = new DateTime(1990, 5, 29),
                        Position = "Бухгалтер",
                        DateOfEmployment = new DateTime(2015, 11, 30)
                    };


                    // Отправляем запрос на добавление нового сотрудника
                    response = await client.PostAsXmlAsync(baseUrl, employee);

                    Assert.IsTrue(response.IsSuccessStatusCode);
                }

                stopwatch.Stop();

                var timePerOperation = (double)(stopwatch.ElapsedMilliseconds) / count;

                Console.WriteLine($"Add operation time: {timePerOperation} ms.");
            }
        }


        private async Task GetEmployee(HttpClient client, Random rnd)
        {
            // Генерируем параметры фильтра
            var fp = EmployeeGenerator.GetandomEmployeeFilterParams();

            var queryString = EmployeeGenerator.GetQueryString(fp);

            var response = await client.GetAsync($"{baseUrl}?{queryString}");

            // Отправляем запрос на получение сотрудников
            var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

            Assert.IsTrue(response.IsSuccessStatusCode);

            foreach (var employee in employees)
            {
                // Возраст сотрудника должен быть больше или равен minAge ...
                Assert.GreaterOrEqual(employee.Age, fp.MinAge);

                // ... и строго меньше MaxAge
                Assert.Less(employee.Age, fp.MaxAge);

                // Опыт работы в компании больше или равен minExperience ...
                Assert.GreaterOrEqual(employee.Experience, fp.MinExperience);

                // ... и строго меньше maxExperience
                Assert.Less(employee.Experience, fp.MaxExperience);

                // Должна сопадать должность сотрудника
                Assert.AreEqual(fp.Position.ToLowerInvariant(), employee.Position.ToLowerInvariant());
            }

            //Console.Write($"{fp.MinAge} {fp.MaxAge} {fp.MinExperience} {fp.MaxExperience} {fp.Position}\n");
        }

        [Test]
        public async Task MultipleGetEmployee()
        {
            var count = 1000;

            var rnd = new Random();

            using (var client = new HttpClient())
            {
                await GetEmployee(client, rnd);

                var stopwatch = Stopwatch.StartNew();

                for (int i = 0; i < count; i++)
                {

                    await GetEmployee(client, rnd);
                }

                stopwatch.Stop();

                var timePerOperation = (double)(stopwatch.ElapsedMilliseconds) / count;

                Console.WriteLine($"Get operation time: {timePerOperation} ms.");

            }
        }

        [Test]
        public async Task MultipleRemoveEmployee()
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
                
                var stopwatch = Stopwatch.StartNew();

                foreach (var employee in employeesToDelete)
                {
                    var url = $"{baseUrl}/{employee.ID}";

                    // Отправляем запрос на удаление сотрудника
                    var response = await client.DeleteAsync(url);

                    Assert.IsTrue(response.IsSuccessStatusCode);
                }

                stopwatch.Stop();

                var timePerOperation = (double)(stopwatch.ElapsedMilliseconds) / employeesToDelete.Count();

                Console.WriteLine($"Delete operation time: {timePerOperation} ms.");

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

                // Возьмем первого сотрудника
                var emp = employees.FirstOrDefault();

                // Поменяем дату приема на работу
                emp.DateOfEmployment = DateTime.Now.AddMonths(-1);

                var url = $"{baseUrl}/{emp.ID}";

                // Запрос на изменение информации о сотруднике
                response = await client.PutAsJsonAsync(url, emp);

                Assert.IsTrue(response.IsSuccessStatusCode);

                var stopwatch = Stopwatch.StartNew();

                var i = 0;
                var count = 1000;
                do
                {
                    foreach (var employee in employees)
                    {
                        // Поменяем дату приема на работу
                        employee.DateOfEmployment = DateTime.Now.AddDays(-i);

                        url = $"{baseUrl}/{employee.ID}";

                        // Запрос на изменение информации о сотруднике
                        response = await client.PutAsJsonAsync(url, employee);

                        Assert.IsTrue(response.IsSuccessStatusCode);

                        i++;
                    }
                } while (i < count);

                stopwatch.Stop();

                var timePerOperation = (double)(stopwatch.ElapsedMilliseconds) / count;

                Console.WriteLine($"Modify operation time: {timePerOperation} ms.");

            }
        }
    }
}
