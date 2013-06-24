using System;
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
                MemoryMappedFile.CreateNew("MumbleLink", 10580);
                
                /*using (var mmf = MemoryMappedFile.OpenExisting("MumbleLink"))
                {
                    using (var accessor = mmf.CreateViewAccessor())
                    {
                        uint[] value = new uint[256];

                        //name (program name, Guild Wars 2) is at location 44
                        //identity (character name) is at 592
                        //context (uint) at 1108
                        accessor.ReadArray<uint>(1108, value, 0, 10);

                        foreach (uint c in value)
                        {
                            MessageBox.Show(c.ToString());
                        }
                    }
                }*/
            }
            catch (Exception)
            {
                MessageBox.Show("Trouble creating the Mumble Link MMF");

                Application.Current.Shutdown();
            }

            System.Windows.Threading.DispatcherTimer refreshTimer = new System.Windows.Threading.DispatcherTimer();
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
                DataWindow.Background = Brushes.Red;
                MessageBox.Show("Trouble accessing Mumble Link MMF");
                ((System.Windows.Threading.DispatcherTimer)sender).Stop();
            }
        }
    }
}
