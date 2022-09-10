using System.Windows;
using System.Windows.Controls;
using WebtronicsTestWork.Classes;

namespace WebtronicsTestWork.Pages
{
    /// <summary>
    /// Логика взаимодействия для FolderInfoPage.xaml
    /// </summary>
    public partial class FolderInfoPage : Page
    {
        /// <summary>
        /// Информация о папке.
        /// </summary>
        private readonly FolderView folderView;

        /// <summary>
        /// Сигнатура методов обработки закрытия панели.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        public delegate void ClosePanelHandle(object sender);

        /// <summary>
        /// Закрыте панели.
        /// </summary>
        public event ClosePanelHandle ClosePanel;

        /// <summary>
        /// Создание страницы информации о папке.
        /// </summary>
        /// <param name="folderView">Информация о папке.</param>
        public FolderInfoPage(FolderView folderView)
        {
            InitializeComponent();

            this.DataContext = folderView;
            this.folderView = folderView;
        }

        /// <summary>
        /// Уничтожение страницы.
        /// </summary>
        ~FolderInfoPage()
        {
            folderView.KillLoadTask();
        }

        private void ClosePanelButtonOnClick(object sender, RoutedEventArgs e)
        {
            folderView.KillLoadTask();
            ClosePanel?.Invoke(this);
        }
    }
}
