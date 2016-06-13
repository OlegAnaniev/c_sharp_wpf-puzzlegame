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

using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;

namespace ExamPuzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PuzzleGameSettings settings;
        string[] fileList;
        string[] picList;
        int picCount;
        BitmapImage[] ImagePreviews;        
        PuzzleGame game;
        ProgressBar progress;
        BackgroundWorker backgroundWorker;
        
        public MainWindow()
        {
            InitializeComponent();
            settings = new PuzzleGameSettings(3, 3, 30);
        }
                
        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            CommonMethods.GridClear(PictureGrid);

            if (!PreviewsGetFileList())
            {
                return;
            }

            PreviewsGetPicList();
            
            if (picCount ==  0)
            {
                return;
            }

            PreviewsMakeGridRowsCols();

            progress = new ProgressBar();
            Grid.SetColumnSpan(progress, PictureGrid.ColumnDefinitions.Count);
            Grid.SetRow(progress, PictureGrid.RowDefinitions.Count / 2);
            progress.Minimum = 0;
            progress.Maximum = picCount - 1;
            progress.Foreground = System.Windows.Media.Brushes.Green;
            PictureGrid.Children.Add(progress);

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            //backgroundWorker.ProgressChanged += PreviewsLoadProgress;
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(PreviewsLoadProgress);
            //backgroundWorker.DoWork += PreviewsLoad;
            backgroundWorker.DoWork += new DoWorkEventHandler(PreviewsLoad);
            //backgroundWorker.RunWorkerCompleted += PreviewsDisplay;
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PreviewsDisplay);
            backgroundWorker.RunWorkerAsync();            
        }

        private bool PreviewsGetFileList()
        {
            try
            {
                fileList = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Pics");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Directory \"Pics\" not found", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void PreviewsGetPicList()
        {
            picList = new string[fileList.Length];
            picCount = 0;
            for (int i = 0; i < fileList.Length; i++)
            {
                if (fileList[i].IndexOf(".jpg") != -1)
                {
                    picList[picCount++] = fileList[i];
                }
            }
        }

        private void PreviewsMakeGridRowsCols()
        {
            int colCount = (int)PictureGrid.ActualWidth / 100;
            CommonMethods.GridAddColumns(PictureGrid, colCount);

            int rowCount;
            if (picCount % colCount == 0)
            {
                rowCount = (int)picCount / colCount;
            }
            else
            {
                rowCount = (int)picCount / colCount + 1;
            }
            CommonMethods.GridAddRows(PictureGrid, rowCount);
        }

        private void PreviewsLoad(object sender, DoWorkEventArgs e)
        {
            BitmapImage[] tempImagePreviews;
            
            tempImagePreviews = new BitmapImage[picCount];
            for (int i = 0; i < picCount; i++)
            {                
                tempImagePreviews[i] = new BitmapImage();
                tempImagePreviews[i].BeginInit();
                tempImagePreviews[i].UriSource = new Uri(@"pack://siteoforigin:,,,/Pics/"
                    + picList[i].Substring(picList[i].LastIndexOf("\\")));
                tempImagePreviews[i].DecodePixelHeight = 50;
                tempImagePreviews[i].EndInit();
                tempImagePreviews[i].Freeze();

                //this.Dispatcher.Invoke((Action)(() =>
                //{
                //    progress.Value = i;
                //}));

                backgroundWorker.ReportProgress(i + 1);
            }            
            e.Result = tempImagePreviews;
        }

        private void PreviewsDisplay(object sender, RunWorkerCompletedEventArgs e)
        {
            ImagePreviews = (BitmapImage[]) e.Result;
            Thread.Sleep(100);
            PictureGrid.Children.Remove(progress);
            ///
            Button temp;

            for (int i = 0; i < PictureGrid.RowDefinitions.Count; i++)
            {
                for (int y = 0; y < PictureGrid.ColumnDefinitions.Count; y++)
                {
                    temp = new Button();
                    temp.Background = 
                        new ImageBrush(ImagePreviews[i * PictureGrid.ColumnDefinitions.Count + y]);
                    temp.Click += ImageButton_Click;
                    temp.Content = (i * PictureGrid.ColumnDefinitions.Count + y).ToString();
                    Grid.SetColumn(temp, y);
                    Grid.SetRow(temp, i);
                    PictureGrid.Children.Add(temp);

                    if (i * PictureGrid.ColumnDefinitions.Count + y + 1 == picCount)
                    {
                        return;
                    }
                }
            }
        }

        private void PreviewsLoadProgress(object sender, ProgressChangedEventArgs e)
        {
            progress.Value = e.ProgressPercentage;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuOptions_Click(object sender, RoutedEventArgs e)
        {
            new Settings(settings).ShowDialog();
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            CommonMethods.GridClear(PictureGrid);

            game = new PuzzleGame(settings, PictureGrid,
                ImagePreviews[Int32.Parse(((Button)sender).Content.ToString())].UriSource.ToString());
                
            AppMainWindow.KeyDown += game.CurrentImageShow;
            game.Start();            
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            if (game != null)
            {
                Stream TestFileStream = File.Create("save.dat");
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(TestFileStream, game);
                TestFileStream.Close();
            }
            else
            {
                MessageBox.Show("Game not started", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Stream TestFileStream = File.Open("save.dat", FileMode.Open);
                BinaryFormatter serializer = new BinaryFormatter();
                game = (PuzzleGame)serializer.Deserialize(TestFileStream);
                TestFileStream.Close();

                CommonMethods.GridClear(PictureGrid);
                AppMainWindow.KeyDown += game.CurrentImageShow;
                game.PuzzleGameLoaded(PictureGrid);
            }
            catch (IOException)
            {
                MessageBox.Show("Saved game not found", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            
        }
    }
}