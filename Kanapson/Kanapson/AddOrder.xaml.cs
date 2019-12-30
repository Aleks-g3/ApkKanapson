﻿using Kanapson.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kanapson
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOrder : ContentPage
    {
        public AddOrder()
        {
            InitializeComponent();
            
        }
        public async void LoadData()
        {
            var content = "";
            HttpClient client = new HttpClient();
            var RestURL = "http://localhost:4000/products";
            client.BaseAddress = new Uri(RestURL);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(RestURL);
            content = await response.Content.ReadAsStringAsync();
            var Items = JsonConvert.DeserializeObject<List<Product>>(content);
            this.FindByName<ListView>("list").ItemsSource = Items;
        }

        private void Back_Clicked(object sender, EventArgs e)
        {

        }

        private void sum_Clicked(object sender, EventArgs e)
        {

        }

        private void AddOrder_Clicked(object sender, EventArgs e)
        {

        }

        private void list_Scrolled(object sender, ScrolledEventArgs e)
        {

        }

        private void AddOrderUser_Clicked(object sender, EventArgs e)
        {

        }
    }
}