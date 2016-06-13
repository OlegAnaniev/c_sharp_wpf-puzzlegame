using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Runtime.Serialization;

namespace ExamPuzzle
{
    [Serializable]
    class PuzzleGame
    {
        PuzzleGameSettings settings;
        [NonSerialized]
        Grid pictureGrid;
        string picturePath;
        [NonSerialized]
        BitmapImage currentImageBitmap;

        [NonSerialized]
        CroppedBitmap[,] imageParts;
        System.Drawing.Point[,] imagePartsPosition;
        int imagesInPositon;

        [NonSerialized]
        Image currentImage;        
        System.Drawing.Point emptyCell;

        public PuzzleGame(PuzzleGameSettings settings, Grid pictureGrid, string picturePath)
        {
            this.settings = settings;
            this.pictureGrid = pictureGrid;
            this.picturePath = picturePath;

            CurrentImageLoad();

            CurrentImageBitmapCrop();
            ImagePartPositionInit();
            CurrentImagePrepare();
            emptyCell = new System.Drawing.Point(settings.ColumnsCount - 1, settings.RowsCount - 1);
        }

        public void PuzzleGameLoaded(Grid pictureGrid)
        {
            this.pictureGrid = pictureGrid;
            FieldInitialize();
        }

        private void CurrentImageLoad()
        {
            currentImageBitmap = new BitmapImage();
            currentImageBitmap.BeginInit();
            currentImageBitmap.UriSource = new Uri(picturePath);
            currentImageBitmap.EndInit();
        }

        private void CurrentImageBitmapCrop()
        {
            imageParts = new CroppedBitmap[settings.ColumnsCount, settings.RowsCount];
            int SideX = (int)currentImageBitmap.PixelWidth / settings.ColumnsCount;
            int SideY = (int)currentImageBitmap.PixelHeight / settings.RowsCount;
            
            //imagePartsPosition = new System.Drawing.Point[settings.ColumnsCount, settings.RowsCount];
            //imagesInPositon = settings.ColumnsCount * settings.RowsCount - 1;
            
            for (int VertPos = 0; VertPos < settings.RowsCount; VertPos++)
            {
                for (int HorPos = 0; HorPos < settings.ColumnsCount; HorPos++)
                {
                    if (VertPos != settings.RowsCount - 1 || HorPos != settings.ColumnsCount - 1)
                    {
                        imageParts[HorPos, VertPos] = new CroppedBitmap(currentImageBitmap,
                        new Int32Rect(HorPos * SideX, VertPos * SideY, SideX, SideY));

                        //imagePartsPosition[HorPos, VertPos].X = HorPos;
                        //imagePartsPosition[HorPos, VertPos].Y = VertPos;
                    }
                    else
                    {
                        //imagePartsPosition[HorPos, VertPos].X = -1;
                        //imagePartsPosition[HorPos, VertPos].Y = -1;
                    }                    
                }
            }
        }

        private void ImagePartPositionInit()
        {
            imagePartsPosition = new System.Drawing.Point[settings.ColumnsCount, settings.RowsCount];
            imagesInPositon = settings.ColumnsCount * settings.RowsCount - 1;

            for (int VertPos = 0; VertPos < settings.RowsCount; VertPos++)
            {
                for (int HorPos = 0; HorPos < settings.ColumnsCount; HorPos++)
                {
                    if (imageParts[HorPos, VertPos] != null)
                    {
                        imagePartsPosition[HorPos, VertPos].X = HorPos;
                        imagePartsPosition[HorPos, VertPos].Y = VertPos;
                    }
                    else
                    {
                        imagePartsPosition[HorPos, VertPos].X = -1;
                        imagePartsPosition[HorPos, VertPos].Y = -1;
                    }
                }
            }
        }

        private void CurrentImagePrepare()
        {
            currentImage = new Image();
            currentImage.Stretch = Stretch.Fill;
            Grid.SetRow(currentImage, 0);
            Grid.SetColumn(currentImage, 0);
            Grid.SetRowSpan(currentImage, settings.RowsCount);
            Grid.SetColumnSpan(currentImage, settings.ColumnsCount);
            currentImage.Source = currentImageBitmap;
        }
        
        public void Start()
        {
            ImagePartsShuffle(settings.ShuffleRounds);
            FieldInitialize();
        }

