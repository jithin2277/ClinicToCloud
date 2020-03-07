using AutoMapper;
using PatientsApp.Api.Models;
using PatientsApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientsApp.Api.Utilities
{
    public class PatientMapperProfile: Profile
    {
        public PatientMapperProfile()
        {
            CreateMap<PatientEntity, Patient>();

            CreateMap<Patient, PatientEntity>();
        }
    }
}
