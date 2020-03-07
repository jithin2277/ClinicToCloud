using Microsoft.EntityFrameworkCore;
using PatientsApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientsApp.Data
{
    public interface IPatientRepository
    {
        Task<IList<PatientEntity>> GetPatients();
        Task<PagedList<PatientEntity>> GetPageListedPatients(int pageNumber, int pageSize);
        Task<PatientEntity> GetPatientById(Guid id);
        Task<PatientEntity> AddPatient(PatientEntity patientEntity);
        Task<int> AddPatients(IList<PatientEntity> patients);
        Task<PatientEntity> UpdatePatient(PatientEntity patient);
    }

    public class PatientRepository : IPatientRepository
    {
        private readonly PatientsContext _patientsContext;

        public PatientRepository(PatientsContext patientsContext) 
            => _patientsContext = patientsContext ?? throw new ArgumentNullException("patientsContext");

        public async Task<PatientEntity> GetPatientById(Guid id)
        {
            return await _patientsContext.Patients.FindAsync(id).ConfigureAwait(false);
        }

        public async Task<IList<PatientEntity>> GetPatients()
        {
            return await _patientsContext.Patients.ToListAsync().ConfigureAwait(false);
        }

        public async Task<PagedList<PatientEntity>> GetPageListedPatients(int pageNumber = 1, int pageSize = 10)
        {
            var patients = _patientsContext.Patients.AsNoTracking();

            return await PagedList<PatientEntity>.CreateAsync(patients, pageNumber, pageSize).ConfigureAwait(false);
        }

        public async Task<PatientEntity> AddPatient(PatientEntity patient)
        {
            _ = await _patientsContext.Patients.AddAsync(patient).ConfigureAwait(false);
            _ = await _patientsContext.SaveChangesAsync().ConfigureAwait(false);

            return await GetPatientById(patient.Id);
        }

        public async Task<int> AddPatients(IList<PatientEntity> patients)
        {
            await _patientsContext.Patients.AddRangeAsync(patients).ConfigureAwait(false);
            return await _patientsContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<PatientEntity> UpdatePatient(PatientEntity patient)
        {
            _ = _patientsContext.Patients.Update(patient);
            _ = await _patientsContext.SaveChangesAsync().ConfigureAwait(false);

            return await GetPatientById(patient.Id);
        }
    }
}
