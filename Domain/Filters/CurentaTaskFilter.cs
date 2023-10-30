using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CurentaCommonCore.Enums.EnumsCollection;

namespace Domain.Filters
{
    public class CurentaTaskFilter
    {
        public long? LoggedInUserId { get; set; }
        public long? CreatedByUserId { get; set; }
        public long? AssignedToUserId { get; set; }
        public long? FacilityId { get; set; }
        public Guid? PatientId { get; set; }

        [EnumDataType(typeof(ETaskSeverity))]
        [JsonConverter(typeof(StringEnumConverter))]
        public ETaskSeverity? TaskSeverity { get; set; }
        public long? IncludeTaskWithId { get; set; }
    }
}
