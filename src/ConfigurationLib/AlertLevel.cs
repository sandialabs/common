using gov.sandia.sld.common.utilities;
using System;

namespace gov.sandia.sld.common.configuration
{
    /// <summary>
    /// This enum extends our old style which only considered
    /// alarms or non-alarms. We made Normal == 0 to match non-alarm,
    /// and Alarm == 1 to match alarm, while adding the new
    /// Information value
    /// </summary>
    public enum EAlertLevel
    {
        Normal,
        Alarm,
        Information,
    }

    public class AlertLevelAttribute : Attribute
    {
        public EAlertLevel Level { get; private set; }

        public AlertLevelAttribute(EAlertLevel level)
        {
            Level = level;
        }
    }

    public static class AlertLevelExtensions
    {
        public static EAlertLevel? GetAlertLevel(this Enum type)
        {
            AlertLevelAttribute attr = type.GetAttribute<AlertLevelAttribute>();
            if (attr != null)
                return attr.Level;
            return null;
        }

        public static bool IsAlertLevel(this Enum type, EAlertLevel level)
        {
            EAlertLevel? alert_level = type.GetAlertLevel();
            return alert_level.HasValue ? alert_level.Value == level : false;
        }

        public static bool IsAlarm(this Enum type)
        {
            return type.IsAlertLevel(EAlertLevel.Alarm);
        }
    }
}
