using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Api;
using NUnit.Framework;
using System.Net.Http;
using System.Net.Http.Headers;
using OrganizationApp.Models;
using System.Net.Http.Formatting;
using System.Configuration;

namespace OrganizationApp.Tests.Controllers
{
    [TestFixture]
    public class EmployeesControllerTest
    {
        string baseUrl;

        public EmployeesControllerTest()
        {
            baseUrl = $"{ConfigurationManager.AppSettings["BaseUrl"]}employees/";
        }

        /// <summary>
        /// Проверяет получение списка всех сотрудников
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetAllEmployeesAsync()
        {
            using (var client = new HttpClient())
            {
                // Отправляем запрос в API
                var response = await client.GetAsync(baseUrl);

                // Ожидаем получить список сотрудников
                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                // Ожидаем успешный статус http-ответа
                Assert.IsTrue(response.IsSuccessStatusCode);

                // Ожидаем, что вернулся не пустой список
                Assert.IsNotEmpty(employees);
            }
        }

        /// <summary>
        /// Проверяет фильтр по должности сотрудника
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesByPosition()
        {
            using (var client = new HttpClient())
            {
                var position = "аналитик";

                var url = $"{baseUrl}?position={position}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                foreach (var employee in employees)
                {
                    Assert.AreEqual(position.ToLowerInvariant(), employee.Position.ToLowerInvariant());
                }
            }
        }

        /// <summary>
        /// Проверяет получение списка сотрудников со стажем работы меньше 0
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesWithExperienceLessThanZero()
        {
            using (var client = new HttpClient())
            {
                var maxExperience = 0;

                var url = $"{baseUrl}?maxExperience={maxExperience}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                // Ожидаем получение пустого списка
                Assert.IsEmpty(employees);
            }
        }


        /// <summary>
        /// Проверяет получение списка сотрудников со стажем работы меньше 100 лет
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesWithExperienceLessThan100()
        {
            using (var client = new HttpClient())
            {
                var hundred = 100;

                var url = $"{baseUrl}?maxExperience={hundred}";

                // Получаем список сотрудников со стажем меньше 100 лет
                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                // Получаем список всех сотрудников компании
                var allResponse = await client.GetAsync(baseUrl);

                var allEmployees = await allResponse.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(allResponse.IsSuccessStatusCode);

                // Ожидаем, что эти списки будут одинаковой длины
                Assert.AreEqual(allEmployees.Count(), employees.Count());
            }
        }


        /// <summary>
        /// Проверяет получение списка сотрудников со стажем работы больше 0
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesWithExperienceGreaterThanZero()
        {
            using (var client = new HttpClient())
            {
                var minExperience = 0;

                var url = $"{baseUrl}?minExperience={minExperience}";

                // Получаем список сотрудников со стажем больше 0
                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                // Получаем список всех сотрудников компании
                var allResponse = await client.GetAsync(baseUrl);

                var allEmployees = await allResponse.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(allResponse.IsSuccessStatusCode);

                // Ожидаем, что эти списки будут одинаковой длины
                Assert.AreEqual(allEmployees.Count(), employees.Count());
            }
        }


        /// <summary>
        /// Проверяет фильтр "старше чем"
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesOlderThan()
        {
            using (var client = new HttpClient())
            {
                var minAge = 31;

                var url = $"{baseUrl}?minAge={minAge}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                // Возраст каждого сотрудника ...
                foreach (var employee in employees)
                {
                    // ... должен быть больше или равен minAge
                    Assert.GreaterOrEqual(employee.Age, minAge);
                }
            }
        }

        /// <summary>
        /// Проверяет фильтр "моложе чем"
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesYoungerThan()
        {
            using (var client = new HttpClient())
            {
                var maxAge = 30;

                var url = $"{baseUrl}?maxAge={maxAge}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                // Возраст каждого сотрудника ...
                foreach (var employee in employees)
                {
                    // ... должен быть строго меньше minAge
                    Assert.Less(employee.Age, maxAge);
                }
            }
        }


        /// <summary>
        /// Проверяет фильтр по диапазону возраста
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesByAgeRange()
        {
            using (var client = new HttpClient())
            {
                var minAge = 30;
                var maxAge = 35;

                var url = $"{baseUrl}?minAge={minAge}&maxAge={maxAge}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                // Возраст каждого сотрудника ...
                foreach (var employee in employees)
                {
                    // ... должен быть больше или равен minAge
                    Assert.GreaterOrEqual(employee.Age, minAge);

                    // ... и строго меньше MaxAge
                    Assert.Less(employee.Age, maxAge);
                }
            }
        }


