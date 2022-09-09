using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WebtronicsTestWork.Classes;
using WebtronicsTestWork.Pages;

namespace WebtronicsTestWork.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Файловый менеджер.
        /// </summary>
        private readonly PathManager pathManager = new PathManager();

        /// <summary>
        /// Таймер для подсчёта времени на нажатие иконок.
        /// </summary>
        private readonly Timer timer = new Timer(300);

        /// <summary>
        /// Количество нажатий на иконку.
        /// </summary>
        private int clickCount;

        /// <summary>
        /// Данные о событии нажатия.
        /// </summary>
        private MouseButtonEventArgs clickEventArgs;

        /// <summary>
        /// Создание главного окна.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            timer.Elapsed += TimerOnElapsed;
            pathManager.OpenFolder("C:\\");
            UpdateDirectory();
        }

        private void BackButtonOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                pathManager.GoBack();
                UpdateDirectory();
            }
            catch (Exception)
            {
               LoadDrives();
            }
        }

        private void SearchTextBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (String.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    InfoViewer.ShowError("Введите шаблон для поиска.");
                }
                else
                {
                    PathTextBox.Text = String.Empty;
                    MainListView.ItemsSource = pathManager.SearchObjects(SearchTextBox.Text);
                }
            }
        }

        private void PathTextBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                pathManager.OpenFolder(PathTextBox.Text);
                UpdateDirectory();
            }
            catch (Exception)
            {
                InfoViewer.ShowError("Папка не найдена.");
                UpdateDirectory();
            }
        }

        private void ListViewItemOnClick()
        {
            ObjectView item = (ObjectView)((FrameworkElement)clickEventArgs.OriginalSource).DataContext;

            if (item != null)
            {
                try
                {
                    Page infoPage;

                    switch (item.ObjectType)
                    {
                        case Enums.ObjectType.File:
                            FileInfo fileInfo = pathManager.GetFileInfo(item.FullName);
                            FileView fileView = new FileView(fileInfo);
                            FileInfoPage fileInfoPage = new FileInfoPage(fileView);
                            fileInfoPage.ClosePanel += ClosePanel;
                            infoPage = fileInfoPage;
                            break;
                        default:
                            DirectoryInfo directoryInfo = pathManager.GetDirectoryInfo(item.FullName);
                            FolderView folderView = new FolderView(directoryInfo);
                            FolderInfoPage folderInfoPage = new FolderInfoPage(folderView);
                            folderInfoPage.ClosePanel += ClosePanel;
                            infoPage = folderInfoPage;
                            break;
                    }

                    InfoFrame.Content = infoPage;
                    InfoFrame.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    InfoViewer.ShowError(ex.Message);
                    UpdateDirectory();
                }
            }
        }

        private void ListViewItemOnDoubleClick()
        {
            ObjectView item = (ObjectView)((FrameworkElement)clickEventArgs.OriginalSource).DataContext;

            if (item != null)
            {
                try
                {
                    if (item.ObjectType == Enums.ObjectType.Folder)
                    {
                        pathManager.OpenFolder(item.FullName);
                        UpdateDirectory();
                    }
                    else
                    {
                        pathManager.OpenFile(item.FullName);
                    }
                }
                catch (Exception ex)
                {
                    InfoViewer.ShowError(ex.Message);
                    UpdateDirectory();
                }
            }
        }

        private void ListViewItemOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            clickCount++;
            timer.Start();

            clickEventArgs = e;
        }
        
        /// <summary>
        /// Загрузка дисков.
        /// </summary>
        private void LoadDrives()
        {
            PathTextBox.Text = String.Empty;
            MainListView.ItemsSource = pathManager.GetDrives();
            InfoFrame.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Обработка нажатий по истиченю времени.
        /// </summary>
        /// <param name="sender">Таймер.</param>
        /// <param name="e">Данные о прошедшем времени.</param>
        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            if (clickCount == 1)
            {
                this.Dispatcher.Invoke(ListViewItemOnClick);
            }
            else
            {
                this.Dispatcher.Invoke(ListViewItemOnDoubleClick);
            }

            clickCount = 0;
        }

        /// <summary>
        /// Обновление открытой папки.
        /// </summary>
        private void UpdateDirectory()
        {
            try
            {
                PathTextBox.Text = pathManager.Path;
                MainListView.ItemsSource = pathManager.GetObjects();
                InfoFrame.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                LoadDrives();
            }
        }

        /// <summary>
        /// Закрытие панели свойств.
        /// </summary>
        /// <param name="sender">Элемент, вызвавший метод.</param>
        private void ClosePanel(object sender)
        {
            InfoFrame.Visibility = Visibility.Collapsed;
        }
    }
}
