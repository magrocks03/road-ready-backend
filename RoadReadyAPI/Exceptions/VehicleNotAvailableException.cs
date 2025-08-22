using System;

namespace RoadReadyAPI.Exceptions
{
    public class VehicleNotAvailableException : Exception
    {
        public VehicleNotAvailableException() : base("The selected vehicle is not available for the requested dates.")
        {
        }

        public VehicleNotAvailableException(string message) : base(message)
        {
        }
    }
}