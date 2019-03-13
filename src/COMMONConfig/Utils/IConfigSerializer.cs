using gov.sandia.sld.common.db.models;

namespace COMMONConfig.Utils
{
    public interface IConfigSerializer
    {
        void Serialize(SystemConfiguration config, string path);
        SystemConfiguration Deserialize(string path);
    }
}