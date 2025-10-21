# Day 2: Service Logs + PDF Reports - Complete Implementation Guide

## Status Overview
✅ **Backend API**: PDF services created and injected  
✅ **Service Log Endpoints**: Already implemented in EquipmentController  
🔄 **Frontend**: Needs service log form + PDF download button  

---

## Backend Status (DONE ✅)

### What's Already Implemented

**1. Service Log Endpoints (EquipmentController.cs)**
```csharp
GET  /api/equipment/{id}/logs        // Get all logs for equipment
POST /api/equipment/{id}/logs        // Add new log
```

**2. PDF Generation Services**
- `PdfReportService`: Professional QuestPDF reports with branding
- `SimplePdfService`: Lightweight fallback PDFs (just created)

**3. Report Endpoints (ReportsController.cs)**
```
GET /api/reports/{equipmentId}/generate          // Full PDF report
GET /api/reports/{equipmentId}/generate-simple   // Simple test PDF
```

**4. Mock Data** 
- 3 equipment units pre-configured with sample service logs
- Ready for immediate testing

---

## Frontend Tasks (TODO ⏳)

### Phase 1: Equipment Detail Page with Service Logs (2-3 hours)

**Location:** `frontend/app/equipment/[id]/page.tsx`

```typescript
'use client';

import { useEffect, useState } from 'react';
import { useParams } from 'next/navigation';
import Link from 'next/link';

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

  // Form state for new log
  const [formData, setFormData] = useState({
    datePerformed: new Date().toISOString().split('T')[0],
    hoursAtService: 0,
    notes: '',
    laborCost: 0,
  });

  // Fetch equipment details
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

  // Handle form submission
  const handleAddLog = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validation
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

      // Refresh equipment data
      const apiUrl2 = process.env.NEXT_PUBLIC_API_BASE_URL;
      const updateRes = await fetch(`${apiUrl2}/api/equipment/${equipmentId}`);
      const updatedEquipment = await updateRes.json();
      setEquipment(updatedEquipment);

      // Reset form
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

  // Generate PDF report
  const handleGeneratePDF = async () => {
    try {
      setGeneratingPDF(true);
      const apiUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
      const res = await fetch(`${apiUrl}/api/reports/${equipmentId}/generate`);
      
      if (!res.ok) throw new Error('Failed to generate PDF');

      // Get the PDF blob
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

  if (loading) return <div className="p-8">Loading...</div>;
  if (error) return <div className="p-8 text-red-500">{error}</div>;
  if (!equipment) return <div className="p-8">Equipment not found</div>;

  return (
    <div className="min-h-screen bg-gray-900 text-white p-8">
      {/* Header */}
      <div className="mb-8">
        <Link href="/" className="text-blue-400 hover:text-blue-300 mb-4 inline-block">
          ← Back to Dashboard
        </Link>
        <h1 className="text-4xl font-bold mt-4">{equipment.name}</h1>
        <p className="text-gray-400 mt-2">ID: {equipment.id}</p>
      </div>

      {/* Equipment Info Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div className="bg-gray-800 p-6 rounded-lg">
          <p className="text-gray-400 text-sm">Status</p>
          <p className={`text-2xl font-bold ${equipment.status === 'operational' ? 'text-green-400' : 'text-red-400'}`}>
            {equipment.status}
          </p>
        </div>
        <div className="bg-gray-800 p-6 rounded-lg">
          <p className="text-gray-400 text-sm">Operating Hours</p>
          <p className="text-2xl font-bold text-blue-400">{equipment.hours}</p>
        </div>
        <div className="bg-gray-800 p-6 rounded-lg">
          <p className="text-gray-400 text-sm">Total Service Records</p>
          <p className="text-2xl font-bold text-purple-400">{equipment.serviceLogs.length}</p>
        </div>
      </div>

      {/* Action Buttons */}
      <div className="flex gap-4 mb-8">
        <button
          onClick={() => setShowNewLogModal(true)}
          className="px-6 py-3 bg-blue-600 hover:bg-blue-700 rounded-lg font-semibold transition"
        >
          ➕ New Service Log
        </button>
        <button
          onClick={handleGeneratePDF}
          disabled={generatingPDF}
          className="px-6 py-3 bg-green-600 hover:bg-green-700 rounded-lg font-semibold transition disabled:opacity-50"
        >
          📄 {generatingPDF ? 'Generating...' : 'Generate Report PDF'}
        </button>
      </div>

      {/* Service Logs Table */}
      <div className="bg-gray-800 rounded-lg overflow-hidden">
        <div className="bg-gray-700 px-6 py-4">
          <h2 className="text-xl font-semibold">Service History</h2>
        </div>
        
        {equipment.serviceLogs.length === 0 ? (
          <div className="p-6 text-gray-400">No service records yet.</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b border-gray-600 text-gray-300 text-sm">
                  <th className="px-6 py-3 text-left">Date</th>
                  <th className="px-6 py-3 text-left">Hours</th>
                  <th className="px-6 py-3 text-left">Service Notes</th>
                  <th className="px-6 py-3 text-right">Labor Cost</th>
                </tr>
              </thead>
              <tbody>
                {equipment.serviceLogs
                  .sort((a, b) => new Date(b.datePerformed).getTime() - new Date(a.datePerformed).getTime())
                  .map((log) => (
                    <tr key={log.id} className="border-b border-gray-700 hover:bg-gray-700 transition">
                      <td className="px-6 py-4">{new Date(log.datePerformed).toLocaleDateString()}</td>
                      <td className="px-6 py-4">{log.hoursAtService.toLocaleString()}</td>
                      <td className="px-6 py-4">{log.notes}</td>
                      <td className="px-6 py-4 text-right font-semibold">${log.laborCost.toFixed(2)}</td>
                    </tr>
                  ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Summary */}
      {equipment.serviceLogs.length > 0 && (
        <div className="mt-6 bg-gray-800 p-6 rounded-lg flex justify-between items-center">
          <span className="text-gray-400">Total Labor Cost:</span>
          <span className="text-2xl font-bold text-green-400">
            ${equipment.serviceLogs.reduce((sum, log) => sum + log.laborCost, 0).toFixed(2)}
          </span>
        </div>
      )}

      {/* New Service Log Modal */}
      {showNewLogModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-gray-800 rounded-lg p-8 max-w-md w-full">
            <h3 className="text-2xl font-bold mb-6">Add Service Log</h3>

            <form onSubmit={handleAddLog} className="space-y-4">
              <div>
                <label className="block text-gray-300 text-sm font-semibold mb-2">
                  Service Date
                </label>
                <input
                  type="date"
                  value={formData.datePerformed}
                  onChange={(e) => setFormData({ ...formData, datePerformed: e.target.value })}
                  className="w-full px-4 py-2 bg-gray-700 border border-gray-600 rounded text-white"
                  required
                />
              </div>

              <div>
                <label className="block text-gray-300 text-sm font-semibold mb-2">
                  Equipment Hours
                </label>
                <input
                  type="number"
                  value={formData.hoursAtService}
                  onChange={(e) => setFormData({ ...formData, hoursAtService: parseInt(e.target.value) || 0 })}
                  className="w-full px-4 py-2 bg-gray-700 border border-gray-600 rounded text-white"
                  required
                />
              </div>

              <div>
                <label className="block text-gray-300 text-sm font-semibold mb-2">
                  Service Notes
                </label>
                <textarea
                  value={formData.notes}
                  onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                  className="w-full px-4 py-2 bg-gray-700 border border-gray-600 rounded text-white h-24"
                  placeholder="Describe the service performed..."
                  required
                />
              </div>

              <div>
                <label className="block text-gray-300 text-sm font-semibold mb-2">
                  Labor Cost ($)
                </label>
                <input
                  type="number"
                  step="0.01"
                  value={formData.laborCost}
                  onChange={(e) => setFormData({ ...formData, laborCost: parseFloat(e.target.value) || 0 })}
                  className="w-full px-4 py-2 bg-gray-700 border border-gray-600 rounded text-white"
                  required
                />
              </div>

              <div className="flex gap-4 pt-4">
                <button
                  type="button"
                  onClick={() => setShowNewLogModal(false)}
                  className="flex-1 px-4 py-2 bg-gray-700 hover:bg-gray-600 rounded transition"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="flex-1 px-4 py-2 bg-blue-600 hover:bg-blue-700 rounded transition font-semibold"
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
```

