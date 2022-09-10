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
                BitmapImage bitmapImage;

                switch (ObjectType)
                {
                    case ObjectType.File:
                        bitmapImage = ImagesCollection.FileSource;
                        break;
                    default:
                        bitmapImage = ImagesCollection.FolderSource;
                        break;
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