        /// <summary>
        /// Проверяет фильтр по сотрудникам с опытом работы более minExperience лет
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesWithExperienceMoreThan()
        {
            using (var client = new HttpClient())
            {
                var minExperience = 5;

                var url = $"{baseUrl}?minExperience={minExperience}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                foreach (var employee in employees)
                {
                    // Опыт работы в компании больше или равен minExperience
                    Assert.GreaterOrEqual(employee.Experience, minExperience);
                }
            }
        }


        /// <summary>
        /// Проверяет фильтр по сотрудникам с опытом работы менее maxExperience лет
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesWithExperienceLessThan()
        {
            using (var client = new HttpClient())
            {
                var maxExperience = 2;

                var url = $"{baseUrl}?maxExperience={maxExperience}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                foreach (var employee in employees)
                {
                    // Опыт работы в компании строго меньше maxExperience
                    Assert.Less(employee.Experience, maxExperience);
                }
            }
        }


        /// <summary>
        /// Проверяет фильтр по сотрудникам со стажем работы в компании более minExperience и менее maxExperience лет
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesByExperienceRange()
        {
            using (var client = new HttpClient())
            {
                var minExperience = 1;
                var maxExperience = 4;

                var url = $"{baseUrl}?minExperience={minExperience}&maxExperience={maxExperience}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                foreach (var employee in employees)
                {
                    // Опыт работы в компании больше или равен minExperience
                    Assert.GreaterOrEqual(employee.Experience, minExperience);

                    // Опыт работы в компании строго меньше maxExperience
                    Assert.Less(employee.Experience, maxExperience);
                }
            }
        }

        /// <summary>
        /// Проверяет получение списка сотрудников заданной должности с опытом работы более minExperience лет
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesByPositionWithExperienceMoreThan()
        {
            using (var client = new HttpClient())
            {
                var minExperience = 5;

                var position = "ПРоГРаММИиСТ";

                var url = $"{baseUrl}?minExperience={minExperience}&position={position}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                foreach (var employee in employees)
                {
                    // ДОлжность сотрудника должна сопадать с должностью в запросе
                    Assert.AreEqual(position.ToLowerInvariant(), employee.Position.ToLowerInvariant());

                    // Опыт работы в компании больше или равен minExperience
                    Assert.GreaterOrEqual(employee.Experience, minExperience);
                }
            }
        }

        /// <summary>
        /// Проверяет фильтр по диапазону возраста и диапазону стажа работы в компании
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesByAgeRangeAndExperienceRange()
        {
            using (var client = new HttpClient())
            {
                var minAge = 28;
                var maxAge = 33;

                var minExperience = 2;
                var maxExperience = 7;

                var url = $"{baseUrl}?minAge={minAge}&maxAge={maxAge}&minExperience={minExperience}&maxExperience={maxExperience}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                foreach (var employee in employees)
                {
                    // Возраст сотрудника должен быть больше или равен minAge ...
                    Assert.GreaterOrEqual(employee.Age, minAge);

                    // ... и строго меньше MaxAge
                    Assert.Less(employee.Age, maxAge);

                    // Опыт работы в компании больше или равен minExperience ...
                    Assert.GreaterOrEqual(employee.Experience, minExperience);

                    // ... и строго меньше maxExperience
                    Assert.Less(employee.Experience, maxExperience);
                }
            }
        }


        /// <summary>
        /// Проверяет получение списка сотрудников *моложе чем* и со стажем работы *больше чем*
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesYoungerThanAndExperienceMoreThan()
        {
            using (var client = new HttpClient())
            {
                var maxAge = 31;
                var minExperience = 2;

                var url = $"{baseUrl}?maxAge={maxAge}&minExperience={minExperience}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                foreach (var employee in employees)
                {
                    // Возраст сотрудника должен быть строго меньше MaxAge
                    Assert.Less(employee.Age, maxAge);

                    // Опыт работы в компании больше или равен minExperience
                    Assert.GreaterOrEqual(employee.Experience, minExperience);
                }
            }
        }


