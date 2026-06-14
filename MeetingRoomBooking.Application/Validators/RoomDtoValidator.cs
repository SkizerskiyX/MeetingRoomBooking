using FluentValidation;
using MeetingRoomBooking.Application.Contracts.RoomContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeetingRoomBooking.Application.Validators
{
    public class RoomDtoValidator : AbstractValidator<RoomDto>
    {
        public RoomDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.Capacity)
                .InclusiveBetween(2, 50);
        }
    }
}
