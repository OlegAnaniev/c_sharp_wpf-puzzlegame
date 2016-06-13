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
using System.Windows.Shapes;

namespace ExamPuzzle
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        PuzzleGameSettings settings;
        PuzzleGameSettings tempSettings;

        public int ColCount
        {
            get { return tempSettings.ColumnsCount; }
            set { tempSettings.ColumnsCount = value; }
        }

        public int RowCount
        {
            get { return tempSettings.RowsCount; }
            set { tempSettings.RowsCount = value; }
        }

        public int ShuffleRounds
        {
            get { return tempSettings.ShuffleRounds; }
            set { tempSettings.ShuffleRounds = value; }
        }

        public Settings(PuzzleGameSettings settings)
        {
            this.settings = settings;
            this.tempSettings = new PuzzleGameSettings(settings.ColumnsCount, settings.RowsCount,
                settings.ShuffleRounds);
            InitializeComponent();
            DifficultyCheck();
            Slider1.ValueChanged += Slider_ValueChanged;
            Slider2.ValueChanged += Slider_ValueChanged;
            Slider3.ValueChanged += Slider_ValueChanged;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            settings.ColumnsCount = tempSettings.ColumnsCount;
            settings.RowsCount = tempSettings.RowsCount;
            settings.ShuffleRounds = tempSettings.ShuffleRounds;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DifficultyCheck()
        {
            if (tempSettings.ColumnsCount == 3 && tempSettings.RowsCount == 3 && 
                tempSettings.ShuffleRounds == 505)
            {
                this.Easy.IsChecked = true;
                SlidersDisable();
            }
            else if (tempSettings.ColumnsCount == 10 && tempSettings.RowsCount == 10 && 
                tempSettings.ShuffleRounds == 5005)
            {
                this.Normal.IsChecked = true;
                SlidersDisable();
            }
            else if (tempSettings.ColumnsCount == 20 && tempSettings.RowsCount == 20 && 
                tempSettings.ShuffleRounds == 10000)
            {
                this.Hard.IsChecked = true;
                SlidersDisable();
            }
            else
            {
                this.Custom.IsChecked = true;
                SlidersEnable();
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DifficultyCheck();
        }

        private void Easy_Click(object sender, RoutedEventArgs e)
        {
            Slider1.Value = 3;
            Slider2.Value = 3;
            Slider3.Value = 505;
        }

        private void Normal_Click(object sender, RoutedEventArgs e)
        {
            Slider1.Value = 10;
            Slider2.Value = 10;
            Slider3.Value = 5005;
        }

        private void Hard_Click(object sender, RoutedEventArgs e)
        {
            Slider1.Value = 20;
            Slider2.Value = 20;
            Slider3.Value = 10000;
        }

        private void Custom_Click(object sender, RoutedEventArgs e)
        {
            SlidersEnable();
        }

        private void SlidersDisable()
        {
            Slider1.IsEnabled = false;
            Slider2.IsEnabled = false;
            Slider3.IsEnabled = false;
        }

        private void SlidersEnable()
        {
            Slider1.IsEnabled = true;
            Slider2.IsEnabled = true;
            Slider3.IsEnabled = true;
        }
    }
}
