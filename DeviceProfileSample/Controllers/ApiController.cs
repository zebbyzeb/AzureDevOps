using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DeviceProfileSample.Models;

namespace DeviceProfileSample.Controllers
{
    public class ApiController
    {
        public AuthenticationHeaderValue authToken { get; set; }
        public ApiController(string pat)
        {
            var newPat = System.Text.Encoding.UTF8.GetBytes("user:" + pat);
            pat = System.Convert.ToBase64String(newPat);
            authToken = new AuthenticationHeaderValue("Basic", pat);
        }
        public async Task<Response<Releases>> TestReleases(AuthenticationHeaderValue authHeader, int testReleaseDefinitionID)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://vsrm.dev.azure.com/appstablishment/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = await client.GetAsync($"appstablishment/_apis/release/releases?definitionId={testReleaseDefinitionID}&$top=1000&api-version=5.0");

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
    }
}
