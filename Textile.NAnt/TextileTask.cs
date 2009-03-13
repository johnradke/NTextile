using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;

namespace Textile.NAnt
{
    /// <summary>
    /// Converts Textile files to HTML files.
    /// </summary>
    /// <example>
    ///   <para>Convert an entire tree of Textile files into its corresponding HTML representation.</para>
    ///   <code>
    ///     <![CDATA[
    /// <textile todir="Deploy/Wiki">
    ///     <fileset basedir="Source/Wiki">
    ///         <include name="**/*.ttl" />
    ///     </fileset>
    /// </textile>
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    ///   <para>
    ///     Convert a single folder of Textile input files into HTML and set all options to non-default values.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <textile todir="Tests/Configured" headerOffset="1" linkRel="section"
    ///     formatFootNotes="false" formatImages="false" formatLinks="false" formatLists="false" formatTables="false">
    ///     <fileset basedir="DressingRoom\TestTexts">
    ///         <include name="*.txt" />
    ///     </fileset>
    /// </textile>
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("textile")]
    public class TextileTask : Task
    {
        #region Private Instance Fields
        private FileSet _inputs = new FileSet();
        private DirectoryInfo _toDirectory;

        private bool m_formatFootNotes = true;
        private bool m_formatImages = true;
        private bool m_formatLinks = true;
        private bool m_formatLists = true;
        private bool m_formatTables = true;
        private int m_headerOffset = 0;
        private string m_linkRel = String.Empty;
        #endregion

        #region Public Instance Properties
        /// <summary>
        /// The set of Textile files to convert.
        /// </summary>
        [BuildElement("fileset", Required = true)]
        public FileSet Inputs
        {
            get
            {
                return _inputs;
            }
            set
            {
                _inputs = value;
            }
        }

        /// <summary>
        /// The directory in which to create the output files.
        /// </summary>
        [TaskAttribute("todir", Required = true)]
        public DirectoryInfo ToDirectory
        {
            get
            {
                return _toDirectory;
            }
            set
            {
                _toDirectory = value;
            }
        }

        /// <summary>
        /// Determines whether footnotes should generate hyperlinks or be left as-is.
        /// The default is <see langword="true"/>
        /// </summary>
        [TaskAttribute("formatFootNotes", Required = false)]
        public bool FormatFootNotes
        {
            get
            {
                return m_formatFootNotes;
            }
            set
            {
                m_formatFootNotes = value;
            }
        }

        /// <summary>
        /// Determines whether URLs to images should generate <c>&lt;img&gt;</c> elements or be left as-is.
        /// The default is <see langword="true"/>
        /// </summary>
        [TaskAttribute("formatImages", Required = false)]
        public bool FormatImages
        {
            get
            {
                return m_formatImages;
            }
            set
            {
                m_formatImages = value;
            }
        }

        /// <summary>
        /// Determines whether URLs should generate hyperlinks or be left as-is.
        /// The default is <see langword="true"/>
        /// </summary>
        [TaskAttribute("formatLinks", Required = false)]
        public bool FormatLinks
        {
            get
            {
                return m_formatLinks;
            }
            set
            {
                m_formatLinks = value;
            }
        }

        /// <summary>
        /// Determines whether list markup should generate HTML lists or be left as-is.
        /// The default is <see langword="true"/>
        /// </summary>
        [TaskAttribute("formatLists", Required = false)]
        public bool FormatLists
        {
            get
            {
                return m_formatLists;
            }
            set
            {
                m_formatLists = value;
            }
        }

        /// <summary>
        /// Determines whether table markup should generate HTML tables or be left as-is.
        /// The default is <see langword="true"/>
        /// </summary>
        [TaskAttribute("formatTables", Required = false)]
        public bool FormatTables
        {
            get
            {
                return m_formatTables;
            }
            set
            {
                m_formatTables = value;
            }
        }

        /// <summary>
        /// The offset for the header tags.
        /// </summary>
        /// When the formatted text is inserted into another page
        /// there might be a need to offset all headers (h1 becomes
        /// h4, for instance). The header offset allows this.
        [TaskAttribute("headerOffset", Required = false)]
        public int HeaderOffset
        {
            get
            {
                return m_headerOffset;
            }
            set
            {
                m_headerOffset = value;
            }
        }

        /// <summary>
        /// The value of the 'rel' attribute to add to all links.
        /// The default is an empty string, which means no <c>rel</c> attributes will be generated.
        /// </summary>
        [TaskAttribute("linkRel", Required = false)]
        public string LinkRel
        {
            get
            {
                return m_linkRel;
            }
            set
            {
                m_linkRel = value;
            }
        }
        #endregion

        protected override void ExecuteTask()
        {
            // ensure base directory is set, even if fileset was not initialized from XML
            if (Inputs.BaseDirectory == null)
            {
                Inputs.BaseDirectory = new DirectoryInfo(Project.BaseDirectory);
            }
            // get the complete path of the base directory of the fileset, ie, c:\work\nant\src
            DirectoryInfo srcBaseInfo = Inputs.BaseDirectory;

            #region Configure TextileFormatter
            StringBuilderTextileFormatter sbtf = new StringBuilderTextileFormatter();
            TextileFormatter tf = new TextileFormatter(sbtf);
            tf.FormatFootNotes = this.FormatFootNotes;
            tf.FormatImages = this.FormatImages;
            tf.FormatLinks = this.FormatLinks;
            tf.FormatLists = this.FormatLists;
            tf.FormatTables = this.FormatTables;
            tf.HeaderOffset = this.HeaderOffset;
            tf.Rel = this.LinkRel;
            #endregion

            int numConvertedFiles = 0;
            foreach (string pathname in Inputs.FileNames)
            {
                FileInfo srcInfo = new FileInfo(pathname);
                if (srcInfo.Exists)
                {
                    #region Compute destination path
                    // Gets the relative path and file info from the full source filepath
                    // pathname = C:\f2\f3\file1, srcBaseInfo=C:\f2, then 
                    // dstRelFilePath=f3\file1
                    string dstRelFilePath = "";
                    if (srcInfo.FullName.IndexOf(srcBaseInfo.FullName, 0) != -1)
                    {
                        dstRelFilePath = srcInfo.FullName.Substring(srcBaseInfo.FullName.Length);
                    }
                    else
                    {
                        dstRelFilePath = srcInfo.Name;
                    }

                    if (dstRelFilePath[0] == Path.DirectorySeparatorChar)
                    {
                        dstRelFilePath = dstRelFilePath.Substring(1);
                    }
                    #endregion

                    // The full filepath of the destination
                    string dstFilePath = Path.Combine(ToDirectory.FullName, dstRelFilePath);

                    string parentFolder = Path.GetDirectoryName(dstFilePath);
                    if (!Directory.Exists(parentFolder))
                    {
                        Directory.CreateDirectory(parentFolder);
                    }

                    #region Read, format and write
                    string inputTextile;

                    Log(Level.Debug, "Reading input file '{0}'", srcInfo.FullName);
                    using (StreamReader sr = new StreamReader(srcInfo.FullName))
                    {
                        inputTextile = sr.ReadToEnd();
                    }

                    Log(Level.Verbose, "Converting textile from file '{0}' to an HTML string...", srcInfo.FullName);
                    tf.Format(inputTextile);
                    string outputHtml = sbtf.GetFormattedText();

                    // TODO: make output extension configurable
                    string outputPath = Path.ChangeExtension(dstFilePath, ".html");
                    // TODO: Add a switch to optionally hide (or at least make it verbose) the following log statement
                    Log(Level.Info, "Generating {0}...", outputPath);
                    using (StreamWriter sw = new StreamWriter(outputPath))
                    {
                        sw.Write(outputHtml);
                    }
                    Log(Level.Verbose, "Generated {0}.", outputPath);
                    #endregion
                    numConvertedFiles++;
                }
                else
                {
                    throw new BuildException(string.Format(CultureInfo.InvariantCulture,
                        "Could not find file '{0}' to convert with Textile.", srcInfo.FullName),
                        Location);
                }
            }
            Log(Level.Verbose, "Converted {0} files.", numConvertedFiles);
        }
    }
}
