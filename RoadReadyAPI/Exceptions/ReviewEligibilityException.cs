using System;

namespace RoadReadyAPI.Exceptions
{
    public class ReviewEligibilityException : Exception
    {
        public ReviewEligibilityException(string message) : base(message)
        {
        }
    }
}