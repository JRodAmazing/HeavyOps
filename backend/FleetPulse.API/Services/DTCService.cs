using FleetPulse.API.Models;

namespace FleetPulse.API.Services;

public class DTCService
{
    // Comprehensive J1939 fault code library
    private static readonly Dictionary<string, FaultCode> FaultCodeLibrary = new()
    {
        // Engine/Powerplant Faults
        ["P0001"] = new FaultCode
        {
            Code = "P0001",
            Description = "Fuel Volume Regulator Control Circuit",
            Severity = "warning",
            Component = "engine",
            PossibleCauses = new[] { "Fuel regulator fault", "Wiring issue", "Fuel pump problem" },
            SuggestedFixes = new[] { "Check fuel regulator", "Inspect wiring connectors", "Test fuel pump operation" }
        },
        ["P0100"] = new FaultCode
        {
            Code = "P0100",
            Description = "Mass or Volume Air Flow Circuit",
            Severity = "warning",
            Component = "engine",
            PossibleCauses = new[] { "MAF sensor dirty", "Air intake leak", "Sensor wiring issue" },
            SuggestedFixes = new[] { "Clean MAF sensor", "Check for air leaks", "Verify sensor connections" }
        },
        ["P0101"] = new FaultCode
        {
            Code = "P0101",
            Description = "Mass or Volume Air Flow Circuit Range/Performance",
            Severity = "warning",
            Component = "engine",
            PossibleCauses = new[] { "Air filter clogged", "MAF sensor malfunction", "Intake manifold leak" },
            SuggestedFixes = new[] { "Replace air filter", "Check MAF sensor", "Inspect manifold gaskets" }
        },
        ["P0110"] = new FaultCode
        {
            Code = "P0110",
            Description = "Intake Air Temperature Sensor Circuit",
            Severity = "warning",
            Component = "engine",
            PossibleCauses = new[] { "Sensor failure", "Wiring open/short", "PCM issue" },
            SuggestedFixes = new[] { "Replace IAT sensor", "Check wiring harness", "Scan for PCM faults" }
        },

        // Oil Pressure Faults
        ["P0520"] = new FaultCode
        {
            Code = "P0520",
            Description = "Engine Oil Pressure Sensor/Switch Circuit",
            Severity = "critical",
            Component = "engine",
            PossibleCauses = new[] { "Low oil level", "Oil pump failure", "Sensor malfunction", "Clogged filter" },
            SuggestedFixes = new[] { "Check oil level immediately", "Inspect oil pump", "Replace oil pressure sensor", "Service oil filter" }
        },
        ["P0521"] = new FaultCode
        {
            Code = "P0521",
            Description = "Engine Oil Pressure Sensor/Switch Circuit Range/Performance",
            Severity = "critical",
            Component = "engine",
            PossibleCauses = new[] { "Low oil viscosity", "Worn bearings", "Sensor circuit fault" },
            SuggestedFixes = new[] { "Verify oil grade and level", "Inspect engine bearings", "Test sensor circuit" }
        },

        // Coolant Temperature Faults
        ["P0115"] = new FaultCode
        {
            Code = "P0115",
            Description = "Engine Coolant Temperature Sensor Circuit",
            Severity = "warning",
            Component = "engine",
            PossibleCauses = new[] { "Thermostat stuck", "Temperature sensor failure", "Wiring issue" },
            SuggestedFixes = new[] { "Check thermostat operation", "Replace coolant temp sensor", "Inspect wiring connectors" }
        },
        ["P0118"] = new FaultCode
        {
            Code = "P0118",
            Description = "Engine Coolant Temperature Sensor Circuit High",
            Severity = "warning",
            Component = "engine",
            PossibleCauses = new[] { "Cooling system failure", "Radiator clogged", "Water pump failure" },
            SuggestedFixes = new[] { "Flush cooling system", "Clean radiator", "Inspect water pump" }
        },

        // Transmission Faults
        ["P0700"] = new FaultCode
        {
            Code = "P0700",
            Description = "Transmission Control System Malfunction",
            Severity = "warning",
            Component = "transmission",
            PossibleCauses = new[] { "Shift solenoid fault", "Transmission fluid low", "TCM fault" },
            SuggestedFixes = new[] { "Check transmission fluid level", "Inspect shift solenoids", "Scan transmission module" }
        },
        ["P0715"] = new FaultCode
        {
            Code = "P0715",
            Description = "Input/Turbine Shaft Speed Sensor Circuit",
            Severity = "warning",
            Component = "transmission",
            PossibleCauses = new[] { "Speed sensor failure", "Wiring issue", "Sensor misalignment" },
            SuggestedFixes = new[] { "Replace speed sensor", "Check wiring harness", "Verify sensor alignment" }
        },

        // Fuel System Faults
        ["P0300"] = new FaultCode
        {
            Code = "P0300",
            Description = "Random/Multiple Cylinder Misfire Detected",
            Severity = "warning",
            Component = "engine",
            PossibleCauses = new[] { "Spark plugs worn", "Fuel injector clogged", "Ignition coil failure" },
            SuggestedFixes = new[] { "Replace spark plugs", "Clean fuel injectors", "Test ignition coils" }
        },
        ["P0401"] = new FaultCode
        {
            Code = "P0401",
            Description = "EGR Flow Insufficient",
            Severity = "warning",
            Component = "engine",
            PossibleCauses = new[] { "EGR valve stuck", "Carbon buildup", "Sensor fault" },
            SuggestedFixes = new[] { "Clean EGR valve", "Remove carbon deposits", "Test EGR sensor" }
        },

        // DPF (Diesel Particulate Filter) Faults - Common on Heavy Equipment
        ["P0480"] = new FaultCode
        {
            Code = "P0480",
            Description = "Diesel Particulate Filter Blockage",
            Severity = "critical",
            Component = "emissions",
            PossibleCauses = new[] { "Soot accumulation", "Regeneration failure", "DPF sensor fault" },
            SuggestedFixes = new[] { "Force DPF regeneration", "Check DPF pressure sensor", "Service filter if needed" }
        },

        // Starter/Alternator Faults
        ["P0622"] = new FaultCode
        {
            Code = "P0622",
            Description = "Generator/Alternator Control Circuit",
            Severity = "warning",
            Component = "electrical",
            PossibleCauses = new[] { "Alternator failure", "Belt wear", "Wiring issue" },
            SuggestedFixes = new[] { "Test alternator output", "Inspect serpentine belt", "Check wiring connections" }
        },

        // Generic J1939 High-Level Faults
        ["J1939-F001"] = new FaultCode
        {
            Code = "J1939-F001",
            Description = "Engine Oil Pressure Low",
            Severity = "critical",
            Component = "engine",
            PossibleCauses = new[] { "Low oil level", "Pump failure", "Bearing wear" },
            SuggestedFixes = new[] { "Top off oil immediately", "Inspect pump", "Check bearings for wear" }
        },
        ["J1939-F002"] = new FaultCode
        {
            Code = "J1939-F002",
            Description = "Engine Coolant Temperature High",
            Severity = "critical",
            Component = "engine",
            PossibleCauses = new[] { "Thermostat failed", "Radiator clogged", "Fan inoperative" },
            SuggestedFixes = new[] { "Replace thermostat", "Flush cooling system", "Check fan operation" }
        },
        ["J1939-F003"] = new FaultCode
        {
            Code = "J1939-F003",
            Description = "Engine Overspeed",
            Severity = "warning",
            Component = "engine",
            PossibleCauses = new[] { "Governor failure", "Fuel control issue", "Sensor malfunction" },
            SuggestedFixes = new[] { "Check governor", "Inspect fuel control", "Test sensors" }
        },
    };

