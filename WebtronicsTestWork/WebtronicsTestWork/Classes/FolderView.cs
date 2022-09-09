using System;
using System.IO;

namespace WebtronicsTestWork.Classes
{
    /// <summary>
    /// Представление папки.
    /// </summary>
    public class FolderView
    {
        /// <summary>
        /// Размер.
        /// </summary>
        private long size;

        /// <summary>
        /// Количество файлов.
        /// </summary>
        private int count;

        /// <summary>
        /// Названте.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Размер.
        /// </summary>
        public long Size
        {
            get => size;
            set => size = value;
        }

        /// <summary>
        /// Количество файлов.
        /// </summary>
        public int Count
        {
            get => count;
            set => count = value;
        }

        /// <summary>
        /// Создание представления папки.
        /// </summary>
        /// <param name="directoryInfo">Информация о каталоге.</param>
        public FolderView(DirectoryInfo directoryInfo)
        {
            Name = directoryInfo.Name;
            GetInfo(directoryInfo, ref count, ref size);
        }

        /// <summary>
        /// Получение информации о папке.
        /// </summary>
        /// <param name="directoryInfo">Папка, о которой следует получить информацию.</param>
        /// <param name="count">Количество фалов.</param>
        /// <param name="size">Размер папки.</param>
        private static void GetInfo(in DirectoryInfo directoryInfo, ref int count, ref long size)
        {
            try
            {
                FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
                count += files.Length;

                foreach (FileInfo filePath in files)
                {
                    size += filePath.Length;
                }

                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                {
                    GetInfo(directory, ref count, ref size);
                }
            }
            catch (Exception) { }
        }
    }
}
