using System.Collections.Generic;
using System.Linq;
using FourDimensionalSpace;
using UnityEngine;

namespace ShapeMetaData
{
    public class FacesGenerator : IGenerator
    {
        private static readonly Dictionary<ShapeType, int> VerticesOnFace = new Dictionary<ShapeType, int>
        {
            [ShapeType.Cell5]   = 3,
            [ShapeType.Cell8]   = 4,
            [ShapeType.Cell16]  = 3,
            [ShapeType.Cell24]  = 3,
            [ShapeType.Cell120] = 5,
            [ShapeType.Cell600] = 3
        };
        
        public void Execute(Shape shape)
        {
            CalculateFaces(shape, VerticesOnFace[shape.ShapeType], shape.Faces.Capacity);
        }
        
        private void CalculateFaces(Shape shape, int limit, int expectedNumberOfFaces)
        {
            for (int i = 0; i < shape.Vertices.Length; i++)
            {
                var color = new byte[shape.Vertices.Length];
                List<int> cycle = new List<int> {i};
                FacesCycleDFS(shape, i, i, color, -1, limit, cycle);
            }
            
            if (shape.Faces.Count != expectedNumberOfFaces)
                throw new MetaDataGenerationException(
                    $"Expected number of faces = {expectedNumberOfFaces}, created = {shape.Faces.Count}");
        }

        private void FacesCycleDFS(Shape shape, int u, int endV, byte[] color, int unavailableEdge, int limit,
            List<int> cycle)
        {
            if (cycle.Count > limit + 1)
                return;

            if (u != endV)
                color[u] = 1;
            else if (cycle.Count == limit + 1)
            {
                // все циклы приводим к виду, при котором цикл начинается с вершины с минимальным индексом
                PutMinElementInZeroPosition(cycle);

                // добавляем найденный цикл только в случае, если раньше он не встречался
                foreach (var face in shape.Faces)
                    if (cycle.Where((t, i) => t == face.GetByIndex(i)).Count() == cycle.Count)
                        return;

                shape.Faces.Add(new Face(cycle));
                return;
            }

            for (int w = 0; w < shape.AdjacencyList.Count; w++)
            {
                if (w == unavailableEdge)
                    continue;
                if (color[shape.AdjacencyList[w].Item2] == 0 && shape.AdjacencyList[w].Item1 == u)
                {
                    List<int> newCycle = new List<int>(cycle) {shape.AdjacencyList[w].Item2};
                    FacesCycleDFS(shape, shape.AdjacencyList[w].Item2, endV, color, w, limit, newCycle);
                    color[shape.AdjacencyList[w].Item2] = 0;
                }
                else if (color[shape.AdjacencyList[w].Item1] == 0 && shape.AdjacencyList[w].Item2 == u)
                {
                    List<int> newCycle = new List<int>(cycle) {shape.AdjacencyList[w].Item1};
                    FacesCycleDFS(shape, shape.AdjacencyList[w].Item1, endV, color, w, limit, newCycle);
                    color[shape.AdjacencyList[w].Item1] = 0;
                }
            }
        }

        private void PutMinElementInZeroPosition(List<int> cycle)
        {
            cycle.RemoveAt(cycle.Count - 1);
            
            int min = cycle.Min();
            int minElementIndex = cycle.IndexOf(min);

            for (int i = 0; i < minElementIndex; i++)
                cycle.Add(cycle[i]);
            if (minElementIndex > 0)
                cycle.RemoveRange(0, minElementIndex);
            cycle.Add(cycle[0]);
        }
    }
}