    /// <summary>
    /// Parse a fault code and return detailed information
    /// </summary>
    public FaultCode ParseFaultCode(string code)
    {
        if (string.IsNullOrEmpty(code))
            return new FaultCode { Description = "Unknown fault code" };

        // Try exact match
        if (FaultCodeLibrary.TryGetValue(code.ToUpper(), out var fault))
            return fault;

        // Try without prefix (e.g., "P0520" vs "0520")
        var cleanCode = code.Replace("P", "").Replace("J1939-F", "").ToUpper();
        foreach (var (key, value) in FaultCodeLibrary)
        {
            if (key.Contains(cleanCode))
                return value;
        }

        // Return generic unknown
        return new FaultCode
        {
            Code = code,
            Description = $"Unknown fault code: {code}",
            Severity = "info",
            PossibleCauses = new[] { "Unknown fault" },
            SuggestedFixes = new[] { "Contact service center" }
        };
    }

    /// <summary>
    /// Get all available fault codes (for frontend reference)
    /// </summary>
    public IEnumerable<FaultCode> GetAllFaultCodes()
    {
        return FaultCodeLibrary.Values;
    }

    /// <summary>
    /// Search fault codes by description
    /// </summary>
    public IEnumerable<FaultCode> SearchFaultCodes(string query)
    {
        if (string.IsNullOrEmpty(query))
            return Enumerable.Empty<FaultCode>();

        var searchTerm = query.ToLower();
        return FaultCodeLibrary.Values
            .Where(f => f.Description.ToLower().Contains(searchTerm) ||
                       f.Code.ToLower().Contains(searchTerm))
            .ToList();
    }
}
