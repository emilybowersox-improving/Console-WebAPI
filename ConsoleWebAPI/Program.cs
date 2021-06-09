using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;


/*// Next Challenge:
Modify your code so that, instead of hardcoding the GUID to update, you parse the return from the GET to pick out the GUID​
Hint: use the Newtonsoft Json library to parse the return and use methods on the JSON object​
Hint: modify your GET query to narrow your result set to only the record you want​*/


namespace ConsoleWebAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "https://org73f2beb8.api.crm.dynamics.com";
            // your demo tenant's username and password
            string userName = "admin@CRM570851.onmicrosoft.com";
            string password = "MAn3C46ZRQ";

            // Azure Active Directory registered app clientid for Microsoft samples
            string clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";

            var userCredential = new UserCredential(userName, password);
            string apiVersion = "9.0";
            string webApiUrl = $"{url}/api/data/v{apiVersion}/";

            //Authenticate using IdentityModel.Clients.ActiveDirectory
            var authParameters = AuthenticationParameters.CreateFromResourceUrlAsync(new Uri(webApiUrl)).Result;
            var authContext = new AuthenticationContext(authParameters.Authority, false);
            var authResult = authContext.AcquireToken(url, clientId, userCredential);
            var authHeader = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(webApiUrl);
                client.DefaultRequestHeaders.Authorization = authHeader;
                //everything above this point is the basic setup





                //GET 

                string accountUri = "accounts?$select=name";
                string uri = accountUri;

                Console.WriteLine("Sending request (HTTP GET)...");

                var response = client.GetAsync(uri).Result;

                Console.WriteLine(response);

                if(response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Request was successful.");

                    //Get the response content and parse it
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    JObject body = JObject.Parse(responseBody);
                    Console.WriteLine(body.ToString());
                }
                else
                {
                    // the {0} grabs the first index of the array of things coming after- in this case there's only one thing: response.ReasonPhrase. 
                    Console.WriteLine("The request failed with a status of {0}", response.ReasonPhrase);
                }
                // these two lines create a break to pause the console, so you can decide when to let it close out
                Console.WriteLine("PressEnterToContinue");
                Console.ReadLine();





                // PATCH

                string updateAccountUri = "accounts(6df963e8-d9bf-eb11-8236-000d3a1c43bc)";
                using (var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), updateAccountUri))
                {
                    requestMessage.Headers.Authorization = authHeader;
                    //adding this header tells it to return all of the info of the entity instance we patched 
                    requestMessage.Headers.Add("Prefer", "return=representation");

                    var payload = @"{
                                ""description"": ""12345fax""
                                } ";

                    requestMessage.Content = new StringContent(payload);
                    requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    Console.WriteLine("Sending update (HTTP PATCH)...");

                    response = client.SendAsync(requestMessage).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Update Successful.");
                        // necessary to Console.Write the "return=representation" so we can see it 
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        JObject body = JObject.Parse(responseBody);
                        Console.WriteLine(body.ToString());
                    }
                    else
                    {
                        Console.WriteLine("The request failed with a status of {0}", response.ReasonPhrase);
                    }
                    Console.WriteLine("PressEnterToContinue");
                    Console.ReadLine();
                }





                // POST (CREATE)

                string postToAccountUri = "accounts";
                using (var requestMessage = new HttpRequestMessage(new HttpMethod("POST"), postToAccountUri))
                {
                    requestMessage.Headers.Authorization = authHeader;

                    var payload = @"{

                ""name"": ""Brand New Account"",
                ""description"": ""Cool new account""

                } ";

                    requestMessage.Content = new StringContent(payload);
                    requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    Console.WriteLine("Creating new account (HTTP POST)...");

                    response = client.SendAsync(requestMessage).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Update Successful.");

                    }
                    else
                    {
                        //console.writing to get a more helpful error message
                        Console.WriteLine(payload);

                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        JObject body = JObject.Parse(responseBody);
                        Console.WriteLine(body.ToString());

                        Console.WriteLine("The request failed with a status of {0}", response.ReasonPhrase);
                    }
                    Console.WriteLine("PressEnterToContinue");
                    Console.ReadLine();
                }



                

                // DELETE

                string deleteAccountUri = "accounts(036c3f4b-1be7-e611-8101-e0071b6af231)";
                using (var requestMessage = new HttpRequestMessage(new HttpMethod("DELETE"), deleteAccountUri))
                {
                    requestMessage.Headers.Authorization = authHeader;

                    Console.WriteLine("Deleting account (HTTP DELETE)...");

                    response = client.SendAsync(requestMessage).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Delete Successful.");

                    }
                    else
                    {
                        Console.WriteLine("The request failed with a status of {0}", response.ReasonPhrase);
                    }
                    Console.WriteLine("PressEnterToContinue");
                    Console.ReadLine();
                }





            }
        }
    }
}
