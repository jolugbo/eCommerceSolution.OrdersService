using BusinessLogicLayer.OrdersService.BusinessLogicLayer.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

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
        var response = await _httpClient.GetAsync($"api/users/{userId}");
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new HttpRequestException("Bad Request", null, HttpStatusCode.BadRequest);
        }
        if (response.IsSuccessStatusCode)
        {
            UserDTO? user = await response.Content.ReadFromJsonAsync<UserDTO>();
            if (user == null)
            {
                throw new HttpRequestException("User not found", null, HttpStatusCode.NotFound);
            }
            return user;
        }
        else
        {
            throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
        }
    }
}
