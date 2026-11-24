using BusinessLogicLayer.OrdersService.BusinessLogicLayer.DTO;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
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
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/users/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    //throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");

                    return new UserDTO(
                      PersonName: "Temporarily Unavailable",
                      Email: "Temporarily Unavailable",
                      Gender: "Temporarily Unavailable",
                      UserId: Guid.Empty);
                }
            }


            UserDTO? user = await response.Content.ReadFromJsonAsync<UserDTO>();

            if (user == null)
            {
                throw new ArgumentException("Invalid User ID");
            }

            return user;
        
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
