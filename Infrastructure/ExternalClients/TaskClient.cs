using CSharpFunctionalExtensions;
using CurentaCommonCore.MessagesContract.PioneerRxMsgs;
using CurentaCommonCore.Models;
using CurentaCommonCore.PatientObjects;
using CurentaCommonCore.ResponseObjects;
using Domain.Abstractions;
using Domain.Filters;
using Domain.ViewModels;
using GraphQL;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalClients
{
    public class TaskClient : ITaskClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        IConfiguration _configuration;

        public TaskClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<Result<List<PatientTaskCountModel>>> GetOpenTasksCountPerPatient(long? facilityId)
        {
            try
            {
                var url = _configuration["Urls:TaskManagerServiceURL"] + "/api/Task/GetOpenTasksCountPerPatient";
                if (facilityId != null && facilityId.Value > 0)
                    url = url + "?facilityId=" + facilityId;
                using var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);
                var res = JsonConvert.DeserializeObject<List<PatientTaskCountModel>>(response.Content.ReadAsStringAsync().Result);
                return res;
            }
            catch (Exception ex)
            {
                return Result.Failure<List<PatientTaskCountModel>>(ex.Message);
            }
        }

        public async Task<Result<GenericResponse<List<CurentaTaskViewModel>>>> GetOpenTasksFiltered(CurentaTaskFilter filter, PaginationFilter paginationFilter)
        {
            try
            {
                var url = _configuration["Urls:TaskManagerServiceURL"] + "/api/Task/GetOpenTasks";
                var queryParameters = new Dictionary<string, string>();

                // Conditionally add parameters to the query dictionary if they have values
                if (filter.LoggedInUserId.HasValue)
                {
                    queryParameters["LoggedInUserId"] = filter.LoggedInUserId.ToString();
                }
                if (filter.CreatedByUserId.HasValue)
                {
                    queryParameters["CreatedByUserId"] = filter.CreatedByUserId.ToString();
                }
                if (filter.AssignedToUserId.HasValue)
                {
                    queryParameters["AssignedToUserId"] = filter.AssignedToUserId.ToString();
                }
                if (filter.FacilityId.HasValue)
                {
                    queryParameters["FacilityId"] = filter.FacilityId.ToString();
                }
                if (filter.PatientId.HasValue)
                {
                    queryParameters["PatientId"] = filter.PatientId.ToString();
                }
                if (filter.TaskSeverity.HasValue)
                {
                    queryParameters["TaskSeverity"] = filter.TaskSeverity.ToString();
                }
                if (filter.IncludeTaskWithId.HasValue)
                {
                    queryParameters["IncludeTaskWithId"] = filter.IncludeTaskWithId.ToString();
                }
                queryParameters["PageNumber"] = paginationFilter.PageNumber.ToString();
                queryParameters["PageSize"] = paginationFilter.PageSize.ToString();

                // Create a query string from the dictionary
                var queryString = string.Join("&", queryParameters.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));

                // Append the query string to the URL
                if (!string.IsNullOrEmpty(queryString))
                {
                    url += "?" + queryString;
                }

                using var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);
                var res = JsonConvert.DeserializeObject < GenericResponse<List<CurentaTaskViewModel>>>(await response.Content.ReadAsStringAsync());

                if (res == null || !res.IsSuccess || ( res.ValidationErrors != null && res.ValidationErrors.Any()))
                {
                    return Result.Failure< GenericResponse<List<CurentaTaskViewModel>>>(string.Join("; ", res.ValidationErrors.Select(e => e.Error)));
                }

                return res;
            }
            catch (Exception ex)
            {
                return Result.Failure< GenericResponse<List<CurentaTaskViewModel>>>(ex.Message);
            }
        }

    }
}
