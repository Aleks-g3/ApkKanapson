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
    public partial class AmountCredit : ContentPage
    {
        private HttpClient client;
        private JwtSecurityTokenHandler jwtHandler;
        private string jwtPayload;
        string urlUser = "http://192.168.1.4:4000/users/?id=";
        private User user;

        public AmountCredit()
        {
            InitializeComponent();
            client = new HttpClient();
            jwtHandler = new JwtSecurityTokenHandler();
            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", Xamarin.Forms.Application.Current.Properties["Token"] as string);
            if (!jwtHandler.CanReadToken(Xamarin.Forms.Application.Current.Properties["Token"].ToString())) throw new Exception("The token doesn't seem to be in a proper JWT format.");

            var token = jwtHandler.ReadJwtToken(Xamarin.Forms.Application.Current.Properties["Token"].ToString());
            jwtPayload = token.Claims.First(c => c.Type == "unique_name").Value;
            getCredit(jwtPayload);
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void getCredit(string id)
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
                    Credit.Text = user.Credit + " zł";


                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}