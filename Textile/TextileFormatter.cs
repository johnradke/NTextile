#region License Statement
// Copyright (c) L.A.B.Soft.  All rights reserved.
//
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Textile.States;
using Textile.Blocks;
#endregion


namespace Textile
{
    /// <summary>
    /// Class for formatting Textile input into HTML.
    /// </summary>
    /// This class takes raw Textile text and sends the
    /// formatted, ready to display HTML string to the
    /// outputter defined in the constructor of the
    /// class.
    public partial class TextileFormatter : GenericFormatter
    {
        /// <summary>
        /// Public constructor, where the formatter is hooked up
        /// to an outputter.
        /// </summary>
        /// <param name="output">The outputter to be used.</param>
        public TextileFormatter(IOutputter output)
            : base(output, typeof(States.ParagraphFormatterState))
        {
            RegisterFormatterState<HeaderFormatterState>();
            RegisterFormatterState<BlockQuoteFormatterState>();
            RegisterFormatterState<ParagraphFormatterState>();
            RegisterFormatterState<FootNoteFormatterState>();
            RegisterFormatterState<OrderedListFormatterState>();
            RegisterFormatterState<UnorderedListFormatterState>();
            RegisterFormatterState<TableFormatterState>();
            RegisterFormatterState<TableRowFormatterState>();
            RegisterFormatterState<CodeFormatterState>();
            RegisterFormatterState<PreFormatterState>();
            RegisterFormatterState<PreCodeFormatterState>();
            RegisterFormatterState<PreBlockFormatterState>();
            RegisterFormatterState<NoTextileFormatterState>();

            RegisterBlockModifier<NoTextileBlockModifier>();
            RegisterBlockModifier<CodeBlockModifier>();
            RegisterBlockModifier<PreBlockModifier>();
            RegisterBlockModifier<HyperLinkBlockModifier>();
            RegisterBlockModifier<ImageBlockModifier>();
            RegisterBlockModifier<GlyphBlockModifier>();
            RegisterBlockModifier<EmphasisPhraseBlockModifier>();
            RegisterBlockModifier<StrongPhraseBlockModifier>();
            RegisterBlockModifier<ItalicPhraseBlockModifier>();
            RegisterBlockModifier<BoldPhraseBlockModifier>();
            RegisterBlockModifier<CitePhraseBlockModifier>();
            RegisterBlockModifier<DeletedPhraseBlockModifier>();
            RegisterBlockModifier<InsertedPhraseBlockModifier>();
            RegisterBlockModifier<SuperScriptPhraseBlockModifier>();
            RegisterBlockModifier<SubScriptPhraseBlockModifier>();
            RegisterBlockModifier<SpanPhraseBlockModifier>();
            RegisterBlockModifier<FootNoteReferenceBlockModifier>();
        }

        /// <summary>
        /// Utility method for quickly formatting a text without having
        /// to create a TextileFormatter with an IOutputter.
        /// </summary>
        /// <param name="input">The string to format</param>
        /// <returns>The formatted version of the string</returns>
        public static string FormatString(string input)
        {
			StringBuilderOutputter output = new StringBuilderOutputter();
			TextileFormatter formatter = new TextileFormatter(output);
            formatter.Format(input);
			return output.GetFormattedText();
        }

        /// <summary>
        /// Utility method for formatting a text with a given outputter.
        /// </summary>
        /// <param name="input">The string to format</param>
        /// <param name="outputter">The IOutputter to use</param>
        public static void FormatString(string input, IOutputter outputter)
        {
            TextileFormatter f = new TextileFormatter(outputter);
            f.Format(input);
        }
    }
}
