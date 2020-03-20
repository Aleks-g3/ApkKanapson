using Kanapson.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class Myorders : ContentPage
    {
        string urlOrder = "http://192.168.1.4:4000/orders/findid/";
        private HttpClient client;
        private ObservableCollection<Order> myOders;
        private string jwtPayload;

        public Myorders()
        {
            InitializeComponent();
            GetMyOrders();
            myorders.RefreshCommand = new Command(() =>
            {
                GetMyOrders();
                myorders.IsRefreshing = false;
            });
        }


        private void back_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void GetMyOrders()
        {
            myorders.IsRefreshing = true;
            myorders.ItemsSource = null;
            client = new HttpClient();
            myOders = new ObservableCollection<Order>();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                if (!jwtHandler.CanReadToken(Application.Current.Properties["Token"] as string)) throw new Exception("The token doesn't seem to be in a proper JWT format.");

                var token = jwtHandler.ReadJwtToken(Application.Current.Properties["Token"] as string);
                 jwtPayload = token.Claims.First(c => c.Type == "unique_name").Value;
                var response = await client.GetAsync(urlOrder+jwtPayload);
                
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var resault = response.Content.ReadAsStringAsync().Result;
                    myOders = JsonConvert.DeserializeObject<ObservableCollection<Order>>(resault);
                    myorders.ItemsSource = myOders;


                }
                else
                {
                    await DisplayAlert("Błąd", JObject.Parse(response.Content.ReadAsStringAsync().Result)["message"].ToString(), "Ok");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                myorders.IsRefreshing = false;
            });
        }
    }
}