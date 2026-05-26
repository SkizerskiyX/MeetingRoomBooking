using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetingRoomBooking.Domain.Entities;

namespace MeetingRoomBooking.Infrastructure.Configuration
{
    internal class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Capacity).IsRequired();

            builder.HasMany(r => r.Booking)
                   .WithOne(b => b.Room)
                   .HasForeignKey(b => b.RoomId)
                   .IsRequired();
        }
    }
}
