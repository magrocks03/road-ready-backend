using AutoMapper;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RoadReadyAPI.Services
{
    public class UserService : IUserService
    {
        // ... (Constructor and Login method are unchanged)
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, ITokenService tokenService, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReturnUserDTO> Login(LoginUserDTO loginUserDTO)
        {
            _logger.LogInformation($"Attempting to log in user with email: {loginUserDTO.Email}");

            var user = await _userRepository.GetUserByEmail(loginUserDTO.Email);

            if (user == null)
            {
                _logger.LogWarning($"Login failed: User with email {loginUserDTO.Email} not found.");
                throw new InvalidCredentialsException("Invalid email or password.");
            }

            using var hmac = new HMACSHA512(user.PasswordHashKey);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginUserDTO.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    _logger.LogWarning($"Login failed: Invalid password for user {loginUserDTO.Email}.");
                    throw new InvalidCredentialsException("Invalid email or password.");
                }
            }

            var loggedInUser = new ReturnUserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.UserRoles.FirstOrDefault()?.Role.Name ?? "Customer",
                Token = _tokenService.GenerateToken(user)
            };

            _logger.LogInformation($"User {loginUserDTO.Email} logged in successfully.");
            return loggedInUser;
        }

        public async Task<ReturnUserDTO> Register(RegisterUserDTO registerUserDTO)
        {
            _logger.LogInformation($"Attempting to register new user with email: {registerUserDTO.Email}");

            var existingUser = await _userRepository.GetUserByEmail(registerUserDTO.Email);
            if (existingUser != null)
            {
                _logger.LogWarning($"Registration failed: Email {registerUserDTO.Email} already exists.");
                throw new UserAlreadyExistsException("A user with this email already exists.");
            }

            var user = _mapper.Map<User>(registerUserDTO);

            using var hmac = new HMACSHA512();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerUserDTO.Password));
            user.PasswordHashKey = hmac.Key;

            var addedUser = await _userRepository.Add(user);

            // --- CHANGE IS HERE ---
            // We now call our new, simple method from the role repository.
            var customerRole = await _roleRepository.GetRoleByNameAsync("Customer");
            if (customerRole == null)
            {
                _logger.LogError("Registration failed: 'Customer' role not found in the database.");
                throw new InvalidOperationException("'Customer' role must be seeded in the database.");
            }

            var userRole = new UserRole { UserId = addedUser.Id, RoleId = customerRole.Id };
            await _userRoleRepository.Add(userRole);

            addedUser.UserRoles.Add(new UserRole { Role = customerRole });

            var registeredUser = new ReturnUserDTO
            {
                Id = addedUser.Id,
                FirstName = addedUser.FirstName,
                LastName = addedUser.LastName,
                Email = addedUser.Email,
                PhoneNumber = addedUser.PhoneNumber,
                Role = customerRole.Name,
                Token = _tokenService.GenerateToken(addedUser)
            };

            _logger.LogInformation($"User {registerUserDTO.Email} registered successfully.");
            return registeredUser;
        }
    }
}