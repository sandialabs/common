using gov.sandia.sld.common.configuration;
using System;
using System.Management;

namespace gov.sandia.sld.common.data.wmi
{
    //public class FirewallCollector : WMIDataCollector
    //{
    //    public FirewallCollector(CollectorID id, Remote remote_info)
    //        : base(new WMIContext("FirewallProduct", "displayName,productState", remote_info),
    //              new DataCollectorContext(id, CollectorType.Firewall))
    //    {
    //    }

    //    public override CollectedData OnAcquire()
    //    {
    //        bool success = true;
    //        ListData<Firewall> firewall = new ListData<Firewall>(Context);
    //        CollectedData cd = new CollectedData(Context, success);

    //        ManagementScope scope = WmiContext.GetManagementScope("SecurityCenter2");

    //        string queryStr = "SELECT displayName,productState FROM FirewallProduct";
    //        try
    //        {
    //            foreach (ManagementBaseObject m in new ManagementObjectSearcher(scope, new ObjectQuery(queryStr)).Get())
    //            {
    //                Firewall f = new Firewall();
    //                f.Name = m["displayName"].ToString();
    //                f.Status = m["productState"].ToString();
    //                firewall.Data.Add(f);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            cd.SetMessage(ex);
    //            success = false;
    //        }

    //        cd.DataIsCollected = success;
    //        cd.D.Add(firewall);

    //        return cd;
    //    }
    //}
}
