using gov.sandia.sld.common.db.models;

namespace COMMONConfig.Boundaries.Configuration
{
    public interface ISaveRequest
    {
        void SaveRequest(SystemConfiguration configuration);
    }
}