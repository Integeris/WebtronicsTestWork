using System;
using System.IO;

namespace WebtronicsTestWork.Classes
{
    /// <summary>
    /// Представление файла.
    /// </summary>
    public class FileView
    {
        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Размер.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Создание представления файла.
        /// </summary>
        /// <param name="fileInfo">Информация о файле.</param>
        public FileView(FileInfo fileInfo)
        {
            Name = fileInfo.Name;
            Size = fileInfo.Length;
            CreationTime = fileInfo.CreationTime;
        }
    }
}
