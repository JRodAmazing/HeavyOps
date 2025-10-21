# FleetPulse Diagnostic Simulator

This tool simulates J1939-style CAN diagnostic frames for testing FleetPulse's live telemetry visualization.

## Quick Start

### Prerequisites
- Node.js 14+ (no additional dependencies)
- FleetPulse backend running on `http://localhost:5038`
- FleetPulse frontend running on `http://localhost:3000`

### Run Simulator

**Normal Operation (2 minutes):**
```bash
node simulator.js
# OR
npm run normal
```

**Overheat Scenario (1 minute):**
```bash
SCENARIO=overheat DURATION=60 node simulator.js
# OR
npm run overheat
```

**Low Oil Pressure Scenario (1 minute):**
```bash
SCENARIO=lowpressure DURATION=60 node simulator.js
# OR
npm run lowpressure
```

## Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `API_BASE_URL` | `http://localhost:5038` | Backend API endpoint |
| `DURATION` | `300` | How long to run simulator (seconds) |
| `SCENARIO` | `normal` | `normal`, `overheat`, or `lowpressure` |

## Example: Custom Duration

```bash
DURATION=30 node simulator.js
```

## What It Does

- Posts **3 equipment units** (CAT320, KOMATSU350, VOLVO240)
- Sends frames every **250ms** (4 frames/second)
- Simulates realistic J1939 engine parameters:
  - **RPM** (600–2500 range)
  - **Coolant Temperature** (80–115°C)
  - **Oil Pressure** (10–50 psi)
- Frame format: `{ equipmentId, timestamp, canId, data }`

## Scenarios

### Normal (default)
- Gradual RPM ramp from 800 to 1800
- Coolant temp rises then cools
- Oil pressure tracks RPM
- Realistic variation (±5% random noise)

### Overheat
- RPM spikes to 2500
- Coolant rapidly climbs above 105°C → alerts trigger 🔴
- Oil pressure rises
- Then system cools down

### Low Pressure
- RPM climbs to 2000
- Oil pressure drops below 20 psi → alerts trigger 🔴
- System recovers

## Output

Terminal shows:
- Real-time frame count
- Equipment state (RPM, temp, pressure)
- Frame posting success/errors
- Summary when complete

## Testing in UI

1. **Start backend:** `dotnet run` in `backend/`
2. **Start frontend:** `npm run dev` in `frontend/`
3. **Run simulator:** `npm run normal` in `tools/diag-sim/`
4. **Open browser:** `http://localhost:3000`
5. **Navigate:** Equipment → CAT320 → "Live Telemetry" tab
6. **Watch:** Gauges animate in real-time as frames arrive

## Troubleshooting

**"Connection refused"**
- Backend not running? Start it: `dotnet run` in `backend/`
- Wrong port? Set: `API_BASE_URL=http://localhost:YOUR_PORT node simulator.js`

**"No frames appearing in UI"**
- Check browser console (F12) for errors
- Check backend logs for any stack traces
- Verify simulator shows "Frames Posted: X" (not all errors)

**"Gauges stuck at zero"**
- Simulator may have stopped—check terminal
- Frontend may not be polling—check browser console
- Try refreshing page

## What's Next

After recording your demo:
1. Generate PDF report
2. Commit everything to GitHub
3. Tag as `v0.1.0-day3`
4. Push to GitHub
5. Share demo video on LinkedIn/Twitter
