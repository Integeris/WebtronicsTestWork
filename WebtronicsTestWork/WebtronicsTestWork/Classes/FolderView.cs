using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace WebtronicsTestWork.Classes
{
    /// <summary>
    /// Представление папки.
    /// </summary>
    public class FolderView : INotifyPropertyChanged
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
        /// Отмена процесса загрузки.
        /// </summary>
        private bool killLoadTask = false;

        /// <summary>
        /// Реализация события обновления при изменении.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
            set
            {
                size = value;
                InvokeNotify(nameof(Size));
            }
        }

        /// <summary>
        /// Количество файлов.
        /// </summary>
        public int Count
        {
            get => count;
            set
            {
                count = value;
                InvokeNotify(nameof(Count));
            }
        }

        /// <summary>
        /// Создание представления папки.
        /// </summary>
        /// <param name="directoryInfo">Информация о каталоге.</param>
        public FolderView(DirectoryInfo directoryInfo)
        {
            Name = directoryInfo.Name;
            Task.Run(() => GetInfo(directoryInfo));
        }

        /// <summary>
        /// Завершает процесс загрузки данных о папке.
        /// </summary>
        public void KillLoadTask()
        {
            killLoadTask = true;
        }

        /// <summary>
        /// Получение информации о папке.
        /// </summary>
        /// <param name="directoryInfo">Папка, о которой следует получить информацию.</param>
        /// <param name="count">Количество фалов.</param>
        /// <param name="size">Размер папки.</param>
        private void GetInfo(in DirectoryInfo directoryInfo)
        {
            if (killLoadTask)
            {
                return;
            }

            try
            {
                FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
                Count += files.Length;

                foreach (FileInfo filePath in files)
                {
                    Size += filePath.Length;
                }

                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                {
                    GetInfo(directory);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Вызов уведомления обновления.
        /// </summary>
        /// <param name="parametrName">Название параметра.</param>
        private void InvokeNotify(string parametrName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(parametrName));
        }
    }
}
