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
    public partial class AddorderAdmin : ContentPage
    {
        private HttpClient client;
        private ObservableCollection<Product> products;
        string urlProduct = "http://192.168.1.4:4000/products/gettoorder";
        string urladdOrder = "http://192.168.1.4:4000/orders/add";
        string urlUser = "http://192.168.1.4:4000/users/findbyid/";
        private Order order;
        private JwtSecurityTokenHandler jwtHandler;
        double sum;
        private User user;
        private string jwtPayload;

        public AddorderAdmin()
        {
            InitializeComponent();
            AddOrder.IsEnabled = false;
            Card.IsEnabled = false;
            Cash.IsEnabled = false;
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            jwtHandler = new JwtSecurityTokenHandler();
            AddOrder.IsEnabled = false;
            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
            if (!jwtHandler.CanReadToken(Xamarin.Forms.Application.Current.Properties["Token"].ToString())) throw new Exception("The token doesn't seem to be in a proper JWT format.");

            var token = jwtHandler.ReadJwtToken(Xamarin.Forms.Application.Current.Properties["Token"].ToString());
            jwtPayload = token.Claims.First(c => c.Type == "unique_name").Value;
            getUser(jwtPayload);
            GetProducts();
            
            order = new Order();
            order.Sum = 0;

            listProduct.RefreshCommand = new Command(() =>
            {
                listProduct.IsRefreshing = true;
                GetProducts();
            });

        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void AddOrder_Clicked(object sender, EventArgs e)
        {
            
            order.User = user;
            //order.user.Username = user.Username;
            try
            {
                var json = JsonConvert.SerializeObject(order);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var responseProduct = await client.PostAsync(urladdOrder, data);

                responseProduct.EnsureSuccessStatusCode();

                if (responseProduct.IsSuccessStatusCode)
                {
                    await DisplayAlert("Powiadomienie", " zamówienie zostało złożone", "Ok");
                    await Navigation.PopModalAsync();


                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", ex.Message, "Ok");
            }
        }

        private async void GetProducts()
        {
            listProduct.ItemsSource = null;
            
            products = new ObservableCollection<Product>();

            
            try
            {
                var responseProduct = await client.GetAsync(urlProduct);

                

                if (responseProduct.IsSuccessStatusCode)
                {
                    var resault = responseProduct.Content.ReadAsStringAsync().Result;
                    products = JsonConvert.DeserializeObject<ObservableCollection<Product>>(resault);
                    listProduct.ItemsSource = products;


                }
                else
                {
                    await DisplayAlert("Błąd", JObject.Parse(responseProduct.Content.ReadAsStringAsync().Result)["message"].ToString(), "Ok");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void add_Clicked(object sender, EventArgs e)
        {
            Xamarin.Forms.Button addProduct = (Xamarin.Forms.Button)sender;
            Grid grid = (Grid)addProduct.Parent;
            Label name = (Label)grid.Children[0];
            Entry Count = (Entry)grid.Children[1];
            Label Amount = (Label)grid.Children[2];
            Label Price = (Label)grid.Children[3];
            try
            {
                if (addProduct.Text == "Dodaj")
                {
                    if (string.IsNullOrWhiteSpace(Count.Text) || Convert.ToUInt16(Count.Text) <= 0 || Convert.ToUInt16(Count.Text) > Convert.ToUInt16(Amount.Text))
                    {
                        await DisplayAlert("", "błędna wartość", "OK");
                    }
                    else
                    {
                        if (order.Products_order == null)
                            order.Products_order = new List<Product_Order>();
                        order.Products_order.Add(new Product_Order() { count = Convert.ToUInt16(Count.Text),product=new Product() { Name = name.Text },PriceEach=Double.Parse(Price.Text) * Convert.ToUInt16(Count.Text) });
                        if (order.Products_order.Count == 1)
                        {
                            Cash.IsEnabled = true;
                            Card.IsEnabled = true;
                        }
                        
                        sum += order.Products_order.FirstOrDefault(p => p.product.Name == name.Text).PriceEach;
                        
                            
                        
                        Sum.Text = sum + " zł";
                        addProduct.Text = "Usuń";
                        Count.IsEnabled = false;



                    }
                }
                else
                {
                    sum -= order.Products_order.FirstOrDefault(p => p.product.Name == name.Text).PriceEach;
                    Sum.Text = sum + " zł";
                    order.Products_order.Remove(order.Products_order.FirstOrDefault(o=>o.product.Name==name.Text));
                    if (order.Products_order.Count == 0)
                    {
                        order.Payment = null;
                        Cash.IsEnabled = false;
                        Card.IsEnabled = false;
                        AddOrder.IsEnabled = false;
                    }
                    Count.IsEnabled = true;
                    addProduct.Text = "Dodaj";
                }

                

            }
            catch(Exception ex)
            {
                await DisplayAlert("Błąd", ex.Message, "Ok");
            }
            
        }
        private async void getUser(string id)
        {
            user = new User();
            try
            {
                var response = await client.GetAsync(urlUser + id);

                

                if (response.IsSuccessStatusCode)
                {
                    var resault = response.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<User>(resault);
                    


                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", ex.Message, "Ok");
            }
        }

        private void Card_Clicked(object sender, EventArgs e)
        {
            order.Payment = Card.Text;
            Card.IsEnabled = false;
            Cash.IsEnabled = true;
            AddOrder.IsEnabled = true;
        }

        private void Cash_Clicked(object sender, EventArgs e)
        {
            order.Payment = Cash.Text;
            Card.IsEnabled = true;
            Cash.IsEnabled = false;
            AddOrder.IsEnabled = true;
        }
    }
}