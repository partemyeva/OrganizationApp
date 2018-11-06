using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrganizationApp.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Diagnostics;
using OrganizationApp.Tests.Utils;

namespace OrganizationApp.Tests.Controllers
{
    [TestFixture]
    public class EmployeesControllerVolumeTest
    {
        readonly string baseUrl;

        public EmployeesControllerVolumeTest()
        {
            baseUrl = $"{ConfigurationManager.AppSettings["BaseUrl"]}employees/";
        }


        private async Task AddOneEmployee()
        {
            using (var client = new HttpClient())
            {
                var stopwatch = Stopwatch.StartNew();
                
                await AddEmployee(client);

                stopwatch.Stop();
                
                Console.WriteLine($"First add operation time: {stopwatch.ElapsedMilliseconds} ms.");
            }
        }


        private async Task AddEmployee(HttpClient client)
        {
            var employee = EmployeeGenerator.GenerateEmployee();

            // Отправляем запрос на добавление нового сотрудника
            var response = await client.PostAsXmlAsync(baseUrl, employee);
            
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
        }


        private async Task AddSomeEmployees(int count)
        {
            using (var client = new HttpClient())
            {
                var stopwatch = Stopwatch.StartNew();

                for (int i = 0; i < count; i++)
                {
                    await AddEmployee(client);
                }

                stopwatch.Stop();

                var timePerOperation = (double)(stopwatch.ElapsedMilliseconds) / count;

                Console.WriteLine($"Add operation time: {timePerOperation} ms.");
            }
        }


        private async Task GetOneEmployee()
        {
            using (var client = new HttpClient())
            {
                var stopwatch = Stopwatch.StartNew();

                await GetEmployee(client);

                stopwatch.Stop();

                Console.WriteLine($"First get operation time: {stopwatch.ElapsedMilliseconds} ms.");
            }
        }


        private async Task GetEmployee(HttpClient client)
        {
            var url = EmployeeGenerator.GetRandomQueryString();

            var response = await client.GetAsync($"{baseUrl}?{url}");

            Assert.IsTrue(response.IsSuccessStatusCode);
        }


        private async Task GetSomeEmployees(int count)
        {
            using (var client = new HttpClient())
            {
                var stopwatch = Stopwatch.StartNew();

                for (int i = 0; i < count; i++)
                {
                    await GetEmployee(client);
                }

                stopwatch.Stop();

                var timePerOperation = (double)(stopwatch.ElapsedMilliseconds) / count;

                Console.WriteLine($"Get operation time: {timePerOperation} ms.");
            }
        }


        [Test]
        public async Task VolumeAddAndGetEmployee()
        {
            var count = 500;

            var iterations = 10;

            //// Делаем одну операцию добавления сотрудника
            //await AddOneEmployee();

            //// Делаем одну операцию получения сотрудников со случайным фильтром
            //await GetOneEmployee();

            for (int i = 0; i < iterations; i++)
            {
                Console.WriteLine($"{(i+1)*count}. --------------------------------------");

                // Делаем одну операцию добавления сотрудника
                await AddOneEmployee();

                // Делаем count операций добавления сотрудников, вычисляем среднее время выполнения
                await AddSomeEmployees(count);

                // Делаем одну операцию получения сотрудников со случайным фильтром
                await GetOneEmployee();

                // Делаем count операций получения сотрудников со случайным фильтром, вычисляем среднее время выполнения
                await GetSomeEmployees(count);
            }

        }
    }
}
