using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public record PatientViewModel : PatientModel
    {
        public int? OpenTasksCount { get; set; } = 0;
    }
}
