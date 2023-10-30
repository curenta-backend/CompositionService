using CSharpFunctionalExtensions;
using CurentaCommonCore.Medications;
using CurentaCommonCore.PatientObjects;
using CurentaCommonCore.ResponseObjects;
using Domain.Filters;
using Domain.Models;
using Domain.Responses;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IPatientClient
    {
        Task<Result<GraphQLPatientResponse>> GetPatientsListFiltered(PatientFilter filter);
        Task<GenericResponse<List<GUIDNameResponse>>> GetPatientsNamesByIds(List<Guid> ids);
        Task<Result<List<PatientMedicationDetailedRecordModel>>> GetActivePatientsMedicationsDetailsForDate(DateOnly date, long? facilityId, Guid? patientId, Guid? patientMedicationId);
    }
}
