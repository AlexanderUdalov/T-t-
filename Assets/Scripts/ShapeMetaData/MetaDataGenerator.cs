using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using FourDimensionalSpace;
using Newtonsoft.Json;
using UnityEngine;

namespace ShapeMetaData
{
    public static class MetaDataGenerator
    {
        private static readonly Dictionary<ShapeType, Func<Shape> > CreationFuncs = new Dictionary<ShapeType, Func<Shape>>
        {
            [ShapeType.Pentachoron]      = CreatePentachoronData,
            [ShapeType.Tesseract]        = CreateTesseractData,
            [ShapeType.Hexadecachoron]   = CreateHexadecachoronData,
            [ShapeType.Icositetrachoron] = CreateIcositetrachoronData
        };
        
        public static void GenerateDataFile(ShapeType shapeType)
        {
            Shape shape = CreationFuncs[shapeType]();
            CalculateAdjacencyList(shape.AdjacencyList, shape.Vertices, shape.AdjacencyList.Capacity);
            
            string data = JsonConvert.SerializeObject(shape);
            File.WriteAllText(
                Path.Combine(Application.streamingAssetsPath, "ShapeMetaData", shapeType + ".json"),
                data);
        }

        private static Shape CreatePentachoronData()
        {
            Shape pentachoron = new Shape(5, 10)
            {
                Vertices =
                {
                    [0] = new Vertex(1, 1, 1, 0),
                    [1] = new Vertex(1, -1, -1, 0),
                    [2] = new Vertex(-1, 1, -1, 0),
                    [3] = new Vertex(-1, -1, 1, 0),
                    [4] = new Vertex(0, 0, 0, (float) Math.Sqrt(5))
                }
            };

            return pentachoron;
        }

        private static Shape CreateTesseractData()
        {
            Shape tesseract = new Shape(16, 32);
            InitVerticesTesseract(tesseract.Vertices, 0);
            return tesseract;
        }

        private static Shape CreateHexadecachoronData()
        {
            Shape hexadecachoron = new Shape(8, 24);
            InitVerticesHexadecachoron(hexadecachoron.Vertices, 0);
            return hexadecachoron;
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        private static Shape CreateIcositetrachoronData()
        {
            Shape icositetrachoron = new Shape(24, 96);

            InitVerticesTesseract(icositetrachoron.Vertices, 0);
            InitVerticesHexadecachoron(icositetrachoron.Vertices, 16);
            
            for (int i = 16; i < 24; i++)
                icositetrachoron.Vertices[i] *= 2;

            return icositetrachoron;
        }
        
        private static void InitVerticesTesseract(Vertex[] vertices, int startIndex)
        {
            int signX = 1, signY = 1, signZ = 1, signW = 1;

            for (int i1 = 0; i1 < 2; i1++)
            {
                for (int i2 = 0; i2 < 2; i2++)
                {
                    for (int i3 = 0; i3 < 2; i3++)
                    {
                        for (int i4 = 0; i4 < 2; i4++)
                        {
                            vertices[startIndex + i1 * 8 + i2 * 4 + i3 * 2 + i4] = 
                                new Vertex(signX, signY, signZ, signW);

                            signX *= -1;
                        }
                        signY *= -1;
                    }
                    signZ *= -1;
                }
                signW *= -1;
            }
        }

        private static void InitVerticesHexadecachoron(Vertex[] vertices, int startIndex)
        {
            vertices[startIndex]     = new Vertex(1, 0, 0, 0);
            vertices[startIndex + 1] = new Vertex(0, 1, 0, 0);
            vertices[startIndex + 2] = new Vertex(0, 0, 1, 0);
            vertices[startIndex + 3] = new Vertex(0, 0, 0, 1);
            vertices[startIndex + 4] = new Vertex(-1, 0, 0, 0);
            vertices[startIndex + 5] = new Vertex(0, -1, 0, 0);
            vertices[startIndex + 6] = new Vertex(0, 0, -1, 0);
            vertices[startIndex + 7] = new Vertex(0, 0, 0, -1);
        }

        private static void CalculateAdjacencyList(List<Tuple<int, int>> adjacencyList, Vertex[] vertices, int expectedNumberOfEdgese)
        {
            float[][] distances = new float[vertices.Length][];
            float minDistance = Single.MaxValue;
            
            for (int i = vertices.Length - 1; i >= 1; i--)
            {
                distances[i] = new float[i];
                
                for (int j = i - 1; j >= 0; j--)
                {
                    float curDistance = Vertex.Distance(vertices[i], vertices[j]);
                    distances[i][j] = curDistance;

                    if (curDistance < minDistance)
                        minDistance = curDistance;
                }
            }

            for (int i = 1; i < distances.GetLength(0); i++)
            {
                for (int j = 0; j < distances[i].Length; j++)
                {
                    if (Math.Abs(distances[i][j] - minDistance) < 0.00001f)
                        adjacencyList.Add(new Tuple<int, int>(i, j));
                }
            }
            
            
            if (adjacencyList.Count != expectedNumberOfEdgese)
                throw new MetaDataGenerationException(
                    $"Expected number of edges = {expectedNumberOfEdgese}, created = {adjacencyList.Count}");
        }
    }
}