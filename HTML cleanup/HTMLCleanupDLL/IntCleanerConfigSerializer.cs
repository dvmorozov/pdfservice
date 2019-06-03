﻿namespace HtmlCleanup
{
    /// <summary>
    /// Defines operations with HTML-cleaner configuration.
    /// </summary>
    public interface ICleanerConfigSerializer
    {
        /// <summary>
        /// Reads configuration objects from file.
        /// </summary>
        /// <param name="fileName">Configuration file name.</param>
        /// <param name="chain">The first member of processing chain.</param>
        void Deserialize(string fileName, BaseHtmlCleaner.TextProcessor chain);

        /// <summary>
        /// Scans processing chain and copies object data into file.
        /// </summary>
        /// <param name="fileName">Configuration file name.</param>
        /// <param name="chain">The first member of processing chain.</param>
        void Serialize(string fileName, BaseHtmlCleaner.TextProcessor chain);

        /// <summary>
        /// Returns location of folder containing configuration files for 
        /// cleaner implementations.
        /// </summary>
        /// <returns></returns>
        string GetConfigurationFilePath();
    }
}