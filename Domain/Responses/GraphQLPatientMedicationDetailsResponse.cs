using CurentaCommonCore.Medications;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Responses
{
    public class GraphQLPatientMedicationDetailsResponse
    {
        public GraphQLPatientMedicationDetailsResponseData data { get; set; }
    }

    public record GraphQLPatientMedicationDetailsResponseData
    {
        public List<PatientMedicationDetailedRecordModel> activePatientsMedicationsDetailsForDate { get; set; }
    }
}
