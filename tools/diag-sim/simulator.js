#!/usr/bin/env node

/**
 * FleetPulse Diagnostic Simulator
 * 
 * Posts realistic J1939 CAN frames to the backend
 * Simulates 3 equipment units with realistic operating patterns
 * 
 * Usage:
 *   npm install (in tools/diag-sim)
 *   npm start
 */

const http = require('http');

// Configuration
const API_BASE_URL = process.env.API_BASE_URL || 'http://localhost:5038';
const FRAME_INTERVAL = 250; // 250ms = 4 frames per second (realistic CAN rate)
const DEMO_DURATION_SECONDS = 120; // 2-minute demo

// Equipment to simulate
const EQUIPMENT = [
  { id: 'CAT320', name: 'Caterpillar 320D Excavator' },
  { id: 'KOMATSU350', name: 'Komatsu PC350 Excavator' },
  { id: 'VOLVO240', name: 'Volvo EC240B Excavator' },
];

/**
 * Realistic J1939 data generator
 * Simulates equipment startup, warmup, and normal operating patterns
 */
class J1939Simulator {
  constructor(equipmentId, startDelay = 0) {
    this.equipmentId = equipmentId;
    this.startTime = Date.now() + startDelay;
    this.state = 'idle';
    this.rpmTarget = 800;
    this.rpmCurrent = 0;
    this.coolantTemp = 20; // Start cold
    this.oilPressure = 0;
    this.fuelLevel = 100;
  }

  /**
   * Get time in seconds since simulator started
   */
  getElapsedSeconds() {
    return (Date.now() - this.startTime) / 1000;
  }

  /**
   * Simulate operating pattern over time
   * 0-10s: Startup and idle
   * 10-30s: Ramp up to normal operating RPM
   * 30-90s: Normal operation (varied load)
   * 90-100s: Thermal stress test
   * 100-120s: Cool down
   */
  simulateOperatingPattern(elapsed) {
    if (elapsed < 10) {
      // Startup phase: idle at 800 RPM
      this.state = 'startup';
      this.rpmTarget = 800;
      this.coolantTemp = Math.max(20, this.coolantTemp + 0.5); // Slow warm-up
    } else if (elapsed < 30) {
      // Ramp up phase
      this.state = 'ramp-up';
      this.rpmTarget = 1200 + ((elapsed - 10) / 20) * 800; // Ramp 1200→2000 RPM
      this.coolantTemp = Math.min(92, this.coolantTemp + 1.5); // Faster warm-up
    } else if (elapsed < 90) {
      // Normal operation: varied load cycle
      const cycleTime = (elapsed - 30) % 15; // 15-second cycle
      if (cycleTime < 5) {
        // Light load
        this.rpmTarget = 1200 + Math.random() * 200;
      } else if (cycleTime < 10) {
        // Medium load
        this.rpmTarget = 1800 + Math.random() * 400;
      } else {
        // Heavy load
        this.rpmTarget = 2100 + Math.random() * 600;
      }
      this.state = 'normal';
      // Maintain stable coolant temp with small variations
      this.coolantTemp = 88 + Math.random() * 4 - 2;
    } else if (elapsed < 100) {
      // Thermal stress test: simulate overheat condition
      this.state = 'thermal-stress';
      this.rpmTarget = 2500; // High RPM
      this.coolantTemp = 92 + ((elapsed - 90) / 10) * 25; // Heat up to 117°C
    } else if (elapsed < 120) {
      // Cool down
      this.state = 'cooldown';
      this.rpmTarget = 800;
      this.coolantTemp = Math.max(88, this.coolantTemp - 1); // Cool down
    } else {
      // Demo complete
      this.state = 'complete';
      this.rpmTarget = 0;
    }

    // Smooth RPM transitions with inertia
    const rpmDiff = this.rpmTarget - this.rpmCurrent;
    this.rpmCurrent += rpmDiff * 0.05; // 5% per frame = 1.25s ramp time

    // Oil pressure is directly related to RPM
    if (this.rpmCurrent < 600) {
      this.oilPressure = 5 + (this.rpmCurrent / 600) * 15; // Ramp 5→20 PSI as RPM increases
    } else if (this.rpmCurrent < 1000) {
      this.oilPressure = 20 + ((this.rpmCurrent - 600) / 400) * 20; // Ramp 20→40 PSI
    } else {
      // Normal running: 60-80 PSI with slight variation
      this.oilPressure = 65 + Math.random() * 10 - 5;
    }

    // Fuel consumption: ~1% per minute of normal operation
    this.fuelLevel = Math.max(0, 100 - (elapsed / 60) * 1);
  }

