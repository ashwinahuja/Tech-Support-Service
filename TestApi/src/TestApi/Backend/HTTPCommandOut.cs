using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;
namespace TestApi.Backend
{
    public class HTTPCommandOut
    {
        /// <summary>
        /// Completes a HTTP Get
        /// 
        /// </summary>
        public HTTPCommandOut() { }
        /// <summary>
        /// Since it's asynchronous, it returns a task. However, in reality, the only
        /// thing which is used from the task return is the result.
        /// </summary>
        /// <param name="url">This is the URL to complete the GET command from</param>
        /// <returns>Returns a Task (of a stream). From here, the stream can be created and the contents read using a StreamReader</returns>
        public async Task<Stream> get(string url)
        {

            using (var client = new HttpClient()) // The using ensures the HTTPClient cannot be used outside of scope.
            {

                client.BaseAddress = new Uri(url); // Important to note that since it is used directly as a URI, the url must be fully defined
                                                    // i.e. also including the http://

                var response = await client.GetAsync(""); // Asynchronously calls the task
                Stream stringResponse = await response.Content.ReadAsStreamAsync(); //Gets the response
                StreamReader x = new StreamReader(stringResponse); //
                return stringResponse; // Returns the task of the stream

            }
        }
    }
}
