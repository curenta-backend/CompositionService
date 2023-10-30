using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Filters
{
    public class PatientFilter
    {
        public long? FacilityId { get; set; }
        public string? PatientName { get; set; }
        public string? PatientStatus { get; set; } = string.Empty;
        public string? Phonenumber { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
        public DateOnly? DOB { get; set; }
        public string? WingId { get; set; } = string.Empty;
        public string? Room { get; set; } = string.Empty;
        public int PageLimit { get; set; } = 50;
        public string? After { get; set; } = string.Empty;
    }
}
