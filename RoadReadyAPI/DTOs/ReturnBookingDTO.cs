using System;
using System.Collections.Generic;

namespace RoadReadyAPI.DTOs
{
    public class ReturnBookingDTO
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; } = string.Empty;

        // Vehicle Details
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;

        // Location Details
        public string PickupLocation { get; set; } = string.Empty;

        // Selected Extras
        public List<ReturnExtraDTO> SelectedExtras { get; set; } = new List<ReturnExtraDTO>();

        // Payment Details
        public ReturnPaymentDTO? Payment { get; set; }
    }
}