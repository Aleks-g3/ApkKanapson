using Kanapson.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kanapson
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        //User user;
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(this.FindByName<Entry>("username").Text)&& string.IsNullOrWhiteSpace(this.FindByName<Entry>("password").Text))
            {
                this.FindByName<Label>("messege").Text = "błedne";
            }
        }

        //public async void LoadData()
        //{
        //    user = new User();
        //    var content = "";
        //    HttpClient client = new HttpClient();
        //    var RestURL = "http://localhost:4000/users/authenticate";
        //client.BaseAddress = new Uri(RestURL);
        //    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //    HttpResponseMessage response = await client.PostAsync(RestURL);
        //    content = await request();
        //    var Items = JsonConvert.DeserializeObject<User>(content);
        //    user.Token = Items.Token;
        //}
    }
}
