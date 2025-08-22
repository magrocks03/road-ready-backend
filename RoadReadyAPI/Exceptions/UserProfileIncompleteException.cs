using System;

namespace RoadReadyAPI.Exceptions
{
    public class UserProfileIncompleteException : Exception
    {
        public UserProfileIncompleteException() : base("User profile is incomplete. Please add your address before making a booking.")
        {
        }

        public UserProfileIncompleteException(string message) : base(message)
        {
        }
    }
}