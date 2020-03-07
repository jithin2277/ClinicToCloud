using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PatientsApp.Data.Entities;
using System;
using System.Collections.Generic;

namespace PatientsApp.Data.Tests
{
    [TestClass]
    public class PatientRespositoryTests
    {
        private PatientRepository _sut;
        private PatientsDbContext _patientsDbContext;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<PatientsDbContext>()
                .UseInMemoryDatabase(databaseName: "PatientsDb")
                .Options;

            _patientsDbContext = new PatientsDbContext(options);
            var patientEntities = new List<PatientEntity>();
            for (int i = 0; i < 100; i++)
            {
                var patientEntity = new PatientEntity()
                {
                    CreatedAt = DateTime.UtcNow.AddDays(i * -1),
                    DOB = $"1/1/2000",
                    Email = $"patient{i}@clinictocloud.com",
                    FirstName = $"First Name {i}",
                    LastName = $"Last Name {i}",
                    Gender = "Male",
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    Phone = $"99999999{i}",
                    UpdatedAt = DateTime.UtcNow.AddDays(i * -1)
                };
                patientEntities.Add(patientEntity);
            }
            _patientsDbContext.Patients.AddRange(patientEntities);
            _patientsDbContext.SaveChanges();

            _sut = new PatientRepository(_patientsDbContext);
        }

        [TestMethod]
        public void GetPatientById_ReturnsRecord()
        {
            var id = _patientsDbContext.Patients.FirstOrDefaultAsync().Result.Id;
            var expected = _patientsDbContext.Patients.Find(id);
            var actual = _sut.GetPatientById(id).Result;

            Assert.AreEqual(expected, actual);
        }
                
        [TestMethod]
        public void GetPatients_ReturnsAllRecords()
        {
            var expected = _patientsDbContext.Patients.ToListAsync().Result;
            var actual = _sut.GetPatients().Result;

            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < actual.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void AddPatient_AddsPatient()
        {
            var newGuid = Guid.NewGuid();
            var patient = new PatientEntity()
            {
                CreatedAt = DateTime.UtcNow,
                DOB = $"1/1/2000",
                Email = $"patient@clinictocloud.com",
                FirstName = $"First Name",
                LastName = $"Last Name",
                Gender = "Male",
                Id = newGuid,
                IsActive = true,
                Phone = $"99999999",
                UpdatedAt = DateTime.UtcNow
            };

            var actual = _sut.AddPatient(patient).Result;
            var expected = _patientsDbContext.Patients.Find(newGuid);

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void UpdatePatient_UpdatesPatient()
        {
            var record = _patientsDbContext.Patients.FirstOrDefaultAsync().Result;
            record.DOB = "2/2/2014";

            var actual = _sut.UpdatePatient(record).Result;
            var expected = _patientsDbContext.Patients.Find(record.Id);

            Assert.AreEqual(actual, expected);
        }
    }
}
