using AutoMapper;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoadReadyAPI.Services
{
    public class IssueService : IIssueService
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<IssueService> _logger;

        public IssueService(
            IIssueRepository issueRepository,
            IBookingRepository bookingRepository,
            IMapper mapper,
            ILogger<IssueService> logger)
        {
            _issueRepository = issueRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReturnIssueDTO> ReportIssueAsync(int userId, CreateIssueDTO createIssueDTO)
        {
            _logger.LogInformation($"User {userId} attempting to report an issue for booking {createIssueDTO.BookingId}");

            // 1. Validate the booking
            var booking = await _bookingRepository.GetBookingDetailsByIdAsync(createIssueDTO.BookingId);
            if (booking == null)
            {
                throw new NoSuchEntityException("Booking not found.");
            }
            if (booking.UserId != userId)
            {
                throw new IssueReportingException("You can only report issues for your own bookings.");
            }

            // 2. Business Rule: Check if the booking is currently active
            var now = DateTime.UtcNow;
            if (now < booking.StartDate || now > booking.EndDate)
            {
                throw new IssueReportingException("You can only report an issue during the active rental period.");
            }

            // 3. Create and add the new issue
            var newIssue = _mapper.Map<Issue>(createIssueDTO);
            newIssue.ReportedAt = now;
            newIssue.Status = "Open"; // Default status

            var addedIssue = await _issueRepository.Add(newIssue);

            // 4. Return the full issue details
            var fullIssue = await _issueRepository.GetIssueDetailsByIdAsync(addedIssue.Id);

            _logger.LogInformation($"Issue {addedIssue.Id} reported successfully for booking {createIssueDTO.BookingId}.");
            return _mapper.Map<ReturnIssueDTO>(fullIssue);
        }

        public async Task<PagedResultDTO<ReturnIssueDTO>> GetUserIssuesAsync(int userId, PaginationDTO pagination)
        {
            _logger.LogInformation($"Fetching paginated issues for user ID {userId}");

            var totalCount = await _issueRepository.GetUserIssuesCountAsync(userId);
            var issues = await _issueRepository.GetPagedUserIssuesAsync(userId, pagination);

            var issueDtos = _mapper.Map<List<ReturnIssueDTO>>(issues);

            return new PagedResultDTO<ReturnIssueDTO>
            {
                Items = issueDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<PagedResultDTO<ReturnIssueDTO>> GetAllIssuesAsync(PaginationDTO pagination)
        {
            _logger.LogInformation("Fetching paginated list of all reported issues.");

            var totalCount = await _issueRepository.GetTotalAllIssuesCountAsync();
            var issues = await _issueRepository.GetPagedAllIssuesAsync(pagination);

            var issueDtos = _mapper.Map<List<ReturnIssueDTO>>(issues);

            return new PagedResultDTO<ReturnIssueDTO>
            {
                Items = issueDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }
    }
}
