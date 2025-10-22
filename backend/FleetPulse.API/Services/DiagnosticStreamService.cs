using FleetPulse.API.Models;

namespace FleetPulse.API.Services
{
    public class DiagnosticStreamService
    {
        private readonly Dictionary<string, Queue<DiagnosticFrame>> _frameBuffer = new();
        private readonly int _maxFramesPerEquipment = 60;

        public void AddFrame(DiagnosticFrame frame)
        {
            if (!_frameBuffer.ContainsKey(frame.EquipmentId))
                _frameBuffer[frame.EquipmentId] = new Queue<DiagnosticFrame>();
            var queue = _frameBuffer[frame.EquipmentId];
            queue.Enqueue(frame);
            while (queue.Count > _maxFramesPerEquipment) queue.Dequeue();
        }

        public DiagnosticFrame? GetLatestFrame(string equipmentId)
        {
            if (!_frameBuffer.ContainsKey(equipmentId) || _frameBuffer[equipmentId].Count == 0)
                return null;
            return _frameBuffer[equipmentId].Last();
        }

        public List<DiagnosticFrame> GetRecentFrames(string equipmentId, int count = 60)
        {
            if (!_frameBuffer.ContainsKey(equipmentId))
                return new List<DiagnosticFrame>();
            return _frameBuffer[equipmentId].TakeLast(count).ToList();
        }
    }
}
