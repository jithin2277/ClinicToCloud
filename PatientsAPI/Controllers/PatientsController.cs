using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PatientsApp.Api.Models;
using PatientsApp.Data;
using PatientsApp.Data.Entities;

namespace PatientsApp.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public PatientsController(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException("patientRepository");
            _mapper = mapper ?? throw new ArgumentNullException("mapper");
        }

        [HttpGet]
        public async Task<ActionResult<PatientResponse>> Get()
        {
            var patientEntities = await _patientRepository.GetPatients();
            var patientResponse = new PatientResponse
            { Patients = _mapper.Map<IList<Patient>>(patientEntities) };

            return Ok(patientResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientResponse>> Get(Guid id)
        {
            var patientEntity = await _patientRepository.GetPatientById(id);
            if (patientEntity != null)
            {
                var patientResponse = new PatientResponse
                {
                    Patients = new List<Patient>() { _mapper.Map<Patient>(patientEntity) }
                };

                return Ok(patientResponse);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("paged")]
        public async Task<ActionResult<PagedResponse>> GetPaged([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            if (pageNumber == null || pageSize == null)
            {
                return ValidationProblem();
            }

            var pageListedPatients = await _patientRepository.GetPageListedPatients(pageNumber.Value, pageSize.Value);

            var pageResponse = new PagedResponse()
            {
                Metadata = new PageMetadata
                {
                    PageNumber = pageNumber.Value,
                    PageSize = pageSize.Value,
                    TotalPages = pageListedPatients.TotalPages,
                    HasNextPage = pageListedPatients.HasNextPage,
                    HasPreviousPage = pageListedPatients.HasPreviousPage,
                    TotalCount = pageListedPatients.TotalCount
                },
                Patients = _mapper.Map<IList<Patient>>(pageListedPatients.ToList())
            };

            return Ok(pageResponse);
        }
        
        [HttpPost]
        public async Task<ActionResult<Patient>> Post([FromBody] Patient patientRecord)
        {
            if (Utilities.Validations.IsValidPatient(patientRecord))
            {
                var newPatientId = Guid.NewGuid();
                var newPatient = _mapper.Map<PatientEntity>(patientRecord);
                newPatient.Id = newPatientId;
                newPatient.UpdatedAt = DateTime.UtcNow;
                newPatient.CreatedAt = DateTime.UtcNow;

                var patientEntity = await _patientRepository.AddPatient(newPatient);
                var createdAt = $"api/v1/patients/{patientEntity.Id}";
                
                return Created(createdAt, _mapper.Map<Patient>(patientEntity));
            }
            var problems = new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Invalid input JSON"                
            };
            
            return ValidationProblem(problems);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Patient>> Put(Guid id, [FromBody] Patient patientRecord)
        {
            if (Utilities.Validations.IsValidPatient(patientRecord))
            {
                var patientEntity = await _patientRepository.GetPatientById(id);
                if (patientEntity != null)
                {
                    patientEntity.DOB = patientRecord.DOB;
                    patientEntity.Email = patientRecord.Email;
                    patientEntity.FirstName = patientRecord.FirstName;
                    patientEntity.Gender = patientRecord.Gender;
                    patientEntity.IsActive = patientRecord.IsActive.Value;
                    patientEntity.LastName = patientRecord.LastName;
                    patientEntity.Phone = patientRecord.Phone;
                    patientEntity.UpdatedAt = DateTime.UtcNow;

                    return Ok(_mapper.Map<Patient>(await _patientRepository.UpdatePatient(patientEntity)));
                }

                return NotFound();
            }

            var problems = new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Invalid input JSON"
            };
            return ValidationProblem(problems);
        }
    }
}
