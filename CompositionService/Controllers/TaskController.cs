using CurentaCommonCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static CurentaCommonCore.Enums.EnumsCollection;
using System.Data;
using Domain.Filters;
using Domain.Responses;
using Application.Abstractions;
using CurentaCommonCore.ResponseObjects;
using Domain.ViewModels;

namespace CompositionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("GetOpenTasks")]
        [Authorize]
        public async Task<IActionResult> GetOpenTasks([FromQuery] CurentaTaskFilter filter, [FromQuery] PaginationFilter paginationFilter)
        {
            try
            {
                //var userRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
                var res = await _taskService.GetOpenTasksFiltered(filter, paginationFilter);
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