---

## Implementation Steps

### Step 1: Create Equipment Detail Directory
```powershell
mkdir frontend/app/equipment/[id]
```

### Step 2: Create the Detail Page
Create `frontend/app/equipment/[id]/page.tsx` with the code above.

### Step 3: Update Equipment List Page
Update `frontend/app/page.tsx` to link to detail pages:
```typescript
// In the Link component, use:
<Link href={`/equipment/${item.id}`}>
```
(This is already done in your current code ✅)

### Step 4: Test Locally
```powershell
# Terminal 1 - Backend
cd backend/FleetPulse.API
dotnet run

# Terminal 2 - Frontend
cd frontend
npm run dev

# Terminal 3 - Optional: Test API directly
# GET http://localhost:5038/api/equipment/CAT320
# GET http://localhost:5038/api/equipment/CAT320/logs
# GET http://localhost:5038/api/reports/CAT320/generate
```

---

## API Testing Checklist

### 1. Get Equipment
```bash
GET http://localhost:5038/api/equipment/CAT320
```
Expected: 200 OK with equipment object

### 2. Get Service Logs
```bash
GET http://localhost:5038/api/equipment/CAT320/logs
```
Expected: 200 OK with array of logs

### 3. Add Service Log
```bash
POST http://localhost:5038/api/equipment/CAT320/logs
Content-Type: application/json

{
  "datePerformed": "2025-10-20T00:00:00Z",
  "hoursAtService": 2450,
  "notes": "Oil change and inspection",
  "laborCost": 150
}
```
Expected: 201 Created with new log

