
using System;
using System.Collections.Generic;
using System.Reflection;

namespace gov.sandia.sld.common.configuration
{
    public enum ECollectorType
    {
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(30)]
        Memory,                 // 0
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(30)]
        Disk,
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(30)]
        CPUUsage,
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(30)]
        NICUsage,
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(1440)]
        Uptime,
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(1440)]
        LastBootTime,           // 5
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(30)]
        Processes,
        [Devices(EDeviceType.System)]
        [DefaultCollectionFrequency(30)]
        Ping,
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(1440)]
        InstalledApplications,
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(30)]
        Services,
        //Database,               // 10
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(30)]
        SystemErrors = 11,
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(30)]
        ApplicationErrors,
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(360)]
        DatabaseSize,
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(30)]
        UPS,
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(30)]
        DiskSpeed,              // 15
        [Devices(EDeviceType.System)]
        // We don't actively go out and collect the configuration. Instead, we record it as it changes,
        // so not setting a frequency indicates no collection.
        //[DefaultCollectionFrequency(30)]
        [SkipConfiguration]
        Configuration,
        [Devices(EDeviceType.Server, EDeviceType.Workstation)]
        [DefaultCollectionFrequency(360)]
        SMART,
        //[Devices(DeviceType.Server, DeviceType.Workstation)]
        //[DefaultCollectionFrequency(30)]
        //AntiVirus,
        //[Devices(DeviceType.Server, DeviceType.Workstation)]
        //[DefaultCollectionFrequency(30)]
        //Firewall,

        [SkipConfiguration]
        Unknown = -1,
    }

    public class DevicesAttribute : Attribute
    {
        public List<EDeviceType> Devices { get; private set; }

        public DevicesAttribute(EDeviceType device)
        {
            Devices = new List<EDeviceType>(new EDeviceType[] { device });
        }

        public DevicesAttribute(EDeviceType device1, EDeviceType device2)
        {
            Devices = new List<EDeviceType>(new EDeviceType[] { device1, device2 });
        }

        public DevicesAttribute(List<EDeviceType> devices)
        {
            Devices = new List<EDeviceType>(devices);
        }
    }

    public class DefaultCollectionFrequencyAttribute : Attribute
    {
        public int FrequencyInMinutes { get; private set; }

        public DefaultCollectionFrequencyAttribute(int frequency_in_minutes)
        {
            FrequencyInMinutes = frequency_in_minutes;
        }
    }

    /// <summary>
    /// Mark that a CollectorType shouldn't be included in the configuration
    /// </summary>
    public class SkipConfigurationAttribute : Attribute
    {
        public bool SkipConfiguration { get { return true; } }

        public SkipConfigurationAttribute()
        {
        }
    }

    public static class CollectorTypeExtensions
    {
        public static List<EDeviceType> GetDevices(this ECollectorType c)
        {
            List<EDeviceType> types = new List<EDeviceType>();
            Type type = c.GetType();
            MemberInfo[] memInfo = type.GetMember(c.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DevicesAttribute), false);
                if (attrs != null)
                {
                    foreach (object o in attrs)
                    {
                        DevicesAttribute attr = o as DevicesAttribute;
                        types.AddRange(attr.Devices);
                    }
                }
            }
            return types;
        }

        public static int GetFrequencyInMinutes(this ECollectorType c)
        {
            int frequency = 0;
            Type type = c.GetType();
            MemberInfo[] memInfo = type.GetMember(c.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DefaultCollectionFrequencyAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    frequency = (attrs[0] as DefaultCollectionFrequencyAttribute).FrequencyInMinutes;
            }
            return frequency;
        }

        public static bool GetSkipConfiguration(this ECollectorType c)
        {
            bool skip_configuration = false;
            Type type = c.GetType();
            MemberInfo[] memInfo = type.GetMember(c.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(SkipConfigurationAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    skip_configuration = (attrs[0] as SkipConfigurationAttribute).SkipConfiguration;
            }
            return skip_configuration;
        }

        public static List<ECollectorType> GetCollectors(this EDeviceType d)
        {
            List<ECollectorType> collectors = new List<ECollectorType>();
            foreach (ECollectorType c in Enum.GetValues(typeof(ECollectorType)))
            {
                List<EDeviceType> devices = c.GetDevices();
                if (devices.Contains(d))
                    collectors.Add(c);
            }
            return collectors;
        }

        public static ECollectorType CollectorTypeFromCollectorNameString(string collector_name)
        {
            ECollectorType type = ECollectorType.Unknown;

            return type;
        }
    }
}
