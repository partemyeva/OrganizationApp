using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrganizationApp.Models
{
    public class EmployeeFilterParams
    {
        public int? MinAge
        { get; set; }

        public int? MaxAge
        { get; set; }

        public int? MinExperience
        { get; set; }

        public int? MaxExperience
        { get; set; }

        public string Position
        { get; set; }
    }
}