using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrganizationApp.Models;

namespace OrganizationApp.Tests.Utils
{
    public static class EmployeeGenerator
    {
        private static Random rnd = new Random();

        public static IList<string> Positions { get; } = new List<string>() {
                                        "Программист",
                                        "Тестировщик",
                                        "Аналитик",
                                        "Дизайнер",
                                        "Верстальщик",
                                        "Администратор БД",
                                        "Системный администатор",
                                        "Менеджер проекта",
                                        "Менеджер продаж",
                                        "Контент менеджер",
                                        "Бухгалтер" };

        private static IList<string> maleFisrtNames = new List<string>{
                                         "Алексей",
                                         "Андрей",
                                         "Арсений",
                                         "Аркадий",
                                         "Иван",
                                         "Илья",
                                         "Александр",
                                         "Олег",
                                         "Николай",
                                         "Егор",
                                         "Павел",
                                         "Петр",
                                         "Платон",
                                         "Роман",
                                         "Степан",
                                         "Юрий" };

        private static IList<string> maleLastNames = new List<string> {
                                        "Смирнов",
                                        "Иванов",
                                        "Попов",
                                        "Соколов",
                                        "Кузнецов",
                                        "Морозов",
                                        "Петров",
                                        "Васильев",
                                        "Семенов",
                                        "Виноградов" };

        private static IList<string> malePatronymics = new List<string>() {
                                        "Геннадиевич",
                                        "Елисеевич",
                                        "Игнатович",
                                        "Константинович",
                                        "Михайлович",
                                        "Родионович",
                                        "Сергеевич",
                                        "Тимофеевич",
                                        "Фёдорович",
                                        "Эдуардович",
                                        "Яковлевич" };


        public static Employee GenerateEmployee()
        {
            var minAge = 16;

            var dateOfBirth = GetRandomDate(minAge, 60);
            var dateOfEmployment = GetRandomDate(0, 40);

            // Если сгенерировалось так, что сотрудник молодой, а опыт у него слишком большой...
            while (dateOfBirth.AddYears(minAge) > dateOfEmployment)
            {
                // ... перегенерируем дату приема на работу
                dateOfEmployment = GetRandomDate(0, 40);
            }

            var employee = new Employee()
            {
                FirstName = maleFisrtNames.RandomElement(),
                LastName = maleLastNames.RandomElement(),
                Patronymic = malePatronymics.RandomElement(),
                DateOfBirth = dateOfBirth,
                Position = Positions.RandomElement(),
                DateOfEmployment = dateOfEmployment
            };

            //Console.WriteLine($"{employee.FirstName} {employee.LastName} {employee.Patronymic} {dateOfBirth} {employee.Position} {dateOfEmployment}");

            return employee;
        }

        private static DateTime GetRandomDate(int minYears, int maxYears)
        {
            var ticks = DateTime.Now.Ticks;

            var years = (int)(ticks % (maxYears - 1));

            if (years < minYears)
                years = minYears;

            var months = (int)(ticks % 12);

            var days = ticks % 30;

            var date = DateTime.Now.AddYears(-years).AddMonths(-months).AddDays(-days);

            return date.Date;
        }


        public static EmployeeFilterParams GetandomEmployeeFilterParams()
        {
            var fp = new EmployeeFilterParams();

            var minAge = 16;
            var maxAge = 60;

            fp.MinAge = rnd.Next(minAge - 1, maxAge + 2);
            // Если сгенеровалось число (minAge-1), пусть в фильтре параметр MinAge будет не задан
            if (fp.MinAge == minAge - 1)
                fp.MinAge = null;

            fp.MaxAge = rnd.Next(minAge - 1, maxAge + 2);
            // Если сгенеровалось число (maxAge+1), пусть в фильтре параметр MaxAge будет не задан
            if (fp.MaxAge == maxAge + 1)
                fp.MaxAge = null;

            if (fp.MinAge.HasValue && fp.MaxAge.HasValue && fp.MinAge > fp.MaxAge)
            {
                var age = fp.MinAge;

                fp.MinAge = fp.MaxAge;
                fp.MaxAge = age;
            }

            var minExp = 0;
            var maxExp = 40;

            fp.MinExperience = rnd.Next(minExp - 1, maxExp + 2);
            // Если сгенерировалось число (minExp-1), пусть в фильтре параметр MinExperience будет не задан
            if (fp.MinExperience == minExp - 1)
                fp.MinExperience = null;

            fp.MaxExperience = rnd.Next(minExp - 1, maxExp + 2);
            // Если сгенерировалось число (maxExp+1), пусть в фильтре параметр MaxExperience будет не задан
            if (fp.MaxExperience == maxExp + 1)
                fp.MaxExperience = null;

            if (fp.MinExperience.HasValue && fp.MaxExperience.HasValue && fp.MinExperience > fp.MaxExperience)
            {
                var exp = fp.MinExperience;

                fp.MinExperience = fp.MaxExperience;
                fp.MaxExperience = exp;
            }

            var count = rnd.Next(-1, Positions.Count());

            // Если сгенерировалось число -1, ...
            if (count == -1)
            {
                // ... пусть в фильтре параметр Position будет не задан
                fp.Position = null;
            }
            else
            {
                // ... иначе берем элемент с номером count
                fp.Position = Positions.Skip(count).FirstOrDefault();
            }

            return fp;
        }

        public static string GetQueryString(EmployeeFilterParams fp)
        {
            var sb = new StringBuilder();

            if (fp.MinAge.HasValue)
                sb.AppendFormat("minAge={0}", fp.MinAge.Value);

            if (sb.Length > 0)
                sb.Append("&");

            if (fp.MaxAge.HasValue)
                sb.AppendFormat("maxAge={0}", fp.MaxAge.Value);

            if (sb.Length > 0)
                sb.Append("&");

            if (fp.MinExperience.HasValue)
                sb.AppendFormat("minExperience={0}", fp.MinExperience.Value);

            if (sb.Length > 0)
                sb.Append("&");

            if (fp.MaxExperience.HasValue)
                sb.AppendFormat("maxExperience={0}", fp.MaxExperience.Value);

            if (sb.Length > 0)
                sb.Append("&");

            if (fp.Position != null)
                sb.AppendFormat("position={0}", fp.Position);

            //var url = $"minAge={fp.MinAge}&maxAge={fp.MaxAge}&minExperience={fp.MinExperience}&maxExperience={fp.MaxExperience}&position={fp.Position}";

            return sb.ToString();
        }

        public static string GetRandomQueryString()
        {
            var fp = GetandomEmployeeFilterParams();
            
            var url = GetQueryString(fp);

            return url;
        }
    }
}
