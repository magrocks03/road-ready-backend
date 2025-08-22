using System;

namespace RoadReadyAPI.Exceptions
{
    public class AdminActionException : Exception
    {
        public AdminActionException(string message) : base(message)
        {
        }
    }
}