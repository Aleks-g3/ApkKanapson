using Kanapson.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kanapson
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangePassword : ContentPage
    {
        private List<User> user;
        private HttpClient client;
        private JwtSecurityTokenHandler jwtHandler;
        private string jwtPayload;
        string urlUser = "http://192.168.1.4:4000/users/?id=";
        string urlupdateUser = "http://192.168.1.4:4000/users";

        public ChangePassword()
        {
            InitializeComponent();
            client = new HttpClient();
            jwtHandler = new JwtSecurityTokenHandler();

            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
            if (!jwtHandler.CanReadToken(Xamarin.Forms.Application.Current.Properties["Token"].ToString())) throw new Exception("The token doesn't seem to be in a proper JWT format.");

            var token = jwtHandler.ReadJwtToken(Xamarin.Forms.Application.Current.Properties["Token"].ToString());
            jwtPayload = token.Claims.First(c => c.Type == "unique_name").Value;
            getUser(jwtPayload);
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void changepassword_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(password.Text))
            {
                await DisplayAlert("", "Hasło nie zostało zmienione", "OK");
            }
            else
            {
                user[0].Password = password.Text;
                try
                {
                    var json = JsonConvert.SerializeObject(user);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var responseProduct = await client.PostAsync(urlupdateUser, data);

                    responseProduct.EnsureSuccessStatusCode();

                    if (responseProduct.IsSuccessStatusCode)
                    {
                        await Navigation.PopModalAsync();


                    }

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }

        private async void getUser(string id)
        {
            user = new List<User>();
            try
            {
                var response = await client.GetAsync(urlUser + id);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var resault = response.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<List<User>>(resault);

                    username.Text = user[0].Username;

                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}