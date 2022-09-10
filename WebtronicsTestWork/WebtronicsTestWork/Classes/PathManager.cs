using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using WebtronicsTestWork.Model.Classes;
using WebtronicsTestWork.Model.Entities;

namespace WebtronicsTestWork.Classes
{
    /// <summary>
    /// Менеджер файловой системы.
    /// </summary>
    public class PathManager
    {
        /// <summary>
        /// Запрещённые символы для поиска.
        /// </summary>
        private static readonly string forbiddenSymbols = @"/\:«<>|\";

        /// <summary>
        /// Замена символов поиска windows на символы поиска Regex.
        /// </summary>
        private static readonly Dictionary<string, string> replaceDictionary = new Dictionary<string, string>()
        {
            { "\\", "\\\\" },
            { ".", "\\." },
            { "?", "." },
            { "*", "\\w*" }
        };

        /// <summary>
        /// Остановлен ли поиск.
        /// </summary>
        private bool searchIsStop;

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
            if (String.IsNullOrWhiteSpace(Path))
            {
                return GetDrives();
            }

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

            if (String.IsNullOrWhiteSpace(Path))
            {
                return files;
            }

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
            fullName = fullName.Replace('/', '\\');

            if (Regex.IsMatch(fullName, @"^\w{1}:$"))
            {
                fullName += "\\";
            }
            else if (Regex.IsMatch(fullName, @"^\w{1}:\\{2,}$"))
            {
                fullName = Regex.Replace(fullName, @"\\+$", "\\");
            }
            else if (!Regex.IsMatch(fullName, @"^\w{1}:\\$") && fullName.EndsWith("\\"))
            {
                fullName = Regex.Replace(fullName, @"\\+$", "");
            }

            if (Directory.Exists(fullName))
            {
                Path = fullName;
            }
            else if (String.IsNullOrWhiteSpace(fullName))
            {
                Path = String.Empty;
            }
            else
            {
                throw new Exception($"Открываемой папки '{fullName}' не существует или том недоступен.");
            }
        }

        /// <summary>
        /// Открыть файл.
        /// </summary>
        /// <param name="fullName">Полное название файла.</param>
        public void OpenFile(string fullName)
        {
            if (File.Exists(fullName))
            {
                try
                {
                    Process.Start(fullName);

                    OpenedFile openedFile = new OpenedFile()
                    {
                        Title = System.IO.Path.GetFileName(fullName),
                        DateVisited = DateTime.Now
                    };

                    Core.AddOpenedFile(openedFile);
                    return;
                }
                catch (Exception ex)
                {
                    throw new Exception("Неудалось открыть файл.", ex);
                }

            }

            throw new Exception($"Открываемого файла({fullName}) не существует.");
        }

        /// <summary>
        /// Вернуться назад.
        /// </summary>
        public void GoBack()
        {
            try
            {
                string newPath = System.IO.Path.GetDirectoryName(Path);
                Path = newPath ?? String.Empty;
            }
            catch (Exception)
            {
                Path = String.Empty;
            }
        }

        /// <summary>
        /// Получить резульат поиска по названию.
        /// </summary>
        /// <param name="template">Название объекта.</param>
        /// <returns>Список найденныйх объектов.</returns>
        public ObservableCollection<ObjectView> SearchObjects(string template)
        {
            ObservableCollection<ObjectView> objectViews = new ObservableCollection<ObjectView>();

            if (template.Any(chr => forbiddenSymbols.Contains(chr)))
            {
                throw new Exception($"Шаблон не может содержать символы {forbiddenSymbols}");
            }

            searchIsStop = false;
            DirectoryInfo directoryInfo;
            IProgress<ObjectView> progress = new Progress<ObjectView>((view) =>
            {
                objectViews.Add(view);

                if (objectViews.Count >= 1000)
                {
                    searchIsStop = true;
                }
            });

            if (String.IsNullOrEmpty(Path))
            {
                foreach (ObjectView item in GetDrives())
                {
                    directoryInfo = new DirectoryInfo(item.FullName);
                    Task.Run(() => GetSearchObjects(directoryInfo, progress, template));
                }
            }
            else
            {
                directoryInfo = new DirectoryInfo(Path);
                Task.Run(() => GetSearchObjects(directoryInfo, progress, template));
            }

            return objectViews;
        }

        /// <summary>
        /// Остановка поиска.
        /// </summary>
        public void StopSearch()
        {
            searchIsStop = true;
        }

        /// <summary>
        /// Асинхронный поиск папок и файлов.
        /// </summary>
        /// <param name="directory">Папка для поиска.</param>
        /// <param name="progress">Поставщик обновлений списка.</param>
        /// <param name="template">Шаблон поиска.</param>
        /// <param name="regexPattern">Шаблон для поиска регулярными выражениями.</param>
        private void GetSearchObjects(DirectoryInfo directory, IProgress<ObjectView> progress, string template, string regexPattern = null)
        {
            if (searchIsStop)
            {
                return;
            }

            if (regexPattern == null)
            {
                regexPattern = template;

                foreach (var item in replaceDictionary)
                {
                    regexPattern = regexPattern.Replace(item.Key, item.Value);
                }
            }

            try
            {
                foreach (DirectoryInfo directryInfo in directory.GetDirectories())
                {
                    if (Regex.IsMatch(directryInfo.Name, regexPattern, RegexOptions.IgnoreCase))
                    {
                        ObjectView folder = new ObjectView(directryInfo.FullName, Enums.ObjectType.Folder);
                        progress.Report(folder);
                    }

                    GetSearchObjects(directryInfo, progress, template, regexPattern);
                }

                foreach (FileInfo item in directory.GetFiles(template))
                {
                    ObjectView file = new ObjectView(item.FullName, Enums.ObjectType.File);
                    progress.Report(file);
                }
            }
            catch (Exception) { }
        }
    }
}
