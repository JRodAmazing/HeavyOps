# FleetPulse - Complete Build Spec & Vision

## 🎯 PROJECT VISION

**FleetPulse** is a full-stack diagnostic & maintenance dashboard for heavy equipment (excavators, loaders, etc.). Built to be **hire-ready** for industrial software roles AND capable of generating freelance revenue.

---

## 📊 WHAT "DONE" LOOKS LIKE (Complete Picture)

### By Week 8 (Nov 4, 2025):

**You Have:**
- ✅ Professional GitHub portfolio (clean code, great README)
- ✅ Working full-stack application (React + .NET + Diagnostics)
- ✅ Demo videos showing real features
- ✅ Deployable to cloud (Vercel + Railway)
- ✅ Revenue-generating capability (freelance path)
- ✅ Hire-ready skills showcase

**Ready For:**
- Job interviews at Diesel Laptops, CAT Digital, Fleet SaaS companies
- Freelance engagements ($300-500 per shop for branded dashboards)
- Further development into full product

---

## 🏗️ ARCHITECTURE

```
FleetPulse (Monorepo)
├── frontend/                    # Next.js 15 React app
│   ├── components/              # Reusable UI (Gauge, LiveTelemetry, etc)
│   ├── pages/                   # Routes (dashboard, equipment, detail)
│   ├── lib/                     # Utilities (j1939Constants, API clients)
│   └── styles/                  # Tailwind CSS
├── backend/                     # .NET 9 Web API
│   ├── FleetPulse.API/          # ASP.NET Core controllers
│   ├── FleetPulse.Data/         # EF Core models
│   └── FleetPulse.Tests/        # Unit tests
├── tools/diag-sim/              # Diagnostic simulator (Node.js)
├── ai/                          # Python AI services (future)
└── docs/                        # Documentation & roadmap
```

---

## 📈 THE PHASES (What You're Building)

### **PHASE 1: MVP (COMPLETE BY END OF DAY)**
**Deliverable:** Hire-ready demo with all core features

**Features:**
- ✅ Equipment CRUD + list/detail views
- ✅ Service log tracking
- ✅ PDF report generation (branded)
- ✅ J1939 diagnostic stream ingestion
- ✅ Live telemetry gauges (RPM, Coolant, Oil, Fuel)
- ✅ Alert thresholds
- ✅ Realistic data simulator
- ✅ Demo video

**Tech Stack:**
- Frontend: Next.js 15, React, TypeScript, Tailwind CSS
- Backend: .NET 9, C#, ASP.NET Core, QuestPDF
- Diagnostics: J1939 CAN frame parsing
- Deployment: Local dev (Vercel/Railway ready)

**Why it works:**
- Shows J1939 protocol knowledge (Diesel Laptops requirement)
- Demonstrates full-stack thinking (React + .NET)
- Professional UI with real graphics (their main concern)
- Clean code, good documentation

---

### **PHASE 2: Production-Ready (Weeks 4-6)**
**Goal:** Make it look enterprise-grade; start applying to jobs

**Add:**

#### 2.1 DTC (Fault Code) Parser
- Parse J1939 fault codes
- Map to human-readable descriptions
- Show probable causes
- Example: `0x523F` → "Engine Oil Pressure Low" + causes + fixes

**Impact:** Shows you understand OBD/diagnostic standards

#### 2.2 Fleet Health Dashboard
- Summary view of all equipment
- Status indicators (green/yellow/red)
- Quick health checks
- Maintenance alerts
- One-click drill-down

**Impact:** Shows you think about multiple equipment, SaaS patterns

#### 2.3 Maintenance Scheduling
- Log maintenance events (oil change, filter, repair)
- Track history per equipment
- Show "due date" for next maintenance
- Integration with PDF reports

**Impact:** Shows business logic, real-world thinking

#### 2.4 Role-Based Auth
- User roles: Admin, Mechanic, Shop Owner
- JWT tokens (boilerplate)
- Secure endpoints with `[Authorize]`
- Middleware for permission checks

**Impact:** Shows you understand security, enterprise patterns

#### 2.5 API Documentation
- Clean Swagger/OpenAPI docs
- Example requests/responses
- Error handling standards

**Impact:** Professional, hire-ready API

**Deliverable:** Tag `v0.2.0-phase2` + updated demo

---

### **PHASE 3: AI & Competitive Edge (Weeks 7-8)**
**Goal:** Differentiate; become a strong candidate

**Add:**