        /// <summary>
        /// Проверяет получение списка сотрудников *страше чем* и со стажем работы *больше чем*
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesOlderThanAndExperienceMoreThan()
        {
            using (var client = new HttpClient())
            {
                var minAge = 30;
                var minExperience = 2;

                var url = $"{baseUrl}?minAge={minAge}&minExperience={minExperience}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                foreach (var employee in employees)
                {
                    // Возраст сотрудника должен быть больше или равен minAge
                    Assert.GreaterOrEqual(employee.Age, minAge);

                    // Опыт работы в компании больше или равен minExperience
                    Assert.GreaterOrEqual(employee.Experience, minExperience);
                }
            }
        }


        /// <summary>
        /// Проверяет получение списка сотрудников *страше чем* и со стажем работы *менее чем*
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesOlderThanAndExperienceLessThan()
        {
            using (var client = new HttpClient())
            {
                var minAge = 33;
                var maxExperience = 3;

                var url = $"{baseUrl}?minAge={minAge}&maxExperience={maxExperience}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                foreach (var employee in employees)
                {
                    // Возраст сотрудника должен быть больше или равен minAge
                    Assert.GreaterOrEqual(employee.Age, minAge);

                    // Опыт работы в компании менее maxExperience
                    Assert.Less(employee.Experience, maxExperience);
                }
            }
        }


        /// <summary>
        /// Проверяет получение списка сотрудников *моложе чем* и со стажем работы *меньше чем*
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesYoungerThanAndExperienceLessThan()
        {
            using (var client = new HttpClient())
            {
                var maxAge = 30;
                var maxExperience = 1;

                var url = $"{baseUrl}?maxAge={maxAge}&maxExperience={maxExperience}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                foreach (var employee in employees)
                {
                    // Возраст сотрудника должен быть меньше MaxAge
                    Assert.Less(employee.Age, maxAge);

                    // Опыт работы в компании меньше maxExperience
                    Assert.Less(employee.Experience, maxExperience);
                }
            }
        }


        /// <summary>
        /// Проверяет получение списка сотрудников *моложе чем* и со стажем работы *больше чем и меньше чем*
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesYoungerThanAndExperienceRange()
        {
            using (var client = new HttpClient())
            {
                var maxAge = 30;
                var minExperience = 1;
                var maxExperience = 2;

                var url = $"{baseUrl}?maxAge={maxAge}&minExperience={minExperience}&maxExperience={maxExperience}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                foreach (var employee in employees)
                {
                    // Возраст сотрудника должен быть меньше MaxAge
                    Assert.Less(employee.Age, maxAge);

                    // Опыт работы в компании больше или равен minExperience
                    Assert.GreaterOrEqual(employee.Experience, minExperience);

                    // Опыт работы в компании меньше maxExperience
                    Assert.Less(employee.Experience, maxExperience);
                }
            }
        }


        /// <summary>
        /// Проверяет фильтр по диапазону возраста, диапазону стажа работы в компании и должности
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetEmployeesByAgeRangeExperienceRangeAndPosition()
        {
            using (var client = new HttpClient())
            {
                var minAge = 30;
                var maxAge = 33;

                var minExperience = 2;
                var maxExperience = 5;

                var position = "программист";

                var url = $"{baseUrl}?minAge={minAge}&maxAge={maxAge}&minExperience={minExperience}&maxExperience={maxExperience}&position={position}";

                var response = await client.GetAsync(url);

                var employees = await response.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(response.IsSuccessStatusCode);

                foreach (var employee in employees)
                {
                    // Возраст сотрудника должен быть больше или равен minAge ...
                    Assert.GreaterOrEqual(employee.Age, minAge);

                    // ... и строго меньше MaxAge
                    Assert.Less(employee.Age, maxAge);

                    // Опыт работы в компании больше или равен minExperience ...
                    Assert.GreaterOrEqual(employee.Experience, minExperience);

                    // ... и строго меньше maxExperience
                    Assert.Less(employee.Experience, maxExperience);

                    // Должна сопадать должность сотрудника
                    Assert.AreEqual(position.ToLowerInvariant(), employee.Position.ToLowerInvariant());
                }
            }
        }


