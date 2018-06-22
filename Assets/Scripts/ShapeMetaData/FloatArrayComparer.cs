using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMetaData
{
    internal class FloatArrayComparer : IEqualityComparer<float[]>
    {
        public bool Equals(float[] x, float[] y)
        {
            if (x == null && y == null) { return true;}
            if (x == null | y == null) { return false;}
            
            if (x.Length != y.Length)
                return false;

            return !x.Where((t, i) => t != y[i]).Any();
        }

        public int GetHashCode(float[] obj)
        {
            StringBuilder hash = new StringBuilder();
            foreach (var f in obj)
                hash.Append(f);
            
            return hash.ToString().GetHashCode();
        }
    }
}