#### 3.1 Mechanic Assistant (LLM)
- Input: Fault codes + service history
- Output: Likely causes + repair steps + parts list
- Integration: OpenAI API
- Frontend: Chat-like interface

**Example Flow:**
```
Input: Detected low oil pressure + engine running hot
AI Output:
- LIKELY CAUSE: Low oil level due to leak
- REPAIR STEPS:
  1. Add 5L of 15W-40 engine oil
  2. Run diagnostic again
  3. If persists, inspect oil pan for leaks
- REQUIRED PARTS: Oil pan gasket ($45), Bolts ($5)
- TIME ESTIMATE: 2-3 hours labor
```

#### 3.2 Predictive Maintenance
- Simple ML model (Python)
- Track: Operating hours → failures → patterns
- Predict: "Equipment X likely needs oil change in 5 days"
- Integrate with frontend alerts

**Example:**
```
Equipment: CAT320
Hours since last oil change: 245
Average failure point: 250 hours
ALERT: Schedule oil change within 5 days
Risk if ignored: 35% chance of engine damage
```

#### 3.3 Live Deployment
- Push frontend to **Vercel** (1-click)
- Push backend to **Railway** or **Render** (cloud .NET hosting)
- Add CI/CD pipeline (GitHub Actions)
- Live URL for interviewers to test

#### 3.4 Testimonials & Case Study
- Deploy for real shop (or simulate)
- Collect feedback
- Create 1-2 page case study
- Show "before/after" metrics

**Deliverable:** Tag `v0.3.0-phase3` + live deployment + case study

---

## 💡 THE END RESULT

### What You Present to Employers:

```
Portfolio Pitch:
"I built FleetPulse, a full-stack diagnostic system for heavy equipment.

✅ Real J1939 CAN protocol handling
✅ Professional UI with live gauges
✅ Real-time data visualization
✅ Alert thresholds & safety logic
✅ PDF report generation
✅ Multi-equipment fleet management
✅ Role-based access control
✅ AI-powered diagnostics (Phase 3)
✅ Cloud deployment

Built in 8 weeks solo. Live demo: [URL]
Code: [GitHub]
"
```

### What You Can Sell:

1. **Branded Dashboard** ($300-500 per shop)
   - Custom logo/branding
   - Pre-configured for their equipment
   - Training included

2. **Predictive Maintenance** ($1000+)
   - ML model trained on their data
   - Monthly maintenance alerts
   - ROI: Prevents expensive breakdowns

3. **Custom Diagnostics** ($2000+)
   - Add equipment-specific protocols
   - Custom fault codes
   - Integration with their workflow

---

## 🎯 KEY FEATURES BREAKDOWN

### 1. Equipment Management
**What:**
- Add/edit/delete equipment
- Track hours, status, location
- Bulk operations

**Why it matters:**
- Basic CRUD shows you can build admin UIs
- Bulk operations show you think about scale

### 2. Service Logs
**What:**
- Create service records (date, hours, notes, cost)
- PDF export per record
- History view

**Why it matters:**
- Shows business logic
- Document generation is valuable skill

### 3. Live Telemetry
**What:**
- Real-time gauges (RPM, Coolant, Oil, Fuel)
- Smooth animations
- Alert thresholds
- Data polling from backend

**Why it matters:**
- Graphics/UI is the main hiring concern for DL/automotive
- Shows you can build professional visualizations
- Real-time data handling

### 4. Diagnostics Parser
**What:**
- Parse J1939 frames
- Extract fault codes
- Map to descriptions
- Historical trends

**Why it matters:**
- Core automotive skill
- Shows protocol knowledge
- Builds on real industry standards

### 5. Alert System
**What:**
- Thresholds (warning/critical)
- Visual/audio alerts
- Alert history
- Action logging

**Why it matters:**
- Safety-critical systems require alerts
- Shows you think about edge cases

### 6. Reporting
**What:**
- Professional PDF reports
- Branded with shop logo
- Include: Equipment data, service history, diagnostics, recommendations
- One-click download

**Why it matters:**
- Document generation = real revenue stream
- Client-facing deliverable = professional

---

## 🚀 TODAY'S WINS

By EOD today, you should have:

1. ✅ **Professional gauges** (not jittery, proper scaling)
2. ✅ **Realistic data** (actual J1939 ranges)
3. ✅ **Working simulator** (posts frames every 250ms)
4. ✅ **Live demo video** (2 mins showing everything)
5. ✅ **Phase 2 plan ready** (DTC parser, fleet dashboard, maintenance, auth)

---

