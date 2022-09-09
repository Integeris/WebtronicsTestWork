using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace WebtronicsTestWork.Classes
{
    /// <summary>
    /// Менеджер файловой системы.
    /// </summary>
    public class PathManager
    {
        /// <summary>
        /// Открытый путь.
        /// </summary>
        public string Path { get; set; } = String.Empty;

        /// <summary>
        /// Создание менеджера файловой системы.
        /// </summary>
        public PathManager() { }

        /// <summary>
        /// Получение списка папок.
        /// </summary>
        /// <returns>Список папок.</returns>
        public List<ObjectView> GetFolders()
        {
            List<ObjectView> folders = new List<ObjectView>();
            DirectoryInfo directoryInfo = new DirectoryInfo(Path);

            foreach (DirectoryInfo item in directoryInfo.GetDirectories())
            {
                ObjectView folder = new ObjectView(item.FullName, Enums.ObjectType.Folder);
                folders.Add(folder);
            }

            return folders;
        }

        /// <summary>
        /// Получение списка фалов.
        /// </summary>
        /// <returns>Список файлов.</returns>
        public List<ObjectView> GetFiles()
        {
            List<ObjectView> files = new List<ObjectView>();
            DirectoryInfo directoryInfo = new DirectoryInfo(Path);

            foreach (FileInfo item in directoryInfo.GetFiles())
            {
                ObjectView file = new ObjectView(item.FullName, Enums.ObjectType.File);
                files.Add(file);
            }

            return files;
        }

        /// <summary>
        /// Получение всех элементов в папке.
        /// </summary>
        /// <returns>Список элементов в папке.</returns>
        public List<ObjectView> GetObjects()
        {
            List<ObjectView> objectViews = GetFolders();
            objectViews.AddRange(GetFiles());
            return objectViews;
        }

        /// <summary>
        /// Получение списка дисков.
        /// </summary>
        /// <returns>Список дисков.</returns>
        public List<ObjectView> GetDrives()
        {
            List<ObjectView> drives = new List<ObjectView>();

            foreach (string item in Environment.GetLogicalDrives())
            {
                drives.Add(new ObjectView(item, Enums.ObjectType.Folder));
            }

            return drives;
        }

        /// <summary>
        /// Получение FileInfo указанного файла.
        /// </summary>
        /// <param name="fullName">Полное название файла.</param>
        /// <returns>FileInfo указанного файла.</returns>
        public FileInfo GetFileInfo(string fullName)
        {
            FileInfo fileInfo = new FileInfo(fullName);

            if (!fileInfo.Exists)
            {
                throw new Exception("Указанного файла не существует.");
            }

            return fileInfo;
        }

        /// <summary>
        /// Получение DirectoryInfo указанной папки.
        /// </summary>
        /// <param name="fullName">Полное название папки.</param>
        /// <returns>DirectoryInfo указанной папки.</returns>
        public DirectoryInfo GetDirectoryInfo(string fullName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(fullName);

            if (!directoryInfo.Exists)
            {
                throw new Exception("Указанной папки не существует.");
            }

            return directoryInfo;
        }

        /// <summary>
        /// Открыть папку.
        /// </summary>
        /// <param name="fullName">Полное название папки.</param>
        public void OpenFolder(string fullName)
        {
            if (Directory.Exists(fullName))
            {
                Path = fullName;
                return;
            }

            throw new Exception($"Открываемой папки({fullName}) не существует.");
        }

        /// <summary>
        /// Открыть файл.
        /// </summary>
        /// <param name="fullName">Полное название файла.</param>
        public void OpenFile(string fullName)
        {
            if (File.Exists(fullName))
            {
                Process.Start(fullName);
                return;
            }

            throw new Exception($"Открываемого файла({fullName}) не существует.");
        }

        /// <summary>
        /// Вернуться назад.
        /// </summary>
        public void GoBack()
        {
            string newPath = System.IO.Path.GetDirectoryName(Path);
            Path = newPath ?? throw new Exception("Невозможно перейти в каталог.");
        }

        /// <summary>
        /// Получить резульат поиска по названию.
        /// </summary>
        /// <param name="template">Название объекта.</param>
        /// <returns>Список найденныйх объектов.</returns>
        public List<ObjectView> SearchObjects(string template)
        {
            List<ObjectView> objectViews = new List<ObjectView>();
            DirectoryInfo directoryInfo = new DirectoryInfo(Path);

            foreach (DirectoryInfo item in directoryInfo.GetDirectories(template, SearchOption.AllDirectories))
            {
                ObjectView folder = new ObjectView(item.FullName, Enums.ObjectType.Folder);
                objectViews.Add(folder);
            }

            foreach (FileInfo item in directoryInfo.GetFiles(template, SearchOption.AllDirectories))
            {
                ObjectView file = new ObjectView(item.FullName, Enums.ObjectType.File);
                objectViews.Add(file);
            }

            return objectViews;
        }
    }
}
