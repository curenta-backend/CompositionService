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
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet("GetPatientsWithTasksCount")]
        [Authorize(Roles = nameof(UserRoles.CurentaSuperAdmin) + "," + nameof(UserRoles.CurentaOps) + "," + nameof(UserRoles.LTCAdmin) + "," + nameof(UserRoles.Nurse) + "," + nameof(UserRoles.Doctor) + "," + nameof(UserRoles.CareGiver) + "," + nameof(UserRoles.MedTech))]
        [EnableCors]
        public async Task<IActionResult> GetPatientsWithTasksCount([FromBody] PatientFilter filters)
        {
            try
            {
                var res =  await _patientService.GetPatientsListWithTasksCountFiltered(filters);
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
