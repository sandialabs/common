using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db.models;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db
{
    public interface IStartEndTime
    {
        DateTimeOffset? Start { get; }
        DateTimeOffset? End { get; }
    }

    public interface IDataStore
    {
        FullDeviceStatus GetDeviceStatuses(long device_id);

        List<CollectorInfo> GetAllCollectors();

        List<NetworkStatus> GetNetworkStatuses(IStartEndTime start_end);
        List<DeviceData> GetDeviceData(long deviceID, ECollectorType collectorType, IStartEndTime start_end);
    }
}
