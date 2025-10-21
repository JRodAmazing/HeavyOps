#!/usr/bin/env node
const http = require('http');
const url = require('url');

const API_BASE_URL = process.env.API_BASE_URL || 'http://localhost:5038';
const DURATION = parseInt(process.env.DURATION) || 300;
const SCENARIO = process.env.SCENARIO || 'normal';
const FRAME_INTERVAL = 250;

const EQUIPMENT_IDS = ['CAT320', 'KOMATSU350', 'VOLVO240'];

const equipmentState = {
  CAT320: {
    rpm: 800,
    rpmTarget: 800,
    coolantTemp: 85,
    coolantTarget: 85,
    oilPressure: 45,
    oilTarget: 45,
    engineHours: 2450
  },
  KOMATSU350: {
    rpm: 750,
    rpmTarget: 750,
    coolantTemp: 82,
    coolantTarget: 82,
    oilPressure: 42,
    oilTarget: 42,
    engineHours: 3120
  },
  VOLVO240: {
    rpm: 700,
    rpmTarget: 700,
    coolantTemp: 80,
    coolantTarget: 80,
    oilPressure: 40,
    oilTarget: 40,
    engineHours: 1890
  }
};

function generateFrame(equipmentId, state) {
  const canId = getCanIdForEquipment(equipmentId);
  updateState(equipmentId, state);
  const rpmValue = Math.floor(state.rpm * 0.5);
  const rpmHex = rpmValue.toString(16).padStart(4, '0');
  const coolantValue = Math.floor(state.coolantTemp + 40);
  const coolantHex = coolantValue.toString(16).padStart(4, '0');
  const oilValue = Math.floor(state.oilPressure * 100 / 5);
  const oilHex = oilValue.toString(16).padStart(4, '0');
  const data = (rpmHex + coolantHex + oilHex + '0000').substring(0, 16).toUpperCase();
  return {
    equipmentId,
    timestamp: new Date().toISOString(),
    canId: `0x${canId}`,
    data: data
  };
}

function getCanIdForEquipment(equipmentId) {
  const canIds = {
    CAT320: '0CF00400',
    KOMATSU350: '0CF00401',
    VOLVO240: '0CF00402'
  };
  return canIds[equipmentId] || '0CF00400';
}

function updateState(equipmentId, state) {
  const elapsedSeconds = Math.floor((Date.now() - startTime) / 1000);
  if (SCENARIO === 'normal') {
    if (elapsedSeconds < 10) {
      state.rpmTarget = 800 + (elapsedSeconds * 60);
      state.coolantTarget = 85;
    } else if (elapsedSeconds < 30) {
      state.rpmTarget = 1400 + (Math.random() - 0.5) * 200;
      state.coolantTarget = Math.min(92, state.coolantTarget + 0.2);
    } else if (elapsedSeconds < 50) {
      state.rpmTarget = 1800 + (Math.random() - 0.5) * 300;
      state.coolantTarget = Math.min(95, state.coolantTarget + 0.3);
    } else if (elapsedSeconds < 60) {
      state.rpmTarget = 1000 + (Math.random() - 0.5) * 100;
      state.coolantTarget = Math.max(85, state.coolantTarget - 0.3);
    } else {
      state.rpmTarget = 900 + (Math.random() - 0.5) * 100;
      state.coolantTarget = 85 + (Math.random() - 0.5) * 2;
    }
    state.oilTarget = 30 + (state.rpmTarget / 120);
  } else if (SCENARIO === 'overheat') {
    if (elapsedSeconds < 10) {
      state.rpmTarget = 800 + (elapsedSeconds * 80);
      state.coolantTarget = 85;
    } else if (elapsedSeconds < 30) {
      state.rpmTarget = 2500;
      state.coolantTarget = Math.min(115, state.coolantTarget + 1.5);
      state.oilTarget = 50;
    } else if (elapsedSeconds < 45) {
      state.rpmTarget = 1200;
      state.coolantTarget = Math.max(100, state.coolantTarget - 0.5);
      state.oilTarget = 35;
    } else {
      state.rpmTarget = 900;
      state.coolantTarget = Math.max(85, state.coolantTarget - 0.8);
      state.oilTarget = 35;
    }
  } else if (SCENARIO === 'lowpressure') {
    if (elapsedSeconds < 10) {
      state.rpmTarget = 800 + (elapsedSeconds * 80);
    } else if (elapsedSeconds < 30) {
      state.rpmTarget = 2000;
      state.oilTarget = Math.max(12, state.oilTarget - 0.3);
    } else if (elapsedSeconds < 45) {
      state.rpmTarget = 1200;
      state.oilTarget = 18;
    } else {
      state.rpmTarget = 900;
      state.oilTarget = 35;
    }
    state.coolantTarget = 85 + (Math.random() - 0.5) * 5;
  }
  state.rpm += (state.rpmTarget - state.rpm) * 0.15;
  state.coolantTemp += (state.coolantTarget - state.coolantTemp) * 0.10;
  state.oilPressure += (state.oilTarget - state.oilPressure) * 0.12;
  state.rpm += (Math.random() - 0.5) * 25;
  state.coolantTemp += (Math.random() - 0.5) * 0.4;
  state.oilPressure += (Math.random() - 0.5) * 0.8;
}

