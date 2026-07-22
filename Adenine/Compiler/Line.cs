namespace Adenine.Compiler
{
    public struct Line
    {
        public string Text { get; set; } = "";

        public int Number { get; set; } = 0;

        public Line(string text, int number)
        {
            Text = text;
            Number = number;
        }

        public override string ToString()
        {
            return $"[line {Number + 1}]: \"{Text}\"";
        }
    }
}
