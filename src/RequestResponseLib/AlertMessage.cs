using gov.sandia.sld.common.configuration;

namespace gov.sandia.sld.common.requestresponse
{
    public class AlertMessage : Message
    {
        public long Device_Id { get; private set; }
        public EStatusType Status { get; private set; }
        public EAlertLevel Level { get; private set; }
        public string Message { get; private set; }

        /// <param name="device_id">The numeric ID of the device whose state changed</param>
        /// <param name="status">The new status type enum</param>
        /// <param name="level">The new alert level</param>
        /// <param name="message">The message to send in the body of the email</param>
        public AlertMessage(long device_id, EStatusType status, EAlertLevel level, string message)
        {
            Device_Id = device_id;
            Status = status;
            Level = level;
            Message = message;
        }

        public override string ToString()
        {
            return $"AlertMessage: DeviceID {Device_Id}, Status {Status}, Level {Level}, Message '{Message}'";
        }
    }
}
