using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Responses
{
    public record GraphQLPatientResponse
    {
        public GraphQLPatientResponseData data { get; set; }
    }

    public record GraphQLPatientResponseData
    {
        public GraphQLSearchPatientsByNameAndFacilityId searchPatients { get; set; }
    }

    public record GraphQLSearchPatientsByNameAndFacilityId
    {
        public PageInfo pageInfo { get; set; }
        public List<PatientModel> nodes { get; set; }

    }

    public record PageInfo
    {
        public bool HasNextPage { get; set; }
        public string StartCursor { get; set; }
        public string EndCursor { get; set; }
    }
}
