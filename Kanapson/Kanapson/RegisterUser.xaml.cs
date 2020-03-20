using Kanapson.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kanapson
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterUser : ContentPage
    {
       const string url = "http://192.168.1.4:4000/users/register";
        HttpClient client;
        User user;
        public RegisterUser()
        {
            InitializeComponent();
            Firstname.Text = Lastname.Text=Username.Text=Password.Text="";

        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Register_Clicked(object sender, EventArgs e)
        {
            user = new User();
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            try
            { 
                if (string.IsNullOrWhiteSpace(Username.Text) || string.IsNullOrWhiteSpace(Password.Text) || string.IsNullOrWhiteSpace(Firstname.Text) || string.IsNullOrWhiteSpace(Lastname.Text))
                {
                    await DisplayAlert("","Wszystkie pola muszą być wypełnione","Ok");
                }
                else
                {
                    user.Username = Username.Text;
                    user.Password = Password.Text;
                    user.FirstName = Firstname.Text;
                    user.LastName = Lastname.Text;
                    var json = JsonConvert.SerializeObject(user);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, data);
                    

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("", "Użytkownik został zarejestrowany", "Ok");
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert("Błąd", JObject.Parse(response.Content.ReadAsStringAsync().Result)["message"].ToString(), "Ok");
                    }
                        
                
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Błąd", ex.Message, "Ok");
            }


        }
    }
}