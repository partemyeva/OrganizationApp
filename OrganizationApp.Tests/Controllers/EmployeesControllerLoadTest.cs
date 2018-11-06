using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OrganizationApp.Models;
using OrganizationApp.Tests.Utils;

namespace OrganizationApp.Tests.Controllers
{
    [TestFixture]
    public class EmployeesControllerLoadTest
    {
        readonly string baseUrl;

        public EmployeesControllerLoadTest()
        {
            baseUrl = $"{ConfigurationManager.AppSettings["BaseUrl"]}employees/";
        }

        private async Task SendGetEmployeeRequest(string url)
        {
            using (var client = new HttpClient())
            {
                // Отправляем запрос на получение списка сотрудников с фильтром
                var response = await client.GetAsync(url);

                // В ответе должен быть список сотрудников, но в нем не обязательно будут элементы
                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Console.WriteLine($"{url} -- '{response.StatusCode}'! : {employees.Count()}");

                // Проверяем, что вернулся ответ с успешным статусом
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        /// <summary>
        /// Параллельно отправляет <code>count</code> запросов в API на получение списка сотрудников со случайно сгенерированным фильтром
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task SendParallelGetEmployeesRequests()
        {
            var count = 1000;

            var urls = new List<string>();

            for (int i = 0; i < count; i++)
            {
                var queryString = EmployeeGenerator.GetRandomQueryString();

                urls.Add($"{baseUrl}?{queryString}");
            }

            await Task.WhenAll(urls.Select(url => SendGetEmployeeRequest(url)));
        }


        private async Task SendAddEmployeeRequest(Employee employee)
        {
            using (var client = new HttpClient())
            {                
                // Отправляем запрос на добавление нового сотрудника
                var response = await client.PostAsXmlAsync(baseUrl, employee);

                // Проверяем статус ответа
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            }
        }

        /// <summary>
        /// Параллельно отправляет <code>count</code> запросов в API на добавление нового сотрудника
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task SendParallelAddEmployeeRequests()
        {
            var count = 1000;

            var employees = new List<Employee>();

            for (int i = 0; i < count; i++)
            {
                var employee = EmployeeGenerator.GenerateEmployee();

                employees.Add(employee);
            }

            await Task.WhenAll(employees.Select(e => SendAddEmployeeRequest(e)));
        }


        private async Task SendModifyEmployeeRequest(int id, Employee employee)
        {
            using (var client = new HttpClient())
            {
                var url = $"{baseUrl}{id}";

                // Отправляем запрос на изменение данных сотрудника
                var response = await client.PutAsXmlAsync(url, employee);

                //Console.WriteLine(response.StatusCode);

                // Проверяем статус ответа
                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            }
        }


        private async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            using (var client = new HttpClient())
            {
                // Отправляем запрос на получение всех сотрудников
                var response = await client.GetAsync(baseUrl);

                // В ответе должен быть список сотрудников
                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                // Проверяем, что вернулся ответ с успешным статусом
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                // Проверяем, что в списке есть элементы
                //Assert.IsNotEmpty(employees);

                return employees;
            }
        }


        /// <summary>
        /// Параллельно отправляет <code>count</code> запросов в API на изменение информации о сотруднике
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task SendParallelModifyEmployeeRequests()
        {
            var count = 1000;

            // Получаем всех сотрудников
            var employees = await GetAllEmployees();
            
            var modifiedEmployees = new Dictionary<int, Employee>();

            for (int i = 0; i < count; i++)
            {
                var emp = employees.Skip(i).FirstOrDefault();

                var newPosition = EmployeeGenerator.Positions.RandomElement(); ;

                Console.WriteLine($"{emp.ID}: {emp.Position} -> {newPosition}");

                // Меняем должность на случайную из списка должностей
                emp.Position = newPosition;

                modifiedEmployees.Add(emp.ID, emp);
            }

            // Запускаем задания параллельно, дожидаемся завершения их всех
            await Task.WhenAll(modifiedEmployees.Select(x => SendModifyEmployeeRequest(x.Key, x.Value)));
        }


        private async Task SendDeleteEmployeeRequest(int id)
        {
            using (var client = new HttpClient())
            {
                var url = $"{baseUrl}{id}";

                // Отправляем запрос на удаление сотрудника с идентификатором id
                var response = await client.DeleteAsync(url);
                
                // Проверяем статус ответа
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        /// <summary>
        /// Параллельно отправляет <code>count</code> запросов в API на удаление сотрудника по идентификатору
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task SendParallelRemoveEmployeeRequest()
        {
            var count = 1000;

            // Получаем всех сотрудников
            var employees = await GetAllEmployees();

            Console.WriteLine($"Employees count before delete operations: {employees.Count()}");

            // Идентификаторы первых count сотрудников в списке
            var employeesIDToDelete = employees.Take(count).Select(x => x.ID).ToList();

            // Запускаем задания параллельно, дожидаемся завершения их всех
            await Task.WhenAll(employeesIDToDelete.Select(x => SendDeleteEmployeeRequest(x)));

            // Получаем всех сотрудников
            employees = await GetAllEmployees();

            Console.WriteLine($"Employees count after {count} delete operations: {employees.Count()}");
        }

    }
}
