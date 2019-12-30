using Kanapson.Models;
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
    public partial class UpdateCredit : ContentPage
    {
        public UpdateCredit()
        {
            InitializeComponent();
        }

        public async void LoadData()
        {
            var content = "";
            HttpClient client = new HttpClient();
            var RestURL = "http://localhost:4000/users";
            client.BaseAddress = new Uri(RestURL);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(RestURL);
            content = await response.Content.ReadAsStringAsync();
            var Items = JsonConvert.DeserializeObject<List<User>>(content);
            this.FindByName<ListView>("list").ItemsSource = Items;
        }

        private void ppdate_Clicked(object sender, EventArgs e)
        {

        }

        private void update_Clicked(object sender, EventArgs e)
        {

        }

        private void back_Clicked(object sender, EventArgs e)
        {

        }
    }
}