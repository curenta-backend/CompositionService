using CurentaCommonCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static CurentaCommonCore.Enums.EnumsCollection;
using System.Data;
using Domain.Filters;
using Domain.Responses;
using Application.Abstractions;

namespace CompositionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientMedicationController : ControllerBase
    {
        private readonly IPatientMedicationService _patientMedicationService;
        public PatientMedicationController(IPatientMedicationService patientMedicationService)
        {
            _patientMedicationService = patientMedicationService;
        }

        [HttpGet("GetActivePatientsMedicationsDetailsForDate")]
        [Authorize(Roles = nameof(UserRoles.CurentaSuperAdmin) + "," + nameof(UserRoles.CurentaOps) + "," + nameof(UserRoles.LTCAdmin) + "," + nameof(UserRoles.Nurse) + "," + nameof(UserRoles.Doctor) + "," + nameof(UserRoles.CareGiver) + "," + nameof(UserRoles.MedTech))]
        [EnableCors]
        public async Task<IActionResult> GetActivePatientsMedicationsDetailsForDate([ FromQuery] DateOnly date, [FromQuery] long? facilityId, [FromQuery] Guid? patientId, [FromQuery] Guid? patientMedicationId)
        {
            try
            {
                var res =  await _patientMedicationService.GetActivePatientsMedicationsDetailsForDate(date, facilityId, patientId, patientMedicationId);
                if (res.IsFailure)
                    return BadRequest(res.Error);
                return Ok(res.Value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
