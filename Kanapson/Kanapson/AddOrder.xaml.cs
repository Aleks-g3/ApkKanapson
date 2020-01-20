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
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace Kanapson
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOrder : ContentPage
    {
        private HttpClient client;
        private List<Product> products;

        string urlProduct = "http://192.168.1.4:4000/products/gettoorder";
        string urladdOrder = "http://192.168.1.4:4000/orders/add";
        string urlUser = "http://192.168.1.4:4000/users/?id=";
        private List<Product_Order> product_Order;
        private Order order;
        double sum;
        private JwtSecurityTokenHandler jwtHandler;
        private string jwtPayload;
        private List<User> user;

        public AddOrder()
        {
            InitializeComponent();
            sum = 0;
            listProduct.On<Android>().SetIsFastScrollEnabled(true);
            GetProducts();
            product_Order = new List<Product_Order>();
            order = new Order();
            order.Sum = 0;
             jwtHandler = new JwtSecurityTokenHandler();
            if (!jwtHandler.CanReadToken(Xamarin.Forms.Application.Current.Properties["Token"].ToString())) throw new Exception("The token doesn't seem to be in a proper JWT format.");

            var token = jwtHandler.ReadJwtToken(Xamarin.Forms.Application.Current.Properties["Token"].ToString());
             jwtPayload = token.Claims.First(c=>c.Type== "unique_name").Value;

            getCredit(jwtPayload);
            listProduct.RefreshCommand = new Command(() =>
            {
                GetProducts();
                listProduct.IsRefreshing = false;
            });
            
        }
        

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        

        

        

        private async void AddOrderUser_Clicked(object sender, EventArgs e)
        {
            if(user[0].Credit>sum)
            {
                order.Product_order = product_Order;
                order.Sum = sum;
                order.user = user[0];
                order.orderTimes = DateTime.Now;
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
            else
            {
                await DisplayAlert("", "Za mał środków na koncie", "OK");
            }
            
        }

        private async void GetProducts()
        {
            listProduct.ItemsSource = null;
            client = new HttpClient();
            products = new List<Product>();

            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
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

        private async void getCredit(string id)
        {
            user = new List<User>();
            try
            {
                var response = await client.GetAsync(urlUser+id);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var resault = response.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<List<User>>(resault);
                    Credit.Text = user[0].Credit + " zł";


                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void addProduct_Clicked(object sender, EventArgs e)
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
                    
                    sum+= product_Order.First(p=>p.product.Name==name.Text).PriceEach;
                    Sum.Text = sum + " zł";
                    addProduct.Text = "Usuń";
                    Count.IsEnabled = false;
                    if (user[0].Credit > sum)
                        rest.Text = user[0].Credit - product_Order.First(p => p.product.Name == name.Text).PriceEach + " zł";


                }
            }
            else
            {
                sum -= product_Order.First(p => p.product.Name == name.Text).PriceEach;
                Sum.Text = sum + " zł";
                rest.Text = user[0].Credit + product_Order.First(p => p.product.Name == name.Text).PriceEach + " zł";
                product_Order.Remove(product_Order.First(p => p.product.Name == name.Text));
                Count.Text = "1";
                Count.IsEnabled = true;
                addProduct.Text = "Dodaj";
            }
            
            
        }
    }
}