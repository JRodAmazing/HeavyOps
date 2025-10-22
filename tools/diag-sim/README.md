# FleetPulse Diagnostic Simulator

Generates and posts realistic J1939 CAN frames to simulate heavy equipment diagnostics.

## Quick Start

```bash
# Run simulator
npm start
```

The simulator will:
- Post frames every 250ms (4 FPS, realistic CAN rate)
- Simulate 3 equipment units with staggered starts
- Run a 2-minute demo with realistic operating patterns
- Log frame data to console

## Operating Patterns

### Timeline (120 seconds total)

- **0-10s**: Startup phase (cold start, idle at 800 RPM)
- **10-30s**: Ramp up to normal operating RPM (800→2000 RPM)
- **30-90s**: Normal operation with varied load cycles
  - Light load: 1200-1400 RPM
  - Medium load: 1800-2200 RPM
  - Heavy load: 2100-2700 RPM
- **90-100s**: Thermal stress test (overheat scenario)
  - RPM: 2500
  - Coolant temp: 92°C → 117°C (crosses critical threshold)
  - This tests your alert system
- **100-120s**: Cool down phase

## Realistic Metrics

### RPM (0-3000)
- Idle: ~800 RPM
- Normal running: 1200-2200 RPM
- High load: 2100-2700 RPM
- Warning: >2500 RPM
- Critical: >2800 RPM

### Coolant Temperature (60-120°C)
- Cold start: 20°C
- Normal operating: 85-95°C
- Warning: >105°C
- Critical: >115°C
- **Overheat test triggers at 90s**

### Oil Pressure (0-100 PSI)
- Startup: 0-5 PSI
- Idle (600-1000 RPM): 15-40 PSI
- Normal running: 60-80 PSI
- Low pressure warning: <20 PSI
- Critical: <10 PSI

### Fuel Level (0-100%)
- Start: 100%
- Consumption: ~1% per minute
- Warning: <20%
- Critical: <5%

## J1939 Frame Format

Each frame posted to `/api/stream/ingest` contains:

```json
{
  "equipmentId": "CAT320",
  "timestamp": "2025-10-22T10:30:45.123Z",
  "canId": "0x0CF00400",
  "data": [0x0F, 0xD0, 0xAB, 0xC3, 0x4B, 0x00, 0x00, 0x00],
  "rpm": 1523.75,
  "coolantTemp": 92.5,
  "oilPressure": 67.3,
  "fuelLevel": 98.5,
  "state": "normal"
}
```

## Frame Data Layout (8 bytes)

- **Byte 0-1**: RPM (16-bit, scale 0.125 RPM per unit)
- **Byte 2**: Coolant temp (0-255 = 0-150°C)
- **Byte 3**: Oil pressure (0-255 = 0-100 PSI)
- **Byte 4**: Fuel level (0-100%)
- **Byte 5-7**: Reserved

## Console Output

```
🚀 FleetPulse Diagnostic Simulator Started
📍 Target API: http://localhost:5038
⏱️  Duration: 120 seconds
📡 Frame rate: 4 FPS (250ms interval)

Equipment Simulation Schedule:
  [0s] Caterpillar 320D Excavator (CAT320)
  [2s] Komatsu PC350 Excavator (KOMATSU350)
  [4s] Volvo EC240B Excavator (VOLVO240)

[2.5s] CAT320 | RPM: 850 | Coolant: 25.3°C | Oil: 12.5 PSI | State: startup
[5.0s] CAT320 | RPM: 1200 | Coolant: 45.2°C | Oil: 32.1 PSI | State: ramp-up
...
```

## Testing Scenarios

### Normal Operation
Just run the simulator—it will show realistic steady-state operation for 60 seconds.

### Overheat Alert
The simulator will trigger high coolant temps (>105°C) between 90-100 seconds. Watch your alerts light up red!

### Low Oil Pressure
During startup (0-10s), oil pressure will be very low (<10 PSI). This triggers your critical alert.

### Low Fuel
Fuel level slowly decreases. By 60s of operation, it will hit the warning threshold.

## Integration with Frontend

The simulator posts to `POST /api/stream/ingest`, which the backend stores.

Your frontend polls `GET /api/stream/{equipmentId}/latest` every 500ms.

Result: **Live gauges in your dashboard respond to simulated data in real-time!**

## Troubleshooting

### "Error connecting to API"
Make sure backend is running:
```bash
cd ../backend/FleetPulse.API
dotnet run
```

### "No frames appearing on frontend"
1. Check backend is running (should see frames logged)
2. Check frontend is polling the correct API endpoint
3. Try manually posting a test frame:
   ```bash
   curl -X POST http://localhost:5038/api/stream/ingest \
     -H "Content-Type: application/json" \
     -d '{"equipmentId":"CAT320","timestamp":"2025-10-22T10:00:00Z","canId":"0x0CF00400","data":[0,0,100,150,50,0,0,0]}'
   ```

## Next Steps

After running simulator:
1. ✅ Open http://localhost:3000
2. ✅ Click equipment card → Live Telemetry tab
3. ✅ Watch gauges animate in real-time
4. ✅ See alerts trigger during overheat (90-100s mark)
5. ✅ Generate PDF report to see it saved successfully

This is your **professional diagnostic demo**. Record this for your portfolio!
