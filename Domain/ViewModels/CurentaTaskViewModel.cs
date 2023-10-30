using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class CurentaTaskViewModel
    {
        public CurentaTaskViewModel()
        {
            TaskAssignees = new List<TaskAssigneeViewModel>();
            TaskPatients = new List<TaskPatientViewModel>();
            TaskCompletionDetails = new List<TaskCompletionDetailsViewModel>();
        }

        public int SortNum { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
        public string? Severity { get; set; }
        public string? TaskAssignmentType { get; set; }
        public long? FacilityId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedByDisplayName { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CompletedBy { get; set; }
        public string? CompletedByName { get; set; }
        public string? CompletedByDisplayName { get; set; }
        public string? RedirectUrl { get; set; }
        public virtual List<TaskAssigneeViewModel>? TaskAssignees { get; set; }
        public virtual List<TaskPatientViewModel>? TaskPatients { get; set; }
        public virtual List<TaskCompletionDetailsViewModel>? TaskCompletionDetails { get; set; }
    }

    public class TaskAssigneeViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class TaskPatientViewModel
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public bool? IsCompleted { get; set; }
        public long? CompletedBy { get; set; }
        public string? CompletedByName { get; set; }
        public string? CompletedByDisplayName { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class TaskCompletionDetailsViewModel
    {
        public Guid? PatientId { get; set; }
        public string? PatientName { get; set; }
        public bool? IsCompleted { get; set; }
        public long? CompletedBy { get; set; }
        public string? CompletedByName { get; set; }
        public string? CompletedByDisplayName { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
