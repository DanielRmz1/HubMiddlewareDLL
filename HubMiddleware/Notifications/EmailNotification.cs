using HubMiddleware.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HubMiddleware.Notifications
{
    public class EmailNotification
    {
        private HttpClient client = new HttpClient();

        public EmailNotification(string url)
        {
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/sjon"));
        }

        public async Task<object> SendEmail(Email email)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/Alerts/sendemail", email);
            response.EnsureSuccessStatusCode();
            return response.Content;
        }

        public async Task<object> SendSMS(SMSModel sms)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/Alerts/sendsms", sms);
            response.EnsureSuccessStatusCode();
            return response.Content;
        }

    }
}
