'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
interface Alert {
  severity: 'critical' | 'warning';
  message: string;
}

interface Equipment {
  id: string;
  name: string;
  status: string;
  hours: number;
  activeAlerts?: Alert[];
}

export default function Dashboard() {
  const [equipment, setEquipment] = useState<Equipment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchEquipment = async () => {
      try {
       const apiUrl = process.env.NEXT_PUBLIC_API_BASE_URL || 'https://fleetpulse-api.onrender.com';
        const res = await fetch(`${apiUrl}/api/equipment`);
        if (!res.ok) throw new Error('Failed to fetch');
        const data = await res.json();
        setEquipment(data);
      } catch (err) {
        setError('Failed to connect to backend');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchEquipment();
    const interval = setInterval(fetchEquipment, 5000);
    return () => clearInterval(interval);
  }, []);

  if (loading) return <div className="p-8">Loading...</div>;
  if (error) return <div className="p-8 text-red-500">{error}</div>;

  return (
    <div className="min-h-screen bg-gray-900 text-white p-8">
      <div className="mb-8">
        <h1 className="text-4xl font-bold">FleetPulse Dashboard</h1>
        <p className="text-gray-400 mt-2">Fleet Diagnostics & Maintenance</p>
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {equipment.map((item) => {
          const hasAlerts = item.activeAlerts && item.activeAlerts.length > 0;
          const hasCritical = item.activeAlerts?.some(a => a.severity === 'critical');
          
          return (
            <Link key={item.id} href={`/equipment/${item.id}`}>
              <div className={`p-6 rounded-lg shadow-lg cursor-pointer transition transform hover:scale-105 ${
                hasCritical ? 'bg-red-900 border-2 border-red-600' : 'bg-gray-800 hover:bg-gray-700'
              }`}>
                <div className="flex justify-between items-start mb-2">
                  <h2 className="text-2xl font-semibold">{item.name}</h2>
                  {hasAlerts && (
                    <span className={`text-xs px-2 py-1 rounded font-semibold ${
                      hasCritical ? 'bg-red-600 text-white' : 'bg-yellow-600 text-white'
                    }`}>
                      {item.activeAlerts!.length} Alert{item.activeAlerts!.length !== 1 ? 's' : ''}
                    </span>
                  )}
                </div>
                <p className="text-gray-400 mb-4">ID: {item.id}</p>
                <div className="space-y-2">
                  <p className="text-lg">
                    <span className="text-gray-400">Status:</span>{' '}
                    <span className={item.status === 'operational' ? 'text-green-400' : 'text-red-400'}>
                      {item.status}
                    </span>
                  </p>
                  <p className="text-lg">
                    <span className="text-gray-400">Hours:</span> {item.hours}
                  </p>
                </div>
                {hasAlerts && (
                  <div className="mt-4 pt-4 border-t border-gray-600">
                    <p className="text-sm text-yellow-200">⚠️ View alerts for details</p>
                  </div>
                )}
              </div>
            </Link>
          );
        })}
      </div>
    </div>
  );
}