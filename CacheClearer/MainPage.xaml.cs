﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace CacheClearer
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            listCacheApps();
        }

        public void listCacheApps()
        {

            // FileSystem

            if (!WP7RootToolsSDK.Environment.HasRootAccess())
            {
                MessageBox.Show("No root access. Please allow through Root Tools");
                return;
            }

            WP7RootToolsSDK.Folder folder = WP7RootToolsSDK.FileSystem.GetFolder("\\Applications\\Data\\");
            List<WP7RootToolsSDK.FileSystemEntry> apps = folder.GetSubItems();
            foreach (WP7RootToolsSDK.FileSystemEntry app in apps)
            {
                if (app.IsFolder)
                {
                    List<WP7RootToolsSDK.FileSystemEntry> items = ((WP7RootToolsSDK.Folder)app).GetSubItems();
                    String appName = WP7RootToolsSDK.Applications.GetApplicationName(new Guid(app.Name));
                    System.Diagnostics.Debug.WriteLine(appName + " - " + app.Name);
                    String cachePath = app.Path + "\\Data\\Cache\\";
                    if (WP7RootToolsSDK.FileSystem.FileExists(cachePath))
                    {
                        //WP7RootToolsSDK.Folder CacheFolder = WP7RootToolsSDK.FileSystem.GetFolder(cachePath);
                        listBox1.Items.Add(new AppListItem(app.Name, appName));
                    }

                    System.Diagnostics.Debug.WriteLine("");
                }
            }


        }
        public class AppListItem
        {
            public string Guid;
            public string AppName;

            public AppListItem() { }
            public AppListItem(string Guid, string AppName)
            {
                this.Guid = Guid;
                this.AppName = AppName;
            }

            public override string ToString()
            {
                return AppName + " - " + Guid.ToString();
            }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (listBox1.SelectedIndex == -1)
                return;

            AppListItem item = (AppListItem)listBox1.SelectedItem;
            // Navigate to the new page
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?appguid=" + item.Guid, UriKind.Relative));

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will permanently erase cache files from your phone.", "Warning", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }

            int saved = 0;

            foreach (AppListItem item in listBox1.Items)
            {
                System.Diagnostics.Debug.WriteLine(item);
                saved += cleanCache.cleanAppCache(item.Guid);
            }

            MessageBox.Show("Cache cleaned.\n\nYou saved " + Utils.readableFileSize(saved) + " of storage space.");
        }

    }
}