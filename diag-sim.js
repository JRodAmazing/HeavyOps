#!/usr/bin/env node

const http = require('http');
const url = require('url');

const API_BASE = 'http://localhost:5092';
const POLL_INTERVAL = 250;

let state = {
  rpm: 800,
  coolantTemp: 85,
  oilPressure: 45,
  rpmTrend: 1,
  tempTrend: 1,
  pressureTrend: 1,
};

let simConfig = {
  equipmentId: process.argv[2] || 'CAT320',
  simulateOverheat: process.argv.includes('--overheat'),
  simulateLowPressure: process.argv.includes('--lowpressure'),
};

function encodeJ1939Frame(rpm, coolantTemp, oilPressure) {
  const bytes = [];

  const rpmRaw = Math.round(rpm / 0.125);
  bytes.push((rpmRaw >> 8) & 0xff);
  bytes.push(rpmRaw & 0xff);

  const tempRaw = Math.round(coolantTemp + 40);
  bytes.push(Math.min(255, Math.max(0, tempRaw)));

  const pressureRaw = Math.round(oilPressure / 4);
  bytes.push(Math.min(255, Math.max(0, pressureRaw)));

  return bytes.map((b) => b.toString(16).padStart(2, '0')).join('').toUpperCase();
}

function updateState() {
  state.rpm += state.rpmTrend * 20;
  if (state.rpm >= 2200) state.rpmTrend = -1;
  if (state.rpm <= 600) state.rpmTrend = 1;

  if (simConfig.simulateOverheat) {
    state.coolantTemp += 1.5;
    if (state.coolantTemp > 115) {
      state.coolantTemp = 85;
      simConfig.simulateOverheat = false;
    }
  } else {
    state.coolantTemp += state.tempTrend * 0.3;
    if (state.coolantTemp >= 95) state.tempTrend = -1;
    if (state.coolantTemp <= 80) state.tempTrend = 1;
  }

  const baselinePressure = (state.rpm / 2000) * 40 + 15;
  if (simConfig.simulateLowPressure) {
    state.oilPressure = Math.max(10, baselinePressure - 10);
  } else {
    state.oilPressure = baselinePressure + Math.random() * 5;
  }

  return {
    rpm: Math.round(state.rpm),
    coolantTemp: Math.round(state.coolantTemp * 10) / 10,
    oilPressure: Math.round(state.oilPressure * 10) / 10,
  };
}

async function postFrame() {
  const values = updateState();
  const hexData = encodeJ1939Frame(values.rpm, values.coolantTemp, values.oilPressure);

  const payload = {
    equipmentId: simConfig.equipmentId,
    timestamp: new Date().toISOString(),
    canId: '0x0CF00400',
    data: hexData,
  };

  return new Promise((resolve, reject) => {
    const options = url.parse(`${API_BASE}/api/stream/ingest`);
    options.method = 'POST';
    options.headers = {
      'Content-Type': 'application/json',
      'Content-Length': Buffer.byteLength(JSON.stringify(payload)),
    };

    const req = http.request(options, (res) => {
      if (res.statusCode >= 400) {
        reject(new Error(`HTTP ${res.statusCode}`));
      }

      res.on('data', () => {});
      res.on('end', () => resolve(values));
    });

    req.on('error', reject);
    req.write(JSON.stringify(payload));
    req.end();
  });
}

async function run() {
  console.log(`
╔════════════════════════════════════════════════╗
║     HeavyOps Diagnostic Simulator V1          ║
╚════════════════════════════════════════════════╝

🎯 Target Equipment: ${simConfig.equipmentId}
🔗 API: ${API_BASE}
📡 Poll Interval: ${POLL_INTERVAL}ms
${simConfig.simulateOverheat ? '🔥 Simulating Overheat' : ''}
${simConfig.simulateLowPressure ? '⚠️  Simulating Low Oil Pressure' : ''}

Starting in 2 seconds...
  `);

  setTimeout(async () => {
    let frameCount = 0;
    let successCount = 0;
    let errorCount = 0;
    const startTime = Date.now();

    console.log('📤 Posting frames...\n');

    const interval = setInterval(async () => {
      try {
        const values = await postFrame();
        successCount++;

        if (successCount % 10 === 0) {
          const elapsed = Math.round((Date.now() - startTime) / 1000);
          console.log(
            `[${elapsed}s] ✅ Frame #${successCount} | RPM: ${values.rpm.toFixed(0).padStart(4)} | ` +
              `Coolant: ${values.coolantTemp.toFixed(1).padStart(5)}°C | ` +
              `Oil: ${values.oilPressure.toFixed(1).padStart(5)} PSI`
          );
        }
      } catch (error) {
        errorCount++;
        if (errorCount === 1) {
          console.error(
            `❌ Error posting frame: ${error.message}\n` +
              `   Make sure backend is running: dotnet run`
          );
        }
      }

      frameCount++;
    }, POLL_INTERVAL);

    process.on('SIGINT', () => {
      clearInterval(interval);
      const elapsed = Math.round((Date.now() - startTime) / 1000);
      console.log(`\n\n📊 Simulation stopped after ${elapsed}s`);
      console.log(`   Total frames: ${frameCount}`);
      console.log(`   Successful: ${successCount}`);
      console.log(`   Errors: ${errorCount}`);
      process.exit(0);
    });
  }, 2000);
}

run().catch(console.error);