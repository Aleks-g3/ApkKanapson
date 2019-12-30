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
    public partial class ListProducts : ContentPage
    {
        public ListProducts()
        {
            InitializeComponent();
        }

        private void update_Clicked(object sender, EventArgs e)
        {

        }
    }
}