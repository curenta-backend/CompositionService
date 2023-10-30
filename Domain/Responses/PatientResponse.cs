using Domain.Models;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Responses
{
    public record PatientResponse 
    {
        public PatientResponseData data { get; set; }
    }

    public record PatientResponseData
    {
        public PatientSearchPatientsByNameAndFacilityId searchPatients { get; set; }
    }

    public record PatientSearchPatientsByNameAndFacilityId
    {
        public PageInfo pageInfo { get; set; }
        public List<PatientViewModel> nodes { get; set; }

    }
}
