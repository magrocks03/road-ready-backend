using AutoMapper;
using RoadReadyAPI.DTOs;
using RoadReadyAPI.Models;
using System.Linq;

namespace RoadReadyAPI.Mappers
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            CreateMap<Extra, ReturnExtraDTO>()
                .ForMember(dest => dest.ExtraId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Payment, ReturnPaymentDTO>()
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Booking, ReturnBookingDTO>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.Vehicle.Id))
                .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle.Name))
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle.Model))
                .ForMember(dest => dest.PickupLocation, opt => opt.MapFrom(src => src.Vehicle.Location.StoreName))
                .ForMember(dest => dest.SelectedExtras, opt => opt.MapFrom(src => src.BookingExtras.Select(be => be.Extra)))
                // --- FIX IS HERE ---
                // We now map directly from the new Payment property.
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment));
        }
    }
}
