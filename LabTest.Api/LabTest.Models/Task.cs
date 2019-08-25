using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LabTest.Models
{
    [Table("Task")]
   public class Task
    {
        [Key]
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [ForeignKey(nameof(Registration))]
        public int AssainedTo { get; set; }      
        public int AsaainTo { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual Registration Registration { get; set; }
        [NotMapped]
        public virtual Registration Users { get; set; }
    }
}
