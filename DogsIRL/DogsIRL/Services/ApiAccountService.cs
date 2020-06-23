﻿using DogsIRL.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DogsIRL.Services
{
    public class ApiAccountService
    {
        HttpClient Client { get; set; }

        public ApiAccountService()
        {
#if DEBUG
            HttpClientHandler insecureHandler = GetInsecureHandler();
            Client = new HttpClient(insecureHandler);
#else
            HttpClient Client = new HttpClient();
#endif
        }

        /// <summary>
        /// Requests Login credentials from API and assigns user name and JWT Token
        /// </summary>
        /// <param name="userSignIn"></param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> RequestLogin(UserSignIn userSignIn)
        {
            var json = JsonConvert.SerializeObject(userSignIn);
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.PostAsync($"{App.ApiUrl}/account/login", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                App.Username = userSignIn.UserName;
                string token = await response.Content.ReadAsStringAsync();
                App.Token = token;
            }
            return response;
        }

        ///!!!!!!!!!!!!!TODO!!!!!!!!!!
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> RequestRegister(RegisterInput input)
        {
            var json = JsonConvert.SerializeObject(input);
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.PostAsync(
                $"{App.ApiUrl}/account/register", httpContent);

            return response;
        }

        /// <summary>
        /// Clears cached data for current user
        /// </summary>
        /// <returns></returns>
        public async Task Logout()
        {
            App.Username = null;
            App.CurrentDog = null;
            App.Token = null;
        }

        public HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();

            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;

                return errors == System.Net.Security.SslPolicyErrors.None;
            };

            return handler;
        }

        public async Task<HttpResponseMessage> RequestForgotPassword(EmailInput input)
        {
            var json = JsonConvert.SerializeObject(input);
            HttpContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await Client.PostAsync(
               $"{App.ApiUrl}/account/forgot-password", httpContent);
            return response;
        }

    }
}
