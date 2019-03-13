using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data.database;
using gov.sandia.sld.common.data.wmi;

namespace gov.sandia.sld.common.data
{
    /// <summary>
    /// Generate a Data object from a JSON string based on the type of collector.
    /// 
    /// Is the reverse of taking a Data object, and converting it to JSON, which is
    /// what is stored in the DB.
    /// </summary>
    public class DataFactory
    {
        public static Data Create(DataCollectorContext context, string value)
        {
            Data d = null;

            switch (context.Type)
            {
                case ECollectorType.Memory:
                    d = MemoryUsageCollector.Create(context, value);
                    break;
                case ECollectorType.Disk:
                    d = DiskUsageCollector.Create(context, value);
                    break;
                case ECollectorType.CPUUsage:
                    d = CPUUsageCollector.Create(context, value);
                    break;
                case ECollectorType.NICUsage:
                    d = NICUsageCollector.Create(context, value);
                    break;
                case ECollectorType.Uptime:
                    d = UptimeCollector.Create(context, value);
                    break;
                case ECollectorType.LastBootTime:
                    d = LastBootTimeCollector.Create(context, value);
                    break;
                case ECollectorType.Processes:
                    d = ProcessesCollector.Create(context, value);
                    break;
                case ECollectorType.Ping:
                    d = PingCollector.Create(context, value);
                    break;
                case ECollectorType.InstalledApplications:
                    d = ApplicationsCollector.Create(context, value);
                    break;
                case ECollectorType.Services:
                    d = ServicesCollector.Create(context, value);
                    break;
                case ECollectorType.SystemErrors:
                    d = EventLogCollector.Create(context, value);
                    break;
                case ECollectorType.ApplicationErrors:
                    d = EventLogCollector.Create(context, value);
                    break;
                case ECollectorType.DatabaseSize:
                    d = DatabaseSizeCollector.Create(context, value);
                    break;
                case ECollectorType.UPS:
                    d = UPSCollector.Create(context, value);
                    break;
                case ECollectorType.DiskSpeed:
                    d = DiskSpeedCollector.Create(context, value);
                    break;
                case ECollectorType.Configuration:
                    break;
                case ECollectorType.SMART:
                    break;
                //case CollectorType.AntiVirus:
                //case CollectorType.Firewall:
                case ECollectorType.Unknown:
                default:
                    break;
            }

            return d;
        }
    }
}
