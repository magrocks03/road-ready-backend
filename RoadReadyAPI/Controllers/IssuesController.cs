using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RoadReadyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly IIssueService _issueService;
        private readonly ILogger<IssuesController> _logger;

        public IssuesController(IIssueService issueService, ILogger<IssuesController> logger)
        {
            _issueService = issueService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ReturnIssueDTO), 201)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [ProducesResponseType(typeof(ErrorModel), 404)]
        public async Task<ActionResult<ReturnIssueDTO>> ReportIssue(CreateIssueDTO createIssueDTO)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue("userId"));
                var result = await _issueService.ReportIssueAsync(userId, createIssueDTO);
                return CreatedAtAction(nameof(ReportIssue), result);
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "Issue reporting failed: {Message}", ex.Message);
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (IssueReportingException ex)
            {
                _logger.LogWarning(ex, "Issue reporting failed: {Message}", ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while reporting an issue.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Gets a list of all issues reported by the currently logged-in user.
        /// </summary>
        [HttpGet("my-issues")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(PagedResultDTO<ReturnIssueDTO>), 200)]
        public async Task<ActionResult<PagedResultDTO<ReturnIssueDTO>>> GetMyIssues([FromQuery] PaginationDTO pagination)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue("userId"));
                var issues = await _issueService.GetUserIssuesAsync(userId, pagination);
                return Ok(issues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user issues.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }
    }
}