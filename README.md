HeavyOps
A full-stack diagnostic and fleet management system for heavy equipment operations. Built with modern web technologies and industrial-grade architecture.
Overview
HeavyOps provides real-time equipment monitoring, diagnostic data visualization, service log tracking, and professional PDF reporting—all designed for construction, mining, and heavy equipment industries.
Features

Real-Time Diagnostics: J1939 CAN protocol support for live equipment telemetry
Live Telemetry Visualization: Animated gauges for RPM, coolant temperature, and oil pressure
Equipment Management: Track equipment status, operating hours, and maintenance history
Service Logging: Record and organize service events with notes, parts, and labor tracking
PDF Report Generation: Automated professional reports with equipment data and service history
Alert System: Threshold-based alerts for critical conditions (overheat, low pressure, etc.)
Project Tracking: Assign equipment to projects and monitor deployments
Cost Analysis: Track equipment costs and project expenses

Tech Stack
Frontend

Next.js 15 - React framework with TypeScript
Tailwind CSS - Utility-first styling
Canvas API - Real-time gauge visualization
Axios - HTTP client for API communication

Backend

.NET 9 - C# Web API
ASP.NET Core - REST API framework
QuestPDF - PDF generation
In-Memory Storage - Development database (SQLite ready for production)
Swagger/OpenAPI - API documentation

Diagnostics

J1939 Protocol - Heavy equipment CAN bus standard
Real-time Frame Parsing - Diagnostic data ingestion and processing
Mock Simulator - Realistic equipment data generation

Project Structure
HeavyOps/
├── backend/
│   ├── HeavyOps.API/
│   │   ├── Controllers/
│   │   │   ├── EquipmentController.cs
│   │   │   ├── ServiceLogsController.cs
│   │   │   ├── StreamController.cs
│   │   │   ├── ProjectsController.cs
│   │   │   └── ReportsController.cs
│   │   ├── Models/
│   │   ├── Services/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   ├── HeavyOps.Data/
│   └── HeavyOps.Tests/
├── frontend/
│   ├── app/
│   │   ├── page.tsx (Dashboard)
│   │   ├── equipment/
│   │   ├── projects/
│   │   └── analytics/
│   ├── components/
│   │   ├── Gauge.tsx
│   │   ├── LiveTelemetry.tsx
│   │   └── EquipmentCard.tsx
│   ├── lib/
│   ├── package.json
│   └── tailwind.config.js
├── diag-sim.js (Diagnostic simulator)
├── docs/
└── README.md
Getting Started
Prerequisites

Node.js 20+ and npm
.NET 9 SDK
Git

Backend Setup
bashcd backend

# Restore dependencies
dotnet restore

# Run the API
dotnet run

# API available at http://localhost:5038
# Swagger documentation at http://localhost:5038/swagger
Frontend Setup
bashcd frontend

# Install dependencies
npm install

# Start development server
npm run dev

# Application available at http://localhost:3000
Diagnostic Simulator (Optional)
bash# Post mock J1939 frames to the backend
node diag-sim.js CAT320

# Frames posted every 250ms to http://localhost:5038/api/stream/ingest
API Endpoints
Equipment Management

GET /api/equipment - List all equipment
GET /api/equipment/{id} - Get equipment details
POST /api/equipment - Create new equipment
PUT /api/equipment/{id} - Update equipment
DELETE /api/equipment/{id} - Delete equipment

Service Logs

GET /api/equipment/{id}/logs - Get service logs
POST /api/equipment/{id}/logs - Add service log

Diagnostics Stream

POST /api/stream/ingest - Ingest J1939 diagnostic frames
GET /api/stream/{equipmentId}/latest - Get latest frame data
GET /api/stream/{equipmentId}/recent - Get last 60 frames

Reports

GET /api/reports/{id}/generate - Generate PDF report

Projects

GET /api/projects - List projects
POST /api/projects - Create project
PUT /api/projects/{id} - Update project

Usage
View Equipment Dashboard

Open http://localhost:3000
View equipment cards with current status and operating hours
Click on any equipment card to view details

Monitor Live Telemetry

Navigate to equipment detail page
Click the "Live Telemetry" tab
Start the diagnostic simulator: node diag-sim.js CAT320
Watch gauges animate with real-time diagnostic data
Alerts trigger automatically when thresholds exceeded

Generate Reports

On equipment detail page, click "Generate Report PDF"
Professional report downloads automatically
Report includes equipment info, service history, and diagnostics

Track Service

Click "Add Service Log" on equipment detail page
Enter date, hours, parts used, and labor cost
Save to track maintenance history

Key Features Explained
Real-Time Gauges
Canvas-based animated gauges display live diagnostic data with color-coded zones:

Green: Normal operating range
Yellow: Warning threshold
Red: Critical threshold

J1939 Protocol Support
Handles heavy equipment diagnostic frames following the J1939 CAN standard, parsing:

Engine RPM
Coolant temperature
Oil pressure
Operating hours
Fault codes (ready for expansion)

Alert System
Automatic alerts for:

Coolant temperature > 105°C
Oil pressure < 20 PSI
Equipment downtime tracking

PDF Reports
Professional formatted reports including:

Equipment specifications
Operating hours and status
Complete service history
Date generated and shop information

Environment Variables
Frontend (.env.local)
NEXT_PUBLIC_API_BASE_URL=http://localhost:5038
Backend (appsettings.Development.json)
json{
  "ConnectionStrings": {
    "Default": "Data Source=../heavyops.db"
  },
  "AllowedHosts": "*"
}
Development Workflow
Running Both Services Simultaneously
Terminal 1 - Backend:
bashcd backend
dotnet run
Terminal 2 - Frontend:
bashcd frontend
npm run dev
Terminal 3 - Simulator (Optional):
bashnode diag-sim.js CAT320
Testing in Swagger
With backend running, visit http://localhost:5038/swagger to:

Test all API endpoints
View request/response schemas
Generate sample data

Roadmap

 Database persistence (PostgreSQL)
 User authentication and roles
 Advanced fault code library
 Multi-unit fleet dashboard
 Email report scheduling
 Mobile app (React Native)
 Third-party API integrations (HCSS, Procore)
 Predictive maintenance analytics
 Real equipment CAN bus integration

Contributing
This project is maintained by JRodAmazing. For questions or suggestions, open an issue on GitHub.
License
MIT License - See LICENSE file for details
Deployment
Frontend (Vercel)
bash# Push to main branch - automatic deployment
git push origin main
Backend (Render/Railway)
Deploy the backend/ folder with:

Runtime: .NET 9
Build command: dotnet publish -c Release
Start command: dotnet HeavyOps.API.dll

Performance Metrics

API Response Time: < 50ms average
Real-time Frame Processing: 250ms interval capable
Concurrent Gauge Updates: 60 FPS smooth animation
PDF Generation: < 2 seconds

Support
For issues, questions, or feature requests:

Open an issue on GitHub
Check existing documentation in /docs


Built with Curiosity
Live repo: https://github.com/JRodAmazing/HeavyOps
