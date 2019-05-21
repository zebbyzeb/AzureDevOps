
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DeviceProfileSample.Models;
using System.IO;
using System.Threading;
using System.Text;

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
            //string token = string.Empty;
            ////Console.WriteLine("Paste in your personal access token");
            //token = Console.ReadLine();
            //ApiController api = new ApiController(token);
            
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            string pat = "2bjkoyzuxhwp5otnn2igv7btcsrzgaez3pqn3bzg2ujposwmsueq";
            var newPat = System.Text.Encoding.UTF8.GetBytes("user:" + pat);
            pat = System.Convert.ToBase64String(newPat);
            var bearerAuthHeader = new AuthenticationHeaderValue("Basic", pat);

            Console.WriteLine("What release do you want the work items for?\n" +
                "Type 1 and press Enter for Preprod ASWeb\n" +
                "Type 2 for Preprod ASServer\n");
            var selection = Convert.ToInt32(Console.ReadLine());

            int preProdReleaseDefinitionID = 0;     //placeholder value
            int testReleaseDefinitionID = 0;        //placeholder value
            int prodReleaseDefinitionID = 0;        //placeholder value
            Nullable<int> prodSynkdDefEnvID = null;
            Nullable<int> preProdDefEnvID = null;
            switch (selection)
            {
                case 1:
                    preProdReleaseDefinitionID = 3;     //ASWeb_Master
                    testReleaseDefinitionID = 2;        //ASWeb_Test
                    prodSynkdDefEnvID = 8;              //ASWeb_Master Prod_Synkd Stage
                    preProdDefEnvID = 3;
                    break;
                case 2:
                    preProdReleaseDefinitionID = 5;
                    prodReleaseDefinitionID = 6;
                    break;
                case 3:
                    //RunDemoMethod(api);
                    break;
                default:
                    Console.WriteLine("Please choose one of the given options.");
                    break;
            }
            Console.WriteLine("Getting your workitems now\n" +
                string.Concat(Enumerable.Repeat("---", 10)));

            List<string> preProdWorkItemsList = new List<string>();
            List<string> prodSynkdWorkItemsList = new List<string>();
            List<string> filteredWorkItems = new List<string>();

            if (selection == 1)
            {
                preProdWorkItemsList = GetWorkItems(bearerAuthHeader, preProdReleaseDefinitionID, preProdDefEnvID).Result;
                prodSynkdWorkItemsList = GetWorkItems(bearerAuthHeader, preProdReleaseDefinitionID, prodSynkdDefEnvID).Result;
                filteredWorkItems = GetPreprodWorkItems(preProdWorkItemsList, prodSynkdWorkItemsList);
                filteredWorkItems = filteredWorkItems.Distinct().ToList();

                await ConsoleDump(bearerAuthHeader, filteredWorkItems);
            }
            
            if (selection == 2)
            {
                preProdWorkItemsList = GetWorkItems(bearerAuthHeader, preProdReleaseDefinitionID, null).Result;
                prodSynkdWorkItemsList = GetWorkItems(bearerAuthHeader, prodReleaseDefinitionID, null).Result;
                filteredWorkItems = GetPreprodWorkItems(preProdWorkItemsList, prodSynkdWorkItemsList);
                filteredWorkItems = filteredWorkItems.Distinct().ToList();

                await ConsoleDump(bearerAuthHeader, filteredWorkItems);
            }

            {
                /////Getting list of
                /////workitemID stringsProd
                /////TestASWeb (definitionId = 2)
                /////
                //var responseTest = await ListTestReleases(bearerAuthHeader, testReleaseDefinitionID);
                //var testWorkItemsList = new List<string>();
                //foreach (var release in responseTest.value)
                //{
                //    var releaseRes = await GetReleaseById(bearerAuthHeader, release.id);
                //    var buildID = Int32.Parse(releaseRes.artifacts.First().definitionReference.version.id);
                //    var testWorkItems = await ListWorkItems(bearerAuthHeader, buildID);
                //    foreach (var work in testWorkItems.value)
                //    {
                //        testWorkItemsList.Add(work.id);
                //    }
                //}

                //Console.WriteLine("-------------------------");
                //foreach(var item in testWorkItemsList)
                //    Console.WriteLine(item);



                /////Getting list of
                /////workitemID stringsProd
                /////Prod Synkd Cloud Services (definitionId=6)
                //var responseProd = await ListReleasesInProd(bearerAuthHeader);
                //var prodWorkItemsList = new List<string>();
                //foreach (var release in responseProd.value)
                //{
                //    var prodReleaseRes = await GetProdReleaseById(bearerAuthHeader, release.id);

                //    var buildID = Int32.Parse(prodReleaseRes.artifacts.First().definitionReference.version.id);

                //    var prodWorkItems = await ListProdWorkItems(bearerAuthHeader, buildID);

                //    foreach(var prodWork in prodWorkItems.value)
                //    {
                //        prodWorkItemsList.Add(prodWork.id);
                //    }
                //}


                //Console.WriteLine("Count of workitems in preprod + prod: " + preProdWorkItemsList.Count);//before comparing
                //Console.WriteLine(prodWorkItemsList.Count);
                //foreach(var pItem in prodWorkItemsList)
                //{
                //    for(var i=0; i < preProdWorkItemsList.Count; i++)
                //    {
                //        if (string.Compare(pItem, preProdWorkItemsList[i]) == 0)
                //        {
                //            preProdWorkItemsList.Remove(preProdWorkItemsList[i]);
                //        }
                //    }
                //}
                //Console.WriteLine("Count of workitems in preprod: " + preProdWorkItemsList.Count);//After Comparing


                /////print the list of workitems in preprod,
                /////with areapath, iteration,
                /////item type and status
                //int countPreProd = 1;
                //foreach (var item in preProdWorkItemsList)
                //{
                //    Console.WriteLine(item);
                //}
                //foreach (var item in preProdWorkItemsList)
                //{
                //    var workByIdRes = await GetWorkById(bearerAuthHeader, Int32.Parse(item));
                //    var fields = workByIdRes.fields;
                //    Console.WriteLine(countPreProd + "\t" + item + "\t" + fields.AreaPath + "\t" + fields.IterationPath + "\t" + "\t\t\t" + fields.WorkItemType + "\t\t" + fields.State);
                //    countPreProd++;
                //    //unassigned work items are throwing null exception
                //}

                //Console.WriteLine("---------------------\n---------------------");

                //int countProdItem = 1;
                //foreach (var item in prodWorkItemsList)
                //{
                //    Console.WriteLine(item);
                //}
                //foreach (var item in prodWorkItemsList)
                //{
                //    var workByIdRes = await GetWorkById(bearerAuthHeader, Int32.Parse(item));
                //    var fields = workByIdRes.fields;
                //    Console.WriteLine(countProdItem + "\t" + item + "\t" + fields.AreaPath + "\t" + fields.IterationPath + "\t" + "\t\t\t" + fields.WorkItemType + "\t\t" + fields.State);
                //    countProdItem++;
                //    //unassigned work items are throwing null exception
                //}

                //await ListProjects(bearerAuthHeader);
            }
        }

        private static void RunDemoMethod(ApiController api)
        {
            
        }

        private static async Task ConsoleDump(AuthenticationHeaderValue bearerAuthHeader, List<string> filteredWorkItems)
        {
            if (filteredWorkItems.Count == 0)
                Console.WriteLine("Looks like a recent release was made.");
            else
            {
                int count = 1;
                var csv = new StringBuilder();

                csv.AppendLine(string.Format("{0},{1},{2},{3},{4}", "\t", "WorkItemID", "WorkItemType", "State", "Title"));

                foreach (var item in filteredWorkItems)
                {

                    List<string> parentURL = new List<string>();

                    var workByIdRes = await GetWorkById(bearerAuthHeader, Int32.Parse(item));
                    var fields = workByIdRes.fields;
                    //if (fields.WorkItemType == "Product Backlog Item")
                    //    fields.WorkItemType = "PBI";
                    if (fields.State == "Ready for Testing")
                        fields.State = "Ready";
                    //if (fields.Title.Length > 100)
                    //    fields.Title = fields.Title.Substring(0, 100);
                    var title = fields.Title.Replace(",", "");
                    if (fields.WorkItemType == "Bug")
                    {
                        Console.WriteLine(count + "\t" + item + "\t" + fields.AreaPath + "\t" + fields.IterationPath + "\t\t\t" + fields.WorkItemType + "\t\t" + fields.State + "\t" + fields.Title);
                        var newline = string.Format("{0},{1},{2},{3},{4}", "Base", item, fields.WorkItemType, fields.State, title);
                        csv.AppendLine(newline);
                        foreach (var x in workByIdRes.relations)
                        {
                            if (x.rel == "System.LinkTypes.Related")
                            {
                                var relatedWork = await GetWorkById(bearerAuthHeader, null, x.url);
                                //if (relatedWork.fields.WorkItemType == "Product Backlog Item")
                                //    fields.WorkItemType = "PBI";
                                if (relatedWork.fields.WorkItemType == "Test Case")
                                    continue;
                                if (relatedWork.fields.State == "Ready for Testing")
                                    relatedWork.fields.State = "Ready";
                                var relTitle = relatedWork.fields.Title.Replace(",", "");

                                if (relatedWork.fields.WorkItemType == "Product Backlog Item")
                                {
                                    Console.WriteLine("\t" + "Related: " + "\t\t" + relatedWork.id + "\t\t" + relatedWork.fields.State + "\t\t" + title);

                                    var relatedline = string.Format("{0},{1},{2},{3},{4}", "--Related PBI", relatedWork.id, relatedWork.fields.WorkItemType, relatedWork.fields.State, relTitle);
                                    csv.AppendLine(relatedline);
                                }
                                else if (relatedWork.fields.WorkItemType == "Bug")
                                {
                                    Console.WriteLine("\t" + "Related: " + "\t\t" + relatedWork.id + "\t\t" + relatedWork.fields.State + "\t\t" + title);

                                    var relatedline = string.Format("{0},{1},{2},{3},{4}", "--Related Bug", relatedWork.id, relatedWork.fields.WorkItemType, relatedWork.fields.State, relTitle);
                                    csv.AppendLine(relatedline);
                                }
                            }

                            else if (x.rel == "System.LinkTypes.Hierarchy-Reverse")
                            {
                                var relatedWork = await GetWorkById(bearerAuthHeader, null, x.url);
                                var relTitle = relatedWork.fields.Title.Replace(",", "");
                                var relatedline = string.Format("{0},{1},{2},{3},{4}", "--Parent Feature", relatedWork.id, relatedWork.fields.WorkItemType, relatedWork.fields.State, relTitle);
                                csv.AppendLine(relatedline);
                            }
                        }
                        count++;
                    }
                    else if (fields.WorkItemType == "Task")
                    {
                        Console.WriteLine(count + "\t" + item + "\t" + fields.AreaPath + "\t" + fields.IterationPath + "\t\t\t" + fields.WorkItemType + "\t\t" + fields.State + "\t" + fields.Title);
                        var newline = string.Format("{0},{1},{2},{3},{4}", "Base", item, fields.WorkItemType, fields.State, title);
                        csv.AppendLine(newline);

                        foreach (var x in workByIdRes.relations)
                        {
                            if (x.rel == "System.LinkTypes.Hierarchy-Reverse")
                            {
                                var relatedWork = await GetWorkById(bearerAuthHeader, null, x.url);
                                Console.WriteLine("\t" + "Related: " + "\t\t" + relatedWork.id + "\t\t" + relatedWork.fields.State + "\t" + relatedWork.fields.WorkItemType + "\t\t" + relatedWork.fields.Title);
                                var relTitle = relatedWork.fields.Title.Replace(",", "");
                                if (relatedWork.fields.WorkItemType == "Product Backlog Item")
                                {
                                    var relatedline = string.Format("{0},{1},{2},{3},{4}", "--Related PBI", relatedWork.id, relatedWork.fields.WorkItemType, relatedWork.fields.State, relTitle);
                                    csv.AppendLine(relatedline);
                                }
                                else if (relatedWork.fields.WorkItemType == "Bug")
                                {
                                    var relatedline = string.Format("{0},{1},{2},{3},{4}", "--Related Bug", relatedWork.id, relatedWork.fields.WorkItemType, relatedWork.fields.State, relTitle);
                                    csv.AppendLine(relatedline);
                                }
                            }
                        }
                        count++;
                    }
                    else if (fields.WorkItemType == "Product Backlog Item")
                    {
                        Console.WriteLine(count + "\t" + item + "\t" + fields.AreaPath + "\t" + fields.IterationPath + "\t\t\t" + fields.WorkItemType + "\t\t" + fields.State + "\t" + fields.Title);

                        var newline = string.Format("{0},{1},{2},{3},{4}", "Base", item, fields.WorkItemType, fields.State, title);
                        csv.AppendLine(newline);

                        foreach (var x in workByIdRes.relations)
                        {
                            if (x.rel == "System.LinkTypes.Hierarchy-Reverse")
                            {
                                var relatedWork = await GetWorkById(bearerAuthHeader, null, x.url);
                                var relTitle = relatedWork.fields.Title.Replace(",", "");
                                var relatedline = string.Format("{0},{1},{2},{3},{4}", "--Parent Feature", relatedWork.id, relatedWork.fields.WorkItemType, relatedWork.fields.State, relTitle);
                                csv.AppendLine(relatedline);
                            }
                        }

                        count++;
                    }

                }
                File.WriteAllText("OutputCSV.csv", csv.ToString());
            }
        }

        private static List<string> GetPreprodWorkItems(List<string> preProdWorkItemsList, List<string> prodSynkdWorkItemsList)
        {
            //bool isFound = false;
            
            foreach (var pItem in prodSynkdWorkItemsList)
            {
                for (var i = 0; i < preProdWorkItemsList.Count; i++)
                {
                    if (string.Compare(pItem, preProdWorkItemsList[i]) == 0)
                    {
                        preProdWorkItemsList.Remove(preProdWorkItemsList[i]);
                        //isFound = true;
                        i -= 1;
                    }
                }
                
                //if(isFound)
                //    prodSynkdWorkItemsList.Remove(prodSynkdWorkItemsList[j]);
            }
            return preProdWorkItemsList;
        }

        private static async Task<List<string>> GetWorkItems(AuthenticationHeaderValue bearerAuthHeader, int preProdReleaseDefinitionID, int? preProdDefEnvID)
        {
            var preProdWorkItemsList = new List<string>();
            var response = await ListReleases(bearerAuthHeader, preProdReleaseDefinitionID, preProdDefEnvID);

            foreach (var release in response.value)
            {
                var releaseRes = await GetReleaseById(bearerAuthHeader, release.id);
                var buildID = Int32.Parse(releaseRes.artifacts.First().definitionReference.version.id);
                var workItemsRes = await ListWorkItems(bearerAuthHeader, buildID);
                foreach (var work in workItemsRes.value)
                {
                    preProdWorkItemsList.Add(work.id);
                }
            }
            return preProdWorkItemsList;
        }

        static async Task<ResponseT<Releases>> ListTestReleases(AuthenticationHeaderValue authHeader, int testReleaseDefinitionID)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vsrmCollectionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = await client.GetAsync($"appstablishment/_apis/release/releases?definitionId={testReleaseDefinitionID}&$top=1000&api-version=5.0");

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    ResponseT<Releases> res = null;
                    res = await response.Content.ReadAsAsync<ResponseT<Releases>>();
                    return res;
                }
                return null;
            }
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
                HttpResponseMessage response = await client.GetAsync($"appstablishment/_apis/build/builds/{buildID}/workitems?$top=200&api-version=5.0");

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
                HttpResponseMessage response = await client.GetAsync("appstablishment/_apis/release/releases?definitionId=6&$top=1000&api-version=5.0");

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

        static async Task<WorkItem> GetWorkById(AuthenticationHeaderValue authHeader, int? id, string url=null)
        {
            using (var client = new HttpClient())
            {
                if (url == null)
                {
                    client.BaseAddress = new Uri(vstsCollectionUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = authHeader;

                    // connect to the REST endpoint            
                    HttpResponseMessage response = await client.GetAsync($"appstablishment/_apis/wit/workitems/{id}?$expand=all&api-version=5.0");

                    // check to see if we have a succesfull response
                    if (response.IsSuccessStatusCode)
                    {
                        WorkItem workItem = null;
                        workItem = await response.Content.ReadAsAsync<WorkItem>();
                        return workItem;
                    }
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization = authHeader;
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        WorkItem workItem = null;
                        workItem = await response.Content.ReadAsAsync<WorkItem>();
                        return workItem;
                    }
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
                HttpResponseMessage response = await client.GetAsync($"appstablishment/_apis/build/builds/{buildID}/workitems?$top=200&api-version=5.0");

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

        static async Task<ResponseT<Releases>> ListReleases(AuthenticationHeaderValue authHeader, int releaseDefinitionID, Nullable<int> defEnvID)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(vsrmCollectionUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;

                HttpResponseMessage response;

                // connect to the REST endpoint 
                if (defEnvID != null)                
                    response = await client.GetAsync($"appstablishment/_apis/release/releases?definitionEnvironmentId={defEnvID}&definitionId={releaseDefinitionID}&$top=100&api-version=5.0");
                else
                    response = await client.GetAsync($"appstablishment/_apis/release/releases?definitionId={releaseDefinitionID}&$top=100&api-version=5.0");

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    ResponseT<Releases> res = null;
                    res = await response.Content.ReadAsAsync<ResponseT<Releases>>();
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