        /// <summary>
        /// Проверяет добавление нового сотрудника
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AddEmployee()
        {
            var employee = new Employee()
            {
                FirstName = "Андрей",
                Patronymic = "Александрович",
                LastName = "Юдин",
                DateOfBirth = new DateTime(1988, 11, 27),
                Position = "Программист",
                DateOfEmployment = new DateTime(2017, 1, 11)
            };

            using (var client = new HttpClient())
            {
                // Отправляем запрос на добавление нового сотрудника
                var response = await client.PostAsXmlAsync(baseUrl, employee);

                Assert.IsTrue(response.IsSuccessStatusCode);

                // Получаем список всех сотрудников
                var getResponse = await client.GetAsync(baseUrl);
                var employees = await getResponse.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(getResponse.IsSuccessStatusCode);

                // Проверяем, чем добавленный сотрудник действительно добавился
                Assert.AreEqual(1, employees.Count(x => x.FirstName == employee.FirstName && x.LastName == employee.LastName && x.Patronymic == employee.Patronymic));
            }
        }


        /// <summary>
        /// Проверяет добавление нового сотрудника с невалидными данными
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AddNotValidEmployee()
        {
            var employee = new Employee()
            {
                FirstName = "Андрей",
                Patronymic = "Александрович",
                LastName = "Юдин",
                DateOfBirth = DateTime.Now.AddYears(-5).Date,       // возраст 5 лет
                Position = "Программист",
                DateOfEmployment = DateTime.Now.AddDays(10).Date    // дата приема на работу: сегодня + 10 дней 
            };

            using (var client = new HttpClient())
            {
                // Отправляем запрос на добавление сотрудника невалидными данными
                var response = await client.PostAsXmlAsync(baseUrl, employee);

                // Ожидаем ответ с неуспешным статусом
                Assert.IsFalse(response.IsSuccessStatusCode);
            }
        }


        /// <summary>
        /// Проверяет удаление сотрудника
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task DeleteEmployee()
        {
            using (var client = new HttpClient())
            {
                // Получаем список всех сотрудников
                var getResponse = await client.GetAsync(baseUrl);
                var employees = await getResponse.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(getResponse.IsSuccessStatusCode);
                Assert.IsNotEmpty(employees);

                // Сотрудник, которого будем удалять
                var employee = employees.LastOrDefault();

                var url = $"{baseUrl}/{employee.ID}";

                // Отправляем запрос на удаление сотрудника
                var response = await client.DeleteAsync(url);

                Assert.IsTrue(response.IsSuccessStatusCode);

                // Получаем список всех сотрудников
                getResponse = await client.GetAsync(baseUrl);
                employees = await getResponse.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(getResponse.IsSuccessStatusCode);

                // Проверяем, чем удаленный сотрудник действительно отсутствует
                Assert.That(employees.Count(x => x.FirstName == employee.FirstName && x.LastName == employee.LastName && x.Patronymic == employee.Patronymic) == 0);
            }
        }

        /// <summary>
        /// Проверяет изменение данных сотрудника
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ModifyEmployee()
        {
            using (var client = new HttpClient())
            {
                // Получаем список всех сотрудников
                var getResponse = await client.GetAsync(baseUrl);
                var employees = await getResponse.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(getResponse.IsSuccessStatusCode);
                Assert.IsNotEmpty(employees);

                // Сотрудник, данные которого будем изменять
                var employee = employees.FirstOrDefault();

                // Поменяем дату приема на работу
                employee.DateOfEmployment = new DateTime(2012, 7, 11);

                // Поменяем отчество
                employee.Patronymic = "Игоревич";

                var url = $"{baseUrl}/{employee.ID}";

                // Запрос на изменение информации о сотруднике
                var response = await client.PutAsJsonAsync(url, employee);

                Assert.IsTrue(response.IsSuccessStatusCode);

                // Получаем список всех сотрудников
                getResponse = await client.GetAsync(baseUrl);
                employees = await getResponse.Content.ReadAsAsync<IEnumerable<Employee>>();

                Assert.IsTrue(getResponse.IsSuccessStatusCode);

                var updatedEmployee = employees.FirstOrDefault(x =>
                                              x.FirstName == employee.FirstName &&
                                              x.LastName == employee.LastName &&
                                              x.Patronymic == employee.Patronymic &&
                                              x.DateOfBirth == employee.DateOfBirth &&
                                              x.Position == employee.Position &&
                                              x.DateOfEmployment == employee.DateOfEmployment);

                // Проверяем, что сотрудник с измененными данными присутствует
                Assert.IsNotNull(updatedEmployee);
            }
        }


    }
}
