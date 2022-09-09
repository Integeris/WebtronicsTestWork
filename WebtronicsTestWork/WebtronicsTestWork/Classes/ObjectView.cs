using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using WebtronicsTestWork.Enums;

namespace WebtronicsTestWork.Classes
{
    /// <summary>
    /// Элемент фаловой системы.
    /// </summary>
    public class ObjectView
    {
        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Полное название.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Тип объекта.
        /// </summary>
        public ObjectType ObjectType { get; set; }

        /// <summary>
        /// Изображение.
        /// </summary>
        public BitmapImage Source
        {
            get
            {
                Bitmap image;
                BitmapImage bitmapImage = new BitmapImage();

                switch (ObjectType)
                {
                    case ObjectType.File:
                        image = Properties.Resources.document;
                        break;
                    default:
                        image = Properties.Resources.folder;
                        break;
                }

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

        /// <summary>
        /// Создание элемента файловой системы.
        /// </summary>
        /// <param name="fullName">Поное название элемента.</param>
        /// <param name="objectType">Тип элемента.</param>
        public ObjectView(string fullName, ObjectType objectType)
        {
            FullName = fullName;
            ObjectType = objectType;

            switch (objectType)
            {
                case ObjectType.File:
                    Name = Path.GetFileName(FullName);
                    break;
                default:
                    Name = new DirectoryInfo(FullName).Name;
                    break;
            }
        }
    }
}
