namespace Textile
{
    /// <summary>
    /// A class that modifies parts of a sentence ("blocks") like words in bold or italics.
    /// </summary>
    public abstract class BlockModifier
    {
        public bool IsEnabled { get; set; } = true;
        public GenericFormatter Formatter { get; set; }
        protected bool UseRestrictedMode => Formatter?.UseRestrictedMode ?? false;

		public abstract string ModifyLine(string line);

        public virtual string Conclude(string line)
        {
            return line;
        }
	}
}
