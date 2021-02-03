using System;
using System.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace DolgozatProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        List<string> plainNames = new List<string>();

        //contains the path of the files
        List<string> list = new List<string>();

        //counter for the forward button
        int counter = 0;

        //helper integer for the 2nd foreach
        int help = 1;

        public MainWindow()
        {
            InitializeComponent();            

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();            

        }

        void timer_Tick(object sender, EventArgs e)
        {
            //set the position of the slide bar
            if ((mediaPlayer.Source != null) && (mediaPlayer.NaturalDuration.HasTimeSpan))
            {
                playTimer.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                playTimer.Value = mediaPlayer.Position.TotalSeconds;
                libStat.Content = String.Format("{0} / {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }        
            else
                libStat.Content = "No file selected...";                        
        }

        private void buttonOpenAudio_Click(object sender, RoutedEventArgs e)
        {            

            //open the file dialog to choose audio
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Szergely's Music Player";
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {          
                foreach (var item in openFileDialog.FileNames)
                {
                    plainNames.Add(openFileDialog.SafeFileName);
                    //check the file extension
                    FileInfo fi = new FileInfo(openFileDialog.FileName);
                    string ext = fi.Extension;
                                        
                    if(ext != ".mp3")
                    {
                        MessageBox.Show("Not the right extension! Accepted extension: MP3");
                        break;
                    }
                    else
                    {                        
                        //if the file extension is correct, put it in the list
                        mediaPlayer.Open(new Uri(item));                        
                        list.Add(item);
                        listBoxWrite.Items.Add(item);
                    }                    
                }                
            }
        }


        //          #########     #########     #########    #########     #########   #########    #           #        #
        //          #                     #     #            #       #     #           #            #            #      #
        //          #                    #      #            #       #     #           #            #              #   #
        //          #########           #       ######       #########     #           ######       #                #
        //                  #          #        #            #  #          #   #####   #            #                #
        //                  #        #          #            #    #        #       #   #            #                #
        //          #########      ########     #########    #       #     #########   #########    #########        #



        private void buttonPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                

                if (list == null)
                {
                    MessageBox.Show("Nothing to play, open some with 'Open File' button", "Music Player by Szergely");
                }                
                else if(listBoxWrite.SelectedItem == null)
                {
                    mediaPlayer.Open(new Uri(list[0]));
                    mediaPlayer.Play();
                }
                else
                {
                    string name = list[listBoxWrite.SelectedIndex];
                    mediaPlayer.Open(new Uri(name));
                    mediaPlayer.Play();
                }
            }
            catch(Exception exc)
            {
                MessageBox.Show("Exception is the next: " + exc.ToString());                
            }
            

        }

        private void buttonPause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {            
            mediaPlayer.Volume = (double)volumeSlider.Value;      
        }

        private void playTimer_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void PlayerPlay()
        {
            try
            {
                counter += 1;

                string name = list[counter];

                mediaPlayer.Open(new Uri(name));
                mediaPlayer.Play();
            }
            catch
            {               
                string name = list[0];
                mediaPlayer.Open(new Uri(name));
                mediaPlayer.Play();             
            }                      

        }

        private void frwd_Click(object sender, RoutedEventArgs e)
        {                        
            PlayerPlay();
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                counter -= 1;

                string name = list[counter];

                mediaPlayer.Open(new Uri(name));
                mediaPlayer.Play();
            }
            catch
            {
                string name = list[0];
                mediaPlayer.Open(new Uri(name));
                mediaPlayer.Play();
            }            
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                list.RemoveAt(listBoxWrite.SelectedIndex);
                listBoxWrite.Items.Remove(listBoxWrite.SelectedItem);
            }
            catch
            {
                return;
            }

        }

        //it changes when the user changing the position of the slider
        private void playTimer_lost(object sender, MouseEventArgs e)
        {
            mediaPlayer.Position = new TimeSpan(0, 0, Convert.ToInt32(playTimer.Value));
        }


        /// </summary>
        /// <typeparam name="T">The type of object being written to the binary file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the binary file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the binary file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        //save the files int a .json file
        private void save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.InitialDirectory = @"C:\";
            save.Title = "Szergely's Save Screen";
            save.DefaultExt = "json";
            save.Filter = "Configuration files (*.json)|*.json|All files (*.*)|*.*";
            save.CheckPathExists = true;
            if (save.ShowDialog() == true)
            {
                WriteToBinaryFile<List<string>>(save.FileName, list);
            }
        }

        //load files from the saved .json file
        private void load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog load = new OpenFileDialog();
            load.Title = "Szergely's Load Screen";
            load.Filter = "Configuration files (*.json)|*.json|All files (*.*)|*.*";
            load.DefaultExt = ".json";
            load.CheckPathExists = true;
            if (load.ShowDialog() == true)
            {                
                list = ReadFromBinaryFile<List<string>>(load.FileName);
                foreach (var item in list)
                {
                    listBoxWrite.Items.Add(item);
                }
            }          
        }
    }
}
