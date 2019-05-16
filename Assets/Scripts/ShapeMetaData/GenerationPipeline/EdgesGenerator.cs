using System;
using FourDimensionalSpace;

namespace ShapeMetaData
{
    public class EdgesGenerator : IGenerator
    {
        public void Execute(Shape shape)
        {
            CalculateAdjacencyList(shape, shape.AdjacencyList.Capacity);
        }
        
        private void CalculateAdjacencyList(Shape shape, int expectedNumberOfEdges)
        {
            float minDistance = Single.MaxValue;
            
            for (int i = 0; i < shape.Vertices.Length - 1; i++)
            {
                for (int j = i + 1; j < shape.Vertices.Length; j++)
                {
                    float curDistance = Vertex.Distance(shape.Vertices[i], shape.Vertices[j]);

                    if (Math.Abs(curDistance - minDistance) < 0.00001f)
                        shape.AdjacencyList.Add(new Tuple<int, int>(i, j));
                    else if (curDistance < minDistance)
                    {
                        shape.AdjacencyList.Clear();
                        shape.AdjacencyList.Add(new Tuple<int, int>(i, j));
                        minDistance = curDistance;
                    }
                }
            }
            
            if (shape.AdjacencyList.Count != expectedNumberOfEdges)
                throw new MetaDataGenerationException(
                    $"Expected number of edges = {expectedNumberOfEdges}, created = {shape.AdjacencyList.Count}");
        }
    }
}