### 4. Generate Report PDF
```bash
GET http://localhost:5038/api/reports/CAT320/generate
```
Expected: PDF file downloads

---

## Frontend File Structure After Day 2

```
frontend/
├── app/
│   ├── equipment/
│   │   └── [id]/
│   │       └── page.tsx          ← NEW (Equipment Detail Page)
│   ├── page.tsx                  ← Dashboard
│   ├── layout.tsx
│   └── globals.css
├── .env.local                     ← Already configured
└── package.json
```

---

## Deliverables for Day 2

✅ **What You Should Have:**
- Equipment detail page with service log list
- "New Service Log" modal form with validation
- "Generate Report" button that downloads PDF
- Service log history table with sorting
- Total labor cost summary

✅ **Demo Video (90-120s):**
1. Click equipment card from dashboard
2. View current service logs
3. Click "New Service Log"
4. Fill out form (date, hours, notes, cost)
5. Submit and see log appear in table
6. Click "Generate Report PDF"
7. Show PDF opens with professional formatting

---

## Troubleshooting

**API Returns 404?**
- Verify backend is running: `dotnet run` from backend directory
- Check port is 5038: `http://localhost:5038/swagger`

**PDF Download Not Working?**
- Check browser console for errors
- Ensure backend is returning PDF with correct headers
- Test endpoint directly in browser: `http://localhost:5038/api/reports/CAT320/generate`

**Form Not Submitting?**
- Verify all required fields filled
- Check browser console for validation errors
- Ensure API response is 201 Created (not 200)

---

## Next: Day 3 Preview

Once Day 2 is complete, Day 3 will add:
- Live diagnostic stream simulation
- Real-time telemetry gauges
- Alert thresholds and badges
- Final demo video with live data
