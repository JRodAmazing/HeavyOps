const axios = require("axios");

const API_URL = "http://localhost:5069/api/stream/ingest";
const EQUIPMENT = ["CAT320", "KOMATSU350", "VOLVO240"];

function getRandomValue(min, max) {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

function generateDiagnosticFrame(equipmentId) {
  // Simulate J1939 data
  // Bytes: [RPM_H, RPM_L, COOLANT, OIL_PRESSURE, ...]
  
  const rpm = getRandomValue(600, 2000);
  const coolant = getRandomValue(85, 110);
  const oilPressure = getRandomValue(25, 60);
  
  const rpmH = Math.floor(rpm / 256);
  const rpmL = rpm % 256;
  
  const data = [rpmH, rpmL, coolant, oilPressure, 0, 0, 0, 0]
    .map(b => b.toString(16).padStart(2, "0").toUpperCase())
    .join("");

  return {
    equipmentId,
    canId: "0x0CF00400",
    data,
    timestamp: new Date().toISOString(),
  };
}

async function postFrame(frame) {
  try {
    await axios.post(API_URL, frame);
    console.log(`✅ ${new Date().toLocaleTimeString()} - Posted frame for ${frame.equipmentId}`);
  } catch (error) {
    console.error(`❌ Error: ${error.message}`);
  }
}

async function runSimulator() {
  console.log("🚀 Diagnostic Simulator started...");
  console.log(`📡 Posting frames every 250ms to ${API_URL}`);
  
  let count = 0;
  setInterval(async () => {
    const equipment = EQUIPMENT[count % EQUIPMENT.length];
    const frame = generateDiagnosticFrame(equipment);
    await postFrame(frame);
    count++;
  }, 250);
}

runSimulator();
