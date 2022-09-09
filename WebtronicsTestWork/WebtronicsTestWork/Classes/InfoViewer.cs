using System.Windows;

namespace WebtronicsTestWork.Classes
{
    /// <summary>
    /// Менеджер сообщений.
    /// </summary>
    public static class InfoViewer
    {
        /// <summary>
        /// Вывести информацию.
        /// </summary>
        /// <param name="text">Текст сообщения.</param>
        public static void ShowInfo(string text)
        {
            MessageBox.Show(text, "Информация.", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Вывести ошибку.
        /// </summary>
        /// <param name="text">Текст сообщения.</param>
        public static void ShowError(string text)
        {
            MessageBox.Show(text, "Ошибка.", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
