import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Calendar, Clock, Building2, Trash2 } from 'lucide-react';
import { format, parseISO } from 'date-fns';
import toast from 'react-hot-toast';
import { roomsApi } from '../api/rooms';
import { bookingsApi } from '../api/bookings';
import type { Room, BookingResponse } from '../types';
import Card from '../components/ui/Card';
import Button from '../components/ui/Button';
import { PageSpinner } from '../components/ui/Spinner';
import { getErrorMessage } from '../utils/error';

interface BookingWithRoom extends BookingResponse {
  roomName?: string;
}

export default function MyBookings() {
  const [bookings, setBookings] = useState<BookingWithRoom[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => { loadAllBookings(); }, []);

  const loadAllBookings = async () => {
    try {
      setIsLoading(true);
      const rooms = await roomsApi.getAll();
      const allBookingsPromises = rooms.map(async (room: Room) => {
        try {
          const roomBookings = await bookingsApi.getByRoom(room.id);
          return roomBookings.map((b: BookingResponse) => ({ ...b, roomName: room.name }));
        } catch { return []; }
      });
      const nested = await Promise.all(allBookingsPromises);
      const flat = nested.flat().sort((a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime());
      setBookings(flat);
    } catch (err) { setError(getErrorMessage(err)); }
    finally { setIsLoading(false); }
  };

  const handleCancel = async (booking: BookingWithRoom) => {
    setDeletingId(booking.id);
    try {
      await bookingsApi.delete(booking.roomId, booking.id);
      setBookings(prev => prev.filter(b => b.id !== booking.id));
      toast.success('Booking cancelled');
    } catch (err) { toast.error(getErrorMessage(err)); }
    finally { setDeletingId(null); }
  };

  const formatDateTime = (iso: string) => {
    const date = parseISO(iso);
    return { date: format(date, 'EEE, MMM d, yyyy'), time: format(date, 'h:mm a') };
  };

  const isPast = (endTime: string) => new Date(endTime) < new Date();
  const upcoming = bookings.filter(b => !isPast(b.endTime));
  const past = bookings.filter(b => isPast(b.endTime));

  if (isLoading) return <PageSpinner />;

  return (
    <div className="space-y-8">
      <motion.div initial={{ opacity: 0, y: -20 }} animate={{ opacity: 1, y: 0 }}>
        <h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100">My Bookings</h1>
        <p className="text-gray-500 mt-1">
          {upcoming.length > 0 ? `You have ${upcoming.length} upcoming booking${upcoming.length > 1 ? 's' : ''}` : 'No upcoming bookings'}
        </p>
      </motion.div>

      {error && (
        <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="p-4 bg-red-50 border border-red-200 rounded-xl text-sm text-red-700">
          {error}
        </motion.div>
      )}

      <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.1 }}>
        <h2 className="text-xl font-semibold text-gray-900 mb-4 flex items-center gap-2">
          <Clock className="h-5 w-5 text-green-500" /> Upcoming ({upcoming.length})
        </h2>
        {upcoming.length === 0 ? (
          <Card className="p-8 text-center">
            <Calendar className="h-12 w-12 text-gray-300 mx-auto mb-3" />
            <p className="text-gray-500">No upcoming bookings</p>
            <Button variant="secondary" size="sm" onClick={() => navigate('/rooms')} className="mt-3">Browse rooms</Button>
          </Card>
        ) : (
          <div className="space-y-3">
            {upcoming.map((booking, index) => {
              const start = formatDateTime(booking.startTime);
              const end = formatDateTime(booking.endTime);
              return (
                <motion.div key={booking.id} initial={{ opacity: 0, x: -20 }} animate={{ opacity: 1, x: 0 }} transition={{ delay: index * 0.05 }}>
                  <Card className="p-5 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
                    <div className="flex items-start gap-4">
                      <div className="w-10 h-10 bg-primary-50 rounded-xl flex items-center justify-center flex-shrink-0">
                        <Building2 className="h-5 w-5 text-primary-600" />
                      </div>
                      <div>
                        <p className="font-medium text-gray-900 dark:text-gray-100">{booking.roomName || 'Unknown Room'}</p>
                        <p className="text-sm text-gray-500 mt-0.5">{start.date}</p>
                        <div className="flex items-center gap-1 mt-1 text-sm text-gray-500">
                          <Clock className="h-3.5 w-3.5" /> {start.time} - {end.time}
                        </div>
                      </div>
                    </div>
                    <Button variant="danger" size="sm" isLoading={deletingId === booking.id} onClick={() => handleCancel(booking)}>
                      <Trash2 className="h-4 w-4" /> Cancel
                    </Button>
                  </Card>
                </motion.div>
              );
            })}
          </div>
        )}
      </motion.div>
      {past.length > 0 && (
        <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: 0.3 }}>
          <details className="group">
            <summary className="cursor-pointer text-sm text-gray-500 hover:text-gray-700 transition-colors font-medium flex items-center gap-2">
              <Clock className="h-4 w-4" /> Past Bookings ({past.length})
            </summary>
            <div className="mt-3 space-y-2">
              {past.map((booking) => {
                const start = formatDateTime(booking.startTime);
                return (
                  <Card key={booking.id} className="p-4 opacity-60">
                    <div className="flex items-center gap-3">
                      <div className="w-8 h-8 bg-gray-100 rounded-lg flex items-center justify-center"><Calendar className="h-4 w-4 text-gray-400" /></div>
                      <div className="text-sm">
                        <p className="text-gray-600">{booking.roomName || 'Unknown Room'}</p>
                        <p className="text-gray-400 text-xs">{start.date} at {start.time}</p>
                      </div>
                    </div>
                  </Card>
                );
              })}
            </div>
          </details>
        </motion.div>
      )}
    </div>
  );
}

