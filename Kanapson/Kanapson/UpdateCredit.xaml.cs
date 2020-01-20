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
    public partial class UpdateCredit : ContentPage
    {
        private HttpClient client;
        private List<User> users;
        string urlUser = "http://192.168.1:4000/users/finduser/";
        string urlUpdateUser= "http://192.168.1.4:4000/users";
        User user;
        

        public UpdateCredit()
        {
            InitializeComponent();
            
        }


        private void SearchButton(object sender, EventArgs e)
        {
           Button searchbtn = (Button)sender;
            Grid grid = (Grid)searchbtn.Parent;
            Editor username =(Editor) grid.Children[0];

            GetUser(username.Text);
            username.Text = null;
        }

        private async void back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void update_Clicked(object sender, EventArgs e)
        {
            Button updatebtn = (Button)sender;
            Grid grid = (Grid)updatebtn.Parent;
            Label username = (Label)grid.Children[0];
            Editor credit = (Editor)grid.Children[1];

            if (string.IsNullOrWhiteSpace(credit.Text)||Double.Parse(credit.Text)<0)
                await DisplayAlert("", "Wszystkie pola muszą być uzupełnione", "OK");
            else
            {

                user = new User()
                {
                    Username = username.Text,
                    Credit = Double.Parse(credit.Text)
                };
                try
                {
                    var json = JsonConvert.SerializeObject(user);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync(urlUpdateUser, data);
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        GetUser(user.Username);
                    }
                    else
                    {
                        await DisplayAlert("", response.Content.ReadAsStringAsync().Result, "OK");
                    }


                }
                catch (Exception ex)
                {
                    await DisplayAlert("", ex.Message, "OK");
                }
            }

        }

        private async void GetUser(string username)
        {
            listUser.ItemsSource = null;
            client = new HttpClient();
            users = new List<User>();

            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
            try
            {
                
                var response = await client.GetAsync(urlUser+username);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var resault = response.Content.ReadAsStringAsync().Result;
                    users = JsonConvert.DeserializeObject<List<User>>(resault);
                    listUser.ItemsSource = users;


                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}