        private void ImagePartsShuffle(int rounds)
        {
            Random random = new Random();
            System.Drawing.Point imagePartToMove = new System.Drawing.Point();

            for (int i = 0; i < rounds; i++)
            {
                imagePartToMove.X = emptyCell.X - (random.Next(-1, 2));
                imagePartToMove.Y = emptyCell.Y - (random.Next(-1, 2));

                if (imagePartToMove.X >= 0 && imagePartToMove.X < settings.ColumnsCount &&
                    imagePartToMove.Y >= 0 && imagePartToMove.Y < settings.RowsCount &&
                    imagePartToMove != emptyCell)
                {
                    //imageParts[emptyCell.X, emptyCell.Y] =
                    //    imageParts[imagePartToMove.X, imagePartToMove.Y];
                    //imageParts[imagePartToMove.X, imagePartToMove.Y] = null;
                    

                    //////////
                    ImagePartVerify(ref imagePartToMove);                    
                }
                else
                {
                    i--;
                }
            }
        }

        private void ImagePartVerify(ref System.Drawing.Point posFrom)
        {
            if (imagePartsPosition[posFrom.X, posFrom.Y].X == posFrom.X &&
                imagePartsPosition[posFrom.X, posFrom.Y].Y == posFrom.Y)
            {
                imagesInPositon--;
            }
            else if (imagePartsPosition[posFrom.X, posFrom.Y].X == emptyCell.X &&
                imagePartsPosition[posFrom.X, posFrom.Y].Y == emptyCell.Y)
            {
                imagesInPositon++;
            }

            imagePartsPosition[emptyCell.X, emptyCell.Y] =
                        imagePartsPosition[posFrom.X, posFrom.Y];
            imagePartsPosition[posFrom.X, posFrom.Y].X = -1;
            imagePartsPosition[posFrom.X, posFrom.Y].Y = -1;

            emptyCell = posFrom;
        }

        private void FieldInitialize()
        {
            CommonMethods.GridAddColumns(pictureGrid, settings.ColumnsCount);
            CommonMethods.GridAddRows(pictureGrid, settings.RowsCount);

            Button temp;

            for (int row = 0; row < pictureGrid.RowDefinitions.Count; row++)
            {
                for (int col = 0; col < pictureGrid.ColumnDefinitions.Count; col++)
                {
                    //if (imageParts[col, row] != null)
                    if (imagePartsPosition[col, row].X != -1 && imagePartsPosition[col, row].Y != -1)
                    {
                        temp = new Button();
                        temp.Background =
                            new ImageBrush(imageParts[imagePartsPosition[col, row].X,
                                imagePartsPosition[col, row].Y]);
                            //new ImageBrush(imageParts[col, row]);
                        temp.Click += ImagePartClick;
                        temp.Style = (Style) pictureGrid.FindResource("SimpleButtonStyle");

                        temp.Focusable = false;

                        Grid.SetColumn(temp, col);
                        Grid.SetRow(temp, row);
                        pictureGrid.Children.Add(temp);
                    }
                    else ///
                    {
                        emptyCell.X = col;
                        emptyCell.Y = row;
                    }           
                }
            }
        }
        
        private void ImagePartClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;            

            System.Drawing.Point posFrom = 
                new System.Drawing.Point(Grid.GetColumn(button), Grid.GetRow(button));
                        
            Grid.SetColumn(button, emptyCell.X);
            Grid.SetRow(button, emptyCell.Y);

            ImagePartVerify(ref posFrom);            

            if (imagesInPositon == settings.RowsCount * settings.ColumnsCount - 1)
            {
                Victory();
            }
        }

        private void Victory()
        {
            CommonMethods.GridClear(pictureGrid);
            Grid.SetRowSpan(currentImage, 1);
            Grid.SetColumnSpan(currentImage, 1);
            pictureGrid.Children.Add(currentImage);            
            MessageBox.Show("Victory!");
        }

        public void CurrentImageShow(object sender, KeyEventArgs e)
        {
            if (currentImage != null && e.Key == Key.Space)
            {
                if (pictureGrid.Children.Contains(currentImage))
                {
                    pictureGrid.Children.Remove(currentImage);
                }
                else
                {
                    pictureGrid.Children.Add(currentImage);
                }
            }
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            CurrentImageLoad();
            CurrentImageBitmapCrop();
            CurrentImagePrepare();            
        }
    }
}