## 📋 QUICK REFERENCE

### Tech Stack Memo
| Layer | Tech | Why |
|-------|------|-----|
| Frontend | Next.js 15 + React + TS | Modern, fast, SSR ready |
| Styling | Tailwind CSS | Professional, scalable |
| Backend | .NET 9 + C# | Industry standard, fast, typed |
| Database | SQLite (dev) → PostgreSQL (prod) | Simple setup, scales well |
| Diagnostics | J1939 CAN parsing | Diesel Laptops requirement |
| PDF | QuestPDF | Professional, C#-native |
| AI (later) | Python + OpenAI API | Easy integration, powerful |
| Deployment | Vercel (frontend) + Railway (backend) | Easy, affordable, production-ready |

### API Endpoints (Phase 1)
```
Equipment:
  GET    /api/equipment
  POST   /api/equipment
  GET    /api/equipment/{id}
  PUT    /api/equipment/{id}
  DELETE /api/equipment/{id}

Service Logs:
  GET    /api/equipment/{id}/logs
  POST   /api/equipment/{id}/logs
  DELETE /api/equipment/{id}/logs/{logId}

Reports:
  GET    /api/reports/{id}/generate  → PDF

Telemetry:
  POST   /api/stream/ingest
  GET    /api/stream/{equipmentId}/latest
  GET    /api/stream/{equipmentId}/recent
```

### Realistic Metrics (Always Use These)
```
RPM:
  Idle: 800 RPM
  Normal: 1200-2200 RPM
  High: 2100-2700 RPM
  Warning: >2500
  Critical: >2800

Coolant (°C):
  Cold: 60
  Warm-up: 60-85
  Normal: 85-95
  Warning: >105
  Critical: >115

Oil Pressure (PSI):
  Idle: 15-40
  Normal: 60-80
  Low warning: <20
  Critical: <10

Fuel (%):
  Normal: 50-100
  Warning: <20
  Critical: <5
```

---

## ✅ SUCCESS CRITERIA

### Phase 1 (Today):
- [ ] Gauges are smooth (no jitter)
- [ ] Values match realistic ranges
- [ ] Simulator posts frames successfully
- [ ] Frontend gauges respond in real-time
- [ ] Alerts trigger correctly
- [ ] Demo video recorded
- [ ] Code committed to GitHub

### Phase 2 (Weeks 4-6):
- [ ] DTC parser working
- [ ] Fleet dashboard showing multi-equipment
- [ ] Maintenance scheduling functional
- [ ] Auth system scaffolded
- [ ] API docs clean
- [ ] Demo updated

### Phase 3 (Weeks 7-8):
- [ ] AI assistant responding to queries
- [ ] Predictive maintenance showing alerts
- [ ] Live deployment working (Vercel + Railway)
- [ ] Case study written
- [ ] Ready to send to employers

---

## 🎓 LEARNING OUTCOMES

By building this, you learn:
- J1939 protocol & CAN diagnostics (automotive industry standard)
- Full-stack development (React + .NET)
- Professional UI/UX (gauges, alerts, dashboards)
- Real-time data visualization
- Document generation (PDFs)
- Database design (EF Core)
- API design & RESTful patterns
- Authentication & authorization
- Cloud deployment
- AI integration
- Business thinking (pricing, positioning, freelance revenue)

---

## 💰 REVENUE POTENTIAL

### Freelance Path:
- **Branded Dashboard:** $300-500/shop
- **Multi-shop licensing:** $100/month per shop
- **Predictive maintenance:** $1000+ per setup
- **Custom integrations:** $2000+ per client

**Realistic Target:** 2-3 shops in first 3 months = $1500-3000

### Employment Path:
- Entry-level diagnostic engineer: $65-75K
- Mid-level automotive software engineer: $85-120K
- Senior fleet SaaS architect: $150K+

This portfolio proves you can do the work.

---

## 🎯 YOUR NORTH STAR

**Remember:** The goal is not perfection. The goal is **demonstrable capability**.

By end of Week 8, you have:
- ✅ Working software (not alpha, not a toy)
- ✅ Professional UI (clients will trust it)
- ✅ Real features (not just buttons)
- ✅ Revenue potential (proves business viability)
- ✅ Hire-ready demo (gets you interviews)

This is enough to:
1. Get hired at a mid-tier automotive software company
2. Generate $500-1000/month in freelance revenue
3. Stand out among junior developers
4. Build a sustainable career in industrial software

---

**NOW GO BUILD. You've got this. 💪**
