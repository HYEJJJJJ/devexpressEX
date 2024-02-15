using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace People_gridview
{
    public class ColumnData
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public ColumnData() { }

        public ColumnData(string Name, string Age, string Gender, string Email, string PhoneNumber)
        {
            this.Name = Name;
            this.Age = Age;
            this.Gender = Gender;
            this.Email = Email;
            this.PhoneNumber = PhoneNumber;
        }
    }
}
