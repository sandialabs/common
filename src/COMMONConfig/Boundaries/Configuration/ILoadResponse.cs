using gov.sandia.sld.common.db.models;

namespace COMMONConfig.Boundaries.Configuration
{
    public interface ILoadResponse
    {
        void ConfigResponse(SystemConfiguration response);
    }
}