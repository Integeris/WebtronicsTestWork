using System.Windows;
using System.Windows.Controls;
using WebtronicsTestWork.Classes;

namespace WebtronicsTestWork.Pages
{
    /// <summary>
    /// Логика взаимодействия для FileInfoPage.xaml
    /// </summary>
    public partial class FileInfoPage : Page
    {
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
        /// Создание страницы информации о файле.
        /// </summary>
        /// <param name="fileView">Информация о файле.</param>
        public FileInfoPage(FileView fileView)
        {
            InitializeComponent();

            this.DataContext = fileView;
        }

        private void ClosePanelButtonOnClick(object sender, RoutedEventArgs e)
        {
            ClosePanel?.Invoke(this);
        }
    }
}
