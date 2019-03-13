using gov.sandia.sld.common.configuration;
using System;
using System.Management;

namespace gov.sandia.sld.common.data.wmi
{
    //public class AntiVirusCollector : MultiPropertyWMIDataCollector
    //{
    //    public AntiVirusCollector(CollectorID id, Remote remote_info)
    //        : base(new WMIContext("AntiVirusProduct", "displayName,productState,Version", remote_info),
    //              new DataCollectorContext(id, CollectorType.AntiVirus))
    //    {
    //    }

    //    public override CollectedData OnAcquire()
    //    {
    //        bool success = true;
    //        ListData<AntiVirus> av = new ListData<AntiVirus>(Context);
    //        CollectedData cd = new CollectedData(Context, success);

    //        ManagementScope scope = WmiContext.GetManagementScope("SecurityCenter2");

    //        string queryStr = "SELECT displayName,productState FROM AntiVirusProduct";

    //        try
    //        {
    //            foreach (ManagementBaseObject m in new ManagementObjectSearcher(scope, new ObjectQuery(queryStr)).Get())
    //            {
    //                AntiVirus a = new AntiVirus();
    //                a.Name = m["displayName"].ToString();
    //                a.Status = m["productState"].ToString();
    //                av.Data.Add(a);
    //            }

    //            scope = WmiContext.GetManagementScope();
    //            foreach (AntiVirus antiVirus in av.Data)
    //            {
    //                string avName = antiVirus.Name;
    //                string query = string.Format("SELECT * FROM Win32_Product where Name LIKE '{0}'", avName);
    //                foreach (ManagementBaseObject m in new ManagementObjectSearcher(scope, new ObjectQuery(query)).Get())
    //                {
    //                    antiVirus.Version = m["Version"].ToString();
    //                }

    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            cd.SetMessage(ex);
    //            success = false;
    //        }

    //        cd.DataIsCollected = success;
    //        cd.D.Add(av);

    //        return cd;
    //    }
    //}
}
