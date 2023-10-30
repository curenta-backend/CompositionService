using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using CurentaCommonCore.Medications;
using CurentaCommonCore.ResponseObjects;
using Domain.Abstractions;
using Domain.Filters;
using Domain.Models;
using Domain.Responses;
using Domain.ViewModels;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalClients
{
    public class PatientClient : IPatientClient
    {
        IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _token;

        public PatientClient(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _token = _httpContextAccessor.HttpContext?.GetTokenAsync("access_token").Result;
        }

        public async Task<Result<GraphQLPatientResponse>> GetPatientsListFiltered(PatientFilter filter)
        {
            try
            {
                var graphqlRequest = new GraphQLRequest(BuildGetPatientsGraphQLQuery(filter));

                var graphQlClient = new GraphQLHttpClient(new GraphQLHttpClientOptions
                {
                    EndPoint = new Uri(_configuration["Urls:PatientServiceURL"]  + "/graphql/"),
                }, new NewtonsoftJsonSerializer());

                graphQlClient.HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);

                var graphQlResponse = await graphQlClient.SendQueryAsync<GraphQLPatientResponseData>(graphqlRequest);

                if (graphQlResponse.Errors != null || graphQlResponse.Data == null)
                {
                    return Result.Failure<GraphQLPatientResponse>("GraphQL query error: " + string.Join("; ", graphQlResponse.Errors.Select(e => e.Message)));
                }

                return Result.Success<GraphQLPatientResponse>(new GraphQLPatientResponse() { data = graphQlResponse.Data });                //return Result.Success<GraphQLPatientResponse>(null); ;
            }
            catch (Exception ex)
            {
                return Result.Failure<GraphQLPatientResponse>(ex.Message);
            }
        }

        private string BuildGetPatientsGraphQLQuery(PatientFilter filters)
        {
            //  searchPatients(first: $pageLimit, after: $endCursor, facilityId: $facilityID, patientFullName: $searchText) {
            var query = @"
{
    searchPatients(
        first: $pageLimit,
        after: $endCursor,
        facilityId: $facilityId,
        patientFullName: $patientFullName,
        patientStatus: $patientStatus,
        phoneNumber: $phoneNumber,
        gender: $gender,
        dob: $dob,
        wingId: $wingId,
        room: $room
    ) {
    pageInfo {
      hasNextPage
      startCursor
      endCursor
    }
    nodes {
      id
      basicInfo {
        firstName
        lastName
        dateOfBirth
        email
        phoneNumber
        gender
      }
      placesOfService {
        facilityId
      }
      addresses {
        id
        fullAddress
        street
        city
        state
        zipCode
        isDefault
        longitude
        latitude
      }
      status
      deliveryNote
      isBubblePackEnabled
      profilePicture
    }
  }
}
";

            // Initialize a dictionary to hold the query parameters
            var queryParameters = new Dictionary<string, object>
            {
                { "pageLimit", filters.PageLimit }
            };

            queryParameters.Add("endCursor", "\"" + filters.After + "\"");

            if (filters.FacilityId.HasValue)
                queryParameters.Add("facilityId", filters.FacilityId);
            else
                queryParameters.Add("facilityId", "null");

            queryParameters.Add("patientFullName", "\"" + filters.PatientName + "\"");

            queryParameters.Add("patientStatus", "\"" + filters.PatientStatus + "\"");

            queryParameters.Add("phoneNumber", "\"" + filters.Phonenumber + "\"");

            queryParameters.Add("gender", "\"" + filters.Gender + "\"");

            if (filters.DOB.HasValue)
                queryParameters.Add("dob", "\"" + filters.DOB + "\"");
            else
                queryParameters.Add("dob", "null");

            queryParameters.Add("wingId", "\"" + filters.WingId + "\"");

            queryParameters.Add("room", "\"" + filters.Room + "\"");

            // Replace the parameter placeholders in the query string
            foreach (var parameter in queryParameters)
            {
                query = query.Replace("$" + parameter.Key, parameter.Value.ToString());
            }

            return query;
        }

        public async Task<GenericResponse<List<GUIDNameResponse>>> GetPatientsNamesByIds(List<Guid> ids)
        {
            var res = new GenericResponse<List<GUIDNameResponse>>();

            try
            {
                var tasks = SplitAndExecuteGraphQLQueriesForGetPatientsNamesByIds(ids);
                var results = await Task.WhenAll(tasks);

                // Merge the results
                var mergedResults = results.SelectMany(result => result.Data).ToList();

                return new GenericResponse<List<GUIDNameResponse>>
                {
                    IsSuccess = true,
                    Data = mergedResults
                };
            }
            catch (Exception ex)
            {
                res = new GenericResponse<List<GUIDNameResponse>>()
                {
                    IsSuccess = false,
                    Message = "Exception: " + ex.Message,
                };
            }

            return res;
        }

        private IEnumerable<Task<GenericResponse<List<GUIDNameResponse>>>> SplitAndExecuteGraphQLQueriesForGetPatientsNamesByIds(List<Guid> ids)
        {
            var idChunks = ChunkIds(ids, 10);
            return idChunks.Select(chunk => ExecuteGraphQLQueryToGetPatientsAsync(chunk));
        }

        private IEnumerable<IEnumerable<Guid>> ChunkIds(IEnumerable<Guid> ids, int chunkSize)
        {
            for (int i = 0; i < ids.Count(); i += chunkSize)
            {
                yield return ids.Skip(i).Take(chunkSize);
            }
        }

        private async Task<GenericResponse<List<GUIDNameResponse>>> ExecuteGraphQLQueryToGetPatientsAsync(IEnumerable<Guid> ids)
        {
            var graphqlRequest = new GraphQLRequest
            {
                Query = @"
            {
              patients(where: { id: { in: [" + string.Join(", ", ids.Select(id => $"\"{id}\"")) + @"] } }) {
                nodes {
                  id
                  basicInfo {
                    firstName
                    lastName
                  }
                }
              }
            }"
            };

            var graphQlClient = new GraphQLHttpClient(new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(_configuration["Urls:PatientServiceURL"] + "/graphql/"),
            }, new NewtonsoftJsonSerializer());

            graphQlClient.HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);

            var graphQlResponse = await graphQlClient.SendQueryAsync<GraphQLResponseGetPatientsNamesByIds>(graphqlRequest);

            if (graphQlResponse.Errors != null)
            {
                return new GenericResponse<List<GUIDNameResponse>>()
                {
                    IsSuccess = false,
                    Message = "GraphQL query error: " + string.Join("; ", graphQlResponse.Errors.Select(e => e.Message)),
                };
            }
            else if (graphQlResponse.Data != null && graphQlResponse.Data.patients != null)
            {
                var patients = graphQlResponse.Data.patients.nodes.Select(node => new GUIDNameResponse
                {
                    Id = new Guid(node.id),
                    Name = node.basicInfo.firstName + " " + node.basicInfo.lastName
                }).ToList();

                return new GenericResponse<List<GUIDNameResponse>>
                {
                    IsSuccess = true,
                    Data = patients
                };
            }

            return new GenericResponse<List<GUIDNameResponse>>();
        }

        public async Task<Result<List<PatientMedicationDetailedRecordModel>>> GetActivePatientsMedicationsDetailsForDate(DateOnly date, long? facilityId, Guid? patientId, Guid? patientMedicationId)
        {
            try
            {
                var graphqlRequest = new GraphQLRequest(BuildGetActivePatientsMedicationsDetailsForDateQuery(date, facilityId, patientId, patientMedicationId));

                var graphQlClient = new GraphQLHttpClient(new GraphQLHttpClientOptions
                {
                    EndPoint = new Uri(_configuration["Urls:PatientServiceURL"] + "/graphql/"),
                }, new NewtonsoftJsonSerializer());

                graphQlClient.HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);

                var graphQlResponse = await graphQlClient.SendQueryAsync<GraphQLPatientMedicationDetailsResponseData>(graphqlRequest);

                if (graphQlResponse.Errors != null || graphQlResponse.Data == null || graphQlResponse.Data.activePatientsMedicationsDetailsForDate == null)
                {
                    return Result.Failure<List<PatientMedicationDetailedRecordModel>>("GraphQL query error: " + string.Join("; ", graphQlResponse.Errors.Select(e => e.Message)));
                }

                return graphQlResponse.Data.activePatientsMedicationsDetailsForDate;
            }
            catch (Exception ex)
            {
                return Result.Failure<List<PatientMedicationDetailedRecordModel>>(ex.Message);
            }
        }

        private string BuildGetActivePatientsMedicationsDetailsForDateQuery(DateOnly date, long? facilityId, Guid? patientId, Guid? patientMedicationId)
        {
            var query = @"
query ActivePatientsMedicationsDetailsForDate {
    activePatientsMedicationsDetailsForDate(
        date: $date
        facilityId: $facilityId
        patientId: $patientId
        patientMedicationId: $patientMedicationId
    ) {
        facilityIdRef
        facilityName
        wingId
        wingName
        room
        patientId
        fname
        lname
        phonenumber
        gender
        dob
        profilePicPath
        patientMedicationId
        medName
        directions
        frequency
        route
        dosage
        quantity
        isPRN
        patientMedicationAdminHourId
        hour
    }
}
";

            // Initialize a dictionary to hold the query parameters
            var queryParameters = new Dictionary<string, object>
            {
                { "date", "\"" + date.ToString() + "\"" }
            };

            if (facilityId.HasValue)
                queryParameters.Add("facilityId", facilityId.Value);
            else
                queryParameters.Add("facilityId", "null");

            if (patientId.HasValue)
                queryParameters.Add("patientId", patientId.Value);
            else
                queryParameters.Add("patientId", "null");

            if (patientMedicationId.HasValue)
                queryParameters.Add("patientMedicationId", patientMedicationId.Value);
            else
                queryParameters.Add("patientMedicationId", "null");

            // Replace the parameter placeholders in the query string
            foreach (var parameter in queryParameters)
            {
                query = query.Replace("$" + parameter.Key, parameter.Value.ToString());
            }

            return query;
        }
    }
}
