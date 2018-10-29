using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace OrganizationApp.Models
{
    public class Employee
    {
        public int ID
        { get; set; }

        public string FirstName
        { get; set; }

        public string LastName
        { get; set; }

        public string Patronymic
        { get; set; }

        public DateTime DateOfBirth
        { get; set; }

        public string Position
        { get; set; }

        public DateTime DateOfEmployment
        { get; set; }

        private int? _age;
        [XmlIgnore]
        public int Age
        {
            get
            {
                if (!_age.HasValue)
                {
                    _age = GetYearsPassedFromDate(DateOfBirth);
                }

                return _age.Value;
            }
            set
            {
                value = GetYearsPassedFromDate(DateOfBirth);
            }
        }

        private int? _experience;
        [XmlIgnore]
        public int Experience
        {
            get
            {
                if (!_experience.HasValue)
                {
                    _experience = GetYearsPassedFromDate(DateOfEmployment);
                }
                
                return _experience.Value;
            }

            set
            {
                value = GetYearsPassedFromDate(DateOfEmployment);
            }
        }

        /// <summary>
        /// Возвращает число полных лет, прошедшее с даты <paramref name="date"/>, или 0, если <paramref name="date"/> > DateTime.Now 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private int GetYearsPassedFromDate(DateTime date)
        {
            if (date > DateTime.Now)
                return 0;

            return new DateTime(DateTime.Now.Subtract(date).Ticks).Year - 1;
        }

    }
}