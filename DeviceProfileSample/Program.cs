using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DeviceProfileSample.Models;

namespace DeviceProfileSample
{
    public class Response<T>
    {
        public int count { get; set; }
        public List<T> value { get; set; }
    }
    public class Program
    {
        //============= Config [Edit these with your settings] =====================
        internal const string vstsCollectionUrl = "https://dev.azure.com/appstablishment/"; //change to the URL of your VSTS account; NOTE: This must use HTTPS
        const string vsrmCollectionUrl = "https://vsrm.dev.azure.com/appstablishment/";
        internal const string clientId = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";        //update this with your Application ID from step 2.6 (do not change this if you have an MSA backed account)
        //==========================================================================

        internal const string VSTSResourceId = "499b84ac-1321-427f-aa17-267ca6975798"; //Static value to target VSTS. Do not change
        

        public static void Main(string[] args)
        {
            //AuthenticationContext ctx = GetAuthenticationContext(null);
            //AuthenticationResult result = null;
            //try
            //{
            //    //DeviceCodeResult codeResult = ctx.AcquireDeviceCodeAsync(VSTSResourceId, clientId).Result;
            //    //Console.WriteLine("You need to sign in.");
            //    //Console.WriteLine("Message: " + codeResult.Message + "\n");
            //    //result = ctx.AcquireTokenByDeviceCodeAsync(codeResult).Result;

            //    //string pat = "2bjkoyzuxhwp5otnn2igv7btcsrzgaez3pqn3bzg2ujposwmsueq";
            //    //var newPat = System.Text.Encoding.UTF8.GetBytes("user:"+pat);
            //    //pat = System.Convert.ToBase64String(newPat);

            //    //var bearerAuthHeader = new AuthenticationHeaderValue("Basic", pat);


            //    //await ListProjects(bearerAuthHeader);
            //}
            //catch (Exception ex)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine("Something went wrong.");
            //    Console.WriteLine("Message: " + ex.Message + "\n");
            //}
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            string pat = "2bjkoyzuxhwp5otnn2igv7btcsrzgaez3pqn3bzg2ujposwmsueq";
            var newPat = System.Text.Encoding.UTF8.GetBytes("user:" + pat);
            pat = System.Convert.ToBase64String(newPat);

            var bearerAuthHeader = new AuthenticationHeaderValue("Basic", pat);

            var response = await ListReleases(bearerAuthHeader);
            
            var release = response.value[0];

            var releaseRes = await GetReleaseById(bearerAuthHeader, release.id);
            var buildID = Int32.Parse(releaseRes.artifacts.First().definitionReference.version.id);

            await ListWorkItems(bearerAuthHeader, buildID);
            

            await ListProjects(bearerAuthHeader);
        }

        static async Task<string> ListWorkItems(AuthenticationHeaderValue authHeader, int buildID)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vstsCollectionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = await client.GetAsync($"appstablishment/_apis/build/builds/{buildID}/workitems?api-version=5.0");

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    string release = null;
                    release = await response.Content.ReadAsStringAsync();
                    return release;
                }
                return null;
            }
        }

        static async Task<Release> GetReleaseById(AuthenticationHeaderValue authHeader, int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vsrmCollectionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = await client.GetAsync($"appstablishment/_apis/release/releases/{id}?api-version=5.0");

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    Release release = null;
                    release = await response.Content.ReadAsAsync<Release>();
                    return release;
                }
                return null;
            }
        }

        private static AuthenticationContext GetAuthenticationContext(string tenant)
        {
            AuthenticationContext ctx = null;
            if (tenant != null)
                ctx = new AuthenticationContext("https://login.microsoftonline.com/" + tenant);
            else
            {
                ctx = new AuthenticationContext("https://login.windows.net/common");
                if (ctx.TokenCache.Count > 0)
                {
                    string homeTenant = ctx.TokenCache.ReadItems().First().TenantId;
                    ctx = new AuthenticationContext("https://login.microsoftonline.com/" + homeTenant);
                }
            }

            return ctx;
        }

        static async Task<Response<Releases>> ListReleases(AuthenticationHeaderValue authHeader)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vsrmCollectionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = await client.GetAsync("appstablishment/_apis/release/releases?api-version=5.0");

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    Response<Releases> res = null;
                    res = await response.Content.ReadAsAsync<Response<Releases>>();
                    return res;
                }
                return null;
            }
        }

        static async Task<Response<Project>> ListProjects(AuthenticationHeaderValue authHeader)
        {
            // use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vstsCollectionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("User-Agent", "VstsRestApiSamples");
                //client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
                
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = await client.GetAsync("appstablishment/classholes/_apis/work/backlogs?api-version=5.0-preview.1");

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    Response<Project> res = null;
                    res = await response.Content.ReadAsAsync<Response<Project>>();
                    //Console.WriteLine("\tSuccesful REST call");
                    //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    //var backlogsList = response.Content.ReadAsStringAsync().Result.ToList();
                    //Console.WriteLine("\n---------------------------------");
                    //Console.WriteLine(backlogsList);
                    return res;
                }
                //else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                //{
                //    throw new UnauthorizedAccessException();
                //}
                //else
                //{
                //    Console.WriteLine("{0}:{1}", response.StatusCode, response.ReasonPhrase);
                //}
                return null;
            }
        }
    }
}