function postFrame(frame) {
  return new Promise((resolve, reject) => {
    try {
      const parsedUrl = new url.URL(API_BASE_URL);
      const options = {
        hostname: parsedUrl.hostname,
        port: parsedUrl.port || 80,
        path: '/api/stream/ingest',
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Content-Length': Buffer.byteLength(JSON.stringify(frame))
        }
      };
      const req = http.request(options, (res) => {
        let data = '';
        res.on('data', chunk => data += chunk);
        res.on('end', () => {
          if (res.statusCode === 200 || res.statusCode === 201 || res.statusCode === 204) {
            resolve(res.statusCode);
          } else {
            reject(new Error(`HTTP ${res.statusCode}: ${data}`));
          }
        });
      });
      req.on('error', (error) => {
        reject(error);
      });
      req.setTimeout(5000, () => {
        req.destroy();
        reject(new Error('Request timeout'));
      });
      req.write(JSON.stringify(frame));
      req.end();
    } catch (error) {
      reject(error);
    }
  });
}

let startTime = Date.now();
let frameCount = 0;
let errorCount = 0;
let lastPrintTime = 0;

async function runSimulation() {
  console.log('\n' + '='.repeat(60));
  console.log('🚀 FleetPulse Diagnostic Simulator');
  console.log('='.repeat(60));
  console.log(`📍 API Base URL:  ${API_BASE_URL}`);
  console.log(`⏱️  Duration:      ${DURATION} seconds`);
  console.log(`🎯 Scenario:      ${SCENARIO}`);
  console.log(`📊 Equipment:     ${EQUIPMENT_IDS.join(', ')}`);
  console.log(`📤 Frame Rate:    Every ${FRAME_INTERVAL}ms`);
  console.log('='.repeat(60));
  console.log('Starting simulation in 2 seconds...\n');
  await new Promise(resolve => setTimeout(resolve, 2000));
  startTime = Date.now();
  const interval = setInterval(async () => {
    const elapsedSeconds = Math.floor((Date.now() - startTime) / 1000);
    const nowMs = Date.now();
    if (elapsedSeconds >= DURATION) {
      clearInterval(interval);
      await new Promise(resolve => setTimeout(resolve, 500));
      printSummary();
      process.exit(0);
    }
    for (const equipmentId of EQUIPMENT_IDS) {
      const frame = generateFrame(equipmentId, equipmentState[equipmentId]);
      try {
        await postFrame(frame);
        frameCount++;
      } catch (error) {
        errorCount++;
        if (errorCount <= 5) {
          console.error(`  ❌ [${equipmentId}] ${error.message}`);
        }
      }
    }
    if (nowMs - lastPrintTime >= 2000) {
      lastPrintTime = nowMs;
      const state1 = equipmentState.CAT320;
      const state2 = equipmentState.KOMATSU350;
      console.log(
        `[${elapsedSeconds.toString().padStart(3)}s]  ` +
        `CAT320: RPM ${Math.floor(state1.rpm).toString().padStart(4)} | ` +
        `${state1.coolantTemp.toFixed(1).padStart(5)}°C | ` +
        `${state1.oilPressure.toFixed(1).padStart(5)} psi`
      );
    }
  }, FRAME_INTERVAL);
}

function printSummary() {
  const elapsed = Math.floor((Date.now() - startTime) / 1000);
  console.log('\n' + '='.repeat(60));
  console.log('✅ Simulation Complete');
  console.log('='.repeat(60));
  console.log(`📊 Total Frames Posted: ${frameCount}`);
  console.log(`❌ Errors:              ${errorCount}`);
  console.log(`⏱️  Duration:           ${elapsed} seconds`);
  console.log('='.repeat(60));
}

process.on('SIGINT', () => {
  console.log('\n\n⛔ Simulator interrupted');
  printSummary();
  process.exit(0);
});

runSimulation().catch(error => {
  console.error('\n❌ Fatal error:', error);
  process.exit(1);
});
