using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureCosmosDBOperations.Service;
using Microsoft.Azure.Cosmos;
using AzureCosmosDBOperations.Entity;
using System.Configuration;

namespace AzureCosmosDBOperations
{
    public static class AzureCosmosDBOperations
    {
        [FunctionName("CreateDocumnet")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,

            ILogger log)
        {



            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                log.LogInformation("RequestBody" + requestBody);

                var input = JsonConvert.DeserializeObject<Item>(requestBody);
                var ss = Environment.GetEnvironmentVariable("CosmosDBURI");



                CosmosClient cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("CosmosDBURI"), Environment.GetEnvironmentVariable("CosmosDBAccountKey"),
            new CosmosClientOptions()
            {
                ApplicationRegion = Regions.WestEurope,
            });

                CosmosDbService _cosmosDbService = new CosmosDbService(cosmosClient, "pracdb", "praccon");

                await _cosmosDbService.AddItemAsync(input);


                return new OkObjectResult("Document created successfully");
            }
            catch (Exception ex)
            {
                log.LogError($"Couldn't insert item. Exception thrown: {ex.Message}");
                return new UnprocessableEntityObjectResult(ex.Message);
            }



        }
    }



}
