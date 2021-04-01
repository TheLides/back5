using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend5.Models.ViewModels
{
    public class WardStaffViewModel
    {
        [Required]
        [MaxLength(30)]
        public String Name { get; set; }

        public String Position { get; set; }
    }
}
