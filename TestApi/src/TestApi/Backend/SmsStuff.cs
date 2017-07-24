using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using TestApi.Backend;
using TestApi.Controllers;
using Newtonsoft.Json;
using TestApi.Types;
using System.Text;

namespace TestApi.Backend
{
    public class sms
    {
        /// <summary>
        /// Send SMS to the number listed.
        /// Method is asynchronous as it calls assynchronous methods in HTTPClient methods.
        /// </summary>
        /// <param name="from">The phone number to send the SMS to</param>
        /// <param name="body">The contents of the message to send</param>
        async void sendSMS(string from, string body)
        {
            Controllers.sms s = new Controllers.sms(); // Creates an SMS object
            s.Body = body;
            s.From = from;
            string json = JsonConvert.SerializeObject(s); // Converts the object into JSON
            string url = "http://178.62.87.28:700/sendSms"; // Defines the URL for the POST call to the SMS server.
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json"); // Adds the correct header to the JSON
            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.PostAsync(url, httpContent);

                if (httpResponse.Content != null) // if there is a response - deal with it.
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync(); // gets the value of the response
                }
            }

        }


    }
}
