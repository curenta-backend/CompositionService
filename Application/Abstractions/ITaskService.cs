using CSharpFunctionalExtensions;
using CurentaCommonCore.Models;
using CurentaCommonCore.ResponseObjects;
using Domain.Filters;
using Domain.Responses;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface ITaskService
    {
        public Task<Result<GenericResponse<List<CurentaTaskViewModel>>>> GetOpenTasksFiltered(CurentaTaskFilter filter, PaginationFilter paginationFilter);
        public Task<Result<GenericResponse<List<CurentaTaskHistoryViewModel>>>> GetTasksHistory(CurentaTaskFilter filter, PaginationFilter paginationFilter);

    }
}
