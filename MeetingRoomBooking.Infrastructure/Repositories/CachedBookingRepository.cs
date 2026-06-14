using MeetingRoomBooking.Domain.Abstraction;
using MeetingRoomBooking.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace MeetingRoomBooking.Infrastructure.Repositories
{
    public class CachedBookingRepository : IBookingRepository
    {
        private readonly IDistributedCache _cache;
        private readonly IBookingRepository _innerRepo;

        private const string RoomBookingsKeyPrefix = "bookings_room_";
        private const string BookingKeyPrefix = "booking_";

        public CachedBookingRepository(IDistributedCache cache, IBookingRepository innerRepo)
        {
            _cache = cache;
            _innerRepo = innerRepo;
        }
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        public async Task AddAsync(Booking booking, CancellationToken ct = default)
        {
            await _innerRepo.AddAsync(booking, ct);
            await _cache.RemoveAsync(RoomBookingsKeyPrefix + booking.RoomId, ct);
            await _cache.RemoveAsync(BookingKeyPrefix + booking.Id, ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var booking = await _innerRepo.GetByIdAsync(id, ct);
            await _innerRepo.DeleteAsync(id, ct);

            if (booking != null)
            {
                await _cache.RemoveAsync(RoomBookingsKeyPrefix + booking.RoomId, ct);
            }

            await _cache.RemoveAsync(BookingKeyPrefix + id, ct);
        }

        public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var key = BookingKeyPrefix + id;
            var cached = await _cache.GetStringAsync(key, ct);
            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<Booking?>(cached, _jsonOptions);
            }

            var booking = await _innerRepo.GetByIdAsync(id, ct);
            if (booking != null)
            {
                var json = JsonSerializer.Serialize(booking, _jsonOptions);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };
                await _cache.SetStringAsync(key, json, options, ct);
            }

            return booking;
        }

        public async Task<IEnumerable<Booking>> GetByRoomIdAsync(Guid roomId, CancellationToken ct = default)
        {
            var key = RoomBookingsKeyPrefix + roomId;
            var cached = await _cache.GetStringAsync(key, ct);
            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<IEnumerable<Booking>>(cached, _jsonOptions) ?? Enumerable.Empty<Booking>();
            }

            var bookings = await _innerRepo.GetByRoomIdAsync(roomId, ct);
            var json = JsonSerializer.Serialize(bookings, _jsonOptions);
            var options = new DistributedCacheEntryOptions
            {
               AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
           

            await _cache.SetStringAsync(key, json, options, ct);

            return bookings;
        }

        public async Task<bool> HasConflictAsync(Guid roomId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken ct = default)
        {
            return await _innerRepo.HasConflictAsync(roomId, startTime, endTime, ct);
        }
    }
}
