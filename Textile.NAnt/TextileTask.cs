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
    [TaskName("textile")]
    public class TextileTask : Task
    {
        #region Private Instance Fields
        private FileSet _inputs = new FileSet();
        private DirectoryInfo _toDirectory;
        #endregion

        // TODO: expose TextileFormatter's parameters as task properties
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
                    string outputHtml = TextileFormatter.FormatString(inputTextile);
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
