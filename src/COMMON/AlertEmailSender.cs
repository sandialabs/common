using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.requestresponse;
using gov.sandia.sld.common.utilities;
using System;

namespace gov.sandia.sld.common
{

    /// <summary>
    /// Distribute an email when the alert status of a device changes
    /// </summary>
    public class AlertEmailSender
    {
        private MailSender _sender;
        private ILog _log;

        /// <summary>
        /// Create a distributor that will send an email when an alarm state changes.
        /// </summary>
        /// <param name="smtp">The SMTP server to send the mail through</param>
        /// <param name="from">The from email address</param>
        /// <param name="tos">A comma-separated list of email addresses the email will be sent to</param>
        /// <param name="log">The ILog to write logged information to</param>
        public AlertEmailSender(string smtp, string from, string tos, ILog log)
        {
            _sender = new MailSender(smtp, from, tos);
            _log = log;
        }

        /// <summary>
        /// Send the email indicating the alert status has changed
        /// </summary>
        public void Distribute(AlertMessage alertDetails)
        {
            if (string.IsNullOrEmpty(alertDetails.Message))
            {
                _log.Error($"AlertEmailSender: null or empty message");
                return;
            }

            // No need to send an email when it's not an alarm
            if (alertDetails.Level.IsAlarm() == false)
            {
                _log.Info($"AlertEmailSender: not sending {alertDetails}");
                return;
            }

            try
            {
                MailSender.SentDetails sent = _sender.SendEmail("COMMON -- " + alertDetails.Status.GetDescription(), alertDetails.Message);
                _log.Info($"Sent alert email -- {sent}");

            }
            catch (Exception ex)
            {
                _log.Error($"Error sending {alertDetails}");
                _log.Error(ex.ToString());
            }
        }
    }

    public class AlertReceiver : IReceiver
    {
        private AlertEmailSender _sender;
        private ILog _log;

        public AlertReceiver(string smtp, string from, string tos, ILog log)
        {
            _sender = new AlertEmailSender(smtp, from, tos, log);
            _log = log;
        }

        public void OnMessage(IMessage message)
        {
            if(message is AlertMessage)
            {
                _sender.Distribute(message as AlertMessage);
                message.Handled();
            }
        }
    }
}
