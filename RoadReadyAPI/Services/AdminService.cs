using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;

namespace RoadReadyAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IRefundRepository _refundRepository;
        private readonly IBookingStatusRepository _statusRepository;
        private readonly IIssueService _issueService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;


        public AdminService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IBookingRepository bookingRepository,
            IRefundRepository refundRepository,
            IBookingStatusRepository statusRepository,
            IIssueService issueService,
            IMapper mapper,
            ILogger<AdminService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _bookingRepository = bookingRepository;
            _refundRepository = refundRepository;
            _statusRepository = statusRepository;
            _issueService = issueService;
            _mapper = mapper;
            _logger = logger;
        }

        // --- METHOD UPDATED ---
        public async Task<PagedResultDTO<AdminReturnUserDTO>> GetAllUsersAsync(PaginationDTO pagination)
        {
            _logger.LogInformation("Admin fetching paginated list of all users.");

            var totalCount = await _userRepository.GetTotalUserCountAsync();
            var users = await _userRepository.GetPagedUsersAsync(pagination);

            var userDtos = _mapper.Map<List<AdminReturnUserDTO>>(users);

            return new PagedResultDTO<AdminReturnUserDTO>
            {
                Items = userDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        // --- METHOD UPDATED ---
        public async Task<PagedResultDTO<ReturnBookingDTO>> GetAllBookingsAsync(PaginationDTO pagination)
        {
            _logger.LogInformation("Admin fetching paginated list of all bookings.");

            var totalCount = await _bookingRepository.GetTotalAllBookingsCountAsync();
            var bookings = await _bookingRepository.GetPagedAllBookingsAsync(pagination);

            var bookingDtos = _mapper.Map<List<ReturnBookingDTO>>(bookings);

            return new PagedResultDTO<ReturnBookingDTO>
            {
                Items = bookingDtos,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        // --- METHOD UPDATED ---
        public async Task<PagedResultDTO<ReturnIssueDTO>> GetAllIssuesAsync(PaginationDTO pagination)
        {
            _logger.LogInformation("Admin fetching all reported issues.");
            // This method now correctly calls the paginated method from the IssueService.
            return await _issueService.GetAllIssuesAsync(pagination);
        }

        public async Task<AdminReturnUserDTO> UpdateUserRoleAsync(UpdateUserRoleDTO updateUserRoleDTO)
        {
            _logger.LogInformation($"Admin attempting to update role for user ID {updateUserRoleDTO.UserId}");

            var user = await _userRepository.GetById(updateUserRoleDTO.UserId);
            if (user == null) throw new NoSuchEntityException("User not found.");

            var newRole = await _roleRepository.GetRoleByNameAsync(updateUserRoleDTO.RoleName);
            if (newRole == null) throw new NoSuchEntityException("The specified role does not exist.");

            var existingRoles = _userRoleRepository.GetAll().Where(ur => ur.UserId == user.Id).ToList();
            foreach (var role in existingRoles)
            {
                await _userRoleRepository.Delete(role.UserId);
            }

            var newUserRole = new UserRole { UserId = user.Id, RoleId = newRole.Id };
            await _userRoleRepository.Add(newUserRole);

            var updatedUser = await _userRepository.GetAllUsersWithRolesAsync().ContinueWith(t => t.Result.First(u => u.Id == user.Id));
            _logger.LogInformation($"Role for user ID {user.Id} updated to {newRole.Name}.");
            return _mapper.Map<AdminReturnUserDTO>(updatedUser);
        }

        public async Task<ReturnBookingDTO> ProcessRefundAsync(ProcessRefundDTO processRefundDTO, int adminUserId)
        {
            _logger.LogInformation($"Admin {adminUserId} attempting to process refund for booking ID {processRefundDTO.BookingId}");

            var booking = await _bookingRepository.GetBookingDetailsByIdAsync(processRefundDTO.BookingId);
            if (booking == null) throw new NoSuchEntityException("Booking not found.");


            // A refund is allowed if the booking was cancelled properly OR if it's completed (for issue-based refunds).
            if (booking.Status.Name != "Cancelled - Refund Pending" && booking.Status.Name != "Completed")
            {
                throw new AdminActionException("This booking is not in a state that allows for a refund (must be 'Completed' or 'Cancelled - Refund Pending').");
            }

            // Use the provided amount, or default to the full cost of the booking.
            var refundAmount = processRefundDTO.Amount ?? booking.TotalCost;

            var refund = new Refund
            {
                BookingId = booking.Id,
                Amount = refundAmount,
                Reason = processRefundDTO.Reason,
                AdminUserId = adminUserId,
                RefundDate = DateTime.UtcNow
            };
            await _refundRepository.Add(refund);

            // Update the booking status to "Cancelled - Refunded"
            var refundedStatus = await _statusRepository.GetStatusByNameAsync("Cancelled - Refunded");
            if (refundedStatus == null) throw new InvalidOperationException("Status 'Cancelled - Refunded' not found.");

            booking.StatusId = refundedStatus.Id;
            await _bookingRepository.Update(booking);

            _logger.LogInformation($"Refund of {refundAmount} processed successfully for booking ID {booking.Id}.");
            var updatedBooking = await _bookingRepository.GetBookingDetailsByIdAsync(booking.Id);
            return _mapper.Map<ReturnBookingDTO>(updatedBooking);
        }

        // --- NEW METHOD IMPLEMENTATIONS ---

        public async Task<AdminReturnUserDTO> CreateUserWithRoleAsync(AdminCreateUserDTO createUserDTO)
        {
            _logger.LogInformation($"Admin attempting to create a new user with email: {createUserDTO.Email}");

            var existingUser = await _userRepository.GetUserByEmail(createUserDTO.Email);
            if (existingUser != null)
            {
                throw new UserAlreadyExistsException("A user with this email already exists.");
            }

            var role = await _roleRepository.GetRoleByNameAsync(createUserDTO.RoleName);
            if (role == null)
            {
                throw new NoSuchEntityException($"The role '{createUserDTO.RoleName}' does not exist.");
            }

            var user = _mapper.Map<User>(createUserDTO);
            using var hmac = new HMACSHA512();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(createUserDTO.Password));
            user.PasswordHashKey = hmac.Key;

            var addedUser = await _userRepository.Add(user);

            var userRole = new UserRole { UserId = addedUser.Id, RoleId = role.Id };
            await _userRoleRepository.Add(userRole);

            var finalUser = await _userRepository.GetAllUsersWithRolesAsync().ContinueWith(t => t.Result.First(u => u.Id == addedUser.Id));
            _logger.LogInformation($"Admin successfully created user {finalUser.Email} with role {role.Name}.");
            return _mapper.Map<AdminReturnUserDTO>(finalUser);
        }

        public async Task<AdminReturnUserDTO> DeactivateUserAsync(int userId)
        {
            _logger.LogInformation($"Admin attempting to deactivate user ID {userId}");

            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                throw new NoSuchEntityException("User not found.");
            }

            // Deactivation is handled by removing all roles from the user.
            var existingRoles = _userRoleRepository.GetAll().Where(ur => ur.UserId == userId).ToList();
            if (!existingRoles.Any())
            {
                _logger.LogWarning($"User {userId} is already deactivated (has no roles).");
            }

            foreach (var role in existingRoles)
            {
                await _userRoleRepository.Delete(role.UserId);
            }

            var deactivatedUser = await _userRepository.GetAllUsersWithRolesAsync().ContinueWith(t => t.Result.First(u => u.Id == userId));
            _logger.LogInformation($"User {userId} has been deactivated (all roles removed).");
            return _mapper.Map<AdminReturnUserDTO>(deactivatedUser);
        }

        // --- METHOD UPDATED ---
        public async Task<AdminDashboardStatsDTO> GetDashboardStatsAsync()
        {
            _logger.LogInformation("Generating dashboard statistics for admin.");

            var totalUsers = await _userRepository.GetTotalUserCountAsync();
            var totalBookings = await _bookingRepository.GetTotalBookingCountAsync();
            var grossRevenue = await _bookingRepository.GetTotalRevenueAsync();
            var totalRefunds = await _refundRepository.GetTotalRefundsAmountAsync(); // <-- Get total refunds
            var popularVehicles = await _bookingRepository.GetMostPopularVehiclesAsync(5);
            var allBookings = await _bookingRepository.GetAllBookingsWithDetailsAsync();

            var stats = new AdminDashboardStatsDTO
            {
                TotalUsers = totalUsers,
                TotalBookings = totalBookings,
                // --- FIX IS HERE ---
                // We now calculate the net revenue by subtracting refunds.
                TotalRevenue = grossRevenue - totalRefunds,
                MostPopularVehicles = popularVehicles.Select(v => new MostPopularVehicleDTO
                {
                    VehicleId = v.Id,
                    VehicleName = v.Name,
                    BookingCount = allBookings.Count(b => b.VehicleId == v.Id && b.Status.Name == "Completed")
                }).ToList()
            };

            return stats;
        }
    }
}