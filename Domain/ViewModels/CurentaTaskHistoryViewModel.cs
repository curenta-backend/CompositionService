using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class CurentaTaskHistoryViewModel : CurentaTaskViewModel
    {
        public int HistoryRecordId { get; set; }
        public string? Action { get; set; }
        public DateTime? ActionAt { get; set; }
        public int? ActionBy { get; set; }
        public string? ActionByName { get; set; }
        public string? ActionByDisplayName { get; set; }
        public int AssigneeUserId { get; set; }
        public Guid? PatientId { get; set; }
    }
}
