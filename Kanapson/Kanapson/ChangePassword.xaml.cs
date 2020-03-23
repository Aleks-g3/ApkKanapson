using Kanapson.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private User user;
        private HttpClient client;
        private JwtSecurityTokenHandler jwtHandler;
        private string jwtPayload;
        string urlUser = "https://kanapson.pl/users/findbyid/";
        string urlupdateUser = "https://kanapson.pl/users/updatepass";

        public ChangePassword()
        {
            InitializeComponent();
            client = new HttpClient();
            jwtHandler = new JwtSecurityTokenHandler();
            client.Timeout = TimeSpan.FromSeconds(10);
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
                await DisplayAlert("Powiadomienie", "Wprowadź hasło", "OK");
            }
            else
            {
                user.Password = password.Text;
                try
                {
                    var json = JsonConvert.SerializeObject(user);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PutAsync(urlupdateUser, data);

                    

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Powiadomienie", "Hasło zostało zmienione", "Ok");
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert("Błąd", JObject.Parse(response.Content.ReadAsStringAsync().Result)["message"].ToString(), "Ok");
                    }

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Błąd", ex.Message, "Ok");
                }
            }
        }

        private async void getUser(string id)
        {
            user = new User();
            try
            {
                var response = await client.GetAsync(urlUser + id);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var resault = response.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<User>(resault);

                    username.Text = user.Username;

                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", ex.Message, "Ok");
            }
        }
    }
}