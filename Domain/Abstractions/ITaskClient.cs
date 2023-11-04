using CSharpFunctionalExtensions;
using CurentaCommonCore.Models;
using CurentaCommonCore.PatientObjects;
using CurentaCommonCore.ResponseObjects;
using Domain.Filters;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface ITaskClient
    {
        Task<Result<List<PatientTaskCountModel>>> GetOpenTasksCountPerPatient(long? facilityId);
        Task<Result<GenericResponse<List<CurentaTaskViewModel>>>> GetOpenTasksFiltered(CurentaTaskFilter filter, PaginationFilter paginationFilter);
        Task<Result<GenericResponse<List<CurentaTaskHistoryViewModel>>>> GetTasksHistory(CurentaTaskFilter filter, PaginationFilter paginationFilter);

    }
}
