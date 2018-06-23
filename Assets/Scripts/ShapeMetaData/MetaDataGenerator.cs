using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FourDimensionalSpace;
using Newtonsoft.Json;
using UnityEngine;

namespace ShapeMetaData
{
    public static class MetaDataGenerator
    {
        private static readonly float F = (1 + Mathf.Sqrt(5)) / 2;

        private static readonly Dictionary<ShapeType, Func<Shape> > CreationFuncs = new Dictionary<ShapeType, Func<Shape>>
        {
            [ShapeType.Cell5]   = Create5CellData,
            [ShapeType.Cell8]   = Create8CellData,
            [ShapeType.Cell16]  = Create16CellData,
            [ShapeType.Cell24]  = Create24CellData,
            [ShapeType.Cell120] = Create120CellData,
            [ShapeType.Cell600] = Create600CellData
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

        private static Shape Create5CellData()
        {
            Shape pentachoron = new Shape(5, 10)
            {
                Vertices =
                {
                    [0] =  new Vertex(1, 1, 1, 0),
                    [1] =  new Vertex(1, -1, -1, 0),
                    [2] =  new Vertex(-1, 1, -1, 0),
                    [3] =  new Vertex(-1, -1, 1, 0),
                    [4] =  new Vertex(0, 0, 0, Mathf.Sqrt(5))
                }
            };
            return pentachoron;
        }

        private static Shape Create8CellData()
        {
            Shape tesseract = new Shape(16, 32);
            InitVerticesPermutation(tesseract.Vertices, new[] {1f, 1f, 1f, 1f}, 0, false, 16);
            return tesseract;
        }

        private static Shape Create16CellData()
        {
            Shape hexadecachoron = new Shape(8, 24);
            InitVerticesPermutation(hexadecachoron.Vertices, new[] {1f, 0f, 0f, 0f}, 0, false, 8);
            return hexadecachoron;
        }

        private static Shape Create24CellData()
        {
            Shape icositetrachoron = new Shape(24, 96);
            InitVerticesPermutation(icositetrachoron.Vertices, new[] {1f, 1f, 1f, 1f}, 0, false, 16);
            InitVerticesPermutation(icositetrachoron.Vertices, new[] {2f, 0f, 0f, 0f}, 16, false, 8);
            return icositetrachoron;
        }

        private static Shape Create120CellData()
        {
            Shape hecatonicosachoron = new Shape(600, 1200);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {0f, 0f, 2f, 2f}, 0, false, 24);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {1f, 1f, 1f, Mathf.Sqrt(5)}, 24, false, 64);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {F, F, F, 1/(F*F)}, 88, false, 64);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {1/F, 1/F, 1/F, F*F}, 152, false, 64);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {0f, 1f, 1/(F*F), F*F}, 216, true, 96);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {0f, 1/F, F, Mathf.Sqrt(5)}, 312, true, 96);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {1f, 2f, 1/F, F}, 408, true, 192);
            return hecatonicosachoron;
        }

        private static Shape Create600CellData()
        {
            Shape hexacosichoron = new Shape(120, 720);
            InitVerticesPermutation(hexacosichoron.Vertices, new[] {1f, 1f, 1f, 1f}, 0, false, 16);
            InitVerticesPermutation(hexacosichoron.Vertices, new[] {2f, 0f, 0f, 0f}, 16, false, 8);
            InitVerticesPermutation(hexacosichoron.Vertices, new[] {0f, 1f, F, 1/F}, 16 + 8, true, 96);
            return hexacosichoron;
        }
        
        private static void InitVerticesPermutation(Vertex[] vertices, float[] numbers, int startIndex, bool even,
            int expectedNumberOfVertices, float multiplier = 1f)
        {    
            HashSet<float[]> unsignedEvenPermutations = new HashSet<float[]>(new FloatArrayComparer());
            int numOfPermutations = 4.Factorial();

            for (int i = 0; i < numOfPermutations; i++)
            {
                float[] currentPermutation = new float[4];
                
                List<float> optionsLeft = numbers.ToList();
                for (int j = 0; j < numbers.Length; j++)
                {
                    int index = i / (3 - j).Factorial() % optionsLeft.Count;
                    currentPermutation[j] = optionsLeft[index];
                    optionsLeft.RemoveAt(index);
                }
                
                if (even && !IsPermutationEven(currentPermutation)) continue;
                unsignedEvenPermutations.Add(currentPermutation);
            }

            int addIndex = startIndex;
            
            foreach (var unsignedPermutation in unsignedEvenPermutations)
            {
                List<float[]> signedPermutations = new List<float[]> { unsignedPermutation.ToArray() };

                for (int i = 0; i < numbers.Length; i++)
                {
                    if (unsignedPermutation[i] == 0) continue;
                    
                    signedPermutations.AddRange(signedPermutations.ToList().Select(perm => (float[])perm.Clone()));
                    for (int j = 0; j < signedPermutations.Count / 2; j++)
                        signedPermutations[j][i] *= -1f;
                }

                foreach (var perm in signedPermutations)
                    vertices[addIndex++] = multiplier * new Vertex(perm[0], perm[1], perm[2], perm[3]);
            }
            
            if (addIndex - startIndex != expectedNumberOfVertices)
                throw new MetaDataGenerationException(
                    $"Expected number of vertices = {expectedNumberOfVertices}, created = {addIndex - startIndex}");
        }

        private static bool IsPermutationEven(float[] permutation)
        {
            int numberOfInversions = 0;
            
            for (int i = 0; i < permutation.Length - 1; i++)
                for (int j = i + 1; j < permutation.Length; j++)
                    if (permutation[i] > permutation[j])
                        numberOfInversions++;

            return numberOfInversions % 2 == 0;
        }

        private static void CalculateAdjacencyList(List<Tuple<int, int>> adjacencyList, Vertex[] vertices, int expectedNumberOfEdgese)
        {
            float minDistance = Single.MaxValue;
            
            for (int i = 0; i < vertices.Length - 1; i++)
            {
                for (int j = i + 1; j < vertices.Length; j++)
                {
                    float curDistance = Vertex.Distance(vertices[i], vertices[j]);

                    if (Math.Abs(curDistance - minDistance) < 0.00001f)
                        adjacencyList.Add(new Tuple<int, int>(i, j));
                    else if (curDistance < minDistance)
                    {
                        adjacencyList.Clear();
                        adjacencyList.Add(new Tuple<int, int>(i, j));
                        minDistance = curDistance;
                    }
                }
            }
            
            if (adjacencyList.Count != expectedNumberOfEdgese)
                throw new MetaDataGenerationException(
                    $"Expected number of edges = {expectedNumberOfEdgese}, created = {adjacencyList.Count}");
        }

        private static int Factorial(this int count) =>   
            count == 0 ? 1 : Enumerable.Range(1, count).Aggregate((i, j) => i*j);
        
    }
}