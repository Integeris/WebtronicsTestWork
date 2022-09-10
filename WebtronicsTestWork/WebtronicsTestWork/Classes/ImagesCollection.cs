using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace WebtronicsTestWork.Classes
{
    /// <summary>
    /// Колекция иконок.
    /// </summary>
    public static class ImagesCollection
    {
        /// <summary>
        /// Иконка папки.
        /// </summary>
        public static BitmapImage FolderSource { get; } = GetBitmapImage(Properties.Resources.folder);

        /// <summary>
        /// Иконкв файла.
        /// </summary>
        public static BitmapImage FileSource { get; } = GetBitmapImage(Properties.Resources.document);

        /// <summary>
        /// Получение иконки из ресурсов.
        /// </summary>
        /// <param name="image">Картинка в ресурсах.</param>
        /// <returns>Иконка из ресурсов.</returns>
        private static BitmapImage GetBitmapImage(Bitmap image)
        {
            BitmapImage bitmapImage = new BitmapImage();
            
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
    }
}
