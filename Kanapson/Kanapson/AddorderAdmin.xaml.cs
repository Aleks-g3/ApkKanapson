using Kanapson.Models;
using Newtonsoft.Json;
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
        Product_Order product_Order;
        private Order order;
        private JwtSecurityTokenHandler jwtHandler;
        double sum;
        private User user;
        private string jwtPayload;

        public AddorderAdmin()
        {
            InitializeComponent();
            AddOrder.IsEnabled = false;
            client = new HttpClient();
            jwtHandler = new JwtSecurityTokenHandler();
            
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
            
            order.Sum = sum;
            order.user = user;
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
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void GetProducts()
        {
            listProduct.ItemsSource = null;
            
            products = new ObservableCollection<Product>();

            
            try
            {
                var responseProduct = await client.GetAsync(urlProduct);

                responseProduct.EnsureSuccessStatusCode();

                if (responseProduct.IsSuccessStatusCode)
                {
                    var resault = responseProduct.Content.ReadAsStringAsync().Result;
                    products = JsonConvert.DeserializeObject<ObservableCollection<Product>>(resault);
                    listProduct.ItemsSource = products;


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
            Editor Count = (Editor)grid.Children[1];
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
                        product_Order = new Product_Order();
                        product_Order.count = Convert.ToUInt16(Count.Text);
                        product_Order.product = new Product() { Name = name.Text };
                        product_Order.PriceEach = Double.Parse(Price.Text) * Convert.ToUInt16(Count.Text);

                        order.Product_order.Add(product_Order);
                        sum += product_Order.PriceEach;
                        Sum.Text = sum + " zł";
                        addProduct.Text = "Usuń";
                        Count.IsEnabled = false;



                    }
                }
                else
                {
                    sum -= order.Product_order.FirstOrDefault(p => p.product.Name == name.Text).PriceEach;
                    Sum.Text = sum + " zł";
                    order.Product_order.Remove(order.Product_order.FirstOrDefault(o=>o.product.Name==name.Text));
                    Count.Text = "1";
                    Count.IsEnabled = true;
                    addProduct.Text = "Dodaj";
                }


                if (order.Product_order.Count > 0)
                    AddOrder.IsEnabled = true;
                else
                    AddOrder.IsEnabled = false;
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            
        }
        private async void getUser(string id)
        {
            user = new User();
            try
            {
                var response = await client.GetAsync(urlUser + id);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var resault = response.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<User>(resault);
                    


                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void Card_Clicked(object sender, EventArgs e)
        {
            order.Payment = Card.ToString();
            Card.IsEnabled = false;
            Cash.IsEnabled = true;
        }

        private void Cash_Clicked(object sender, EventArgs e)
        {
            order.Payment = Cash.ToString();
            Card.IsEnabled = true;
            Cash.IsEnabled = false;
        }
    }
}