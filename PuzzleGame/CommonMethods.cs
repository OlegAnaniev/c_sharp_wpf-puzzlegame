using System.Windows.Controls;

namespace ExamPuzzle
{
    class CommonMethods
    {
        public static void GridClear(Grid grid)
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
        }

        public static void GridAddRows(Grid grid, int count)
        {
            for (int i = 0; i < count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
        }

        public static void GridAddColumns(Grid grid, int count)
        {
            for (int i = 0; i < count; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
    }
}
