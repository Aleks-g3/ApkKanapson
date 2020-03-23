using Kanapson.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    public partial class RegisterAdmin : ContentPage
    {
        private HttpClient client;
        private User user;

        const string url = "https://kanapson.pl/users/registeradmin";
        public RegisterAdmin()
        {
            InitializeComponent();
        }

        private async void back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void registerAdmin_Clicked(object sender, EventArgs e)
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
            
            try
            {
                user = new User();

                if (string.IsNullOrWhiteSpace(Firstname.Text) || string.IsNullOrWhiteSpace(Lastname.Text))
                {
                    await DisplayAlert("","Wszystkie pola muszą być wypełnione","Ok");
                }
                else
                {
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
                await DisplayAlert("Błąd", ex.ToString(), "Ok");
            }
            
        }
    }
}