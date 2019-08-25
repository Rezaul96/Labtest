using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LabTest.Web.Models
{
    public class TaskModelView
    {
        public int TaskId { get; set; }
        [Required,Display(Name ="Task Name")]
        public string TaskName { get; set; }
        [Required]
        public string Description { get; set; }        
        [Required, Display(Name = "Start Date") ,DisplayFormat( DataFormatString = "{0:yyyy/MM/dd hh:mm}")]
        public DateTime StartDate { get; set;}
        [Required, Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Required, Display(Name = "Assained To")]
        public int AssainedTo { get; set; }
        [Required, Display(Name = "Asaain To")]
        public int AsaainTo { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual RegistrationModelView Registration { get; set; }
        public virtual List<RegistrationModelView> RegistrationModels { get; set; }

        public virtual RegistrationModelView Users { get; set; }
    }
}
