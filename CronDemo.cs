using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace CronFunctionDemo
{
    public static class CronDemo
    {
        const string ApiEndpointKeyLabel = "ApiEndpoint";
        [FunctionName("CronDemo")]
        public static async Task Run([TimerTrigger("*/10 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string apiEndpoint = System.Environment.GetEnvironmentVariable(ApiEndpointKeyLabel, EnvironmentVariableTarget.Process);
            log.LogInformation($"{apiEndpoint} loaded");
            if (string.IsNullOrEmpty(apiEndpoint))
            {
                log.LogInformation($"{apiEndpoint} not available");
                return;
            }

            try
            {
                HttpClient newClient = new HttpClient(); 
                Uri baseUri = new Uri(apiEndpoint);
                Uri versionUri = new Uri(baseUri, "version");
                HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, versionUri);

                HttpResponseMessage response = await newClient.SendAsync(newRequest);
                string apiVersion = await response.Content.ReadAsAsync<string>();

                log.LogInformation($"{apiEndpoint} has version {apiVersion}");
            }
            catch (System.Exception ex)
            {
                log.LogError(ex, $"Error accessing {apiEndpoint}");
            }
        }
    }
}
