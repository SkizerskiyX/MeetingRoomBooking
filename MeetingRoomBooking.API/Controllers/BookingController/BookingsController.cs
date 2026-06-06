using MeetingRoomBooking.Domain.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.API.Controllers.BookingController
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        public BookingsController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        [HttpGet]

         
    }
}
