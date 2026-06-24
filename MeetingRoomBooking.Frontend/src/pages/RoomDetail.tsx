import { useState, useEffect, type FormEvent } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import { Building2, Users, Calendar, Clock, ArrowLeft, Plus, X, Trash2 } from 'lucide-react';
import { format, parseISO } from 'date-fns';
import toast from 'react-hot-toast';
import { roomsApi } from '../api/rooms';
import { bookingsApi } from '../api/bookings';
import type { BookingResponse } from '../types';
import Card from '../components/ui/Card';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import { PageSpinner } from '../components/ui/Spinner';
import { getErrorMessage } from '../utils/error';

export default function RoomDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [room, setRoom] = useState<any>(null);
  const [bookings, setBookings] = useState<BookingResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  // improved date/time inputs: separate date and time for better UX
  const [startDate, setStartDate] = useState('');
  const [startTimeOfDay, setStartTimeOfDay] = useState('');
  const [endDate, setEndDate] = useState('');
  const [endTimeOfDay, setEndTimeOfDay] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formError, setFormError] = useState('');

  useEffect(() => { if (id) loadData(); }, [id]);

  // initialize default start/end values to next hour and +1h
  useEffect(() => {
    const now = new Date();
    const next = new Date(now);
    next.setMinutes(0, 0, 0);
    next.setHours(next.getHours() + 1);
    const later = new Date(next);
    later.setHours(later.getHours() + 1);

    // Use local date/time methods consistently (not mixing UTC toISOString with local toTimeString)
    const yyyyLocal = (d: Date) => {
      const y = d.getFullYear();
      const m = String(d.getMonth() + 1).padStart(2, '0');
      const day = String(d.getDate()).padStart(2, '0');
      return `${y}-${m}-${day}`;
    };
    const hhmmLocal = (d: Date) => {
      const h = String(d.getHours()).padStart(2, '0');
      const m = String(d.getMinutes()).padStart(2, '0');
      return `${h}:${m}`;
    };

    setStartDate(yyyyLocal(next));
    setStartTimeOfDay(hhmmLocal(next));
    setEndDate(yyyyLocal(later));
    setEndTimeOfDay(hhmmLocal(later));
  }, []);

  const loadData = async () => {
    if (!id) return;
    try {
      setIsLoading(true);
      setError('');
      const [roomData, bookingsData] = await Promise.all([
        roomsApi.getById(id),
        bookingsApi.getByRoom(id),
      ]);
      setRoom(roomData);
      setBookings(bookingsData);
    } catch (err) {
      const message = getErrorMessage(err);
      if (message === 'Resource not found') {
        setError('Room not found');
      } else {
        setError(message);
      }
    } finally { setIsLoading(false); }
  };

  const handleCreateBooking = async (e: FormEvent) => {
    e.preventDefault();
    setFormError('');
    try {
      if (!startDate || !startTimeOfDay || !endDate || !endTimeOfDay) { setFormError('Please select both start and end date/time'); return; }
      const start = new Date(`${startDate}T${startTimeOfDay}`);
      const end = new Date(`${endDate}T${endTimeOfDay}`);
    if (end <= start) { setFormError('End time must be after start time'); return; }
    if (start < new Date()) { setFormError('Cannot book in the past'); return; }
      setIsSubmitting(true);
      if (!id) throw new Error('Invalid room id');
      const booking = await bookingsApi.create(id, { startTime: start.toISOString(), endTime: end.toISOString() });
      setBookings(prev => [...prev, booking].sort((a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime()));
      setShowForm(false);
      // reset to defaults (next hour) using local date formatting consistently
      const now = new Date();
      const next = new Date(now);
      next.setMinutes(0,0,0);
      next.setHours(next.getHours() + 1);
      const later = new Date(next);
      later.setHours(later.getHours() + 1);
      const toLocalDate = (d: Date) => {
        const y = d.getFullYear();
        const m = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        return `${y}-${m}-${day}`;
      };
      const toLocalTime = (d: Date) => {
        const h = String(d.getHours()).padStart(2, '0');
        const m = String(d.getMinutes()).padStart(2, '0');
        return `${h}:${m}`;
      };
      setStartDate(toLocalDate(next));
      setStartTimeOfDay(toLocalTime(next));
      setEndDate(toLocalDate(later));
      setEndTimeOfDay(toLocalTime(later));
      toast.success('Booking created successfully!');
    } catch (err) {
      setFormError(getErrorMessage(err));
    } finally { setIsSubmitting(false); }
  };

  const handleDeleteBooking = async (bookingId: string) => {
    if (!id) return;
    setDeletingId(bookingId);
    try {
      await bookingsApi.delete(id, bookingId);
      setBookings(prev => prev.filter(b => b.id !== bookingId));
      toast.success('Booking cancelled');
    } catch (err: any) {
      const status = err?.response?.status;
      if (status === 401) {
        toast.error('You can only delete your own bookings');
      } else {
        toast.error(getErrorMessage(err));
      }
    } finally { setDeletingId(null); }
  };

  const formatDateTime = (iso: string) => {
    const date = parseISO(iso);
    return { date: format(date, 'EEE, MMM d, yyyy'), time: format(date, 'h:mm a') };
  };

  const getDuration = (start: string, end: string) => {
    const diff = parseISO(end).getTime() - parseISO(start).getTime();
    const hours = Math.floor(diff / 3600000);
    const minutes = Math.floor((diff % 3600000) / 60000);
    if (hours === 0) return `${minutes}m`;
    return `${hours}h${minutes > 0 ? ` ${minutes}m` : ''}`;
  };

  const isBookingPast = (endTime: string) => new Date(endTime) < new Date();

  if (isLoading) return <PageSpinner />;

  if (error || !room) {
    return (
      <div className="text-center py-20">
        <Building2 className="h-16 w-16 text-gray-300 mx-auto mb-4" />
        <h3 className="text-lg font-medium text-gray-900 dark:text-gray-100">{error || 'Room not found'}</h3>
        <Button variant="secondary" onClick={() => navigate('/rooms')} className="mt-4">Back to rooms</Button>
      </div>
    );
  }

  const upcomingBookings = bookings.filter(b => !isBookingPast(b.endTime));
  const pastBookings = bookings.filter(b => isBookingPast(b.endTime));

  return (
    <div className="space-y-8">
      <motion.div initial={{ opacity: 0, x: -20 }} animate={{ opacity: 1, x: 0 }}>
        <button onClick={() => navigate('/rooms')} className="inline-flex items-center gap-2 text-sm text-gray-500 hover:text-gray-700 transition-colors">
          <ArrowLeft className="h-4 w-4" /> Back to rooms
        </button>
      </motion.div>

      <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.4 }}>
        <Card className="p-8">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
            <div className="flex items-start gap-4">
              <div className="w-16 h-16 bg-gradient-to-br from-primary-100 to-primary-200 rounded-2xl flex items-center justify-center flex-shrink-0">
                <Building2 className="h-8 w-8 text-primary-600" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">{room.name}</h1>
                <div className="flex items-center gap-4 mt-2 text-sm text-gray-500">
                  <span className="flex items-center gap-1.5"><Users className="h-4 w-4" /><strong className="text-gray-700">{room.capacity}</strong> people</span>
                  <span className="flex items-center gap-1.5"><Calendar className="h-4 w-4" /><strong className="text-gray-700">{bookings.length}</strong> bookings</span>
                </div>
              </div>
            </div>
            <div className="flex items-center gap-3">
              <Button onClick={() => setShowForm(!showForm)}><Plus className="h-4 w-4" />{showForm ? 'Cancel' : 'Book Room'}</Button>
            </div>
          </div>
        </Card>
      </motion.div>

      {/* Booking Form */}
      <AnimatePresence>
        {showForm && (
          <motion.div initial={{ opacity: 0, height: 0 }} animate={{ opacity: 1, height: 'auto' }} exit={{ opacity: 0, height: 0 }} transition={{ duration: 0.3 }} className="overflow-hidden">
            <Card className="p-6 border-l-4 border-l-primary-500">
              <div className="flex items-center justify-between mb-4">
                <h2 className="text-lg font-semibold text-gray-900 flex items-center gap-2"><Calendar className="h-5 w-5 text-primary-600" /> New Booking</h2>
                <button onClick={() => setShowForm(false)} className="p-1 hover:bg-gray-100 rounded-lg transition-colors"><X className="h-5 w-5 text-gray-400" /></button>
              </div>
              {formError && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">{formError}</div>}
              <form onSubmit={handleCreateBooking} className="space-y-4">
                <div className="grid grid-cols-1 sm:grid-cols-4 gap-4">
                  <Input label="Start Date" type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} required />
                  <Input label="Start Time" type="time" value={startTimeOfDay} onChange={(e) => setStartTimeOfDay(e.target.value)} required />
                  <Input label="End Date" type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} required />
                  <Input label="End Time" type="time" value={endTimeOfDay} onChange={(e) => setEndTimeOfDay(e.target.value)} required />
                </div>
                <Button type="submit" isLoading={isSubmitting}><Plus className="h-4 w-4" /> Create Booking</Button>
              </form>
            </Card>
          </motion.div>
        )}
      </AnimatePresence>

      {/* Bookings Sections */}
      <div className="space-y-6">
        <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.2 }}>
          <h2 className="text-xl font-semibold text-gray-900 mb-4 flex items-center gap-2">
            <Clock className="h-5 w-5 text-green-500" /> Upcoming Bookings{upcomingBookings.length > 0 && <span className="text-sm font-normal text-gray-500"> ({upcomingBookings.length})</span>}
          </h2>
          {upcomingBookings.length === 0 ? (
            <Card className="p-8 text-center">
              <Calendar className="h-12 w-12 text-gray-300 mx-auto mb-3" />
              <p className="text-gray-500">No upcoming bookings</p>
              <Button variant="secondary" size="sm" onClick={() => setShowForm(true)} className="mt-3"><Plus className="h-4 w-4" /> Book this room</Button>
            </Card>
          ) : (
            <div className="space-y-3">
              {upcomingBookings.map((booking, index) => {
                const start = formatDateTime(booking.startTime);
                const end = formatDateTime(booking.endTime);
                return (
                  <motion.div key={booking.id} initial={{ opacity: 0, x: -20 }} animate={{ opacity: 1, x: 0 }} transition={{ delay: index * 0.05 }}>
                    <Card className="p-5 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
                      <div className="flex items-start gap-4">
                        <div className="w-10 h-10 bg-green-50 rounded-xl flex items-center justify-center flex-shrink-0"><Calendar className="h-5 w-5 text-green-600" /></div>
                        <div>
                          <p className="font-medium text-gray-900 dark:text-gray-100">{start.date}</p>
                          <div className="flex items-center gap-3 mt-1 text-sm text-gray-500">
                            <span className="flex items-center gap-1"><Clock className="h-3.5 w-3.5" /> {start.time} - {end.time}</span>
                            <span className="px-2 py-0.5 bg-gray-100 rounded-full text-xs font-medium text-gray-600">{getDuration(booking.startTime, booking.endTime)}</span>
                          </div>
                        </div>
                      </div>
                      <Button variant="danger" size="sm" isLoading={deletingId === booking.id} onClick={() => handleDeleteBooking(booking.id)}><Trash2 className="h-4 w-4" /> Cancel</Button>
                    </Card>
                  </motion.div>
                );
              })}
            </div>
          )}
        </motion.div>

        {pastBookings.length > 0 && (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: 0.4 }}>
            <details className="group">
              <summary className="cursor-pointer text-sm text-gray-500 hover:text-gray-700 transition-colors font-medium flex items-center gap-2">
                <Clock className="h-4 w-4" /> Past Bookings ({pastBookings.length})
              </summary>
              <div className="mt-3 space-y-2">
                {pastBookings.map((booking) => {
                  const start = formatDateTime(booking.startTime);
                  return (
                    <Card key={booking.id} className="p-4 opacity-60">
                      <div className="flex items-center gap-3">
                        <div className="w-8 h-8 bg-gray-100 rounded-lg flex items-center justify-center"><Calendar className="h-4 w-4 text-gray-400" /></div>
                        <div className="text-sm"><p className="text-gray-600">{start.date}</p><p className="text-gray-400 text-xs">{start.time}</p></div>
                      </div>
                    </Card>
                  );
                })}
              </div>
            </details>
          </motion.div>
        )}
      </div>
    </div>
  );
}
