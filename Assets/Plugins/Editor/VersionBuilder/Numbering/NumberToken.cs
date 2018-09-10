using System.Text;

namespace VersionBuilder.Numbering
{
    //TODO: может value не string, а int?
    internal class NumberToken : Token
    {
        public string Value;

        public NumberToken(int index, string text, string value = "0"): base(index, text)
        {
            Value = value;
        }
        
        public static NumberToken operator ++(NumberToken t) { return t.Increment(); }
        public static NumberToken operator --(NumberToken t) { return t.Decrement(); }
        
        private NumberToken Decrement()
        {
            throw new System.NotImplementedException();
        }

        private NumberToken Increment()
        {
            //TMP
            int value = int.Parse(Value);
            Value = (++value).ToString();
            return this;
        }

        public override string ToString()
        {
            return Value;
        }

        public override string ToPatternString()
        {
            var sb = new StringBuilder();
            if (int.Parse(Value) == 0)
                return sb
                    .Append('[')
                    .Append(Text)
                    .Append(']')
                    .ToString();

            else return sb
                    .Append('[')
                    .Append(Text)
                    .Append('=')
                    .Append(Value)
                    .Append(']')
                    .ToString();
        }
    }
}