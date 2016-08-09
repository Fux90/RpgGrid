using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MailSenderLib
{
    public sealed class MailSender
    {
        //TODO - MAke config file
        private const string smtpHost = "smtp.live.com";
        private const string user = "";

        public string Pwd { get; set; }

        private static MailSender current;
        public static MailSender Current
        {
            get
            {
                if(current == null)
                {
                    current = new MailSender();
                }
                return current;
            }
        }

        private string myAddress;
        public string MyAddress
        {
            get
            {
                return myAddress;
            }

            set
            {
                //TODO - Check address correctness
                myAddress = value;
            }
        }

        private MailSender()
        {

        }

        public bool SendTo(string receiverAddress, string subject, string msg)
        {
            using (var client = new SmtpClient(smtpHost, 587)
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(user, Pwd)
            })
            {
                using (var mail = new MailMessage(MyAddress, receiverAddress)
                {
                    Subject = subject,
                    Body = msg
                })
                {
                    try
                    {
                        client.Send(mail);
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
