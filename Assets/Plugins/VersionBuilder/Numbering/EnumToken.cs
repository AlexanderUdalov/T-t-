using System.Text;

namespace VersionBuilder.Numbering
{
    internal class EnumToken: Token
    {
        public string[] Variants;
        public int ValueIndex;

        public EnumToken(int index, string text, string[] variants, int valueIndex) : base(index, text)
        {
            Variants = variants;
            ValueIndex = valueIndex;
        }

        public override string ToString()
        {
            return Variants[ValueIndex];
        }

        public override string ToPatternString()
        {
            var sb = new StringBuilder();
            sb.Append('[')
                .Append(Text)
                .Append('{');
            for (int i = 0; i < Variants.Length - 1; i++)
            {
                if (i == ValueIndex)
                    sb.Append(Variants[i]).Append('!').Append(',');
                else sb.Append(Variants[i]).Append(',');
            }
            sb.Append(Variants[Variants.Length - 1]).Append("}]");

            return sb.ToString();
        }
    }
}