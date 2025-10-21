using FleetPulse.API.Models;

namespace FleetPulse.API.Services;

public class DiagnosticStreamService
{
    private static Dictionary<string, List<DiagnosticFrame>> _equipmentFrames = new();
    private const int MAX_FRAMES_PER_EQUIPMENT = 240; // ~60 seconds at 4 Hz

    public void IngestFrame(DiagnosticFrame frame)
    {
        if (!_equipmentFrames.ContainsKey(frame.EquipmentId))
        {
            _equipmentFrames[frame.EquipmentId] = new List<DiagnosticFrame>();
        }

        var frames = _equipmentFrames[frame.EquipmentId];
        frames.Add(frame);

        // Keep only last N frames (rolling window)
        if (frames.Count > MAX_FRAMES_PER_EQUIPMENT)
        {
            frames.RemoveAt(0);
        }
    }

    public List<DiagnosticFrame> GetRecentFrames(string equipmentId, int count = 60)
    {
        if (!_equipmentFrames.ContainsKey(equipmentId))
            return new List<DiagnosticFrame>();

        var frames = _equipmentFrames[equipmentId];
        return frames.Skip(Math.Max(0, frames.Count - count)).ToList();
    }

    public DiagnosticFrame? GetLatestFrame(string equipmentId)
    {
        if (!_equipmentFrames.ContainsKey(equipmentId))
            return null;

        var frames = _equipmentFrames[equipmentId];
        return frames.Count > 0 ? frames[^1] : null;
    }

    public Dictionary<string, object> ParseJ1939Frame(string data)
    {
        // Mock parser for now—converts hex to interpreted values
        // Real implementation would decode actual J1939 PGNs
        
        try
        {
            byte[] bytes = Convert.FromHexString(data);

            return new Dictionary<string, object>
            {
                { "rpm", (bytes.Length > 3 ? bytes[3] : 0) * 8 },
                { "coolant_temp", (bytes.Length > 0 ? bytes[0] : 0) - 40 },
                { "oil_pressure", bytes.Length > 1 ? bytes[1] * 4 : 0 },
                { "raw_bytes", data }
            };
        }
        catch
        {
            return new Dictionary<string, object> { { "error", "Invalid frame data" } };
        }
    }
}
