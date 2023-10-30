using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Responses
{
    public class GraphQLResponseGetPatientsNamesByIds
    {
        public PatientsResponseGetPatientsNamesByIds patients { get; set; }
    }

    public class PatientsResponseGetPatientsNamesByIds
    {
        public List<PatientNodeGetPatientsNamesByIds> nodes { get; set; }
    }

    public class PatientNodeGetPatientsNamesByIds
    {
        public string id { get; set; }
        public PatientBasicInfoGetPatientsNamesByIds basicInfo { get; set; }
    }

    public class PatientBasicInfoGetPatientsNamesByIds
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
