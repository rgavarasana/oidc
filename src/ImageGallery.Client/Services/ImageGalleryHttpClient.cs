using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageGallery.Client.Services
{
    public class ImageGalleryHttpClient : IImageGalleryHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _httpClient = new HttpClient();

        public ImageGalleryHttpClient(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<HttpClient> GetClient()
        {
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var expiresIn = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.ExpiresIn);

            if (accessToken != null)
            {
                _httpClient.SetBearerToken(accessToken);
            }

            _httpClient.BaseAddress = new Uri("https://localhost:44347/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return _httpClient;
        }
        
        //public async Task<string> RenewTokens()
        //{
        //    var currentContext = _httpContextAccessor.HttpContext;

        //    //get the metadata
        //    var discoveryClient = new DiscoveryClient("https://localhost:44356/");
        //    var metaDataResponse = await discoveryClient.GetAsync();

        //    //create a new token client to get the tokens
        //    var tokenClient = new TokenClient(metaDataResponse.TokenEndpoint, "imagegalleryclient", "secret");

        //    //get the saved refresh token from context
        //    var currentRefreshToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            
        //    //refresh token from IDP
        //    var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);

        //    if (!tokenResult.IsError)
        //    {
        //        var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
        //        var expiresClaim = currentContext.User.FindFirst(c => c.Type == "exp").Value;
                
        //    }
        //}
    }
}

