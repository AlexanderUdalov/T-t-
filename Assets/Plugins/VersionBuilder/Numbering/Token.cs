namespace VersionBuilder.Numbering
{
    internal class Token
    {
        public int Index;
        public string Text;

        public Token(int index, string text)
        {
            Index = index;
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }

        public virtual string ToPatternString()
        {
            return Text;
        }
    }
}