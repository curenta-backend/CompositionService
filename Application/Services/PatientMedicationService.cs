using Application.Abstractions;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using CurentaCommonCore.Medications;
using CurentaCommonCore.MessagesContract.PioneerRxMsgs;
using Domain.Abstractions;
using Domain.Filters;
using Domain.Responses;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PatientMedicationService : IPatientMedicationService
    {
        private readonly IPatientClient _patientClient;
        public PatientMedicationService(IPatientClient patientClient)
        {
            _patientClient = patientClient;
        }
        public async Task<Result<List<PatientMedicationDetailedRecordModel>>> GetActivePatientsMedicationsDetailsForDate(DateOnly date, long? facilityId, Guid? patientId, Guid? patientMedicationId)
        {
            try
            {
                var medicationsResult = await _patientClient.GetActivePatientsMedicationsDetailsForDate(date,facilityId,patientId, patientMedicationId);
                if(medicationsResult.IsFailure)
                    return Result.Failure<List<PatientMedicationDetailedRecordModel>>(medicationsResult.Error);

                return medicationsResult.Value;
            }
            catch (Exception ex)
            {
                return Result.Failure<List<PatientMedicationDetailedRecordModel>>(ex.Message);
            }
        }
    }
}
