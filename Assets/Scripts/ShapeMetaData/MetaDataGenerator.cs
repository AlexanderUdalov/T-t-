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
            [ShapeType.Pentachoron]    = CreatePentachoronData,
            [ShapeType.Tesseract]      = CreateTesseractData,
            [ShapeType.Hexadecachoron] = CreateHexadecachoronData,
            [ShapeType.Icositetrachoron] = CreateIcositetrachoronData
        };
        
        public static void GenerateDataFile(ShapeType shapeType)
        {
            Shape shape = CreationFuncs[shapeType]();
            
            string data = JsonConvert.SerializeObject(shape);
            File.WriteAllText(
                Path.Combine(Application.streamingAssetsPath, "ShapeMetaData", shapeType + ".json"),
                data);
        }

        private static Shape CreatePentachoronData()
        {
            Shape pentachoron = new Shape(5)
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

            for (int i = 0; i < pentachoron.AdjacencyMatrix.GetLength(0); i++)
                for (int j = 0; j < pentachoron.AdjacencyMatrix.GetLength(0); j++)
                    pentachoron.AdjacencyMatrix[i, j] = 1;

            return pentachoron;
        }

        private static Shape CreateTesseractData()
        {
            Shape tesseract = new Shape(16);
            InitVerticesTesseract(tesseract.Vertices, 0);

            //Matrix initializing
            for (int i = 0; i < 16; i += 4)
            {
                tesseract.AdjacencyMatrix[i, i + 1] = 1;
                tesseract.AdjacencyMatrix[i + 1, i + 3] = 1;
                tesseract.AdjacencyMatrix[i + 3, i + 2] = 1;
                tesseract.AdjacencyMatrix[i + 2, i] = 1;
            }
            for (int i = 0; i < 4; i++)
            {
                tesseract.AdjacencyMatrix[i, i + 4] = 1;
            }
            for (int i = 8; i < 12; i++)
            {
                tesseract.AdjacencyMatrix[i, i + 4] = 1;
            }
            for (int i = 0; i < 8; i++)
            {
                tesseract.AdjacencyMatrix[i, i + 8] = 1;
            }

            return tesseract;
        }

        private static Shape CreateHexadecachoronData()
        {
            Shape hexadecachoron = new Shape(8);
            InitVerticesHexadecachoron(hexadecachoron.Vertices, 0);

            for (int i = 0; i < hexadecachoron.AdjacencyMatrix.GetLength(0); i++)
                for (int j = 0; j < hexadecachoron.AdjacencyMatrix.GetLength(0); j++)
                    if (Math.Abs(i - j) != 4)
                        hexadecachoron.AdjacencyMatrix[i, j] = 1;
            
            return hexadecachoron;
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        private static Shape CreateIcositetrachoronData()
        {
            Shape icositetrachoron = new Shape(24);

            InitVerticesTesseract(icositetrachoron.Vertices, 0);
            InitVerticesHexadecachoron(icositetrachoron.Vertices, 16);
            for (int i = 16; i < 24; i++)
                icositetrachoron.Vertices[i] *= 2;

            for (int i = 0; i < icositetrachoron.Vertices.Length - 1; i++)
            {
                for (int j = i + 1; j < icositetrachoron.Vertices.Length; j++)
                {
                    // ребром соединены те вершины, у которых все четыре координаты различаются на 1
                    // или одна из координат различается на 2, а остальные совпадают
                    if (Math.Abs(icositetrachoron.Vertices[i].x - icositetrachoron.Vertices[j].x) == 1f &&
                        Math.Abs(icositetrachoron.Vertices[i].y - icositetrachoron.Vertices[j].y) == 1f &&
                        Math.Abs(icositetrachoron.Vertices[i].z - icositetrachoron.Vertices[j].z) == 1f &&
                        Math.Abs(icositetrachoron.Vertices[i].w - icositetrachoron.Vertices[j].w) == 1f
                        ||
                        Math.Abs(icositetrachoron.Vertices[i].x - icositetrachoron.Vertices[j].x) == 2f &&
                        icositetrachoron.Vertices[i].y == icositetrachoron.Vertices[j].y &&
                        icositetrachoron.Vertices[i].z == icositetrachoron.Vertices[j].z &&
                        icositetrachoron.Vertices[i].w == icositetrachoron.Vertices[j].w
                        ||
                        Math.Abs(icositetrachoron.Vertices[i].y - icositetrachoron.Vertices[j].y) == 2f &&
                        icositetrachoron.Vertices[i].x == icositetrachoron.Vertices[j].x &&
                        icositetrachoron.Vertices[i].z == icositetrachoron.Vertices[j].z &&
                        icositetrachoron.Vertices[i].w == icositetrachoron.Vertices[j].w
                        ||
                        Math.Abs(icositetrachoron.Vertices[i].z - icositetrachoron.Vertices[j].z) == 2f &&
                        icositetrachoron.Vertices[i].x == icositetrachoron.Vertices[j].x &&
                        icositetrachoron.Vertices[i].y == icositetrachoron.Vertices[j].y &&
                        icositetrachoron.Vertices[i].w == icositetrachoron.Vertices[j].w
                        ||
                        Math.Abs(icositetrachoron.Vertices[i].w - icositetrachoron.Vertices[j].w) == 2f &&
                        icositetrachoron.Vertices[i].x == icositetrachoron.Vertices[j].x &&
                        icositetrachoron.Vertices[i].y == icositetrachoron.Vertices[j].y &&
                        icositetrachoron.Vertices[i].z == icositetrachoron.Vertices[j].z
                        )
                    {
                        icositetrachoron.AdjacencyMatrix[i, j] = 1;
                        icositetrachoron.AdjacencyMatrix[j, i] = 1;
                    }
                }
            }

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
    }
}