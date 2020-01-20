using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kanapson
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserMenu : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        public UserMenu()
        {
            InitializeComponent();

            
        }

        

        private void mycredit_Clicked(object sender, EventArgs e)
        {

        }

        private void changePassword_Clicked(object sender, EventArgs e)
        {

        }

        private void myorders_Clicked(object sender, EventArgs e)
        {

        }

        private async void addorder_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddOrder());
        }

        private void Logout_Clicked(object sender, EventArgs e)
        {

        }
    }
}
