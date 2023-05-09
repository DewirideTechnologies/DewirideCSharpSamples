using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiGraphMS
{
	class Program
	{
		static async Task Main(string[] args)
		{
			// Create an instance of HttpClient
			var httpClient = new HttpClient();

			// Use Azure.Identity library to obtain an access token
			var credential = new ClientSecretCredential(
				"<Tenant Id>",
				"<Client Id>",
				"<Client Secret>"
			);
			var accessToken = await credential.GetTokenAsync(new TokenRequestContext(
				new[] { "https://graph.microsoft.com/.default" }
			));

			// Use the access token to call the Microsoft Graph API
			try
			{
				// Create the request
				var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/users");
				request.Headers.Add("Authorization", "Bearer " + accessToken.Token);

				// Send the request
				HttpResponseMessage response = await httpClient.SendAsync(request);

				// Check the response
				if (response.IsSuccessStatusCode)
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					Console.WriteLine(responseContent);
				}
				else
				{
					Console.WriteLine($"Error: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}

			// Wait for user input before exiting the program
			Console.ReadLine();
		}
	}
}

