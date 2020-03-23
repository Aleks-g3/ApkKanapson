using Kanapson.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        
        string urlUser = "https://kanapson.pl/users/finduser/";
        string urlUpdateUser= "https://kanapson.pl/users/updateCredit";
        User user;
        private ObservableCollection<User> users;

        public UpdateCredit()
        {
            InitializeComponent();
            
        }


        private void SearchButton(object sender, EventArgs e)
        {
           Button searchbtn = (Button)sender;
            Grid grid = (Grid)searchbtn.Parent;
            Entry username =(Entry) grid.Children[0];

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
            Entry credit = (Entry)grid.Children[1];

            if (string.IsNullOrWhiteSpace(credit.Text)||Double.Parse(credit.Text)<0)
                await DisplayAlert("", "Wszystkie pola muszą być uzupełnione", "OK");
            else
            {

                
                try
                {
                    user.Credit = Double.Parse(credit.Text);
                    var json = JsonConvert.SerializeObject(user);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync(urlUpdateUser, data);
                    

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("", "Stan konta został zmieniony", "Ok");
                        GetUser(user.Username);
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

        private async void GetUser(string username)
        {
            listUser.IsRefreshing = true;
            listUser.ItemsSource = null;
            client = new HttpClient();
            users = new ObservableCollection<User>();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
            try
            {
                
                var response = await client.GetAsync(urlUser+username);

                

                if (response.IsSuccessStatusCode)
                {
                    var resault = response.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<User>(resault);
                    if (user != null)
                    {
                        users.Add(user);
                        listUser.ItemsSource = users;
                    }
                        


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
            Device.BeginInvokeOnMainThread(() =>
            {
                listUser.IsRefreshing = false;
            });
        }
    }
}