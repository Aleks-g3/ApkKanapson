using Kanapson.Models;
using Newtonsoft.Json;
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

        const string url = "http://192.168.1.4:4000/users/registeradmin";
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
                    user.Firstname = Firstname.Text;
                    user.Lastname = Lastname.Text;
                    var json = JsonConvert.SerializeObject(user);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, data);
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("", "Użytkownik został zarejestrowany", "Ok");
                        await Navigation.PopModalAsync();
                    }
                }

            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Ok");
            }
            
        }
    }
}