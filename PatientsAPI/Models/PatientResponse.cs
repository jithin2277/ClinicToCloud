using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientsApp.Api.Models
{
    public class PatientResponse
    {
        public IList<Patient> Patients { get; set; }
    }
}
