'use client';

import { useEffect, useRef } from 'react';

interface GaugeProps {
  label: string;
  value: number;
  min: number;
  max: number;
  unit: string;
  warningThreshold?: number;
  criticalThreshold?: number;
}

export default function Gauge({
  label,
  value,
  min,
  max,
  unit,
  warningThreshold,
  criticalThreshold,
}: GaugeProps) {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const width = canvas.width;
    const height = canvas.height;
    const centerX = width / 2;
    const centerY = height / 2;
    const radius = Math.min(width, height) / 2 - 10;

    // Clear canvas
    ctx.clearRect(0, 0, width, height);

    // Draw background circle
    ctx.fillStyle = '#1f2937';
    ctx.beginPath();
    ctx.arc(centerX, centerY, radius, 0, 2 * Math.PI);
    ctx.fill();

    // Draw gauge arc (background track)
    ctx.strokeStyle = '#374151';
    ctx.lineWidth = 15;
    ctx.beginPath();
    ctx.arc(centerX, centerY, radius - 10, (Math.PI * 0.75), (Math.PI * 2.25));
    ctx.stroke();

    // Determine color based on value
    let gaugeColor = '#10b981'; // green
    if (criticalThreshold && value >= criticalThreshold) {
      gaugeColor = '#ef4444'; // red
    } else if (warningThreshold && value >= warningThreshold) {
      gaugeColor = '#f59e0b'; // amber
    }

    // Draw value arc (colored)
    const percentage = (value - min) / (max - min);
    const endAngle = (Math.PI * 0.75) + (percentage * (Math.PI * 1.5));

    ctx.strokeStyle = gaugeColor;
    ctx.lineWidth = 15;
    ctx.beginPath();
    ctx.arc(centerX, centerY, radius - 10, (Math.PI * 0.75), endAngle);
    ctx.stroke();

    // Draw needle
    const needleAngle = (Math.PI * 0.75) + (percentage * (Math.PI * 1.5));
    const needleLength = radius - 30;
    const needleX = centerX + needleLength * Math.cos(needleAngle);
    const needleY = centerY + needleLength * Math.sin(needleAngle);

    ctx.strokeStyle = gaugeColor;
    ctx.lineWidth = 3;
    ctx.beginPath();
    ctx.moveTo(centerX, centerY);
    ctx.lineTo(needleX, needleY);
    ctx.stroke();

    // Draw center circle
    ctx.fillStyle = '#1f2937';
    ctx.beginPath();
    ctx.arc(centerX, centerY, 8, 0, 2 * Math.PI);
    ctx.fill();

    ctx.strokeStyle = gaugeColor;
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.arc(centerX, centerY, 8, 0, 2 * Math.PI);
    ctx.stroke();

    // Draw labels
    ctx.fillStyle = '#e5e7eb';
    ctx.font = 'bold 24px sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText(`${Math.round(value)}`, centerX, centerY + 15);

    ctx.font = '12px sans-serif';
    ctx.fillText(unit, centerX, centerY + 35);

    // Draw min/max labels
    ctx.font = '11px sans-serif';
    ctx.fillStyle = '#9ca3af';

    const minX = centerX + (radius - 25) * Math.cos(Math.PI * 0.75);
    const minY = centerY + (radius - 25) * Math.sin(Math.PI * 0.75);
    ctx.textAlign = 'center';
    ctx.fillText(min.toString(), minX, minY);

    const maxX = centerX + (radius - 25) * Math.cos(Math.PI * 2.25);
    const maxY = centerY + (radius - 25) * Math.sin(Math.PI * 2.25);
    ctx.fillText(max.toString(), maxX, maxY);
  }, [value, min, max, unit, warningThreshold, criticalThreshold]);

  return (
    <div className="flex flex-col items-center">
      <h3 className="text-sm font-semibold text-gray-300 mb-2">{label}</h3>
      <canvas
        ref={canvasRef}
        width={180}
        height={180}
        className="rounded-full"
      />
    </div>
  );
}