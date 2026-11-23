using BusinessLogicLayer.OrdersService.BusinessLogicLayer.DTO;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace BusinessLogicLayer.OrdersService.BusinessLogicLayer.HTTPClients;

public  class UsersMicroserviceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersMicroserviceClient> _logger;
    public UsersMicroserviceClient(HttpClient httpClient, ILogger<UsersMicroserviceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<UserDTO?> GetUserById(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }
        using HttpResponseMessage response = await _httpClient.GetAsync($"api/users/{userId}");
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new HttpRequestException(
                    $"Users service request failed with {(int)response.StatusCode} {response.ReasonPhrase}. " +
                    $"Response body: {responseBody}",
                    null,
                    response.StatusCode);
            }
            else
            {
                return new UserDTO(
                    PersonName: "Temporarily Unavailable",
                    Email: "Temporarily Unavailable",
                    Gender: "Temporarily Unavailable",
                    UserId: Guid.Empty
                    ); 
            }   
            throw new HttpRequestException(
                $"Users service request failed with {(int)response.StatusCode} {response.ReasonPhrase}. " +
                $"Response body: {responseBody}",
                null,
                response.StatusCode);
        }

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            throw new HttpRequestException(
                $"Users service returned {(int)response.StatusCode} {response.ReasonPhrase} with an empty response body. " +
                $"Expected a UserDTO for user {userId}. Content-Type: {response.Content.Headers.ContentType}");
        }

            UserDTO? user = JsonSerializer.Deserialize<UserDTO>(
                responseBody,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return user ?? throw new HttpRequestException(
                $"Users service returned JSON null for user {userId}.");
        }
        catch (BrokenCircuitException exception)
        {
            _logger.LogError($"{exception}, Request Failed because Circuit breaker is in open state, returning dummy data.");
            return new UserDTO(
                      PersonName: "Temporarily Unavailable",
                      Email: "Temporarily Unavailable",
                      Gender: "Temporarily Unavailable",
                      UserId: Guid.Empty
                      );
        }
    }
}
