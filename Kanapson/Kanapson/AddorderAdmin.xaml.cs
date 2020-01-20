using Kanapson.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private List<Product> products;
        string urlProduct = "http://192.168.1.4:4000/products/gettoorder";
        string urladdOrder = "http://192.168.1.4:4000/orders/add";
        string urlUser = "http://192.168.1.4:4000/users/?id=";
        private List<Product_Order> product_Order;
        private Order order;
        private JwtSecurityTokenHandler jwtHandler;
        double sum;
        private List<User> user;
        private string jwtPayload;

        public AddorderAdmin()
        {
            InitializeComponent();
            client = new HttpClient();
            jwtHandler = new JwtSecurityTokenHandler();
            
            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
            if (!jwtHandler.CanReadToken(Xamarin.Forms.Application.Current.Properties["Token"].ToString())) throw new Exception("The token doesn't seem to be in a proper JWT format.");

            var token = jwtHandler.ReadJwtToken(Xamarin.Forms.Application.Current.Properties["Token"].ToString());
            jwtPayload = token.Claims.First(c => c.Type == "unique_name").Value;
            getUser(jwtPayload);
            GetProducts();
            product_Order = new List<Product_Order>();
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
            order.Product_order = product_Order;
            order.Sum = sum;
            order.user = user[0];
            try
            {
                var json = JsonConvert.SerializeObject(order);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var responseProduct = await client.PostAsync(urladdOrder, data);

                responseProduct.EnsureSuccessStatusCode();

                if (responseProduct.IsSuccessStatusCode)
                {
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
            
            products = new List<Product>();

            
            try
            {
                var responseProduct = await client.GetAsync(urlProduct);

                responseProduct.EnsureSuccessStatusCode();

                if (responseProduct.IsSuccessStatusCode)
                {
                    var resault = responseProduct.Content.ReadAsStringAsync().Result;
                    products = JsonConvert.DeserializeObject<List<Product>>(resault);
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

            if (addProduct.Text == "Dodaj")
            {
                if (string.IsNullOrWhiteSpace(Count.Text) || Convert.ToUInt16(Count.Text) <= 0 || Convert.ToUInt16(Count.Text) > Convert.ToUInt16(Amount.Text))
                {
                    await DisplayAlert("", "błędna wartość", "OK");
                }
                else
                {
                    product_Order.Add(new Product_Order() { count = Convert.ToUInt16(Count.Text), product = new Product() { Name = name.Text }, PriceEach = Double.Parse(Price.Text) * Convert.ToUInt16(Count.Text) });

                    sum += product_Order.First(p => p.product.Name == name.Text).PriceEach;
                    Sum.Text = sum + " zł";
                    addProduct.Text = "Usuń";
                    Count.IsEnabled = false;
 


                }
            }
            else
            {
                sum -= product_Order.First(p => p.product.Name == name.Text).PriceEach;
                Sum.Text = sum + " zł";
                product_Order.Remove(product_Order.First(p => p.product.Name == name.Text));
                Count.Text = "1";
                Count.IsEnabled = true;
                addProduct.Text = "Dodaj";
            }

        }
        private async void getUser(string id)
        {
            user = new List<User>();
            try
            {
                var response = await client.GetAsync(urlUser + id);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var resault = response.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<List<User>>(resault);
                    


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