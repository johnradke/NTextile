using System.Text;

namespace Textile.UnitTests
{
    public class TrimmingOutputter : IOutputter
    {
        StringBuilder _output = new StringBuilder();

        public void Begin()
        {
            _output.Length = 0;
        }

        public void Write(string text)
        {
            text = text.Trim(' ');
            _output.Append(text);
        }

        public void WriteLine(string line)
        {
            line = line?.Trim(' ');
            _output.AppendLine(line);
        }

        public void End() { }

        public override string ToString()
        {
            return _output.ToString();
        }
    }
}
