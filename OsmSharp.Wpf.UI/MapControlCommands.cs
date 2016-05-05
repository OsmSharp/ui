using System.Windows.Input;

namespace OsmSharp.Wpf.UI
{
    /// <summary>
    /// MapControl commands
    /// </summary>
    public static class MapControlCommands
    {
        public static RoutedUICommand ZoomIn = new RoutedUICommand("Приблизить", "ZoomIn", typeof(MapControl));
        public static RoutedUICommand ZoomOut = new RoutedUICommand("Отдалить", "ZoomOut", typeof(MapControl));

        public static RoutedUICommand ShowFullMap = new RoutedUICommand("Вся карта", "ShowFullMap", typeof(MapControl));
    }
}