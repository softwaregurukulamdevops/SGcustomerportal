using CustomerPortel.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace CustomerPortel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        public readonly TrainingDBContext trainingDBContext;
        private static readonly HttpClient client = new HttpClient();
        private const string serviceEndpoint = "https://pmosearchservice.search.windows.net";
        private const string apiKey = "zTq6TW0mCLal9KLplagGTL5vRIbKZl5MXZ5lR6o8E9AzSeAxNcOy";
        private const string indexName = "evoke-projects-index-latest";
        public CustomerController(TrainingDBContext _trainingDBContext)
        {
            trainingDBContext = _trainingDBContext;
        }
        [HttpGet("GetCustomerDetails")]
        public List<Customer> GetCustomerDetails()
        {
            List<Customer> lstCustomer = new List<Customer>();
            try
            {
                lstCustomer = trainingDBContext.Customer.ToList();
                return lstCustomer;
            }
            catch (Exception ex)
            {
                lstCustomer = new List<Customer>();
                return lstCustomer;
            }
        }
        [HttpPost("AddCustomer")]
        public string AddCustomer(Customer customer)
        {
            string message = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(customer.CustomerName))
                {
                    trainingDBContext.Add(customer);
                    trainingDBContext.SaveChanges();
                    message = "Customer added successfully";
                }
                else
                    message = "Customer name required.";

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        [HttpGet("GetSearchResults")]
        public async Task GetSearchResults()
        {
            // Set up HttpClient with base address and default headers
            client.DefaultRequestHeaders.Add("Api-Key", apiKey);

            var requestUri = $"{serviceEndpoint}/indexes/{indexName}/docs/search?api-version=2021-04-30-Preview";

            // Create the search query object
            var searchQuery = new
            {
                search = "T&M"
            };

            // Serialize the search query to JSON
            string jsonString = JsonSerializer.Serialize(searchQuery);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            // Make the HTTP POST request
            var response = await client.PostAsync(requestUri, content);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            // Read and output the response content
            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
        }
    }
}
