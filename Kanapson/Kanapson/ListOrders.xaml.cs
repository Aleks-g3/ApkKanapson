using Kanapson.Models;
using Kanapson.Values;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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
            listOrders.RefreshCommand = new Command(() =>
            {
                getOrders();
                listOrders.IsRefreshing = false;
            });

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
                order = orders.FirstOrDefault(o => o.orderTimes.ToString() == orderTimes.Text);
                if (order != null)
                {
                    
                        var json = JsonConvert.SerializeObject(order);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await client.PutAsync(urlupdateOrder, data);
                    if (!response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Błąd", JObject.Parse(response.Content.ReadAsStringAsync().Result)["message"].ToString(), "Ok");
                    }
                    getOrders();
                }
                
            }
            catch(Exception ex)
            {
                await DisplayAlert("Błąd", ex.Message, "Ok");
            }
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void getOrders()
        {
            listOrders.IsRefreshing = true;
            listOrders.ItemsSource = null;
            client = new HttpClient();
            orders = new ObservableCollection<Order>();
            client.Timeout = TimeSpan.FromSeconds(10);
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
                        IEnumerable<PropertyInfo> pInfos = (listOrders as ItemsView<Cell>).GetType().GetRuntimeProperties();
                        var templatedItems = pInfos.FirstOrDefault(info => info.Name == "TemplatedItems");
                        if (templatedItems != null)
                        {
                            var cells = templatedItems.GetValue(listOrders);


                            foreach (ViewCell cell in cells as Xamarin.Forms.ITemplatedItemsList<Xamarin.Forms.Cell>)
                            {
                                if (cell.BindingContext != null)
                                {
                                    Grid grid = (Grid)cell.FindByName<Grid>("grid");
                                    Label orderTimes = (Label)grid.Children[0];
                                    Button actionBtn = (Button)grid.Children[4];
                                    if (orders.FirstOrDefault(o => o.orderTimes.ToString() == orderTimes.Text).Status == Status.Preparing)
                                        actionBtn.Text = Status.ready_to_receive;
                                    else
                                        actionBtn.Text = Status.received;
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Błąd", JObject.Parse(responseOrders.Content.ReadAsStringAsync().Result)["message"].ToString(), "Ok");
                        }

                    }
                Device.BeginInvokeOnMainThread(() =>
                {
                    listOrders.IsRefreshing = false;
                });

            }
            catch (Exception ex)
            {
                 await DisplayAlert("Błąd", ex.Message, "Ok");
            }
            
        }

        private void showProducts_Clicked(object sender, EventArgs e)
        {
            Button showProductsBtn = (Button)sender;
            Grid grid = (Grid)showProductsBtn.Parent;
            Label orderTimes = (Label)grid.Children[0];
            var Products_order = orders.FirstOrDefault(p => p.orderTimes.ToString() == orderTimes.Text).Products_order.ToList();
            listProducts.ItemsSource = Products_order;
            popupProducts.IsVisible = true;
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            popupProducts.IsVisible = false;
        }
    }
}