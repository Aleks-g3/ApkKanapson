using Kanapson.Models;
using Newtonsoft.Json;
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

            messagelbl.Text = "";
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Register_Clicked(object sender, EventArgs e)
        {
            user = new User();
            client = new HttpClient();
            if (string.IsNullOrWhiteSpace(Username.Text) || string.IsNullOrWhiteSpace(Password.Text) || string.IsNullOrWhiteSpace(Firstname.Text) || string.IsNullOrWhiteSpace(Lastname.Text))
            {
                messagelbl.Text = "Wszystkie pola muszą być wypełnione";
            }
            else
            {
                user.Username = Username.Text;
                user.Password = Password.Text;
                user.Firstname = Firstname.Text;
                user.Lastname = Lastname.Text;
                var json = JsonConvert.SerializeObject(user);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, data);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                    await Navigation.PopModalAsync();
                else
                    messagelbl.Text = response.Content.ReadAsStringAsync().Result;
            }
            
        }
    }
}