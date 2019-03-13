namespace COMMONConfig.Utils
{
    //public class SystemConfigurationXMLSerializer : IConfigSerializer
    //{
    //    private const string Configuration = @"Configurations";
    //    private const string RootElement = @"SystemConfiguration";

    //    private static SerializableDictionary<string, ConfigurationData> Convert(SystemConfiguration config)
    //    {
    //        var serialDictionary =
    //            new SerializableDictionary<string, ConfigurationData>();

    //        foreach (var entry in config.configuration)
    //        {
    //            serialDictionary.Add(entry.Key, entry.Value);
    //        }
    //        return serialDictionary;
    //    }

    //    public SystemConfiguration Deserialize(string path)
    //    {
    //        var configuration = new SystemConfiguration();
    //        var serializableDictionary =
    //            new SerializableDictionary<string, ConfigurationData>();

    //        var reader = XmlReader.Create(path);

    //        reader.MoveToContent();
    //        reader.ReadStartElement();
    //        reader.ReadStartElement();
    //        serializableDictionary.ReadXml(reader);
    //        configuration.configuration = serializableDictionary;
    //        configuration.devices = ReadElement(reader, configuration.devices) as List<DeviceInfo>;
    //        configuration.languages = ReadElement(reader, configuration.languages) as List<LanguageConfiguration>;
    //        reader.ReadEndElement();

    //        reader.Close();

    //        return configuration;
    //    }

    //    public void Serialize(SystemConfiguration config, string path)
    //    {
    //        var serialDictionary = Convert(config);

    //        var settings = new XmlWriterSettings {Indent = true};
    //        var writer = XmlWriter.Create(path, settings);

    //        writer.WriteStartDocument();

    //        writer.WriteStartElement(RootElement);

    //        writer.WriteStartElement(Configuration);
    //        serialDictionary.WriteXml(writer);
    //        writer.WriteEndElement();

    //        WriteElement(writer, config.devices);
    //        WriteElement(writer, config.languages);
    //        writer.WriteEndElement();

    //        writer.WriteEndDocument();
    //        writer.Close();
    //    }

    //    private static object ReadElement(XmlReader reader, object value)
    //    {
    //        var serializer = new XmlSerializer(value.GetType());
    //        return serializer.Deserialize(reader);
    //    }

    //    private static void WriteElement(XmlWriter writer, object value)
    //    {
    //        var serializer = new XmlSerializer(value.GetType());
    //        serializer.Serialize(writer, value);
    //    }
    //}
}