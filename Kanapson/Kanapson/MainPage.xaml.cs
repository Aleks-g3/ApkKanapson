using Jose;
using JWT.Builder;
using Kanapson.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kanapson
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        User user;
       const string url = "https://kanapson.pl/users/authenticate";
        HttpClient client;
        //User user;

        public MainPage()
        {
            InitializeComponent();
            username.Text = "";
            password.Text = "";
            
        }
        
        private async void Button_Clicked(object sender, EventArgs e)
        {
            
            user = new User();
            string obj;
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            if (string.IsNullOrWhiteSpace(username.Text) || string.IsNullOrWhiteSpace(password.Text))
            {
                await DisplayAlert("","Wszystkie pola muszą być uzupełnione","Ok");
            }
            else
            {
                user.Username = username.Text;
                user.Password = password.Text;

                try
                {
                    var json = JsonConvert.SerializeObject(user);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, data);
                    

                    if (response.IsSuccessStatusCode)
                    {
                        obj = response.Content.ReadAsStringAsync().Result;
                        //obj = JObject.Parse(response.Content.ReadAsStringAsync().Result)["tokenString"].ToString();
                        Application.Current.Properties["Token"] = obj;
                        
                        var jwtHandler = new JwtSecurityTokenHandler();
                        if (!jwtHandler.CanReadToken(obj)) throw new Exception("The token doesn't seem to be in a proper JWT format.");
                        
                        var token = jwtHandler.ReadJwtToken(obj);
                        var jwtPayload = token.Claims.First(c => c.Type=="role" ).Value;

                        if (jwtPayload == "User")
                            await Navigation.PushModalAsync(new UserMenu());
                        if (jwtPayload == "Admin")
                            await Navigation.PushModalAsync(new AdminMenu());
                        username.Text = null;
                        password.Text = null;
                        
                    }
                    else
                    {
                        await DisplayAlert("Błąd", JObject.Parse(response.Content.ReadAsStringAsync().Result)["message"].ToString(), "Ok");
                    }
                    
                    
                }
                catch(Exception ex)
                {
                    await DisplayAlert("Błąd", ex.Message,"Ok");
                }
                
            }
            

            }

        private async void Registerbtn_Clicked(object sender, EventArgs e)
        {
           await Navigation.PushModalAsync(new RegisterUser());
        }
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("", "Czy chcesz wyjść z aplikacji?", "Tak", "Nie"))
                    Thread.CurrentThread.Abort();
            });
            return true;
        }


    }
}
