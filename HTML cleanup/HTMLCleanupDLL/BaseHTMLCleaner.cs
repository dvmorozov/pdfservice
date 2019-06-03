using HtmlCleanup.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlCleanup
{
    //  Must be public to be accessible from unit-tests.
    //  After any changes in nested class configuration
    //  regenerate configuration files.
    public abstract class BaseHtmlCleaner : IHtmlCleaner
    {
        public class HtmlTag
        {
            private string _startTag;
            private string _endTag;

            public HtmlTag(string startTag, string endTag)
            {
                _startTag = startTag;
                _endTag = endTag;
            }

            public string StartTag
            {
                get
                {
                    return _startTag;
                }
            }

            public string EndTag
            {
                get
                {
                    return _endTag;
                }
            }
        }

        public class HtmlElement
        {
            private string _text;
            private int _startPos = 0;
            private string _startTag;
            private string _endTag;
            private bool _found;
            private int _pos1;      //  Start tag position.
            private int _pos2;      //  Start tag closing bracket position.
            private int _pos3;      //  End tag position.

            public string Text
            {
                get
                {
                    return _text;
                }
            }

            private ITagFormatter _formatter;

            public HtmlElement(string startTag /*Should not include closing >.*/, string endTag, string text, ITagFormatter formatter)
            {
                _text = text;
                _startTag = startTag;
                _endTag = endTag;
                _formatter = formatter;
            }

            public static HtmlElement FindNext(List<Tag> tags, string text, ITagFormatter formatter)
            {
                var bracketPos = 0;
                while (true)
                {
                    bracketPos = text.IndexOf("<", bracketPos, StringComparison.OrdinalIgnoreCase);
                    if (bracketPos != -1)
                    {
                        var subString = text.Substring(bracketPos);
                        //  Compares with tag signatures.
                        for (var i = 0; i < tags.Count; i++)
                        {
                            var t = tags[i];
                            if (subString.StartsWith(t.StartTag))
                            {
                                //  Tag has been found in the list.
                                var htmlElement = new HtmlElement(t.StartTag, t.EndTag, text, formatter);
                                //  Properly initializes internal state.
                                htmlElement.FindNext();
                                return htmlElement;
                            }
                        }
                        bracketPos++;
                    }
                    else
                        break;
                }
                return null;
            }

            /// <summary>
            /// Searches for text tag of given type.
            /// </summary>
            public bool FindNext()
            {
                _found = false;
                _pos1 = _text.IndexOf(_startTag, _startPos, StringComparison.OrdinalIgnoreCase);
                if (_pos1 != -1)
                {
                    //  Start tag was found.
                    //  Skips attributes.
                    _pos2 = _text.IndexOf(">", _pos1 + _startTag.Length);

                    //  Empty closing tag is permitted.
                    if (_endTag != String.Empty)
                    {
                        _pos3 = _text.IndexOf(_endTag, _pos2 + 1, StringComparison.OrdinalIgnoreCase);
                        if (_pos3 != -1)
                        {
                            //  Calculates number of nested start tags.
                            if (_pos3 - _pos2 - 1 > 0) {
                                var subString = _text.Substring(_pos2 + 1, _pos3 - _pos2 - 1);
                                var startCount = 0;
                                var pos = 0;
                                while (true) {
                                    pos = subString.IndexOf(_startTag, pos, StringComparison.OrdinalIgnoreCase);
                                    if (pos == -1)
                                        break;
                                    else {
                                        pos++;
                                        startCount++;
                                    }
                                }

                                //  Move position to proper closing tag.
                                while(startCount != 0) {
                                    _pos3 = _text.IndexOf(_endTag, _pos3 + 1, StringComparison.OrdinalIgnoreCase);
                                    startCount--;
                                }
                            }

                            //  End tag was found.
                            _found = true;
                            //  Go to next tag.
                            _startPos = _pos3 + _endTag.Length;
                        }
                        else
                        {
                            //  Error (TODO: throw exception).
                            _found = false;
                        }
                    }
                    else
                    {
                        _pos3 = _pos2 + 1;
                        _found = true;
                        _startPos = _pos3;
                    }
                }
                return _found;
            }

            /// <summary>
            /// Returns tag internal text.
            /// </summary>
            /// <returns>Tag internal text.</returns>
            public string GetText()
            {
                if (_found)
                {
                    return _text.Substring(_pos2 + 1, _pos3 - _pos2 - 1);
                }
                return String.Empty;
            }

            public string GetAttr(string attrName)
            {
                if (_found)
                {
                    //  Searches for closing bracket.
                    var endBracketPos = _text.IndexOf(">", _pos1);
                    var attrPos = _text.IndexOf(attrName, _pos1, StringComparison.OrdinalIgnoreCase);
                    if (attrPos != -1 && attrPos < endBracketPos)
                    {
                        //  Copies tag text.
                        var tagCopy = _text.Substring(_pos1, endBracketPos - _pos1 + 1);
                        //  Replaces quotation marks by spaces.
                        tagCopy = tagCopy.Replace('"', ' ');
                        tagCopy = tagCopy.Replace('\'', ' ');
                        var attrValStartPos = tagCopy.IndexOf("=", attrPos - _pos1);
                        if (attrValStartPos != -1)
                        {
                            //  Skips all spaces.
                            do
                            {
                                attrValStartPos++;
                            }
                            while (tagCopy[attrValStartPos] == ' ');

                            var attrValEndPos = tagCopy.IndexOfAny(new char[] { ' ', '>' }, attrValStartPos + 1);
                            if (attrValEndPos != -1)
                            {
                                //  Copies attribute value.
                                return tagCopy.Substring(attrValStartPos, attrValEndPos - attrValStartPos).Trim();
                            }
                            else
                            {
                                //  Error. TODO: add error message to text.
                            }
                        }
                    }
                    else
                    {
                        //  Error. TODO: add error message to text.
                    }
                }
                return String.Empty;
            }

            /// <summary>
            /// Removes tag.
            /// </summary>
            /// <returns>Internal text.</returns>
            public string RemoveTag()
            {
                var len1 = _pos2 - _pos1 + 1;
                //  Removes start tag.
                _text = _text.Remove(_pos1, len1);
                _pos3 -= len1;

                //  Removes end tag.
                _text = _text.Remove(_pos3, _endTag.Length);

                var innerText = _text.Substring(_pos1, _pos3 - _pos1);

                //  Removes inner text.
                _text = _text.Remove(_pos1, _pos3 - _pos1);

                return innerText;
            }

            /// <summary>
            /// FinalizeTagFormatting must be called.
            /// </summary>
            private bool _callFinalizeFormatting;

            /// <summary>
            /// Initialize state machine of tag formatting.
            /// It is necessary in processing of lists and should be stored at element level.
            /// </summary>
            /// <returns>Formatted text.</returns>
            public string InitializeTagFormatting(string text)
            {
                return _formatter.InitializeTagFormatting(new Tag(_startTag, _endTag), text, out _callFinalizeFormatting);
            }

            /// <summary>
            /// Finalize state machine of tag formatting.
            /// It is necessary in processing of lists and should be stored at element level.
            /// </summary>
            public void FinalizeTagFormatting(string finalText)
            {
                if (_callFinalizeFormatting)
                {
                    _formatter.FinalizeTagFormatting(finalText);
                    _callFinalizeFormatting = false;
                }
            }

            /// <summary>
            /// Insert text in current position.
            /// </summary>
            /// <param name="innerText">Text to insert.</param>
            public void InsertText(string innerText)
            {
                _text = _text.Insert(_pos1, innerText);
                //  Corrects end position.
                _pos3 = _pos1 + innerText.Length;
                //  Tags can be nested, proceed from the same position.
                _startPos = _pos1;
            }

            /// <summary>
            /// Replaces content with formatted text.
            /// </summary>
            public void ReplaceContent()
            {
                if (_found)
                {
                    //  Removes tag and its original content from text.
                    var innerText = RemoveTag();
                    //  Formats text.
                    innerText = InitializeTagFormatting(innerText);
                    //  Finializes immediately.
                    FinalizeTagFormatting(innerText);
                    //  Inserts formatted text instead of original content.
                    InsertText(innerText);
                    //  Blocks repeated execution.
                    _found = false;
                }
            }

            /// <summary>
            /// Removes content completely.
            /// </summary>
            public void RemoveContent()
            {
                if (_found)
                {
                    var len1 = _pos3 + _endTag.Length - _pos1;
                    _text = _text.Remove(_pos1, len1);
                    _pos2 = _pos1;
                    _pos3 = _pos1;
                    _startPos = _pos1;

                    //  Blocks repeated execution.
                    _found = false;
                }
            }

            /// <summary>
            /// Replaces tags together with internal text by given text.
            /// </summary>
            public void ReplaceTagsWithText(string text)
            {
                if (_found)
                {
                    RemoveContent();
                    _text = _text.Insert(_startPos, text);
                    //  Skips inserted text.
                    _startPos += text.Length;
                }
            }
        }

        protected ICleanerConfigSerializer _configSerializer;

        public BaseHtmlCleaner(ICleanerConfigSerializer configSerializer)
        {
            _configSerializer = configSerializer;
        }

        /// <summary>
        /// Base class of text processors.
        /// </summary>
        public abstract class TextProcessor
        {
            private readonly TextProcessor _next;
            /// <summary>
            /// Controls if object of inherited class is actually used in the processing chain.
            /// </summary>
            private bool _skipped;

            /// <summary>
            /// Returns next processing object.
            /// </summary>
            public TextProcessor Next
            {
                get
                {
                    return _next;
                }
            }

            public bool Skipped
            {
                get
                {
                    return _skipped;
                }
                set
                {
                    _skipped = value;
                }
            }

            protected ITagFormatter _formatter;

            public TextProcessor(TextProcessor next, ITagFormatter formatter)
            {
                _next = next;
                _formatter = formatter;
            }

            /// <summary>
            /// Does actual text processing. Should be implemented in 
            /// descendant classes.
            /// </summary>
            /// <param name="original">HTML text partially processed at previous stages.</param>
            /// <returns>Processed HTML text.</returns>
            public abstract string DoProcessing(string original);

            /// <summary>
            /// Processes text and executes next text processor in the chain.
            /// </summary>
            /// <param name="original">Original text.</param>
            /// <returns>Processed text.</returns>
            public string Process(string original)
            {
                //  Does processing, if enabled and then calls
                //  next processing algorithm in the chain.
                var processed = original;
                if (!Skipped)
                    processed = DoProcessing(original);

                if (_next != null) return _next.Process(processed);
                else return processed;
            }

            public abstract void LoadSettings(HTMLCleanupConfig config);
            public abstract void SaveSettings(HTMLCleanupConfig config);
        }

        /// <summary>
        /// Creates and initializes domain-specific instance of ParagraphExtractor.
        /// </summary>
        /// <param name="next">Next processing object in the chain.</param>
        /// <returns>Instance of ParagraphExtractor specific for the domain supported
        /// by inherited class.</returns>
        protected abstract ParagraphExtractor GetParagraphExtractor(TextProcessor next, ITagFormatter formatter);

        /// <summary>
        /// Extracts text paragraph.
        /// </summary>
        public class ParagraphExtractor : TextProcessor
        {
            /// <summary>
            /// Paragraph tag.
            /// </summary>
            private HtmlTag _tag;

            public HtmlTag Tag
            {
                get
                {
                    return _tag;
                }
                set
                {
                    _tag = value;
                }
            }

            public ParagraphExtractor(TextProcessor next, ITagFormatter formatter) : base(next, formatter)
            {
                //  By default isn't used in processing chain.
                //  Can be enabled by configuration file, using
                //  should be consistent with using other parts.
                Skipped = true;
                //  Default paragraph tag.
                _tag = new HtmlTag("<p", "</p>");
            }

            public override string DoProcessing(string text)
            {
                string result = String.Empty;
                //  Can extract only paragraphs.
                HtmlElement el = new HtmlElement(_tag.StartTag, _tag.EndTag, text, _formatter);
                do
                {
                    var b = el.FindNext();
                    if (!b) break;

                    //  Separates paragraphs.
                    result += Environment.NewLine + Environment.NewLine;
                    //  Adds indent at the beginning of next paragraph.
                    result += "    ";
                    //  Tag text.
                    result += el.GetText();
                }
                while (true);

                return result;
            }

            public override void LoadSettings(HTMLCleanupConfig config)
            {
                Skipped = config.ParagraphExtractorConfig.Skipped;
            }

            public override void SaveSettings(HTMLCleanupConfig config)
            {
                config.ParagraphExtractorConfig = new ParagraphExtractorType
                {
                    Skipped = Skipped
                };
            }
        }

        public class SpecialHtmlSymbol
        {
            private readonly string _specialHtml;
            private readonly string _replacement;

            public string SpecialHtml { get { return _specialHtml; } }
            //  TODO: include decimal code.
            public string Replacement { get { return _replacement; } }

            public SpecialHtmlSymbol(string specialHtml, string replacement)
            {
                _specialHtml = specialHtml;
                _replacement = replacement;
            }
        }

        /// <summary>
        /// Replaces special HTML characters.
        /// </summary>
        public class SpecialHtmlRemover : TextProcessor
        {
            //  According to this list special HTML characters are replaced
            //  or removed (depending on configuration). Similarly decimal
            //  codes are processed.
            private List<SpecialHtmlSymbol> _specialHtml = new List<SpecialHtmlSymbol>(new SpecialHtmlSymbol[] {
                new SpecialHtmlSymbol( "&#8211;", "-" ),
                new SpecialHtmlSymbol( "&#8217;", "'" ),
                new SpecialHtmlSymbol( "&#8220;", "\"" ),
                new SpecialHtmlSymbol( "&#8221;", "\"" ),
                new SpecialHtmlSymbol( "&lt;", "<" ),
                new SpecialHtmlSymbol( "&gt;", ">" ),
                new SpecialHtmlSymbol( "&amp;", "&" )
            });

            public List<SpecialHtmlSymbol> SpecialHtml
            {
                get
                {
                    return _specialHtml;
                }
            }

            public SpecialHtmlRemover(TextProcessor next, ITagFormatter formatter) : base(next, formatter)
            {
            }

            public override string DoProcessing(string text)
            {
                foreach (var sp in _specialHtml)
                {
                    text = text.Replace(sp.SpecialHtml, sp.Replacement);
                }

                return text;
            }

            public override void LoadSettings(HTMLCleanupConfig config)
            {
                Skipped = config.SpecialHTMLRemoverConfig.Skipped;
                SpecialHtml.Clear();
                foreach (var t in config.SpecialHTMLRemoverConfig.SpecialHTML)
                {
                    SpecialHtml.Add(new BaseHtmlCleaner.SpecialHtmlSymbol(t.SpecialHTML, t.Replacement));
                }

                //  This symbol is added bypassing configuration file
                //  because string consisting only from spaces is read
                //  from XML as completely empty despite it is stored
                //  correctly (workaround).
                _specialHtml.Add(new SpecialHtmlSymbol("&nbsp;", " "));
            }

            public override void SaveSettings(HTMLCleanupConfig config)
            {
                config.SpecialHTMLRemoverConfig = new SpecialHTMLRemoverType
                {
                    Skipped = Skipped,
                    SpecialHTML = new SpecialHTMLSymbolType[SpecialHtml.Count]
                };

                for (var i = 0; i < SpecialHtml.Count; i++)
                {
                    config.SpecialHTMLRemoverConfig.SpecialHTML[i] = new SpecialHTMLSymbolType
                    {
                        SpecialHTML = SpecialHtml[i].SpecialHtml,
                        Replacement = SpecialHtml[i].Replacement
                    };
                }
            }
        }

        public class Tag
        {
            private readonly string _startTag;   // Should not contain closing ">",
                                                 // to skip attributes.
            private readonly string _endTag;

            public string StartTag
            {
                get
                {
                    return _startTag;
                }
            }

            public string EndTag
            {
                get
                {
                    return _endTag;
                }
            }

            public Tag(string startTag, string endTag)
            {
                _startTag = startTag;
                _endTag = endTag;
            }
        }

        protected abstract InnerTextProcessor GetInnerTextProcessor(TextProcessor next, ITagFormatter formatter);

        /// <summary>
        /// Removes tags inside paragraphs saving internal text.
        /// It is inherited from SpecialHtmlRemover for final cleaning up text.
        /// </summary>
        public class InnerTextProcessor : SpecialHtmlRemover
        {
            /// <summary>
            /// List of tags for removing (it is filled from configuration file).
            /// All tags representig text data which should be presaved. Tags must
            /// be arranged in the reverse lexigraphical order. The first tag must
            /// not include closing bracket.
            /// </summary>
            private List<Tag> _tags;

            public List<Tag> Tags
            {
                get
                {
                    return _tags;
                }
                //  Writeable for external initialization.
                set
                {
                    _tags = value;
                }
            }

            public InnerTextProcessor(TextProcessor next, ITagFormatter formatter) : base(next, formatter)
            {
            }

            public override string DoProcessing(string text)
            {
                while (true)
                {
                    var el = HtmlElement.FindNext(_tags, text, _formatter);
                    if (el == null)
                        return text;

                    //  Removes tag and its original content from text.
                    var innerText = el.RemoveTag();
                    //  Extracts innter tag text.
                    innerText = el.InitializeTagFormatting(innerText);
                    //  Makes recursive call.
                    var finalText = DoProcessing(innerText);
                    //  Inserts formatted text instead of original content.
                    el.InsertText(finalText);
                    //  Finalizes previous state.
                    finalText = base.DoProcessing(finalText);
                    el.FinalizeTagFormatting(finalText);
                    text = el.Text;
                }
            }

            public override void LoadSettings(HTMLCleanupConfig config)
            {
                base.LoadSettings(config);
                //  Skipped must be read after base settings.
                Skipped = config.InnerTagRemoverConfig.Skipped;
                Tags.Clear();
                foreach (var t in config.InnerTagRemoverConfig.Tags)
                {
                    Tags.Add(new BaseHtmlCleaner.Tag(t.StartTagWithoutBracket, t.EndTag));
                }
            }

            public override void SaveSettings(HTMLCleanupConfig config)
            {
                base.SaveSettings(config);

                config.InnerTagRemoverConfig = new InnerTagRemoverType()
                {
                    Skipped = Skipped,
                    Tags = new TagToRemoveType[Tags.Count]
                };

                for (var i = 0; i < Tags.Count; i++)
                {
                    config.InnerTagRemoverConfig.Tags[i] = new TagToRemoveType
                    {
                        StartTagWithoutBracket = Tags[i].StartTag,
                        EndTag = Tags[i].EndTag
                    };
                }
            }
        }

        /// <summary>
        /// Creates and initializes domain-specific instance of TagRemover.
        /// </summary>
        /// <param name="next">Next processing object in the chain.</param>
        /// <returns>Instance of TagRemover specific for the domain supported
        /// by inherited class.</returns>
        protected abstract TagRemover GetTagRemover(TextProcessor next, ITagFormatter formatter);

        /// <summary>
        /// Removes tags together with internal text.
        /// </summary>
        public class TagRemover : TextProcessor
        {
            /// <summary>
            /// List of tags for removing (it is filled from configuration file).
            /// Filled by default values. When tag doesn't have closing counterpart,
            /// corresponding value should be empty string. Tags must be in the 
            /// reverse lexigraphical order.
            /// </summary>
            private List<Tag> _tags;

            public List<Tag> Tags
            {
                get
                {
                    return _tags;
                }
                //  Writeable for external initialization.
                set
                {
                    _tags = value;
                }
            }

            public TagRemover(TextProcessor next, ITagFormatter formatter) : base(next, formatter)
            {
            }

            public override string DoProcessing(string text)
            {
                foreach (var t in _tags)
                {
                    HtmlElement el = new HtmlElement(t.StartTag, t.EndTag, text, _formatter);
                    do
                    {
                        var b = el.FindNext();
                        if (!b) break;
                        el.RemoveContent();
                    }
                    while (true);
                    text = el.Text;
                }
                return text;
            }

            public override void LoadSettings(HTMLCleanupConfig config)
            {
                Skipped = config.TagWithTextRemoverConfig.Skipped;
                Tags.Clear();
                foreach (var t in config.TagWithTextRemoverConfig.Tags)
                {
                    Tags.Add(new BaseHtmlCleaner.Tag(t.StartTagWithoutBracket, t.EndTag));
                }
            }

            public override void SaveSettings(HTMLCleanupConfig config)
            {
                config.TagWithTextRemoverConfig = new TagWithTextRemoverType()
                {
                    Skipped = Skipped,
                    Tags = new TagToRemoveType[Tags.Count]
                };

                for (var i = 0; i < Tags.Count; i++)
                {
                    config.TagWithTextRemoverConfig.Tags[i] = new TagToRemoveType()
                    {
                        StartTagWithoutBracket = Tags[i].StartTag,
                        EndTag = Tags[i].EndTag
                    };
                }
            }
        }

        /// <summary>
        /// Encloses URL into square brackets.
        /// </summary>
        public class UrlFormatter : TextProcessor
        {
            public UrlFormatter(TextProcessor next, ITagFormatter formatter) : base(next, formatter)
            {
            }

            public override string DoProcessing(string text)
            {
                string result = String.Empty;
                HtmlElement el = new HtmlElement("<a", "</a>", text, _formatter);

                do
                {
                    var b = el.FindNext();
                    if (!b) break;
                    var href = el.GetAttr("href");
                    if (href != String.Empty)
                    {
                        el.ReplaceTagsWithText("[" + href + "]");
                    }
                }
                while (true);

                text = el.Text;

                return text;
            }

            public override void LoadSettings(HTMLCleanupConfig config)
            {
                Skipped = config.URLFormatterConfig.Skipped;
            }

            public override void SaveSettings(HTMLCleanupConfig config)
            {
                config.URLFormatterConfig = new URLFormatterType
                {
                    Skipped = Skipped
                };
            }
        }

        /// <summary>
        /// Splits text into lines with width not more than _max characters.
        /// </summary>
        public class TextFormatter : TextProcessor
        {
            /// <summary>
            /// List of separating characters (it is filled from configuration file).
            /// Should have default value for creating configuration template.
            /// </summary>
            private char[] _delimiters = { ' ', ',', '.', ':', ';', '?', '.', '!' };
            //  +1 allows to handle the case when word ends exactly at the boundary.
            //  TODO: make configurable.
            private const int _max = 81;

            public char[] Delimiters
            {
                get
                {
                    return _delimiters;
                }

                set
                {
                    _delimiters = value;
                }
            }

            public TextFormatter(TextProcessor next, ITagFormatter formatter) : base(next, formatter)
            {
            }

            public override string DoProcessing(string text)
            {
                var pos = 0;
                var processed = String.Empty;
                do
                {
                    if (text.Length - pos > _max - 1)
                    {
                        var substring = text.Substring(pos, _max);

                        //  Checks for end of line.
                        var pos1 = substring.LastIndexOfAny(new char[] { '\x0a', '\x0d' });
                        if (pos1 != -1)
                        {
                            //  End of line was found, cuts the line by its position.
                            processed += substring.Substring(0, pos1) + Environment.NewLine;
                            pos += pos1 + 1;
                        }
                        else
                        {
                            //  Searches for other separators.
                            pos1 = substring.LastIndexOfAny(_delimiters);

                            if (pos1 == -1)
                            {
                                processed += substring + Environment.NewLine;
                                pos += _max;
                            }
                            else
                            {
                                //  Cuts the line by the last separator.
                                processed += substring.Substring(0, pos1 + 1) + Environment.NewLine;
                                pos += pos1 + 1;
                            }
                        }
                    }
                    else
                    {
                        processed += text.Substring(pos, text.Length - pos) + Environment.NewLine;
                        break;
                    }
                }
                while (true);

                //  Removes lines containing only white spaces.
                var strings = new List<string>(processed.Split('\n'));
                var index = 0;
                while (index < strings.Count)
                {
                    var s = strings[index];
                    var m = s.Trim(' ', '\t', '\r');
                    if (m == "")
                        strings.Remove(s);
                    else
                    {
                        //  Replaces original string with string without white spaces.
                        //  Leading spaces are left to safe formatting.
                        strings[index] = s.Trim('\t', '\r').TrimEnd();
                        index++;
                    }
                }
                return string.Join("\n", strings);
            }

            public override void LoadSettings(HTMLCleanupConfig config)
            {
                Skipped = config.TextFormatterConfig.Skipped;

                var b = new byte[config.TextFormatterConfig.Delimiters.Length];
                for (var i = 0; i < config.TextFormatterConfig.Delimiters.Length; i++)
                {
                    b[i] = config.TextFormatterConfig.Delimiters[i].SymbolCode;
                }

                Delimiters = Encoding.ASCII.GetChars(b);
            }

            public override void SaveSettings(HTMLCleanupConfig config)
            {
                config.TextFormatterConfig = new TextFormatterType()
                {
                    Skipped = Skipped,
                    Delimiters = new DelimiterSymbolType[Delimiters.Length]
                };

                for (var i = 0; i < Delimiters.Length; i++)
                {
                    var b = Encoding.ASCII.GetBytes(Delimiters);
                    config.TextFormatterConfig.Delimiters[i] = new DelimiterSymbolType()
                    {
                        SymbolCode = b[i]
                    };
                }
            }
        }

        protected virtual TextProcessor CreateProcessingChain() {
            return  //  Creates processing chain (nesting is important).
                GetTagRemover(
                    //  Replacing tags with text is done before replacing
                    //  special characters to avoid interpreting text as HTML tags.
                    GetInnerTextProcessor(
                        //  By default ParagraphExtractor is disabled (see constructor).
                        GetParagraphExtractor(
                            new UrlFormatter(
                                new TextFormatter(null,
                                _formatter), 
                            _formatter),
                        _formatter),
                    _formatter),
                _formatter);
        }

        /// <summary>
        /// Returns full configuration file name including path provided by configuration serializer.
        /// </summary>
        /// <returns>Configuration file name.</returns>
        protected abstract string GetConfigurationFileName();

        protected void ReadConfiguration(TextProcessor chain)
        {
            var pathToConfig = GetConfigurationFileName();
            _configSerializer.Deserialize(pathToConfig, chain);
        }

        public void WriteConfiguration()
        {
            TextProcessor processingChain = CreateProcessingChain();

            var pathToConfig = GetConfigurationFileName();
            _configSerializer.Serialize(pathToConfig, processingChain);
        }

        public string Process(string html)
        {
            //  Creates processing chain.
            var processingChain = CreateProcessingChain();

            //  Reads configuration.
            ReadConfiguration(processingChain);

            //  Processes HTML.
            return processingChain.Process(html);
        }

        protected ITagFormatter _formatter;

        public void SetFormatter(ITagFormatter formatter)
        {
            _formatter = formatter;
        }

        public ITagFormatter GetFormatter()
        {
            return _formatter;
        }
    }
}
