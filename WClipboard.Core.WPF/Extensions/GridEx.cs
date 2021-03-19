using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WClipboard.Core.WPF.Extensions
{
    public enum OrderDirection
    {
        Row = 1,
        Column = 2,
        Auto = 3
    }

    public static class GridEx
    {
        public static readonly DependencyProperty AutoOrderDirectionProperty = DependencyProperty.RegisterAttached("AutoOrderDirection", typeof(OrderDirection), typeof(GridEx), new FrameworkPropertyMetadata(OnAutoOrderDirectionChanged));
        public static readonly DependencyProperty AutoExtendProperty = DependencyProperty.RegisterAttached("AutoExtend", typeof(bool), typeof(GridEx), new FrameworkPropertyMetadata(false, OnAutoExtendChanged));

        public static void SetAutoOrderDirection(Grid target, OrderDirection value)
        {
            target.SetValue(AutoOrderDirectionProperty, value);
        }

        public static OrderDirection GetAutoOrderDirection(Grid target)
        {
            return (OrderDirection)target.GetValue(AutoOrderDirectionProperty);
        }

        public static void SetAutoExtend(Grid target, bool value)
        {
            target.SetValue(AutoExtendProperty, value);
        }

        public static bool GetAutoExtend(Grid target)
        {
            return (bool)target.GetValue(AutoExtendProperty);
        }

        private static void OnAutoOrderDirectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is int x && x != 0) return;

            if(obj is Grid grid)
            {
                if (grid.IsLoaded)
                {
                    Update(grid);
                }
                else
                {
                    grid.Loaded += Grid_Loaded;
                }
            }
        }

        private static void Update(Grid grid)
        {
            var direction = GetOrderDirection(grid);
            if (direction == OrderDirection.Row)
            {
                int r = SetRowDirection(grid);
                if (GetAutoExtend(grid))
                {
                    ExtendRowDefitionitions(grid, r);
                }
            }
            else if (direction == OrderDirection.Column)
            {
                int c = SetColumnDirection(grid);
                if (GetAutoExtend(grid))
                {
                    ExtendColumnDefitionitions(grid, c);
                }
            }
        }

        private static void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if(sender is Grid grid)
            {
                grid.Loaded -= Grid_Loaded;
                Update(grid);
            }
        }

        private static void OnAutoExtendChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue is bool x && x)
                throw new InvalidOperationException($"The {nameof(GridEx)}.{nameof(AutoExtendProperty)} can only be set once");

            if (e.NewValue is bool y && !y)
                return;

            if (obj is Grid grid && grid.IsLoaded && grid.ReadLocalValue(AutoOrderDirectionProperty) != DependencyProperty.UnsetValue)
            {
                var direction = GetAutoOrderDirection(grid);
                if (direction == OrderDirection.Row)
                {
                    ExtendRowDefitionitions(grid, grid.Children.Cast<UIElement>().Max(c => Grid.GetRow(c)));
                }
                else if (direction == OrderDirection.Column)
                {
                    ExtendColumnDefitionitions(grid, grid.Children.Cast<UIElement>().Max(c => Grid.GetColumn(c)));
                }
            }
        }

        private static OrderDirection GetOrderDirection(Grid grid)
        {
            var currentType = GetAutoOrderDirection(grid);
            if(currentType == OrderDirection.Auto)
            {
                if(grid.ColumnDefinitions.Count > 0 && grid.RowDefinitions.Count == 0)
                {
                    SetAutoOrderDirection(grid, OrderDirection.Row);
                    return OrderDirection.Row;
                } 
                else if(grid.ColumnDefinitions.Count == 0 && grid.RowDefinitions.Count > 0)
                {
                    SetAutoOrderDirection(grid, OrderDirection.Column);
                    return OrderDirection.Column;
                }
                else
                {
                    throw new InvalidOperationException($"In order to make {nameof(GridEx)}.{nameof(AutoOrderDirectionProperty)} work you must set the {nameof(Grid.ColumnDefinitions)} or {nameof(Grid.RowDefinitions)} not both. Or chose a differtent {nameof(OrderDirection)}");
                }
            } 
            else
            {
                return currentType;
            }
        }

        private static int SetRowDirection(Grid grid)
        {
            var columnCount = grid.ColumnDefinitions.Count;

            int r = 0;
            int c = 0;

            foreach(var child in grid.Children.Cast<UIElement>())
            {
                if(child.ReadLocalValue(Grid.ColumnProperty) == DependencyProperty.UnsetValue)
                {
                    Grid.SetColumn(child, c);
                }
                else
                {
                    c = Grid.GetColumn(child);
                }

                if (child.ReadLocalValue(Grid.RowProperty) == DependencyProperty.UnsetValue)
                {
                    Grid.SetRow(child, r);
                }
                else
                {
                    r = Grid.GetRow(child);
                }

                c += Grid.GetColumnSpan(child);

                if(c >= columnCount)
                {
                    c %= columnCount;
                    r++;
                }

                if(Grid.GetRowSpan(child) != 1)
                {
                    throw new InvalidOperationException($"If you use {nameof(GridEx)}.{nameof(AutoOrderDirectionProperty)} with {nameof(OrderDirection)}.{nameof(OrderDirection.Row)} then {nameof(Grid)}.{nameof(Grid.RowSpanProperty)} most not been set");
                }
            }

            return r - (c == 0 ? 1 : 0);
        }

        private static int SetColumnDirection(Grid grid)
        {
            var rowCount = grid.RowDefinitions.Count;

            int r = 0;
            int c = 0;

            foreach (var child in grid.Children.Cast<UIElement>())
            {
                if (child.ReadLocalValue(Grid.ColumnProperty) == DependencyProperty.UnsetValue)
                {
                    Grid.SetColumn(child, c);
                }
                else
                {
                    c = Grid.GetColumn(child);
                }

                if (child.ReadLocalValue(Grid.RowProperty) == DependencyProperty.UnsetValue)
                {
                    Grid.SetRow(child, r);
                }
                else
                {
                    r = Grid.GetRow(child);
                }

                r += Grid.GetRowSpan(child);

                if (r >= rowCount)
                {
                    r %= rowCount;
                    c++;
                }

                if (Grid.GetColumnSpan(child) != 1)
                {
                    throw new InvalidOperationException($"If you use {nameof(GridEx)}.{nameof(AutoOrderDirectionProperty)} with {nameof(OrderDirection)}.{nameof(OrderDirection.Column)} then {nameof(Grid)}.{nameof(Grid.ColumnSpanProperty)} most not been set");
                }
            }

            return c - (r == 0 ? 1 : 0);
        }

        private static void ExtendRowDefitionitions(Grid grid, int rowCount)
        {
            for(int r = grid.RowDefinitions.Count; r < rowCount; r++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
        }

        private static void ExtendColumnDefitionitions(Grid grid, int columnCount)
        {
            for (int c = grid.ColumnDefinitions.Count; c < columnCount; c++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
    }
}
