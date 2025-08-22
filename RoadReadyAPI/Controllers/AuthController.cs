using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Exceptions;
using RoadReadyAPI.Interfaces;
using System;
using System.Threading.Tasks;

namespace RoadReadyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService; 
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IPasswordService passwordService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _passwordService = passwordService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ReturnUserDTO), 201)]
        [ProducesResponseType(typeof(ErrorModel), 409)]
        public async Task<ActionResult<ReturnUserDTO>> Register(RegisterUserDTO registerUserDTO)
        {
            try
            {
                var result = await _userService.Register(registerUserDTO);
                return CreatedAtAction(nameof(Register), result);
            }
            catch (UserAlreadyExistsException ex)
            {
                _logger.LogWarning(ex.Message);
                return Conflict(new ErrorModel(409, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during registration.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ReturnUserDTO), 200)]
        [ProducesResponseType(typeof(ErrorModel), 401)]
        public async Task<ActionResult<ReturnUserDTO>> Login(LoginUserDTO loginUserDTO)
        {
            try
            {
                var result = await _userService.Login(loginUserDTO);
                return Ok(result);
            }
            catch (InvalidCredentialsException ex)
            {
                _logger.LogWarning(ex.Message);
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during login.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }



        /// <summary>
        /// Initiates the password reset process for a user.
        /// </summary>
        /// <param name="forgotPasswordDTO">The DTO containing the user's email.</param>
        /// <returns>A success message.</returns>
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(ErrorModel), 500)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            try
            {
                await _passwordService.ForgotPasswordAsync(forgotPasswordDTO);
                // For security, always return a generic success message.
                return Ok(new { message = "If an account with that email exists, a password reset link has been sent." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the forgot password process.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }

        /// <summary>
        /// Resets a user's password using a valid token.
        /// </summary>
        /// <param name="resetPasswordDTO">The DTO containing the token and new password.</param>
        /// <returns>A success message.</returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var result = await _passwordService.ResetPasswordAsync(resetPasswordDTO);
                if (result)
                {
                    return Ok(new { message = "Password has been reset successfully." });
                }
                // This line is unlikely to be hit due to exceptions being thrown, but is good practice.
                return BadRequest(new ErrorModel(400, "Password reset failed."));
            }
            catch (InvalidCredentialsException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the password reset process.");
                return StatusCode(500, new ErrorModel(500, "An internal server error occurred."));
            }
        }
    }
}
