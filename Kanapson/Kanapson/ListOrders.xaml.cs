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
    public partial class ListOrders : ContentPage
    {
        private HttpClient client;
        private ObservableCollection<Order> orders;

        string urlOrder = "http://192.168.1.4:4000/orders/status";
        string urlupdateOrder = "http://192.168.1.4:4000/orders/update";
        private Order order;

        public ListOrders()
        {
            InitializeComponent();
            getOrders();

        }

        private async void action_Clicked(object sender, EventArgs e)
        {
            Button updatebtn = (Button)sender;
            Grid grid = (Grid)updatebtn.Parent;
            Label orderTimes = (Label)grid.Children[0];
            Label status = (Label)grid.Children[2];

            try
            {
                order = new Order();
                order = orders.First(o => o.orderTimes == DateTime.Parse(orderTimes.Text));
                if (order != null)
                {
                    if (updatebtn.Text == StatusValue.ready_to_receive)
                    {
                        order.Status = StatusValue.ready_to_receive;
                        updatebtn.Text = StatusValue.received;
                        var json = JsonConvert.SerializeObject(order);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        var responseProduct = await client.PostAsync(urlupdateOrder, data);
                    }
                    if(updatebtn.Text == StatusValue.received)
                    {
                        order.Status = StatusValue.received;
                        var json = JsonConvert.SerializeObject(order);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        var responseProduct = await client.PostAsync(urlupdateOrder, data);
                    }
                        
                }
                
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void getOrders()
        {
            listOrders.ItemsSource = null;
            client = new HttpClient();
            orders = new ObservableCollection<Order>();

            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
            try
                {
                    var responseOrders = await client.GetAsync(urlOrder);

                    responseOrders.EnsureSuccessStatusCode();

                    if (responseOrders.IsSuccessStatusCode)
                    {
                        var resault = responseOrders.Content.ReadAsStringAsync().Result;
                        orders = JsonConvert.DeserializeObject<ObservableCollection<Order>>(resault);
                        listOrders.ItemsSource = orders;


                    }

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            
        }
    }
}