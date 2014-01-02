using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Reflection;
using log4net;
using GreeNova.AppMonitor.Helpers;
using System.IO;
using GreeNova.AppMonitor.Configuration;
using System.Net.Mail;

namespace GreeNova.AppMonitor.Infrastructure {
    
    public class AppMonitor {

        private readonly static ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AppMonitor(AppMonitorConfiguration configuration, string name) {
            _configuration = configuration;
            _monitorTarget = _configuration.MonitorTargets[name];
        }

        public event Action<MonitorTargetItem, Exception> OnNotifyError;

        public void Monitor() {
            Logger.Info("AppMonitor for {0} started.".WithTokens(_monitorTarget.Name));
            try {
                var request = WebRequest.Create(_monitorTarget.ApplicationUrl) as HttpWebRequest;
                request.Timeout = 1000 * _monitorTarget.TimeOutInSeconds;
                Logger.Info("Submitting request to {0}".WithTokens(_monitorTarget.ApplicationUrl));
                var response = request.GetResponse() as HttpWebResponse;
                string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                Logger.Info("AppMonitor for {0} returned result: {1}".WithTokens(_monitorTarget.Name, response.StatusCode));
            }
            catch (Exception ex) {
                Logger.Warn("Error retrieving response from {0}. Reason: {1}".WithTokens(_monitorTarget.ApplicationUrl, ex.Message));
                NotifyError(ex);
            }
        }

        protected virtual void NotifyError(Exception error) {
            if (OnNotifyError != null) OnNotifyError(_monitorTarget, error);
            
            if (_monitorTarget.ErrorNotificationMailRecipients.IsNullOrEmpty()) {
                Logger.Info("No notification recipients configured for {0}.".WithTokens(_monitorTarget.Name));
                Logger.Fatal("AppMonitor failed for {0} ({1}). Reason: {2}".WithTokens(_monitorTarget.Name, _monitorTarget.ApplicationUrl, error.Message), error);
                return;
            }
            
            Logger.Info("Notifying error monitors: {0}".WithTokens(_monitorTarget.ErrorNotificationMailRecipients));
            string[] recipients = _monitorTarget.ErrorNotificationMailRecipients.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            string emailBody = "AppMonitor failed for {0} ({1}). Reason: {2}".WithTokens(_monitorTarget.Name, _monitorTarget.ApplicationUrl, error.ToString());
            if (error is WebException) {
                WebException webEx = error as WebException;
                var response = webEx.Response;
                string responseString = response == null 
                    ? webEx.Message 
                    : new StreamReader(response.GetResponseStream()).ReadToEnd();

                emailBody = responseString;
            }

            var sender = new MailAddress(_configuration.MonitorMailSender);

            MailMessage mail = new MailMessage {
                Priority = MailPriority.High,
                From = sender,
                Sender = sender,
                Subject = "AppMonitor failed for {0} ({1}). Reason: {2}".WithTokens(_monitorTarget.Name, _monitorTarget.ApplicationUrl, error.Message),
                Body = emailBody,
                IsBodyHtml = true,                         
            };

            foreach (var recipient in recipients) {
                mail.To.Add(new MailAddress(recipient));
            }

            try {
                var smtp = new SmtpClient();
                smtp.Send(mail);
                Logger.Info("AppMonitor error notification sent to {0}".WithTokens(_monitorTarget.ErrorNotificationMailRecipients));
            }
            catch (Exception ex) {
                Logger.Fatal("AppMonitor error notification failed. Reason: {0}".WithTokens(ex.Message), ex);
                throw;
            }
            
        }

        private readonly AppMonitorConfiguration _configuration;
        private readonly MonitorTargetItem _monitorTarget;
    }
}
