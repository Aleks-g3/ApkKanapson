using Kanapson.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public partial class ListProducts : ContentPage
    {
        private HttpClient client;
        private ObservableCollection<Product> products;

        string urlProduct = "https://kanapson.pl/products";
        string urladdProduct = "https://kanapson.pl/products/addproduct";
        
        private Product product;

        public ListProducts()
        {
            InitializeComponent();
            GetProducts();
            listProduct.RefreshCommand = new Command(() =>
            {
                GetProducts();
                listProduct.IsRefreshing = false;
            });
            
        }

        private async void update_Clicked(object sender, EventArgs e)
        {
            Button updatebtn = (Button)sender;
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

                    if (response.IsSuccessStatusCode)
                    {
                        GetProducts();
                        listProduct.IsRefreshing = false;
                    }
                    else
                    {
                        await DisplayAlert("Błąd", JObject.Parse(response.Content.ReadAsStringAsync().Result)["message"].ToString(), "Ok");
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
            
            Entry Name = (Entry)AddView.Children[0];
            Entry Amount= (Entry)AddView.Children[1];
            Entry Price= (Entry)AddView.Children[2];
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
                    

                    if (response.IsSuccessStatusCode)
                    {
                        Name.Text = Amount.Text = Price.Text = "";
                        await DisplayAlert("Powiadomienie", "Produkt został dodany pomyślnie", "Ok");
                        GetProducts();
                    }
                    else
                    {
                        await DisplayAlert("Błąd", JObject.Parse(response.Content.ReadAsStringAsync().Result)["message"].ToString(), "Ok");
                    }


                }
                catch (Exception ex)
                {
                    await DisplayAlert("Błąd", ex.Message, "OK");
                }
            }
        }

        private async void GetProducts()
        {
            listProduct.IsRefreshing = false;
            listProduct.ItemsSource = null;
            client = new HttpClient();
            products = new ObservableCollection<Product>();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
            try
            {
                var responseProduct = await client.GetAsync(urlProduct);

               

                if (responseProduct.IsSuccessStatusCode)
                {
                    var resault= responseProduct.Content.ReadAsStringAsync().Result;
                    products=JsonConvert.DeserializeObject<ObservableCollection<Product>>(resault);
                    listProduct.ItemsSource = products;
                    
                    
                }
                else
                {
                    await DisplayAlert("Błąd", JObject.Parse(responseProduct.Content.ReadAsStringAsync().Result)["message"].ToString(), "Ok");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", ex.Message, "Ok");
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                listProduct.IsRefreshing = false;
            });
        }

        private async void back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void delete_Clicked(object sender, EventArgs e)
        {
            Button deleteBtn = (Button)sender;
            Grid grid = (Grid)deleteBtn.Parent;
            Label Name = (Label)grid.Children[0];

            product = products.FirstOrDefault(p => p.Name == Name.Text);
            if (product != null)
            {
                try
                {
                    var response = await client.DeleteAsync(urlProduct + "/" + product.Id);
                    if (response.IsSuccessStatusCode)
                    {
                        GetProducts();
                    }
                    else
                    {
                        await DisplayAlert("Błąd", JObject.Parse(response.Content.ReadAsStringAsync().Result)["message"].ToString(), "Ok");
                    }
                }
                catch(Exception ex)
                {
                    await DisplayAlert("Błąd", ex.Message, "Ok");
                }
            }
        }
    }
}