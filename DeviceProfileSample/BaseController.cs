using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DeviceProfileSample
{
    public class BaseController
    {
        protected AuthenticationHeaderValue bearerAuthHeader { get; set; }

        public BaseController(string pat)
        {
            var newPat = System.Text.Encoding.UTF8.GetBytes("user:" + pat);
            pat = System.Convert.ToBase64String(newPat);
            bearerAuthHeader = new AuthenticationHeaderValue("Basic", pat);

            client = GetBaseClient(bearerAuthHeader);
        }

        private HttpClient GetBaseClient(AuthenticationHeaderValue bearerAuthHeader)
        {
            var _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = bearerAuthHeader;

            return _client;
        }

        protected HttpClient client { get; set; }
    }
}
