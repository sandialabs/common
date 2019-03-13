using System.Collections.Generic;

namespace gov.sandia.sld.common.configuration
{
    public enum EDeviceType
    {
        Server,
        Workstation,
        Camera,
        RPM,
        System,
        Generic,

        Unknown = -1,
    }

    public static class DeviceTypeExtensions
    {
        public static bool IsWindowsMachine(this EDeviceType d)
        {
            return d == EDeviceType.Server || d == EDeviceType.Workstation;
        }

        public static bool IsValidCollector(this EDeviceType d, ECollectorType c)
        {
            bool is_collector = false;

            if (c_device_to_collectors.ContainsKey(d))
                is_collector = c_device_to_collectors[d].Contains(c);

            return is_collector;
        }

        static DeviceTypeExtensions()
        {
            c_device_to_collectors = new Dictionary<EDeviceType, List<ECollectorType>>();
            c_device_to_collectors[EDeviceType.System] = EDeviceType.System.GetCollectors();
            c_device_to_collectors[EDeviceType.Server] = EDeviceType.Server.GetCollectors();
            c_device_to_collectors[EDeviceType.Workstation] = EDeviceType.Workstation.GetCollectors();
        }

        private static Dictionary<EDeviceType, List<ECollectorType>> c_device_to_collectors;
    }
}
