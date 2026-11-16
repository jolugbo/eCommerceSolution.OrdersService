using BusinessLogicLayer.DTO;
using BusinessLogicLayer.OrdersService.BusinessLogicLayer.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace BusinessLogicLayer.OrdersService.BusinessLogicLayer.HTTPClients;

public  class ProductsMicroserviceClient
{
    private readonly HttpClient _httpClient;
    public ProductsMicroserviceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<ProductDTO?> GetProductById(Guid productID)
    {
        if(productID == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productID));
        }
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

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            throw new HttpRequestException(
                $"Products service returned {(int)response.StatusCode} {response.ReasonPhrase} with an empty response body. " +
                $"Expected a ProductDTO for product {productID}. Content-Type: {response.Content.Headers.ContentType}");
        }

        try
        {
            ProductDTO? product = JsonSerializer.Deserialize<ProductDTO>(
                responseBody,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return product ?? throw new HttpRequestException(
                $"Products service returned JSON null for product {productID}");
        }
        catch (JsonException exception)
        {
            throw new HttpRequestException(
                $"Products service returned invalid JSON for product     {productID}. " +
                $"Content-Type: {response.Content.Headers.ContentType}. Response body: {responseBody}",
                exception);
        }
    }
}
