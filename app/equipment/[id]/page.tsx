'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useParams } from 'next/navigation';

interface Equipment {
  id: string;
  name: string;
  status: string;
  hours: number;
}

export default function EquipmentDetail() {
  const params = useParams();
  const id = params.id as string;
  const [equipment, setEquipment] = useState<Equipment | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const apiUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
    fetch(`${apiUrl}/api/equipment/${id}`)
      .then(res => res.json())
      .then(data => setEquipment(data))
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) return <div className="p-8">Loading...</div>;
  if (!equipment) return <div className="p-8 text-red-500">Equipment not found</div>;

  return (
    <div className="min-h-screen bg-gray-900 text-white p-8">
      <Link href="/" className="text-blue-400 hover:underline mb-8 block">
        ← Back to Dashboard
      </Link>
      
      <div className="bg-gray-800 p-8 rounded-lg max-w-2xl">
        <h1 className="text-4xl font-bold mb-4">{equipment.name}</h1>
        <div className="space-y-4">
          <p><span className="text-gray-400">ID:</span> {equipment.id}</p>
          <p><span className="text-gray-400">Status:</span> <span className={equipment.status === 'operational' ? 'text-green-400' : 'text-red-400'}>{equipment.status}</span></p>
          <p><span className="text-gray-400">Hours:</span> {equipment.hours}</p>
        </div>
      </div>
    </div>
  );
}