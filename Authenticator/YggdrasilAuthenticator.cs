﻿using System.Net;
using System.Threading.Tasks;
using ModuleLauncher.Re.Service;
using ModuleLauncher.Re.Service.DataEntity.Authenticator;
using ModuleLauncher.Re.Service.Extensions;
using ModuleLauncher.Re.Service.Utils;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Authenticator
{
    //head
    public partial class YggdrasilAuthenticator
    {
        public string Username
        {
            get => Payload.Username;
            set => Payload.Username = value;
        }

        public string Password
        {
            get => Payload.Password;
            set => Payload.Password = value;
        }
        public string ClientToken 
        { 
            get => Payload.ClientToken;
            set => Payload.ClientToken = value;
        }

        private AuthenticatorPayload Payload;
        public YggdrasilAuthenticator(string username = "", string password = "", string clientToken = "")
        {
            Payload = new AuthenticatorPayload
            {
                Username = username,
                Password = password,
                ClientToken = clientToken
            };
        }

        private const string AuthDomain = "https://authserver.mojang.com/authenticate";
    }
    
    //async
    public partial class YggdrasilAuthenticator
    {
        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            var payload = Payload.GetAuthenticatePayload();
            var result = await HttpHelper.PostHttpAsync(AuthDomain, payload);
            var response = JObject.Parse(result.Content);

            return result.StatusCode == HttpStatusCode.OK
                ? new AuthenticateResult
                {
                    AccessToken = response["accessToken"]?.ToString(),
                    ClientToken = response["clientToken"]?.ToString(),
                    Username = response["selectedProfile"]?["name"]?.ToString(),
                    Uuid = response["selectedProfile"]?["id"]?.ToString(),
                    Verified = true
                }
                : new AuthenticateResult
                {
                    Error = response["error"]?.ToString(),
                    ErrorMessage = response["errorMessage"]?.ToString(),
                    Verified = false
                };
        }
    }
    
    //sync
    public partial class YggdrasilAuthenticator
    {
        public AuthenticateResult Authenticate() => AuthenticateAsync().GetResult();
    }
}