using System.Text;

namespace Textile.Test
{
    class TestOutputter : IOutputter
    {
        StringBuilder _builder;

        public void Begin()
        {
            _builder = new StringBuilder();
        }

        public void Write(string text)
        {
            _builder.Append(text);
        }

        public void WriteLine(string line)
        {
            _builder.Append(line + "\n");
        }

        public void End() { }

        public string GetOutput()
        {
            return _builder.ToString();
        }
    }
}
