using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kanapson
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminMenu : ContentPage
    {
        public AdminMenu()
        {
            InitializeComponent();
        }

        private async void addorder_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddorderAdmin());
        }

        private async void listorders_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ListOrders());
        }

        

        private async void listproducts_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ListProducts());
        }

        private async void adduser_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new RegisterAdmin());
        }

        private async void updatecredit_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new UpdateCredit());
        }

        private async void changepassword_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ChangePassword());
        }

        

        private async void Logout_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties["Token"] = null;
            await Navigation.PopModalAsync();

        }

        private async void Exit_Clicked(object sender, EventArgs e)
        {
            try
            {
                Xamarin.Forms.Application.Current.Properties.Clear();
                Application.Current.Quit();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }
    }
}