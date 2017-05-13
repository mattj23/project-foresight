using System.Collections.Generic;

namespace Foresight
{
    public class Organization
    {
        public string Name { get; set; }
        public List<EmployeeType> EmployeeTypes { get; set; }

        public List<Employee> Employees { get; set; }

        public Organization()
        {
            this.EmployeeTypes = new List<EmployeeType>();
            this.Employees = new List<Employee>();
        }
    }
}