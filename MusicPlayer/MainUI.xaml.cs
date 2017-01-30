using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace StreamPlayer
{
    public partial class MainUI : PhoneApplicationPage
    {
        public MainUI()
        {
            InitializeComponent();
            RegisterProviders();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var searchItems = PrepareSearchQuery(QueryText.Text);
            Search(searchItems.ToString(), (items) =>
            {
                Dispatcher.BeginInvoke(() =>
                                          {
                                              Queue.ItemsSource = items;
                                          });
            });
        }

    }
}