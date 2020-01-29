using Kanapson.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public partial class ListProducts : ContentPage
    {
        private HttpClient client;
        private List<Product> products;

        string urlProduct = "http://192.168.1.5:4000/products";
        string urladdProduct = "http://192.168.1.5:4000/products/addproduct";
        
        private Product product;

        public ListProducts()
        {
            InitializeComponent();
            listProduct.On<Android>().SetIsFastScrollEnabled(true);
            GetProducts();
            listProduct.RefreshCommand = new Command(() =>
            {
                GetProducts();
                listProduct.IsRefreshing = false;
            });
            
        }

        private async void update_Clicked(object sender, EventArgs e)
        {
            Xamarin.Forms.Button updatebtn = (Xamarin.Forms.Button)sender;
            Grid grid = (Grid)updatebtn.Parent;
            Label Name = (Label)grid.Children[0];
            Editor Amount = (Editor)grid.Children[1];
            Editor Price = (Editor)grid.Children[2];

            if ( string.IsNullOrWhiteSpace(Amount.Text) || string.IsNullOrWhiteSpace(Price.Text))
                await DisplayAlert("", "Wszystkie pola muszą być uzupełnione", "OK");
            else
            {

                product = new Product
                {
                    Name = Name.Text,
                    Amount = Convert.ToUInt16(Amount.Text),
                    Price = Double.Parse(Price.Text)
                };
                try
                {
                    var json = JsonConvert.SerializeObject(product);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync(urladdProduct, data);
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        GetProducts();
                        listProduct.IsRefreshing = false;
                    }
                    else
                    {
                        await DisplayAlert("", response.Content.ReadAsStringAsync().Result, "OK");
                    }


                }
                catch (Exception ex)
                {
                    await DisplayAlert("", ex.Message, "OK");
                }
            }

        }

        private async void AddProduct_Clicked(object sender, EventArgs e)
        {
            
            Editor Name = (Editor)AddView.Children[0];
            Editor Amount= (Editor)AddView.Children[1];
            Editor Price= (Editor)AddView.Children[2];
            if (string.IsNullOrWhiteSpace(Name.Text) || string.IsNullOrWhiteSpace(Amount.Text) || string.IsNullOrWhiteSpace(Price.Text))
                await DisplayAlert("", "Wszystkie pola muszą być uzupełnione","OK");
            else
            {

                product = new Product
                {
                    Name = Name.Text,
                    Amount = Convert.ToUInt16(Amount.Text),
                    Price = Double.Parse(Price.Text)
                };
                try
                {
                    var json = JsonConvert.SerializeObject(product);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(urladdProduct, data);
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        Name.Text = Amount.Text = Price.Text = "";
                        GetProducts();
                        listProduct.IsRefreshing = false;
                    }
                    else
                    {
                        await DisplayAlert("",response.Content.ReadAsStringAsync().Result,"OK");
                    }


                }
                catch (Exception ex)
                {
                    await DisplayAlert("", ex.Message, "OK");
                }
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
                    var resault= responseProduct.Content.ReadAsStringAsync().Result;
                    products=JsonConvert.DeserializeObject<List<Product>>(resault);
                    listProduct.ItemsSource = products;
                    
                    
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void back_Clicked(object sender, EventArgs e)
        {

        }
    }
}