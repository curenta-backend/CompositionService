using Application.Abstractions;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using CurentaCommonCore.MessagesContract.PioneerRxMsgs;
using CurentaCommonCore.Models;
using CurentaCommonCore.ResponseObjects;
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
    public class TaskService : ITaskService
    {
        private readonly IPatientClient _patientClient;
        private readonly ITaskClient _taskClient;
        public TaskService(IPatientClient patientClient, ITaskClient taskClient)
        {
            _patientClient = patientClient;
            _taskClient = taskClient;
        }

        public async Task<Result< GenericResponse<List<CurentaTaskViewModel>>>> GetOpenTasksFiltered(CurentaTaskFilter filter, PaginationFilter paginationFilter)
        {
            try
            {
                var tasksResult = await _taskClient.GetOpenTasksFiltered(filter, paginationFilter);
                if (tasksResult.IsFailure)
                    return Result.Failure<GenericResponse<List<CurentaTaskViewModel>>>(tasksResult.Error);

                tasksResult.Value.Data = await FillTaskAdditinalDetails(tasksResult.Value.Data);

                var sortNum = 1;
                tasksResult.Value.Data = tasksResult.Value.Data.Select(t => { t.SortNum = sortNum++; return t; }).ToList();

                return tasksResult.Value;
            }
            catch (Exception ex)
            {
                return Result.Failure<GenericResponse<List<CurentaTaskViewModel>>>(ex.Message);
            }      
        }

        private async Task<List<CurentaTaskViewModel>> FillTaskAdditinalDetails(List<CurentaTaskViewModel> tasks)
        {
            try
            {
                //tasks = await FillUsersNamesAsync(tasks);//todo
                tasks = await FillPatientsNamesAsync(tasks);
                //tasks = await FillTaskCompletionAsync(tasks);//todo
                return tasks;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private async Task<List<CurentaTaskViewModel>> FillPatientsNamesAsync(List<CurentaTaskViewModel> tasks)
        {
            try
            {
                var patientsIds = new List<Guid>();

                patientsIds.AddRange(tasks.Where(t => t.TaskPatients != null).SelectMany(t => t.TaskPatients).Select(t => t.PatientId).Distinct().ToList());

                if (patientsIds != null && patientsIds.Any())
                {
                    var patientsResult = await _patientClient.GetPatientsNamesByIds(patientsIds);
                    if (patientsResult.IsSuccess == false)
                        return tasks;

                    if (patientsResult != null && patientsResult.IsSuccess && patientsResult.Data != null)
                    {
                        foreach (var task in tasks)
                        {
                            foreach (var patient in task.TaskPatients)
                            {
                                patient.PatientName = patientsResult.Data.FirstOrDefault(u => u.Id == patient.PatientId) != null ? (patientsResult.Data.FirstOrDefault(u => u.Id == patient.PatientId)?.Name) : "Patient Not Found";
                            }
                        }
                    }
                }
                return tasks;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
