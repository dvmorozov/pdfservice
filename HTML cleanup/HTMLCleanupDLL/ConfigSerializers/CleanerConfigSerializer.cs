/*
 “Commons Clause” License Condition v1.0
The Software is provided to you by the Licensor under the License, as defined below, subject to the following condition.
Without limiting other conditions in the License, the grant of rights under the License will not include, and the License
does not grant to you, right to Sell the Software. For purposes of the foregoing, “Sell” means practicing any or all of
the rights granted to you under the License to provide to third parties, for a fee or other consideration (including
without limitation fees for hosting or consulting/ support services related to the Software), a product or service whose
value derives, entirely or substantially, from the functionality of the Software.  Any license notice or attribution
required by the License must also include this Commons Cause License Condition notice.

Software: HTMLCleanupDLL

License: 
Copyright 2020 Dmitry Morozov

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Licensor: Dmitry Morozov
 */
using System.IO;
using System.Xml.Serialization;
using HtmlCleanup.Config;

namespace HtmlCleanup
{
    /// <summary>
    /// Reads configuration from XML-file and creates objects.
    /// Decouples serialization/deserialization from definitions
    /// of data classes automatically generated from XSD-description.
    /// </summary>
    public abstract class CleanerConfigSerializer : ICleanerConfigSerializer
    {
        /// <summary>
        /// Reads configuration objects from file.
        /// </summary>
        /// <param name="fileName">Configuration file name.</param>
        /// <param name="chain">The first member of processing chain.</param>
        public void Deserialize(string fileName, BaseHtmlCleaner.TextProcessor chain)
        {
            //  Reads settings from file.
            HTMLCleanupConfig config = new HTMLCleanupConfig();
            using (StreamReader reader = new StreamReader(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HTMLCleanupConfig));
                config = (HTMLCleanupConfig)serializer.Deserialize(reader);
            }
            //  Updates objects in the chain.
            while (chain != null)
            {
                if (chain.GetType() == typeof(BaseHtmlCleaner.ParagraphExtractor))
                    ((BaseHtmlCleaner.ParagraphExtractor)chain).LoadSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.SpecialHtmlRemover))
                    ((BaseHtmlCleaner.SpecialHtmlRemover)chain).LoadSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.InnerTextProcessor))
                    ((BaseHtmlCleaner.InnerTextProcessor)chain).LoadSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.TagRemover))
                    ((BaseHtmlCleaner.TagRemover)chain).LoadSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.UrlFormatter))
                    ((BaseHtmlCleaner.UrlFormatter)chain).LoadSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.TextFormatter))
                    ((BaseHtmlCleaner.TextFormatter)chain).LoadSettings(config);

                chain = chain.Next;
            }
        }

        /// <summary>
        /// Scans processing chain and copies object data into file.
        /// </summary>
        /// <param name="fileName">Configuration file name.</param>
        /// <param name="chain">The first member of processing chain.</param>
        public void Serialize(string fileName, BaseHtmlCleaner.TextProcessor chain)
        {
            HTMLCleanupConfig config = new HTMLCleanupConfig();
            //  Fills the settings container.
            while (chain != null)
            {
                if (chain.GetType() == typeof(BaseHtmlCleaner.ParagraphExtractor))
                    ((BaseHtmlCleaner.ParagraphExtractor)chain).SaveSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.SpecialHtmlRemover))
                    ((BaseHtmlCleaner.SpecialHtmlRemover)chain).SaveSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.InnerTextProcessor))
                    ((BaseHtmlCleaner.InnerTextProcessor)chain).SaveSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.TagRemover))
                    ((BaseHtmlCleaner.TagRemover)chain).SaveSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.UrlFormatter))
                    ((BaseHtmlCleaner.UrlFormatter)chain).SaveSettings(config);

                if (chain.GetType() == typeof(BaseHtmlCleaner.TextFormatter))
                    ((BaseHtmlCleaner.TextFormatter)chain).SaveSettings(config);

                chain = chain.Next;
            }
            //  Writes data to file.
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HTMLCleanupConfig));
                serializer.Serialize(writer, config);
            }
        }

        public abstract string GetConfigurationFilePath();
    }
}