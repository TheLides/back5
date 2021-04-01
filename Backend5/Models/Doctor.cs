using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend5.Models
{
    public class Doctor
    {
        public Int32 Id { get; set; }

        [Required]
        [MaxLength(30)]
        public String Name { get; set; }

        public String Speciality { get; set; }

        public ICollection<HospitalDoctor> Hospitals { get; set; }

        public ICollection<DoctorPatient> Patients { get; set; }
    }
}
