using System.Collections.Generic;

namespace gov.sandia.sld.common.configuration
{
    public class MonitoredDriveManager
    {
        public Dictionary<string, MonitoredDrive> driveMap { get; set; }

        public MonitoredDriveManager()
        {
            driveMap = new Dictionary<string, MonitoredDrive>();
        }

        public void Clear()
        {
            driveMap.Clear();
        }

        public void Add(DriveInfo dn)
        {
            driveMap[dn.letter] = new MonitoredDrive()
            {
                letter = dn.letter,
                name = dn.name,
                type = dn.type
            };
        }

        public void AddRange(List<DriveInfo> names)
        {
            names.ForEach(n => Add(n));
        }

        public bool IsDriveMonitored(string drive_letter)
        {
            // Default to being monitored
            bool is_monitored = true;
            MonitoredDrive d = null;
            if (driveMap.TryGetValue(drive_letter, out d))
                is_monitored = d.isMonitored;
            return is_monitored;
        }
    }
}
