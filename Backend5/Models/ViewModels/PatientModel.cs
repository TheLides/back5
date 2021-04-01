using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend5.Models.ViewModels
{
    public class PatientModel
    {
        [Required]
        [MaxLength(30)]
        public String Name { get; set; }

        public String Address { get; set; }

        public DateTime Birthday { get; set; }

        [Required]
        public String Gender { get; set; }
    }
}
