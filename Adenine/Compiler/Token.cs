namespace Adenine.Compiler
{
    public struct Token
    {
        public string Text { get; set; } = "";

        public int LineNumber { get; set; } = 0;

        public Token(string text, int lineNumber)
        {
            Text = text;
            LineNumber = lineNumber;
        }

        public override string ToString()
        {
            return $"[line {LineNumber + 1}]: \"{Text}\"";
        }
    }
}
