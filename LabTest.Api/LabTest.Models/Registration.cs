using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LabTest.Models
{
    [Table("Registration")]
    public class Registration
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }      
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }

        [NotMapped]
        public string FirstLastName => (FirstName + " " + LastName).Trim();
        [NotMapped]
        public string Password { get; set; }
    }
}
