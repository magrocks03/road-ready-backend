using System;

namespace RoadReadyAPI.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when attempting to register a user with an email that already exists.
    /// </summary>
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException() : base("A user with this email already exists.")
        {
        }

        public UserAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
