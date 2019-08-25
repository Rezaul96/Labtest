using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LabTest.Web.Models
{
    public class RegistrationModelView
    {
        public int Id { get; set; }
        [Required,Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required, Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Required, Display(Name = "Address")]
        public string Address { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }      
        public string FirstLastName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required,Display(Name ="Confirm Password")]
        public string ReTypePasseord { get; set; }
    }
}
