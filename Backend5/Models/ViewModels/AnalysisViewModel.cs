using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend5.Models.ViewModels
{
    public class AnalysisViewModel
    {
        public Int32 LabId { get; set; }
        public String Type { get; set; }

        public DateTime Date { get; set; }

        [MaxLength(15)]
        public String Status { get; set; }
    }
}
