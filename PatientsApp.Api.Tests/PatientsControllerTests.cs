using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PatientsApp.Api.Controllers;
using PatientsApp.Api.Models;
using PatientsApp.Data;
using PatientsApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PatientsApp.Api.Tests
{
    [TestClass]
    public class PatientsControllerTests
    {
        private Mock<IPatientRepository> _mockPatientRepository;
        private Mock<IMapper> _mockMapper;
        private IList<PatientEntity> _testPatientEntities;
        private IList<Patient> _testPatients;
        private PatientsController _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            _mockPatientRepository = new Mock<IPatientRepository>(MockBehavior.Strict);

            var currentDate = DateTime.UtcNow;

            _testPatientEntities = new List<PatientEntity> {
                new PatientEntity { CreatedAt = currentDate, DOB = "1/1/2000", Email = "a@b.com", FirstName="Name1", Gender = "M", Id = Guid.NewGuid(), IsActive = true, LastName = "Name", Phone = "99999999", UpdatedAt = currentDate},
                new PatientEntity { CreatedAt = currentDate, DOB = "1/1/2000", Email = "a@b.com", FirstName="Name2", Gender = "M", Id = Guid.NewGuid(), IsActive = true, LastName = "Name", Phone = "99999999", UpdatedAt = currentDate},
                new PatientEntity { CreatedAt = currentDate, DOB = "1/1/2000", Email = "a@b.com", FirstName="Name3", Gender = "M", Id = Guid.NewGuid(), IsActive = true, LastName = "Name", Phone = "99999999", UpdatedAt = currentDate},
                new PatientEntity { CreatedAt = currentDate, DOB = "1/1/2000", Email = "a@b.com", FirstName="Name4", Gender = "M", Id = Guid.NewGuid(), IsActive = true, LastName = "Name", Phone = "99999999", UpdatedAt = currentDate},
            };

            _testPatients = new List<Patient>();
            foreach (var item in _testPatientEntities)
            {
                _testPatients.Add(new Patient
                {
                    CreatedAt = item.CreatedAt,
                    DOB = item.DOB,
                    Email = item.Email,
                    FirstName = item.FirstName,
                    Gender = item.Gender,
                    Id = item.Id,
                    IsActive = item.IsActive,
                    LastName = item.LastName,
                    Phone = item.Phone,
                    UpdatedAt = item.UpdatedAt
                });
            }

            _sut = new PatientsController(_mockPatientRepository.Object, _mockMapper.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockMapper.VerifyAll();
            _mockPatientRepository.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_WhenAllParametersAreNull_ThrowsException()
        {
            _ = new PatientsController(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_WhenPatientRepositoryIsNull_ThrowsException()
        {
            _ = new PatientsController(null, _mockMapper.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_WhenMapperIsNull_ThrowsException()
        {
            _ = new PatientsController(_mockPatientRepository.Object, null);
        }

        [TestMethod]
        public void Get_WhenCalled_ReturnsAllPatientList()
        {
            var expected = new PatientResponse { Patients = _testPatients };
            _mockPatientRepository.Setup(s => s.GetPatients()).ReturnsAsync(_testPatientEntities);
            _mockMapper.Setup(s => s.Map<IList<Patient>>(_testPatientEntities)).Returns(_testPatients);

            var actionResult = _sut.Get().Result;
            var result = actionResult.Result as OkObjectResult;
            var actualResponse = result.Value as PatientResponse;

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(expected.Patients.Count, actualResponse.Patients.Count);
            for (int i = 0; i < actualResponse.Patients.Count; i++)
            {
                var expectedPatient = expected.Patients[i];
                var actualPatient = actualResponse.Patients[i];
                Assert.AreEqual(expectedPatient.CreatedAt, actualPatient.CreatedAt);
                Assert.AreEqual(expectedPatient.DOB, actualPatient.DOB);
                Assert.AreEqual(expectedPatient.Email, actualPatient.Email);
                Assert.AreEqual(expectedPatient.FirstName, actualPatient.FirstName);
                Assert.AreEqual(expectedPatient.Gender, actualPatient.Gender);
                Assert.AreEqual(expectedPatient.Id, actualPatient.Id);
                Assert.AreEqual(expectedPatient.IsActive, actualPatient.IsActive);
                Assert.AreEqual(expectedPatient.LastName, actualPatient.LastName);
                Assert.AreEqual(expectedPatient.Phone, actualPatient.Phone);
                Assert.AreEqual(expectedPatient.UpdatedAt, actualPatient.UpdatedAt);
            }
        }

        [TestMethod]
        public void Get_WhenIdPassedAndIdExists_GetsPatientWithId()
        {
            var testPatientEntity = _testPatientEntities.First();
            var patientId = testPatientEntity.Id;
            var testPatient = _testPatients.Where(w => w.Id == patientId).First();

            _mockPatientRepository.Setup(s => s.GetPatientById(patientId)).ReturnsAsync(testPatientEntity);
            _mockMapper.Setup(s => s.Map<Patient>(testPatientEntity)).Returns(testPatient);

            var actionResult = _sut.Get(patientId).Result;
            var result = actionResult.Result as OkObjectResult;
            var actualResponse = result.Value as PatientResponse;

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(testPatientEntity.CreatedAt, actualResponse.Patients[0].CreatedAt);
            Assert.AreEqual(testPatientEntity.DOB, actualResponse.Patients[0].DOB);
            Assert.AreEqual(testPatientEntity.Email, actualResponse.Patients[0].Email);
            Assert.AreEqual(testPatientEntity.FirstName, actualResponse.Patients[0].FirstName);
            Assert.AreEqual(testPatientEntity.Gender, actualResponse.Patients[0].Gender);
            Assert.AreEqual(testPatientEntity.Id, actualResponse.Patients[0].Id);
            Assert.AreEqual(testPatientEntity.IsActive, actualResponse.Patients[0].IsActive);
            Assert.AreEqual(testPatientEntity.LastName, actualResponse.Patients[0].LastName);
            Assert.AreEqual(testPatientEntity.Phone, actualResponse.Patients[0].Phone);
            Assert.AreEqual(testPatientEntity.UpdatedAt, actualResponse.Patients[0].UpdatedAt);
        }

        [TestMethod]
        public void Get_WhenIdPassedAndIdDoesNotExist_ReturnsNotFound()
        {
            var patientId = Guid.NewGuid();
            _mockPatientRepository.Setup(s => s.GetPatientById(patientId)).ReturnsAsync((PatientEntity)null);

            var actionResult = _sut.Get(patientId).Result;

            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetPaged_WhenPageNumberIsNull_ReturnsValidationProblemResult()
        {
            var actionResult = _sut.GetPaged(null, 1).Result;

            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void GetPaged_WhenPageSizeIsNull_ReturnsValidationProblemResult()
        {
            var actionResult = _sut.GetPaged(1, null).Result;

            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void GetPaged_WhenParametersAreNotNull_ReturnsPagedResults()
        {
            int pageNumber = 1;
            int pageSize = 2;
            var pagedPatientEntities = _testPatientEntities
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var pagedPatients = _testPatients
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var pageList = new PagedList<PatientEntity>(
                pagedPatientEntities,
                _testPatientEntities.Count,
                pageNumber,
                pageSize);
            var expectedPagedResponse = new PagedResponse
            {
                Metadata = new PageMetadata
                {
                    HasNextPage = pageList.HasNextPage,
                    HasPreviousPage = pageList.HasPreviousPage,
                    PageNumber = pageList.PageNumber,
                    PageSize = pageSize,
                    TotalCount = pageList.TotalCount,
                    TotalPages = pageList.TotalPages
                },
                Patients = pagedPatients
            };

            _mockPatientRepository.Setup(s => s.GetPageListedPatients(pageNumber, pageSize)).ReturnsAsync(pageList);
            _mockMapper.Setup(s => s.Map<IList<Patient>>(pageList))
                .Returns(_testPatients
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList());

            var actionResult = _sut.GetPaged(pageNumber, pageSize).Result;
            var result = actionResult.Result as OkObjectResult;
            var actualResponse = result.Value as PagedResponse;

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(expectedPagedResponse.Patients.Count, actualResponse.Patients.Count);
            for (int i = 0; i < actualResponse.Patients.Count; i++)
            {
                var expectedPatient = expectedPagedResponse.Patients[i];
                var actualPatient = actualResponse.Patients[i];
                Assert.AreEqual(expectedPatient.CreatedAt, actualPatient.CreatedAt);
                Assert.AreEqual(expectedPatient.DOB, actualPatient.DOB);
                Assert.AreEqual(expectedPatient.Email, actualPatient.Email);
                Assert.AreEqual(expectedPatient.FirstName, actualPatient.FirstName);
                Assert.AreEqual(expectedPatient.Gender, actualPatient.Gender);
                Assert.AreEqual(expectedPatient.Id, actualPatient.Id);
                Assert.AreEqual(expectedPatient.IsActive, actualPatient.IsActive);
                Assert.AreEqual(expectedPatient.LastName, actualPatient.LastName);
                Assert.AreEqual(expectedPatient.Phone, actualPatient.Phone);
                Assert.AreEqual(expectedPatient.UpdatedAt, actualPatient.UpdatedAt);
            }
        }


        [TestMethod]
        public void Post_WhenPatientRecordIsInValid_ReturnsBadResult()
        {
            var patientRecord = new Patient
            {
                DOB = "1/2/2000",
                Email = "a@a.com",
                FirstName = "Name 1",
                Gender = "M",
                Id = Guid.NewGuid(),
                IsActive = true,
                LastName = "Name 2",
                Phone = "999999"
            };

            // Patient Record is Null
            var actionResult = _sut.Post(null).Result;
            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestObjectResult));

            // No DOB
            patientRecord.DOB = string.Empty;
            var actionResultNoDOB = _sut.Post(patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoDOB.Result, typeof(BadRequestObjectResult));
            patientRecord.DOB = "1/2/2000";

            // No Email
            patientRecord.Email = string.Empty;
            var actionResultNoEmail = _sut.Post(patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoEmail.Result, typeof(BadRequestObjectResult));
            patientRecord.Email = "a@a.com";

            // No FirstName
            patientRecord.FirstName = string.Empty;
            var actionResultNoFirstName = _sut.Post(patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoEmail.Result, typeof(BadRequestObjectResult));
            patientRecord.FirstName = "Name 1";

            // No LastName
            patientRecord.LastName = string.Empty;
            var actionResultNoLastName = _sut.Post(patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoEmail.Result, typeof(BadRequestObjectResult));
            patientRecord.LastName = "Name 2";

            // No Gender
            patientRecord.Gender = string.Empty;
            var actionResultNoGender = _sut.Post(patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoEmail.Result, typeof(BadRequestObjectResult));
            patientRecord.Gender = "M";

            // No LastName
            patientRecord.Phone = string.Empty;
            var actionResultNoPhone = _sut.Post(patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoEmail.Result, typeof(BadRequestObjectResult));
            patientRecord.Phone = "999999";

            // No IsActive
            patientRecord.IsActive = null;
            var actionResultNoIsActive = _sut.Post(patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoEmail.Result, typeof(BadRequestObjectResult));
            patientRecord.IsActive = true;
        }

        [TestMethod]
        public void Post_WhenPatientRecordIsValid_AddsNewParent()
        {
            var newGuid = Guid.NewGuid();
            var patientEntity = new PatientEntity
            {
                DOB = "1/2/2000",
                Email = "a@a.com",
                FirstName = "Name 1",
                Gender = "M",
                Id = newGuid,
                IsActive = true,
                LastName = "Name 2",
                Phone = "999999"
            };
            var patient = new Patient
            {
                DOB = "1/2/2000",
                Email = "a@a.com",
                FirstName = "Name 1",
                Gender = "M",
                Id = newGuid,
                IsActive = true,
                LastName = "Name 2",
                Phone = "999999"
            };

            _mockMapper.Setup(s => s.Map<PatientEntity>(patient)).Returns(patientEntity);
            _mockMapper.Setup(s => s.Map<Patient>(patientEntity)).Returns(patient);
            _mockPatientRepository.Setup(s => s.AddPatient(_mockMapper.Object.Map<PatientEntity>(patient))).ReturnsAsync(patientEntity);

            var actionResult = _sut.Post(patient).Result;
            var result = actionResult.Result as CreatedResult;
            var actualResponse = result.Value as Patient;

            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(patient, actualResponse);
        }

        [TestMethod]
        public void Put_WhenPatientRecordIsInValid_ReturnsBadResult()
        {
            var newGuid = Guid.NewGuid();
            var patientRecord = new Patient
            {
                DOB = "1/2/2000",
                Email = "a@a.com",
                FirstName = "Name 1",
                Gender = "M",
                Id = Guid.NewGuid(),
                IsActive = true,
                LastName = "Name 2",
                Phone = "999999"
            };

            // Patient Record is Null
            var actionResult = _sut.Put(newGuid, null).Result;
            Assert.IsInstanceOfType(actionResult.Result, typeof(BadRequestObjectResult));

            // No DOB
            patientRecord.DOB = string.Empty;
            var actionResultNoDOB = _sut.Put(newGuid, patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoDOB.Result, typeof(BadRequestObjectResult));
            patientRecord.DOB = "1/2/2000";

            // No Email
            patientRecord.Email = string.Empty;
            var actionResultNoEmail = _sut.Put(newGuid, patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoEmail.Result, typeof(BadRequestObjectResult));
            patientRecord.Email = "a@a.com";

            // No FirstName
            patientRecord.FirstName = string.Empty;
            var actionResultNoFirstName = _sut.Put(newGuid, patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoEmail.Result, typeof(BadRequestObjectResult));
            patientRecord.FirstName = "Name 1";

            // No LastName
            patientRecord.LastName = string.Empty;
            var actionResultNoLastName = _sut.Put(newGuid, patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoEmail.Result, typeof(BadRequestObjectResult));
            patientRecord.LastName = "Name 2";

            // No Gender
            patientRecord.Gender = string.Empty;
            var actionResultNoGender = _sut.Put(newGuid, patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoEmail.Result, typeof(BadRequestObjectResult));
            patientRecord.Gender = "M";

            // No LastName
            patientRecord.Phone = string.Empty;
            var actionResultNoPhone = _sut.Put(newGuid, patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoEmail.Result, typeof(BadRequestObjectResult));
            patientRecord.Phone = "999999";

            // No IsActive
            patientRecord.IsActive = null;
            var actionResultNoIsActive = _sut.Put(newGuid, patientRecord).Result;
            Assert.IsInstanceOfType(actionResultNoEmail.Result, typeof(BadRequestObjectResult));
            patientRecord.IsActive = true;
        }

        [TestMethod]
        public void Put_WhenPatientRecordIsValidAndRecordDoesNotExists_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            var patientRecord = new Patient
            {
                DOB = "1/2/2000",
                Email = "a@a.com",
                FirstName = "Name 1",
                Gender = "M",
                IsActive = true,
                LastName = "Name 2",
                Phone = "999999"
            };

            _mockPatientRepository.Setup(s => s.GetPatientById(id)).ReturnsAsync((PatientEntity)null);

            var actionResult = _sut.Put(id, patientRecord).Result;
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Put_WhenPatientRecordIsValidAndRecordExists_UpdatesRecord()
        {
            var newGuid = Guid.NewGuid();
            var patientEntity = new PatientEntity
            {
                DOB = "1/2/2000",
                Email = "a@a.com",
                FirstName = "Name 1",
                Gender = "M",
                Id = newGuid,
                IsActive = true,
                LastName = "Name 2",
                Phone = "999999"
            };
            var patient = new Patient
            {
                DOB = "1/2/2000",
                Email = "a@a.com",
                FirstName = "Name 1",
                Gender = "M",
                Id = newGuid,
                IsActive = true,
                LastName = "Name 2",
                Phone = "999999"
            };

            _mockPatientRepository.Setup(s => s.GetPatientById(newGuid)).ReturnsAsync(patientEntity);
            _mockPatientRepository.Setup(s => s.UpdatePatient(patientEntity)).ReturnsAsync(patientEntity);
            _mockMapper.Setup(s => s.Map<Patient>(patientEntity)).Returns(patient);

            var actionResult = _sut.Put(newGuid, patient).Result;
            var result = actionResult.Result as OkObjectResult;
            var actualResponse = result.Value as Patient;

            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(patient, actualResponse);
        }
    }
}
