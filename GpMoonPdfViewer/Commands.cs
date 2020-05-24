using System.Windows.Input;

namespace GpMoonPdfViewer
{
    public static class RoutedCommands
    {
        public static readonly RoutedCommand PdfViewerNext = new RoutedCommand();
        public static readonly RoutedCommand PdfViewerBack = new RoutedCommand();
        public static readonly RoutedCommand PdfViewerOpen = new RoutedCommand();
        public static readonly RoutedCommand PdfViewerSave = new RoutedCommand();
        public static readonly RoutedCommand PdfViewerPrint = new RoutedCommand();
        public static readonly RoutedCommand UniformGridPdfPrint = new RoutedCommand();
    }
}
