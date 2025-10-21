'use client';

import { useEffect, useState } from 'react';
import { useParams } from 'next/navigation';
import Link from 'next/link';
import LiveTelemetry from '@/components/LiveTelemetry';

interface ServiceLog {
  id: string;
  equipmentId: string;
  datePerformed: string;
  hoursAtService: number;
  notes: string;
  laborCost: number;
}

interface Equipment {
  id: string;
  name: string;
  status: string;
  hours: number;
  serviceLogs: ServiceLog[];
}

export default function EquipmentDetail() {
  const params = useParams();
  const equipmentId = params.id as string;

  const [equipment, setEquipment] = useState<Equipment | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showNewLogModal, setShowNewLogModal] = useState(false);
  const [generatingPDF, setGeneratingPDF] = useState(false);
  const [activeTab, setActiveTab] = useState<'history' | 'telemetry'>('history');

  const [formData, setFormData] = useState({
    datePerformed: new Date().toISOString().split('T')[0],
    hoursAtService: 0,
    notes: '',
    laborCost: 0,
  });

  useEffect(() => {
    const fetchEquipment = async () => {
      try {
        const apiUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
        const res = await fetch(`${apiUrl}/api/equipment/${equipmentId}`);
        if (!res.ok) throw new Error('Equipment not found');
        const data = await res.json();
        setEquipment(data);
      } catch (err) {
        setError('Failed to load equipment');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchEquipment();
  }, [equipmentId]);

  const handleAddLog = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.hoursAtService || !formData.notes.trim()) {
      alert('Please fill in all required fields');
      return;
    }

    try {
      const apiUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
      const res = await fetch(`${apiUrl}/api/equipment/${equipmentId}/logs`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          datePerformed: new Date(formData.datePerformed),
          hoursAtService: parseInt(formData.hoursAtService.toString()),
          notes: formData.notes,
          laborCost: parseFloat(formData.laborCost.toString()),
        }),
      });

      if (!res.ok) throw new Error('Failed to add log');

      const apiUrl2 = process.env.NEXT_PUBLIC_API_BASE_URL;
      const updateRes = await fetch(`${apiUrl2}/api/equipment/${equipmentId}`);
      const updatedEquipment = await updateRes.json();
      setEquipment(updatedEquipment);

      setFormData({
        datePerformed: new Date().toISOString().split('T')[0],
        hoursAtService: 0,
        notes: '',
        laborCost: 0,
      });
      setShowNewLogModal(false);
    } catch (err) {
      alert('Failed to add service log');
      console.error(err);
    }
  };

  const handleGeneratePDF = async () => {
    try {
      setGeneratingPDF(true);
      const apiUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
      const res = await fetch(`${apiUrl}/api/reports/${equipmentId}/generate`);

      if (!res.ok) throw new Error('Failed to generate PDF');

      const blob = await res.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `Report_${equipmentId}_${new Date().toISOString().split('T')[0]}.pdf`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } catch (err) {
      alert('Failed to generate PDF');
      console.error(err);
    } finally {
      setGeneratingPDF(false);
    }
  };

  if (loading) return <div className="p-8 bg-gray-900 text-white min-h-screen">Loading...</div>;
  if (error) return <div className="p-8 bg-gray-900 text-white min-h-screen text-red-500">{error}</div>;
  if (!equipment) return <div className="p-8 bg-gray-900 text-white min-h-screen">Equipment not found</div>;

  const totalLaborCost = equipment.serviceLogs.reduce((sum, log) => sum + log.laborCost, 0);

  return (
    <div className="min-h-screen bg-gray-900 text-white p-8">
      <div className="mb-8">
        <Link href="/" className="text-blue-400 hover:text-blue-300 mb-4 inline-block">
          ← Back to Dashboard
        </Link>
        <h1 className="text-4xl font-bold mt-4">{equipment.name}</h1>
        <p className="text-gray-400 mt-2">ID: {equipment.id}</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div className="bg-gray-800 p-6 rounded-lg border border-gray-700">
          <p className="text-gray-400 text-sm font-semibold mb-2">Status</p>
          <p className={`text-2xl font-bold ${equipment.status === 'operational' ? 'text-green-400' : 'text-red-400'}`}>
            {equipment.status.charAt(0).toUpperCase() + equipment.status.slice(1)}
          </p>
        </div>
        <div className="bg-gray-800 p-6 rounded-lg border border-gray-700">
          <p className="text-gray-400 text-sm font-semibold mb-2">Operating Hours</p>
          <p className="text-2xl font-bold text-blue-400">{equipment.hours.toLocaleString()}</p>
        </div>
        <div className="bg-gray-800 p-6 rounded-lg border border-gray-700">
          <p className="text-gray-400 text-sm font-semibold mb-2">Service Records</p>
          <p className="text-2xl font-bold text-purple-400">{equipment.serviceLogs.length}</p>
        </div>
      </div>

      <div className="flex gap-4 mb-8 flex-wrap">
        <button
          onClick={() => setShowNewLogModal(true)}
          className="px-6 py-3 bg-blue-600 hover:bg-blue-700 rounded-lg font-semibold transition transform hover:scale-105"
        >
          ➕ New Service Log
        </button>
        <button
          onClick={handleGeneratePDF}
          disabled={generatingPDF}
          className="px-6 py-3 bg-green-600 hover:bg-green-700 rounded-lg font-semibold transition transform hover:scale-105 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          📄 {generatingPDF ? 'Generating...' : 'Generate Report PDF'}
        </button>
      </div>

      <div className="mb-6 flex gap-4 border-b border-gray-700">
        <button
          onClick={() => setActiveTab('history')}
          className={`px-4 py-3 font-semibold transition ${
            activeTab === 'history'
              ? 'text-blue-400 border-b-2 border-blue-400'
              : 'text-gray-400 hover:text-white'
          }`}
        >
          📋 Service History
        </button>
        <button
          onClick={() => setActiveTab('telemetry')}
          className={`px-4 py-3 font-semibold transition ${
            activeTab === 'telemetry'
              ? 'text-blue-400 border-b-2 border-blue-400'
              : 'text-gray-400 hover:text-white'
          }`}
        >
          📊 Live Telemetry
        </button>
      </div>

      {activeTab === 'history' && (
        <>
          <div className="bg-gray-800 rounded-lg overflow-hidden border border-gray-700 mb-6">
            <div className="bg-gray-700 px-6 py-4 border-b border-gray-600">
              <h2 className="text-xl font-semibold">Service History</h2>
            </div>

            {equipment.serviceLogs.length === 0 ? (
              <div className="p-6 text-gray-400 text-center">
                <p>No service records yet. Start by adding a new service log.</p>
              </div>
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="border-b border-gray-600 text-gray-300 text-sm bg-gray-750">
                      <th className="px-6 py-3 text-left font-semibold">Date</th>
                      <th className="px-6 py-3 text-left font-semibold">Hours</th>
                      <th className="px-6 py-3 text-left font-semibold">Service Notes</th>
                      <th className="px-6 py-3 text-right font-semibold">Labor Cost</th>
                    </tr>
                  </thead>
                  <tbody>
                    {equipment.serviceLogs
                      .sort((a, b) => new Date(b.datePerformed).getTime() - new Date(a.datePerformed).getTime())
                      .map((log) => (
                        <tr key={log.id} className="border-b border-gray-700 hover:bg-gray-750 transition">
                          <td className="px-6 py-4 text-sm">{new Date(log.datePerformed).toLocaleDateString()}</td>
                          <td className="px-6 py-4 text-sm">{log.hoursAtService.toLocaleString()}</td>
                          <td className="px-6 py-4 text-sm text-gray-300">{log.notes}</td>
                          <td className="px-6 py-4 text-sm text-right font-semibold text-green-400">${log.laborCost.toFixed(2)}</td>
                        </tr>
                      ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>

          {equipment.serviceLogs.length > 0 && (
            <div className="bg-gray-800 p-6 rounded-lg border border-gray-700 flex justify-between items-center">
              <span className="text-gray-300 font-semibold">Total Labor Cost:</span>
              <span className="text-3xl font-bold text-green-400">
                ${totalLaborCost.toFixed(2)}
              </span>
            </div>
          )}
        </>
      )}

      {activeTab === 'telemetry' && (
        <div className="bg-gray-800 rounded-lg p-8 border border-gray-700">
          <LiveTelemetry equipmentId={equipmentId} />
        </div>
      )}

      {showNewLogModal && (
        <div className="fixed inset-0 bg-black bg-opacity-70 flex items-center justify-center p-4 z-50">
          <div className="bg-gray-800 rounded-lg p-8 max-w-md w-full border border-gray-700 shadow-2xl">
            <h3 className="text-2xl font-bold mb-6">Add Service Log</h3>

            <form onSubmit={handleAddLog} className="space-y-4">
              <div>
                <label className="block text-gray-300 text-sm font-semibold mb-2">
                  Service Date <span className="text-red-500">*</span>
                </label>
                <input
                  type="date"
                  value={formData.datePerformed}
                  onChange={(e) => setFormData({ ...formData, datePerformed: e.target.value })}
                  className="w-full px-4 py-2 bg-gray-700 border border-gray-600 rounded text-white focus:outline-none focus:border-blue-500"
                  required
                />
              </div>

              <div>
                <label className="block text-gray-300 text-sm font-semibold mb-2">
                  Equipment Hours <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  value={formData.hoursAtService || ''}
                  onChange={(e) => setFormData({ ...formData, hoursAtService: parseInt(e.target.value) || 0 })}
                  className="w-full px-4 py-2 bg-gray-700 border border-gray-600 rounded text-white focus:outline-none focus:border-blue-500"
                  placeholder="0"
                  required
                />
              </div>

              <div>
                <label className="block text-gray-300 text-sm font-semibold mb-2">
                  Service Notes <span className="text-red-500">*</span>
                </label>
                <textarea
                  value={formData.notes}
                  onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                  className="w-full px-4 py-2 bg-gray-700 border border-gray-600 rounded text-white h-24 focus:outline-none focus:border-blue-500 resize-none"
                  placeholder="Describe the service performed..."
                  required
                />
              </div>

              <div>
                <label className="block text-gray-300 text-sm font-semibold mb-2">
                  Labor Cost ($) <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  step="0.01"
                  value={formData.laborCost || ''}
                  onChange={(e) => setFormData({ ...formData, laborCost: parseFloat(e.target.value) || 0 })}
                  className="w-full px-4 py-2 bg-gray-700 border border-gray-600 rounded text-white focus:outline-none focus:border-blue-500"
                  placeholder="0.00"
                  required
                />
              </div>

              <div className="flex gap-4 pt-6 border-t border-gray-600">
                <button
                  type="button"
                  onClick={() => setShowNewLogModal(false)}
                  className="flex-1 px-4 py-2 bg-gray-700 hover:bg-gray-600 rounded transition font-semibold"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="flex-1 px-4 py-2 bg-blue-600 hover:bg-blue-700 rounded transition font-semibold transform hover:scale-105"
                >
                  Save Log
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}