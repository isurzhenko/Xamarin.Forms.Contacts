using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plugin.ContactService.Shared;
using Xamarin.Forms;

namespace Sample.ContactService
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.GetContacs().ContinueWith(contacts =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    lstContacts.BindingContext = contacts;
                });
            });
        }

        async Task<IEnumerable<Contact>> GetContacs()
        {
            return await Plugin.ContactService.CrossContactService.Current.GetContactListAsync();
        }
    }
}
