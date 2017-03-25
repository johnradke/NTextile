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

using System;
using Textile.States;

namespace Textile
{
    /// <summary>
    /// Base class for formatter states.
    /// </summary>
    /// A formatter state describes the current situation
    /// of the text being currently processed. A state can
    /// write HTML code when entered, exited, and can modify
    /// each line of text it receives.
    public abstract class FormatterState
    {
        /// <summary>
        /// A convenience property to get whether the owner <see cref="GenericFormatter"/>
        /// has the 'restrcted' mode turned on. If there is no owning formatter, 
        /// return false.
        /// </summary>
        protected bool UseRestrictedMode => Formatter?.UseRestrictedMode ?? false;

        /// <summary>
        /// Gets the formatter this state belongs to.
        /// </summary>
        public GenericFormatter Formatter { get; internal set; }

        protected void Write(string text) => Formatter.Output.Write(text);

        protected void WriteLine(string line = null) => Formatter.Output.WriteLine(line);

        protected void ChangeState(FormatterState newState) => Formatter.StateManager.ChangeState(newState);

        protected FormatterState CurrentState => Formatter.StateManager.CurrentState;

        /// <summary>
        /// Gets or sets whether the formatter is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        public abstract string Consume(FormatterStateConsumeContext context);

        /// <summary>
        /// Method called when the state is entered.
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// Method called when the state is exited.
        /// </summary>
        public virtual void Exit() { }

        /// <summary>
        /// Method called when a line of text should be written
        /// to the web form.
        /// </summary>
        /// <param name="input">The line of text.</param>
        public abstract void FormatLine(string input);

        public abstract bool ShouldExit(string input, string inputLookAhead);

        public virtual bool ShouldNestState(FormatterState other) => false;

        /// <summary>
        /// Returns whether block formatting (quick phrase modifiers, etc.) should be
        /// applied to this line.
        /// </summary>
        /// <param name="input">The line of text</param>
        /// <returns>Whether the line should be formatted for blocks</returns>
        public virtual bool ShouldFormatBlocks(string input) => true;

        /// <summary>
        /// Returns whether the current state accepts being superceded by another one
        /// we would possibly find by parsing the input line of text.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual bool ShouldParseForNewFormatterState(string input) => true;

        /// <summary>
        /// Returns whether post-processors should be applied to this line.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual bool ShouldPostProcess(string input) => true;

        /// <summary>
        /// Gets the formatting state we should fallback to if we don't find anything
        /// relevant in a line of text.
        /// </summary>
        public virtual Type FallbackFormattingState => typeof(ParagraphFormatterState);
    }
}
