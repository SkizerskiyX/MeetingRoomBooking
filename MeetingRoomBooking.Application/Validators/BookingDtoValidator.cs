using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Validators;
using MeetingRoomBooking.Application.Contracts.BookingContacts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeetingRoomBooking.Application.Validators
{
    public class BookingDtoValidator : AbstractValidator<BookingDto>
    {
        public BookingDtoValidator()
        {
            RuleFor(x => x.StartTime)
                .NotEmpty()
                .LessThan(x => x.EndTime)
                .Must(x => x > DateTimeOffset.UtcNow);
            RuleFor(x => x.EndTime)
           .GreaterThan(x => x.StartTime);
        }
    }
}
