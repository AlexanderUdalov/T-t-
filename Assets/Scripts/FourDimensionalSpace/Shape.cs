using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace FourDimensionalSpace
{
    public class Shape
    {
        public Vertex[] Vertices { get; set; }
        public List<Tuple<int, int>> AdjacencyList { get; set; }
        public List<List<int>> Faces { get; set; }
        public List<List<int>> Cells { get; set; }

        public Shape(int numberOfVertices, int numberOfEdges, int numberOfFaces, int numberOfCells)
        {
            Vertices = new Vertex[numberOfVertices];
            AdjacencyList = new List<Tuple<int, int>>(numberOfEdges);
            //Граней в 2 раза больше, т.к. грань только с одной нормалью 
            //Соответственно для двухсторонней грани необъодимо две односторонних
            Faces = new List<List<int>>(numberOfFaces * 2);
            Cells = new List<List<int>>(numberOfCells);
        }
        
        public void Rotate(float angle, Planes plane)
        {
            Vertices.AsParallel()
                .ForAll(vertex => Vertex.Rotate(ref vertex, CreateRotationMatrix(plane, angle)));
        }

        public async void RotateAsync(float angle, Planes plane)
        {
            await Task.Run(() => Vertices.AsParallel()
                .ForAll(vertex => Vertex.Rotate(ref vertex, CreateRotationMatrix(plane, angle))));
        }

        private float [,] CreateRotationMatrix(Planes plane, float angle)
        {
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            switch (plane)
            {
                case Planes.XoW:
                    {
                        return new float[,]
                        {
                            { cos, 0, 0, -sin },
                            { 0, 1, 0, 0 },
                            { 0, 0, 1, 0 },
                            { sin, 0, 0, cos }
                        };
                    }
                case Planes.XoY:
                    {
                        return new float[,]
                        {
                            { cos, -sin, 0, 0 },
                            { sin, cos, 0, 0 },
                            { 0, 0, 1, 0 },
                            { 0, 0, 0, 1 }
                        };
                    }
                case Planes.XoZ:
                    {
                        return new float[,]
                         {
                            { cos, 0, -sin, 0 },
                            { 0, 1, 0, 0 },
                            { sin, 0, 1, 0 },
                            { 0, 0, 0, 1 }
                         };
                    }
                case Planes.YoW:
                    {
                        return new float[,]
                        {
                            { 1, 0, 0, 0 },
                            { 0, cos, 0, -sin },
                            { 0, 0, 1, 0 },
                            { 0, sin, 0, cos }
                        };
                    }
                case Planes.YoZ:
                    {
                        return new float[,]
                        {
                            { 1, 0, 0, 0 },
                            { 0, cos, -sin, 0 },
                            { 0, sin, cos, 0 },
                            { 0, 0, 0, 1 }
                        };
                    }
                case Planes.ZoW:
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
}
