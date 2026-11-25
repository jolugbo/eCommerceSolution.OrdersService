using BusinessLogicLayer.DTO;
using BusinessLogicLayer.OrdersService.BusinessLogicLayer.DTO;
using eCommerce.OrdersService.BusinessLogicLayer.Policies;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace BusinessLogicLayer.OrdersService.BusinessLogicLayer.HTTPClients;

public  class ProductsMicroserviceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductsMicroservicePolicies> _logger;
    public ProductsMicroserviceClient(HttpClient httpClient, ILogger<ProductsMicroservicePolicies> logger)
    {
        _logger = logger;
        _httpClient = httpClient;
    }
    public async Task<ProductDTO?> GetProductById(Guid productID)
    {
        if(productID == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productID));
        }


        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync($"api/product/search/product-id/{productID}");
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Products service request failed with {(int)response.StatusCode} {response.ReasonPhrase}. " +
                $"Response body: {responseBody}",
                null,
                response.StatusCode);
        }
            ProductDTO? product = JsonSerializer.Deserialize<ProductDTO>(
                responseBody,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return product ?? throw new HttpRequestException(
                $"Products service returned JSON null for product {productID}");
        }
        catch (BulkheadRejectedException exception)
        {
            _logger.LogError(exception, $"Bulkhead rejected the request for product {productID} because the request queue is full");
            ProductDTO product = new ProductDTO(
            ProductID: Guid.Empty,
            ProductName: "Temporarily Unavailable(Bulkhead)",
            Category: "Temporarily Unavailable(Bulkhead)",
            UnitPrice: 0,
            QuantityInStock: 0
            );
            return product;
        }
    }
}
