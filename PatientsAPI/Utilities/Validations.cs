using PatientsApp.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientsApp.Api.Utilities
{
    public class Validations
    {
        public static bool IsValidPatient(Patient patient)
        {
            if (patient == null
                || patient.IsActive == null
                || string.IsNullOrEmpty(patient.FirstName)
                || string.IsNullOrEmpty(patient.LastName)
                || string.IsNullOrEmpty(patient.DOB)
                || string.IsNullOrEmpty(patient.Email)
                || string.IsNullOrEmpty(patient.Gender)
                || string.IsNullOrEmpty(patient.Phone))
            {
                return false; 
            }

            return true;
        }
    }
}
