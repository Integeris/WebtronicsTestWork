using System;

namespace WebtronicsTestWork.Model.Entities
{
    /// <summary>
    /// Открытый файл.
    /// </summary>
    public class OpenedFile
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int IdOpenedFile { get; set; }

        /// <summary>
        /// Название.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Дата открытия.
        /// </summary>
        public DateTime DateVisited { get; set; }
    }
}