  /**
   * Generate J1939 frame data (8 bytes)
   * Simplified mock format for demo purposes
   */
  generateFrame() {
    this.simulateOperatingPattern(this.getElapsedSeconds());

    // Create 8-byte CAN frame
    // Byte 0-1: RPM (16-bit, each unit = 0.125 RPM)
    // Byte 2: Coolant temp (0-255, scaled to 0-150°C)
    // Byte 3: Oil pressure (0-255, scaled to 0-100 PSI)
    // Byte 4: Fuel level (0-100%)
    // Byte 5-7: Reserved

    const rpmValue = Math.round(this.rpmCurrent / 0.125);
    const tempValue = Math.round((this.coolantTemp / 150) * 255);
    const pressureValue = Math.round((this.oilPressure / 100) * 255);
    const fuelValue = Math.round(this.fuelLevel);

    const byteArray = [
      (rpmValue >> 8) & 0xff,
      rpmValue & 0xff,
      tempValue & 0xff,
      pressureValue & 0xff,
      fuelValue & 0xff,
      0,
      0,
      0,
    ];

    // Convert byte array to hex string (e.g., "1122334455667788")
    const hexData = byteArray.map(b => b.toString(16).padStart(2, '0')).join('').toUpperCase();

    return {
      equipmentId: this.equipmentId,
      timestamp: new Date().toISOString(),
      canId: '0x0CF00400', // J1939 EEC1 (Electronic Engine Controller 1)
      data: hexData, // Send as hex string, not array
      state: this.state,
      rpm: Math.round(this.rpmCurrent * 100) / 100,
      coolantTemp: Math.round(this.coolantTemp * 100) / 100,
      oilPressure: Math.round(this.oilPressure * 100) / 100,
      fuelLevel: Math.round(this.fuelLevel * 100) / 100,
    };
  }
}

/**
 * POST frame to backend API
 */
async function postFrame(frame) {
  return new Promise((resolve, reject) => {
    const url = new URL(`${API_BASE_URL}/api/stream/ingest`);
    const data = JSON.stringify(frame);

    const options = {
      hostname: url.hostname,
      port: url.port || 5038,
      path: url.pathname + url.search,
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Content-Length': Buffer.byteLength(data),
      },
    };

    const req = http.request(options, (res) => {
      let responseData = '';
      res.on('data', (chunk) => {
        responseData += chunk;
      });
      res.on('end', () => {
        if (res.statusCode === 200 || res.statusCode === 201) {
          resolve({ success: true, status: res.statusCode });
        } else {
          reject(new Error(`HTTP ${res.statusCode}: ${responseData}`));
        }
      });
    });

    req.on('error', reject);
    req.write(data);
    req.end();
  });
}

/**
 * Main simulator loop
 */
async function runSimulation() {
  console.log('\n🚀 FleetPulse Diagnostic Simulator Started');
  console.log(`📍 Target API: ${API_BASE_URL}`);
  console.log(`⏱️  Duration: ${DEMO_DURATION_SECONDS} seconds`);
  console.log(`📡 Frame rate: ${1000 / FRAME_INTERVAL} FPS (${FRAME_INTERVAL}ms interval)\n`);

  // Initialize simulators for all equipment
  const simulators = EQUIPMENT.map(
    (eq, idx) => new J1939Simulator(eq.id, idx * 2000) // Stagger starts by 2 seconds
  );

  const startTime = Date.now();
  let frameCount = 0;
  let errorCount = 0;

  console.log('Equipment Simulation Schedule:');
  EQUIPMENT.forEach((eq, idx) => {
    console.log(`  [${idx * 2}s] ${eq.name} (${eq.id})`);
  });
  console.log('\n');

  // Simulation loop
  const intervalId = setInterval(async () => {
    const elapsed = (Date.now() - startTime) / 1000;
    const progress = (elapsed / DEMO_DURATION_SECONDS) * 100;

    // Generate and post frames for each equipment
    for (const simulator of simulators) {
      const frame = simulator.generateFrame();

      try {
        await postFrame(frame);
        frameCount++;

        // Print status every 20 frames
        if (frameCount % 20 === 0) {
          console.log(
            `[${elapsed.toFixed(1)}s] ${frame.equipmentId} | ` +
              `RPM: ${frame.rpm.toFixed(0)} | ` +
              `Coolant: ${frame.coolantTemp.toFixed(1)}°C | ` +
              `Oil: ${frame.oilPressure.toFixed(1)} PSI | ` +
              `State: ${frame.state}`
          );
        }
      } catch (error) {
        errorCount++;
        if (errorCount <= 3) {
          // Only log first 3 errors to avoid spam
          console.error(`❌ Failed to post frame: ${error.message}`);
        }
      }
    }

    // Check if simulation complete
    if (elapsed >= DEMO_DURATION_SECONDS) {
      clearInterval(intervalId);

      console.log('\n✅ Simulation Complete!');
      console.log(`📊 Stats:`);
      console.log(`   Total Frames Posted: ${frameCount}`);
      console.log(`   Errors: ${errorCount}`);
      console.log(`   Duration: ${elapsed.toFixed(1)}s`);
      console.log(`   Equipment: ${EQUIPMENT.length}`);
      console.log(`\n💡 Check http://localhost:3000 to see live gauges responding!\n`);

      process.exit(0);
    }
  }, FRAME_INTERVAL);

  // Graceful shutdown
  process.on('SIGINT', () => {
    clearInterval(intervalId);
    console.log('\n\n⛔ Simulation stopped by user');
    console.log(`📊 Frames posted: ${frameCount}`);
    process.exit(0);
  });
}

// Run simulator
runSimulation().catch((error) => {
  console.error('Fatal error:', error);
  process.exit(1);
});
