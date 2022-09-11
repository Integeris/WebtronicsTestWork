using MySqlConnector;
using System;
using System.Threading.Tasks;
using WebtronicsTestWork.Model.Entities;

namespace WebtronicsTestWork.Model.Classes
{
    /// <summary>
    /// Менеджер подключения к базе данных.
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// Построитель строки подключения.
        /// </summary>
        private static readonly MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder()
        {
            Server = Properties.Settings.Default.Server,
            Port = Properties.Settings.Default.Port,
            Database = Properties.Settings.Default.Database,
            UserID = Properties.Settings.Default.UserId,
            Password = Properties.Settings.Default.Password
        };

        /// <summary>
        /// Асинхронно занести информацию о открытом файле в базу данных.
        /// </summary>
        /// <param name="openedFile">Открытый файл.</param>
        public static void AddOpenedFileAsync(OpenedFile openedFile)
        {
            Task.Run(() => AddOpenedFile(openedFile));
        }

        /// <summary>
        /// Занести информацию о открытом файле в базу данных.
        /// </summary>
        /// <param name="openedFile">Открытый файл.</param>
        private static void AddOpenedFile(OpenedFile openedFile)
        {
            MySqlConnection connection = new MySqlConnection()
            {
                ConnectionString = builder.ConnectionString
            };

            MySqlCommand command = new MySqlCommand()
            {
                Connection = connection,
                CommandText = "INSERT INTO `OpenedFile`(Title, DateVisited)\r\nVALUES(@Title, @Date);"
            };

            MySqlParameter[] parameters = new MySqlParameter[2]
            {
                new MySqlParameter("@Title", MySqlDbType.VarChar) { Value = openedFile.Title },
                new MySqlParameter("@Date", MySqlDbType.DateTime) { Value = openedFile.DateVisited }
            };

            command.Parameters.AddRange(parameters);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception) { }
            finally
            {
                connection.Close();
            }
        }
    }
}
