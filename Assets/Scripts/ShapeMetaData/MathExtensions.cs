using System.Linq;

namespace ShapeMetaData
{
    public static class MathExtensions
    {
        public static int Factorial(this int count) =>   
            count == 0 ? 1 : Enumerable.Range(1, count).Aggregate((i, j) => i*j);
    }
}