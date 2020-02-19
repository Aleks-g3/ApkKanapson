using Kanapson.Models;
using Newtonsoft.Json;
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
        
        string urlUser = "http://192.168.1:4000/users/finduser/";
        string urlUpdateUser= "http://192.168.1.4:4000/users";
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

                
                try
                {
                    user = new User()
                    {
                        Username = username.Text,
                        Credit = Double.Parse(credit.Text)
                    };
                    var json = JsonConvert.SerializeObject(user);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync(urlUpdateUser, data);
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("", "Stan konta został zmieniony", "Ok");
                        GetUser(user.Username);
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
            listUser.IsRefreshing = true;
            listUser.ItemsSource = null;
            client = new HttpClient();
            users = new ObservableCollection<User>();

            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
            try
            {
                
                var response = await client.GetAsync(urlUser+username);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var resault = response.Content.ReadAsStringAsync().Result;
                    users = JsonConvert.DeserializeObject<ObservableCollection<User>>(resault);
                    listUser.ItemsSource = users;


                }
                listUser.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}