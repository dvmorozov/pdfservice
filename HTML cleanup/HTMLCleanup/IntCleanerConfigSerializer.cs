namespace HTMLCleanup
{
    /// <summary>
    /// Defines operations with HTML-cleaner configuration.
    /// </summary>
    interface ICleanerConfigSerializer
    {
        /// <summary>
        /// Восстанавливает конфигурацию объектов по данным из файла.
        /// </summary>
        /// <param name="fileName">Имя файла для чтения конфигурации.</param>
        /// <param name="chain">Первый объект в цепочке обработчиков.</param>
        void Deserialize(string fileName, BaseHTMLCleaner.TextProcessor chain);

        /// <summary>
        /// Сканирует цепочку обработчиков и копирует их данные.
        /// Затем сериализует их в XML-файл.
        /// </summary>
        /// <param name="fileName">Имя файла для сохранения конфигурации.</param>
        /// <param name="chain">Первый объект в цепочке обработчиков.</param>
        void Serialize(string fileName, BaseHTMLCleaner.TextProcessor chain);
    }
}