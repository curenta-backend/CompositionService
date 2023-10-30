using CSharpFunctionalExtensions;
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
    public interface IPatientService
    {
        Task<Result<PatientResponse>> GetPatientsListWithTasksCountFiltered(PatientFilter filter);
    }
}
