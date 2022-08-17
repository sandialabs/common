using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace gov.sandia.sld.common.utilities
{
    public class MailSender
    {
        protected SmtpClient _client;
        protected MailAddress _sender;
        protected string _tos;
        protected List<MailAddress> _recipients;

        public class SentDetails
        {
            public string From { get; set; }
            public string Tos { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }

            public override string ToString()
            {
                return $"From: '{From}', To: '{Tos}', Subject: '{Subject}', Body: '{Body}'";
            }
        }

        /// <summary>
        /// Create a distributor that will send an email when an alarm state changes.
        /// </summary>
        /// <param name="smtp">The SMTP server to send the mail through</param>
        /// <param name="from">The from email address</param>
        /// <param name="tos">A comma-separated list of email addresses the email will be sent to</param>
        public MailSender(string smtp, string from, string tos)
        {
            // Don't throw an exception. Just don't enable mail sending.
            if (string.IsNullOrEmpty(smtp) || string.IsNullOrEmpty(from) || string.IsNullOrEmpty(tos))
                throw new Exception($"MailSender: empty SMTP ({smtp}), from ({from}), or recipients ({tos})");

            List<string> toList = new List<string>(tos.Split(','));
            if (toList.Count == 0)
                throw new Exception($"MailSender: no recipients ({tos}) -- provide a comma-separated list of email addresses");

            _client = new SmtpClient(smtp);
            _sender = new MailAddress(from);
            _tos = tos;
            _recipients = new List<MailAddress>();

            toList.ForEach(recipient => _recipients.Add(new MailAddress(recipient.Trim())));
        }

        public SentDetails SendEmail(string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = _sender,
                };
                _recipients.ForEach(r => mail.To.Add(r));

                mail.Subject = subject;
                mail.SubjectEncoding = Encoding.UTF8;

                mail.Body = body;
                mail.BodyEncoding = Encoding.UTF8;

                _client.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new SentDetails
            {
                From = _sender.Address,
                Tos = _tos,
                Subject = subject,
                Body = body
            };
        }
    }
}
