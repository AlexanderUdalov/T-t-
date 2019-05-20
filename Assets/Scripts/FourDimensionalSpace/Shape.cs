using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShapeMetaData;
using UnityEngine;

namespace FourDimensionalSpace
{
    public class Shape
    {
        public ShapeType ShapeType { get; set; }
        
        public Vertex[] Vertices { get; set; }
        public List<Tuple<int, int>> AdjacencyList { get; set; }
        public List<Face> Faces { get; set; }
        public List<Cell> Cells { get; set; }

        public Shape(int numberOfVertices, int numberOfEdges, int numberOfFaces, int numberOfCells)
        {
            Vertices = new Vertex[numberOfVertices];
            AdjacencyList = new List<Tuple<int, int>>(numberOfEdges);
            //Граней в 2 раза больше, т.к. грань только с одной нормалью 
            //Соответственно для двухсторонней грани необъодимо две односторонних
            Faces = new List<Face>(numberOfFaces * 2);
            Cells = new List<Cell>(numberOfCells);
        }
        
        public void Rotate(float angle, Plane plane)
        {
            var rotationMatrix = CreateRotationMatrix(plane, angle);
            
            Vertices.AsParallel()
                .ForAll(vertex => Vertex.Rotate(ref vertex, rotationMatrix));
        }

        public async void RotateAsync(float angle, Plane plane)
        {
            var rotationMatrix = CreateRotationMatrix(plane, angle);
            
            await Task.Run(() => Vertices.AsParallel()
                .ForAll(vertex => Vertex.Rotate(ref vertex, rotationMatrix)));
        }

        private float[,] CreateRotationMatrix(Plane plane, float angle)
        {
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            switch (plane)
            {
                case Plane.XoW:
                    {
                        return new float[,]
                        {
                            { cos, 0, 0, -sin },
                            { 0, 1, 0, 0 },
                            { 0, 0, 1, 0 },
                            { sin, 0, 0, cos }
                        };
                    }
                case Plane.XoY:
                    {
                        return new float[,]
                        {
                            { cos, -sin, 0, 0 },
                            { sin, cos, 0, 0 },
                            { 0, 0, 1, 0 },
                            { 0, 0, 0, 1 }
                        };
                    }
                case Plane.XoZ:
                    {
                        return new float[,]
                         {
                            { cos, 0, -sin, 0 },
                            { 0, 1, 0, 0 },
                            { sin, 0, cos, 0 },
                            { 0, 0, 0, 1 }
                         };
                    }
                case Plane.YoW:
                    {
                        return new float[,]
                        {
                            { 1, 0, 0, 0 },
                            { 0, cos, 0, -sin },
                            { 0, 0, 1, 0 },
                            { 0, sin, 0, cos }
                        };
                    }
                case Plane.YoZ:
                    {
                        return new float[,]
                        {
                            { 1, 0, 0, 0 },
                            { 0, cos, -sin, 0 },
                            { 0, sin, cos, 0 },
                            { 0, 0, 0, 1 }
                        };
                    }
                case Plane.ZoW:
                    {
                        return new float[,]
                        {
                            { 1, 0, 0, 0 },
                            { 0, 1, 0, 0 },
                            { 0, 0, cos, -sin },
                            { 0, 0, sin, cos }
                        };
                    }
                default:
                    throw new ArgumentException();
            }
        }
    }

    public class Cell
    {
        [JsonProperty] private readonly List<int> _faceIndexes;
        [JsonIgnore] public int Count => _faceIndexes.Count;

        public Cell()
        {
            _faceIndexes = new List<int>();
        }

        public Cell(List<int> cycle)
        {
            _faceIndexes = cycle.ToList();
        }
        
        public void AddFaceIndex(int index)
        {
            _faceIndexes.Add(index);
        }

        public int GetByIndex(int index)
        {
            return _faceIndexes[index];
        }

        public List<int> ToList()
        {
            return _faceIndexes;
        }
    }

    public class Face
    {
        [JsonProperty] private readonly List<int> _vertexIndexes;
        [JsonIgnore] public int Count => _vertexIndexes.Count;

        public Face()
        {
            _vertexIndexes = new List<int>();
        }

        public Face(List<int> cycle)
        {
            _vertexIndexes = cycle.ToList();
        }
        
        public void AddVertexIndex(int index)
        {
            _vertexIndexes.Add(index);
        }
        
        public int GetByIndex(int index)
        {
            return _vertexIndexes[index];
        }

        public List<int> ToList()
        {
            return _vertexIndexes;
        }
    }
}
