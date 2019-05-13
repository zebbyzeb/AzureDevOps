//using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DeviceProfileSample.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

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


            ///Getting list of 
            ///workitemID strings from
            ///Preprod Synkd Cloud Services (definitionId=5)
            var response = await ListReleases(bearerAuthHeader);
            var workItemsList = new List<string>();
            foreach (var release in response.value)
            {
                var releaseRes = await GetReleaseById(bearerAuthHeader, release.id);

                var buildID = Int32.Parse(releaseRes.artifacts.First().definitionReference.version.id);

                var workItemsRes = await ListWorkItems(bearerAuthHeader, buildID);

                foreach (var work in workItemsRes.value)
                {
                    workItemsList.Add(work.id);
                }
            }


            ///Getting list of
            ///workitemID stringsProd
            ///Prod Synkd Cloud Services (definitionId=6)
            var responseProd = await ListReleasesInProd(bearerAuthHeader);
            var prodWorkItemsList = new List<string>();
            foreach (var release in responseProd.value)
            {
                var prodReleaseRes = await GetProdReleaseById(bearerAuthHeader, release.id);

                var buildID = Int32.Parse(prodReleaseRes.artifacts.First().definitionReference.version.id);

                var prodWorkItems = await ListProdWorkItems(bearerAuthHeader, buildID);

                foreach(var prodWork in prodWorkItems.value)
                {
                    prodWorkItemsList.Add(prodWork.id);
                }
            }


            Console.WriteLine("Count of workitems in preprod + prod: " + workItemsList.Count);//before comparing
            Console.WriteLine(prodWorkItemsList.Count);
            foreach(var pItem in prodWorkItemsList)
            {
                for(var i=0; i < workItemsList.Count; i++)
                {
                    if (string.Compare(pItem, workItemsList[i]) == 0)
                    {
                        workItemsList.Remove(workItemsList[i]);
                    }
                }
            }
            Console.WriteLine("Count of workitems in preprod: " + workItemsList.Count);//After Comparing


            ///print the list of workitems in preprod,
            ///with areapath, iteration,
            ///item type and status
            int count = 1;
            foreach (var item in workItemsList)
            {
                Console.WriteLine(item);
            }
            foreach (var item in workItemsList)
            {
                var workByIdRes = await GetWorkById(bearerAuthHeader, Int32.Parse(item));
                var fields = workByIdRes.fields;
                Console.WriteLine(count + "\t" + item + "\t" + fields.AreaPath + "\t" + fields.IterationPath + "\t" + "\t\t\t" + fields.WorkItemType + "\t\t" + fields.State);
                count++;
                //unassigned work items are throwing null exception
            }

            Console.WriteLine("---------------------\n---------------------");

            int countProdItem = 1;
            foreach (var item in prodWorkItemsList)
            {
                Console.WriteLine(item);
            }
            foreach (var item in prodWorkItemsList)
            {
                var workByIdRes = await GetWorkById(bearerAuthHeader, Int32.Parse(item));
                var fields = workByIdRes.fields;
                Console.WriteLine(countProdItem + "\t" + item + "\t" + fields.AreaPath + "\t" + fields.IterationPath + "\t" + "\t\t\t" + fields.WorkItemType + "\t\t" + fields.State);
                countProdItem++;
                //unassigned work items are throwing null exception
            }

            //await ListProjects(bearerAuthHeader);
        }

        static async Task<Response<WorkItems>> ListProdWorkItems(AuthenticationHeaderValue authHeader, int buildID)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vstsCollectionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = await client.GetAsync($"appstablishment/_apis/build/builds/{buildID}/workitems?$top=100&api-version=5.0");

                // check to see if we have a succesfull response
                if (response.IsSuccessStatusCode)
                {
                    Response<WorkItems> workItem = null;
                    workItem = await response.Content.ReadAsAsync<Response<WorkItems>>();
                    return workItem;
                }
                return null;
            }
        }

        static async Task<Release> GetProdReleaseById(AuthenticationHeaderValue authHeader, int id)
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

        static async Task<Response<Releases>> ListReleasesInProd(AuthenticationHeaderValue authHeader)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vsrmCollectionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = await client.GetAsync("appstablishment/_apis/release/releases?definitionId=6&$top=100&api-version=5.0");

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

        static async Task<WorkItem> GetWorkById(AuthenticationHeaderValue authHeader, int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vstsCollectionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = await client.GetAsync($"appstablishment/_apis/wit/workitems/{id}?api-version=5.0");

                // check to see if we have a succesfull response
                if (response.IsSuccessStatusCode)
                {
                    WorkItem workItem = null;
                    workItem = await response.Content.ReadAsAsync<WorkItem>();
                    return workItem;
                }
                return null;
            }
        }

        static async Task<Response<WorkItems>> ListWorkItems(AuthenticationHeaderValue authHeader, int buildID)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vstsCollectionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = await client.GetAsync($"appstablishment/_apis/build/builds/{buildID}/workitems?$top=100&api-version=5.0");

                // check to see if we have a succesfull response
                if (response.IsSuccessStatusCode)
                {
                    Response<WorkItems> workItem = null;
                    workItem = await response.Content.ReadAsAsync<Response<WorkItems>>();
                    return workItem;
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

        static async Task<Response<Releases>> ListReleases(AuthenticationHeaderValue authHeader)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vsrmCollectionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = await client.GetAsync("appstablishment/_apis/release/releases?definitionId=5&$top=100&api-version=5.0");

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
