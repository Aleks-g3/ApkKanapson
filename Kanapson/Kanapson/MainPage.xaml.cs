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
       const string url = "http://192.168.1.5:4000/users/authenticate";
        HttpClient client;
        //User user;

        public MainPage()
        {
            InitializeComponent();
            username.Text = "test";
            password.Text = "test";
            
        }
        
        private async void Button_Clicked(object sender, EventArgs e)
        {
            
            user = new User();
            string obj;
            client = new HttpClient();
            if (string.IsNullOrWhiteSpace(username.Text) || string.IsNullOrWhiteSpace(password.Text))
            {
                messege.Text = "Wszystkie pola muszą być uzupełnione";
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
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        obj = JObject.Parse(response.Content.ReadAsStringAsync().Result)["tokenString"].ToString();
                        Application.Current.Properties["Token"] = obj;
                        
                        var jwtHandler = new JwtSecurityTokenHandler();
                        if (!jwtHandler.CanReadToken(obj)) throw new Exception("The token doesn't seem to be in a proper JWT format.");
                        
                        var token = jwtHandler.ReadJwtToken(obj);
                        var jwtPayload = token.Claims.First(c => c.Type=="role" ).Value;

                        if (jwtPayload == "Normal")
                            await Navigation.PushModalAsync(new UserMenu());
                        if (jwtPayload == "Admin")
                            await Navigation.PushModalAsync(new AdminMenu());
                        
                    }
                    else
                    {
                        messege.Text= response.Content.ReadAsStringAsync().Result;
                    }
                    
                    
                }
                catch(Exception ex)
                {
                    messege.Text = ex.Message;
                }
                
            }
            

            }

        private async void Registerbtn_Clicked(object sender, EventArgs e)
        {
           await Navigation.PushModalAsync(new RegisterUser());
        }

        //public async void LoadData()
        //{
        //    user = new User();
        //    var content = "";
        //    HttpClient client = new HttpClient();
        //    var RestURL = "http://localhost:4000/users/authenticate";
        //client.BaseAddress = new Uri(RestURL);
        //    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //    HttpResponseMessage response = await client.PostAsync(RestURL);
        //    content = await request();
        //    var Items = JsonConvert.DeserializeObject<User>(content);
        //    user.Token = Items.Token;
        //}
    }
}
