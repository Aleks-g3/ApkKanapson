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
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace Kanapson
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOrder : ContentPage
    {
        private HttpClient client;
        private ObservableCollection<Product> products;

        string urlProduct = "http://192.168.1.4:4000/products/gettoorder";
        string urladdOrder = "http://192.168.1.4:4000/orders/add";
        string urlUser = "http://192.168.1.4:4000/users/?id=";
        private Product_Order product_Order;
        private Order order;
        double sum;
        private JwtSecurityTokenHandler jwtHandler;
        private string jwtPayload;
        private User user;

        public AddOrder()
        {
            InitializeComponent();
            AddOrderUser.IsEnabled = false;
            sum = 0;
            listProduct.On<Android>().SetIsFastScrollEnabled(true);
            GetProducts();
            
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
            if(user.Credit>sum)
            {
                
                order.Sum = sum;
                order.user.Id = user.Id;
                order.user.Username = user.Username;
                try
                {
                    var json = JsonConvert.SerializeObject(order);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var responseProduct = await client.PostAsync(urladdOrder, data);

                    responseProduct.EnsureSuccessStatusCode();

                    if (responseProduct.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Powiadomienie", "Twoje zamówienie zostało złożone", "Ok");
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
            listProduct.IsRefreshing = true;
            client = new HttpClient();
            products = new ObservableCollection<Product>();

            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
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
                listProduct.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void getCredit(string id)
        {
            user = new User();
            try
            {
                var response = await client.GetAsync(urlUser+id);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var resault = response.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<User>(resault);
                    Credit.Text = user.Credit + " zł";


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
                        
                        product_Order=new Product_Order() { count = Convert.ToUInt16(Count.Text), product = new Product() { Name = name.Text }, PriceEach = Double.Parse(Price.Text) * Convert.ToUInt16(Count.Text) };

                        sum += product_Order.PriceEach;
                        Sum.Text = sum + " zł";
                        addProduct.Text = "Usuń";
                        Count.IsEnabled = false;
                        if (user.Credit > sum)
                            rest.Text = user.Credit - product_Order.PriceEach + " zł";


                    }
                }
                else
                {
                    sum -= order.Product_order.FirstOrDefault(p => p.product.Name == name.Text).PriceEach;
                    Sum.Text = sum + " zł";
                    rest.Text = user.Credit + order.Product_order.FirstOrDefault(p => p.product.Name == name.Text).PriceEach + " zł";
                    order.Product_order.Remove(order.Product_order.FirstOrDefault(p => p.product.Name == name.Text));
                    Count.Text = "1";
                    Count.IsEnabled = true;
                    addProduct.Text = "Dodaj";
                }
                if (order.Product_order.Count > 0)
                    AddOrderUser.IsEnabled = true;
                else
                    AddOrderUser.IsEnabled = false;
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            
        }
    }
}