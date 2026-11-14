using BusinessLogicLayer.OrdersService.BusinessLogicLayer.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace BusinessLogicLayer.OrdersService.BusinessLogicLayer.HTTPClients;

public  class UsersMicroserviceClient
{
    private readonly HttpClient _httpClient;
    public UsersMicroserviceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<UserDTO?> GetUserById(Guid userId)
    {
        if(userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }
        using HttpResponseMessage response = await _httpClient.GetAsync($"api/users/{userId}");
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
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

        try
        {
            UserDTO? user = JsonSerializer.Deserialize<UserDTO>(
                responseBody,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return user ?? throw new HttpRequestException(
                $"Users service returned JSON null for user {userId}.");
        }
        catch (JsonException exception)
        {
            throw new HttpRequestException(
                $"Users service returned invalid JSON for user {userId}. " +
                $"Content-Type: {response.Content.Headers.ContentType}. Response body: {responseBody}",
                exception);
        }
    }
}
