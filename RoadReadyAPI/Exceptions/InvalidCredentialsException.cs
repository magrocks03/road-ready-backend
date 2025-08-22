using System;

namespace RoadReadyAPI.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when a user provides an invalid email or password during login.
    /// </summary>
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Invalid email or password.")
        {
        }

        public InvalidCredentialsException(string message) : base(message)
        {
        }
    }
}