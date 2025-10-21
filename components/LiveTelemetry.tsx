'use client';

import { useEffect, useState } from 'react';
import Gauge from './Gauge';

interface TelemetryData {
  rpm: number;
  coolant_temp: number;
  oil_pressure: number;
  raw_bytes: string;
}

interface FrameData {
  frame: {
    id: string;
    equipmentId: string;
    timestamp: string;
    canId: string;
    data: string;
  };
  parsed: TelemetryData;
}

export default function LiveTelemetry({ equipmentId }: { equipmentId: string }) {
  const [telemetry, setTelemetry] = useState<TelemetryData | null>(null);
  const [frameCount, setFrameCount] = useState(0);
  const [isConnected, setIsConnected] = useState(false);
  const [alerts, setAlerts] = useState<string[]>([]);

  useEffect(() => {
    let interval: NodeJS.Timeout | null = null;

    const fetchLatestFrame = async () => {
      try {
        const response = await fetch(
          `http://localhost:5038/api/stream/${equipmentId}/latest`
        );

        if (response.ok) {
          const data: FrameData = await response.json();
          setTelemetry(data.parsed);
          setIsConnected(true);

          // Check for alerts
          const newAlerts: string[] = [];
          if (data.parsed.coolant_temp > 105) {
            newAlerts.push('⚠️ Coolant overheat risk');
          }
          if (data.parsed.oil_pressure < 20) {
            newAlerts.push('⚠️ Low oil pressure');
          }
          setAlerts(newAlerts);

          // Increment frame counter
          setFrameCount((prev) => prev + 1);
        } else {
          setIsConnected(false);
        }
      } catch (error) {
        console.error('Error fetching telemetry:', error);
        setIsConnected(false);
      }
    };

    // Fetch immediately
    fetchLatestFrame();

    // Then poll every 500ms
    interval = setInterval(fetchLatestFrame, 500);

    return () => {
      if (interval) clearInterval(interval);
    };
  }, [equipmentId]);

  if (!telemetry) {
    return (
      <div className="text-center text-gray-400">
        Waiting for diagnostic data...
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Status Badge */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <div
            className={`h-3 w-3 rounded-full ${
              isConnected ? 'bg-green-500' : 'bg-red-500'
            } animate-pulse`}
          ></div>
          <span className="text-sm text-gray-300">
            {isConnected ? 'Connected' : 'Disconnected'}
          </span>
        </div>
        <span className="text-xs text-gray-500">
          Frames received: {frameCount}
        </span>
      </div>

      {/* Alerts */}
      {alerts.length > 0 && (
        <div className="space-y-2">
          {alerts.map((alert, i) => (
            <div
              key={i}
              className="rounded-lg bg-red-900/30 border border-red-700 px-4 py-2 text-sm text-red-200"
            >
              {alert}
            </div>
          ))}
        </div>
      )}

      {/* Gauges */}
      <div className="grid grid-cols-3 gap-8">
        <Gauge
          label="RPM"
          value={telemetry.rpm}
          min={0}
          max={2400}
          unit="RPM"
          warningThreshold={2000}
          criticalThreshold={2200}
        />
        <Gauge
          label="Coolant Temperature"
          value={telemetry.coolant_temp}
          min={0}
          max={120}
          unit="°C"
          warningThreshold={100}
          criticalThreshold={105}
        />
        <Gauge
          label="Oil Pressure"
          value={telemetry.oil_pressure}
          min={0}
          max={100}
          unit="PSI"
          warningThreshold={25}
          criticalThreshold={15}
        />
      </div>

      {/* Raw Data */}
      <div className="rounded-lg bg-gray-900 p-4">
        <h4 className="text-xs font-semibold text-gray-400 mb-2">Raw Frame Data</h4>
        <code className="text-xs text-green-400 font-mono break-all">
          {telemetry.raw_bytes}
        </code>
      </div>
    </div>
  );
}