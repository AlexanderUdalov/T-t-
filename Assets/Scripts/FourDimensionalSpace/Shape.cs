using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourDimensionalSpace
{
    public class Shape
    {
        public List<Tuple<int, int>> AdjacencyList { get; set; }
        public Vertex[] Vertices { get; set; }

        public Shape(int numberOfVertices, int numberOfEdges)
        {
            Vertices = new Vertex[numberOfVertices];
            AdjacencyList = new List<Tuple<int, int>>(numberOfEdges);
        }
        
        public void Rotate(float angle, Planes plane)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertex.Rotate(ref Vertices[i],  CreateRotationMatrix(plane, angle));
            }
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
