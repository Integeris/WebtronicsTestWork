using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
        /// Количество нажатий на иконку.
        /// </summary>
        private int clickCount;

        /// <summary>
        /// Происходит ли поиск.
        /// </summary>
        private bool isSearching = false;

        /// <summary>
        /// Файловый менеджер.
        /// </summary>
        private readonly PathManager pathManager = new PathManager();

        /// <summary>
        /// Таймер для подсчёта времени на нажатие иконок.
        /// </summary>
        private readonly Timer timer = new Timer(300);

        /// <summary>
        /// Нажатый элемент.
        /// </summary>
        private ObjectView clickedItem;

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

            pathManager.SearchComplite += PathManagerOnSearchComplite;
            timer.Elapsed += TimerOnElapsed;
            pathManager.OpenFolder("C:\\");
            UpdateDirectory();
        }

        private void BackButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (!isSearching)
            {
                try
                {
                    pathManager.StopSearch();
                    pathManager.GoBack();
                }
                catch (Exception ex)
                {
                    InfoViewer.ShowError(ex.Message);
                }
            }
            else
            {
                isSearching = false;
            }

            UpdateDirectory();
        }

        private void SearchTextBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                pathManager.StopSearch();

                if (String.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    PathTextBox.Text = pathManager.Path;
                    UpdateDirectory();
                }
                else
                {
                    try
                    {
                        MainListView.ItemsSource = pathManager.SearchObjects(SearchTextBox.Text);
                        isSearching = true;
                        PathTextBox.Text = "Поиск...";
                    }
                    catch (Exception ex)
                    {
                        InfoViewer.ShowError(ex.Message);
                    }
                }
            }
        }

        private void PathTextBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                pathManager.StopSearch();

                try
                {
                    pathManager.OpenFolder(PathTextBox.Text);
                    isSearching = false;
                    UpdateDirectory();
                }
                catch (Exception ex)
                {
                    InfoViewer.ShowError(ex.Message);
                    PathTextBox.Text = pathManager.Path;
                }
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
                        pathManager.StopSearch();
                        pathManager.OpenFolder(item.FullName);
                        isSearching = false;
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

            ObjectView item = (ObjectView)((FrameworkElement)e.OriginalSource).DataContext;

            if (item != null)
            {
                if (item != clickedItem)
                {
                    clickCount = 0;
                }

                clickCount++;
                clickEventArgs = e;
                clickedItem = item;

                timer.Start();
            }
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
            catch (UnauthorizedAccessException ex)
            {
                InfoViewer.ShowError(ex.Message);
                pathManager.GoBack();
                UpdateDirectory();
            }
            catch (Exception ex)
            {
                InfoViewer.ShowError(ex.Message);
                pathManager.OpenFolder(String.Empty);
                UpdateDirectory();
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

        /// <summary>
        /// Обработка завершения поиска.
        /// </summary>
        /// <param name="searchIsStop">Остановлен ли поиск.</param>
        private void PathManagerOnSearchComplite(bool searchIsStop)
        {
            if (!searchIsStop)
            {
                PathTextBox.Dispatcher.Invoke(() => PathTextBox.Text = "Поиск завершён.");
            }
        }
    }
}
