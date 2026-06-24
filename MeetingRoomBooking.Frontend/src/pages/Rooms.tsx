import { useState, useEffect, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import {
  Building2,
  Users,
  ArrowRight,
  Search,
  Plus,
  X,
} from 'lucide-react';
import toast from 'react-hot-toast';
import { roomsApi } from '../api/rooms';
import type { Room } from '../types';
import Card from '../components/ui/Card';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import { PageSpinner } from '../components/ui/Spinner';
import { getErrorMessage } from '../utils/error';

export default function Rooms() {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  const [search, setSearch] = useState('');
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [newRoomName, setNewRoomName] = useState('');
  const [newRoomCapacity, setNewRoomCapacity] = useState('');
  const [isCreating, setIsCreating] = useState(false);
  const [createError, setCreateError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    loadRooms();
  }, []);

  const handleCreateRoom = async (e: FormEvent) => {
    e.preventDefault();
    setCreateError('');
    const name = newRoomName.trim();
    const capacity = parseInt(newRoomCapacity, 10);
    if (!name) { setCreateError('Room name is required'); return; }
    if (isNaN(capacity) || capacity < 2 || capacity > 50) { setCreateError('Capacity must be between 2 and 50'); return; }
    try {
      setIsCreating(true);
      const room = await roomsApi.create({ name, capacity });
      setRooms(prev => [...prev, room]);
      setShowCreateForm(false);
      setNewRoomName('');
      setNewRoomCapacity('');
      toast.success(`Room "${room.name}" created!`);
    } catch (err) {
      setCreateError(getErrorMessage(err));
    } finally {
      setIsCreating(false);
    }
  };

  const loadRooms = async () => {
    try {
      setIsLoading(true);
      const data = await roomsApi.getAll();
      setRooms(data);
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
  };

  const filteredRooms = rooms.filter(
    (room) =>
      room.name.toLowerCase().includes(search.toLowerCase())
  );

  if (isLoading) return <PageSpinner />;

  return (
    <div className="space-y-8">
      {/* Header */}
      <motion.div
        initial={{ opacity: 0, y: -20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.4 }}
      >
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100">Meeting Rooms</h1>
            <p className="text-gray-500 dark:text-gray-400 mt-1">
              Browse and book available meeting rooms
            </p>
          </div>
          <div className="flex items-center gap-3">
            <Button variant="primary" size="sm" onClick={() => setShowCreateForm(true)}>
              <Plus className="h-4 w-4" /> Create Room
            </Button>
            <div className="w-56">
              <Input
                placeholder="Search rooms..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                icon={<Search className="h-4 w-4" />}
              />
            </div>
          </div>
        </div>
      </motion.div>

      {/* Create Room Modal */}
      <AnimatePresence>
        {showCreateForm && (
          <motion.div
            initial={{ opacity: 0, height: 0 }}
            animate={{ opacity: 1, height: 'auto' }}
            exit={{ opacity: 0, height: 0 }}
            className="overflow-hidden"
          >
            <Card className="p-6 border-l-4 border-l-primary-500">
              <div className="flex items-center justify-between mb-4">
                <h2 className="text-lg font-semibold text-gray-900 flex items-center gap-2">
                  <Building2 className="h-5 w-5 text-primary-600" /> New Meeting Room
                </h2>
                <button onClick={() => { setShowCreateForm(false); setCreateError(''); }} className="p-1 hover:bg-gray-100 rounded-lg transition-colors">
                  <X className="h-5 w-5 text-gray-400" />
                </button>
              </div>
              {createError && (
                <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">{createError}</div>
              )}
              <form onSubmit={handleCreateRoom} className="space-y-4">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <Input
                    label="Room Name"
                    type="text"
                    placeholder="e.g. Conference Room A"
                    value={newRoomName}
                    onChange={(e) => setNewRoomName(e.target.value)}
                    required
                  />
                  <Input
                    label="Capacity"
                    type="number"
                    placeholder="e.g. 10"
                    min={2}
                    max={50}
                    value={newRoomCapacity}
                    onChange={(e) => setNewRoomCapacity(e.target.value)}
                    required
                  />
                </div>
                <div className="flex justify-end gap-3">
                  <Button variant="secondary" type="button" onClick={() => { setShowCreateForm(false); setCreateError(''); }}>
                    Cancel
                  </Button>
                  <Button type="submit" isLoading={isCreating}>
                    <Plus className="h-4 w-4" /> Create Room
                  </Button>
                </div>
              </form>
            </Card>
          </motion.div>
        )}
      </AnimatePresence>

      {/* Error */}
      {error && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          className="p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-xl text-sm text-red-700 dark:text-red-400"
        >
          {error}
        </motion.div>
      )}

      {/* Rooms Grid */}
      {filteredRooms.length === 0 && !error ? (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          className="text-center py-20"
        >
          <Building2 className="h-16 w-16 text-gray-300 dark:text-gray-600 mx-auto mb-4" />
          <h3 className="text-lg font-medium text-gray-900 dark:text-gray-100">No rooms found</h3>
          <p className="text-gray-500 dark:text-gray-400 mt-1">
            {search
              ? 'Try adjusting your search query'
              : 'No meeting rooms are available yet'}
          </p>
        </motion.div>
      ) : (
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {filteredRooms.map((room, index) => (
            <motion.div
              key={room.id}
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: index * 0.05, duration: 0.3 }}
            >
                <Card
                hover
                onClick={() => navigate(`/rooms/${room.id}`)}
                className="p-6 h-full flex flex-col dark:bg-gray-800 dark:border-gray-700"
              >
                <div className="flex items-start justify-between mb-4">
                  <div className="w-12 h-12 bg-gradient-to-br from-primary-100 to-primary-200 dark:from-primary-900 dark:to-primary-800 rounded-xl flex items-center justify-center">
                    <Building2 className="h-6 w-6 text-primary-600 dark:text-primary-400" />
                  </div>
                </div>
                <h3 className="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-2">
                  {room.name}
                </h3>
                <div className="flex items-center gap-2 text-sm text-gray-500 dark:text-gray-400 mb-4">
                  <Users className="h-4 w-4 text-gray-400 dark:text-gray-500" />
                  <span>Capacity: <strong className="text-gray-700 dark:text-gray-300">{room.capacity}</strong> people</span>
                </div>
                <div className="mt-auto flex items-center text-sm font-medium text-primary-600 dark:text-primary-400 group">
                  View details
                  <ArrowRight className="h-4 w-4 ml-1 transition-transform group-hover:translate-x-1" />
                </div>
              </Card>
            </motion.div>
          ))}
        </div>
      )}

      {/* Stats bar */}
      {rooms.length > 0 && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 0.3 }}
          className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 p-4 flex items-center justify-center gap-8 text-sm text-gray-500 dark:text-gray-400"
        >
          <div className="flex items-center gap-2">
            <Building2 className="h-4 w-4 text-primary-500" />
            <span><strong className="text-gray-900 dark:text-gray-100">{rooms.length}</strong> rooms</span>
          </div>
          <div className="flex items-center gap-2">
            <Users className="h-4 w-4 text-primary-500" />
            <span>
              <strong className="text-gray-900 dark:text-gray-100">
                {rooms.reduce((sum, r) => sum + r.capacity, 0)}
              </strong>{' '}
              total capacity
            </span>
          </div>
        </motion.div>
      )}
    </div>
  );
}
