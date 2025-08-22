using System;

namespace RoadReadyAPI.Exceptions
{
    /// <summary>
    /// A generic exception for when an entity (like a User, Vehicle, etc.) is not found in the database.
    /// </summary>
    public class NoSuchEntityException : Exception
    {
        public NoSuchEntityException() : base("The requested entity was not found.")
        {
        }

        public NoSuchEntityException(string message) : base(message)
        {
        }
    }
}