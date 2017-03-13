using Textile.Blocks;

namespace Textile.States
{
    public abstract class SimpleBlockFormatterState : FormatterState
    {
        internal const string TextilePatternBegin = @"^\s*(?<tag>";
        internal const string TextilePatternEnd = @")" + TextileGlobals.AlignPattern + TextileGlobals.BlockModifiersPattern + @"\.(?:\s+)?(?<content>.*)$";
        internal const string TextileDoubleDotPatternEnd = @")" + TextileGlobals.AlignPattern + TextileGlobals.BlockModifiersPattern + @"\.(?<ddot>\.)?(?:\s+)?(?<content>.*)$";

        protected string Tag { get; private set; }
        protected string AlignInfo { get; private set; }
        protected string AttInfo { get; private set; }

		public override string Consume(FormatterStateConsumeContext context)
        {
            Tag = context.Match.Groups["tag"].Value;
            AlignInfo = context.Match.Groups["align"].Value;
            AttInfo = context.Match.Groups["atts"].Value;
            var input = context.Match.Groups["content"].Value;

            OnContextAcquired();

            ChangeState(this);

            return input;
        }

        public override bool ShouldNestState(FormatterState other)
        {
            var blockFormatterState = (SimpleBlockFormatterState)other;

            return (blockFormatterState.Tag != Tag ||
                    blockFormatterState.AlignInfo != AlignInfo ||
                    blockFormatterState.AttInfo != AttInfo);
        }

        protected virtual void OnContextAcquired() { }

        protected string FormattedStylesAndAlignment()
        {
            return BlockAttributesParser.Parse(AlignInfo + AttInfo, UseRestrictedMode);
        }
    }
}
