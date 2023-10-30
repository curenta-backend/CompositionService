using CSharpFunctionalExtensions;
using CurentaCommonCore.Medications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface IPatientMedicationService
    {
        public Task<Result<List<PatientMedicationDetailedRecordModel>>> GetActivePatientsMedicationsDetailsForDate(DateOnly date, long? facilityId, Guid? patientId, Guid? patientMedicationId);
    }
}
