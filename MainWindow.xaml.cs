﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.MemoryMappedFiles;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Net;

namespace GW2MumbleLinkTesting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MumbleLink data = new MumbleLink();
        
        public MainWindow()
        {
            // Sizeof(Link) is 10580 as reported by the python app
            InitializeComponent();
            
            try
            {
                //MemoryMappedFile.CreateOrOpen("MumbleLink", 10580);
                MemoryMappedFile.CreateNew("MumbleLink", 10580);
            }
            catch (Exception)
            {
                MessageBox.Show("Fatal: Trouble creating the Mumble Link MMF", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);

                //Application.Current.Shutdown();
            }

            DispatcherTimer refreshTimer = new DispatcherTimer();
            refreshTimer.Tick += refreshTimer_Tick;
            refreshTimer.Interval = new TimeSpan(TimeSpan.TicksPerSecond / 30);
            refreshTimer.Start();
        }

        void refreshTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                using (var mmf = MemoryMappedFile.OpenExisting("MumbleLink"))
                {
                    using (var accessor = mmf.CreateViewAccessor())
                    {
                        float[] avPos = new float[3];
                        accessor.ReadArray<float>(2 * sizeof(uint), avPos, 0, 3);
                        tbXPos.Text = avPos[0].ToString();
                        tbYPos.Text = avPos[1].ToString();
                        tbZPos.Text = avPos[2].ToString();

                        float[] avFront = new float[3];
                        accessor.ReadArray<float>(2 * sizeof(uint) + 3 * sizeof(float), avFront, 0, 3);
                        tbXAvFront.Text = avFront[0].ToString();
                        tbYAvFront.Text = avFront[1].ToString();
                        tbZAvFront.Text = avFront[2].ToString();

                        char[] ident = new char[256];
                        accessor.ReadArray<char>(592, ident, 0, 256);
                        string charName = new string(ident);
                        Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                        charName = rgx.Replace(charName, "");
                        tbCharName.Text = charName;

                        uint[] context = new uint[10];
                        accessor.ReadArray<uint>(1108, context, 0, 10);
                        tbMapID.Text = context[7].ToString();
                        tbWorldID.Text = context[9].ToString();
                    }
                }
            }
            catch (Exception)
            {
                overlayGrid.Visibility = System.Windows.Visibility.Visible;
                MessageBox.Show("Trouble accessing Mumble Link MMF");
                ((System.Windows.Threading.DispatcherTimer)sender).Stop();
            }
        }

        private void tbWorldID_TextChanged(object sender, TextChangedEventArgs e)
        {
            var w = new WebClient();
            var response = w.DownloadString(@"https://api.guildwars2.com/v1/world_names.json");

            var j = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GW2APILibrary.World>>(response);

            var world = from i in j
                        where i.ID == int.Parse(tbWorldID.Text)
                        select i;

            if (world.Count() > 0)
                tbServerName.Text = world.First<GW2APILibrary.World>().Name;
        }

        private void tbMapID_TextChanged(object sender, TextChangedEventArgs e)
        {
            string mapID = tbMapID.Text;
            var w = new WebClient();
            var response = w.DownloadString(@"https://api.guildwars2.com/v1/maps.json?map_id=" + mapID);

            dynamic map = Newtonsoft.Json.Linq.JObject.Parse(response);

            tbMapName.Text = map["maps"][mapID]["map_name"];

        }
    }
}
