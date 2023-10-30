using Application.Abstractions;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
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
    public class PatientService : IPatientService
    {
        private readonly IPatientClient _patientClient;
        private readonly ITaskClient _taskClient;
        public PatientService(IPatientClient patientClient, ITaskClient taskClient)
        {
            _patientClient = patientClient;
            _taskClient = taskClient;
        }
        public async Task<Result<PatientResponse>> GetPatientsListWithTasksCountFiltered(PatientFilter filter)
        {
            try
            {
                var patientsResult = await _patientClient.GetPatientsListFiltered(filter);
                if(patientsResult.IsFailure)
                    return Result.Failure<PatientResponse>(patientsResult.Error);

                var tasksCountResult = await _taskClient.GetOpenTasksCountPerPatient(filter?.FacilityId != null ?  filter.FacilityId.Value : null);
                if(tasksCountResult.IsFailure)
                    return Result.Failure<PatientResponse>(tasksCountResult.Error);

                return new PatientResponse()
                {
                    data = new PatientResponseData()
                    {
                        searchPatients = new PatientSearchPatientsByNameAndFacilityId()
                        {
                            pageInfo = patientsResult.Value.data.searchPatients.pageInfo,
                            nodes = patientsResult.Value.data.searchPatients.nodes.Select(p => new PatientViewModel()
                            {
                                Id = p.Id,
                                BasicInfo = p.BasicInfo,
                                PlacesOfService = p.PlacesOfService,
                                Addresses = p.Addresses,
                                Status = p.Status,
                                ProfilePicture = p.ProfilePicture,
                                DeliveryNote = p.DeliveryNote,
                                IsBubblePackEnabled = p.IsBubblePackEnabled,
                                OpenTasksCount = tasksCountResult.Value.FirstOrDefault(t => t.PatientId == p.Id)?.NumberOfTasks
                            }).ToList()
                        }
                    }
               
                };
            }
            catch (Exception ex)
            {
                return Result.Failure<PatientResponse>(ex.Message);
            }
        }
    }
}
