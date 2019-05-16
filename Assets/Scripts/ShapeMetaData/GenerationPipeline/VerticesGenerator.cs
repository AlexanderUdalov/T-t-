using System;
using System.Collections.Generic;
using System.Linq;
using FourDimensionalSpace;
using UnityEngine;

namespace ShapeMetaData
{
    public class VerticesGenerator : IGenerator
    {
        private static readonly float F = (1 + Mathf.Sqrt(5)) / 2;
        
        private Dictionary<ShapeType, Action<Shape> > CreationFuncs 
            = new Dictionary<ShapeType, Action<Shape>>
        {
            [ShapeType.Cell5]   = Fill5CellData,
            [ShapeType.Cell8]   = Fill8CellData,
            [ShapeType.Cell16]  = Fill16CellData,
            [ShapeType.Cell24]  = Fill24CellData,
            [ShapeType.Cell120] = Fill120CellData,
            [ShapeType.Cell600] = Fill600CellData
        };
        
        public void Execute(Shape shape)
        {
            CreationFuncs[shape.ShapeType](shape);
        }
        
        private static void Fill5CellData(Shape pentachoron)
        {
            pentachoron.Vertices = new[]
            {
                new Vertex(1, 1, 1, 0),
                new Vertex(1, -1, -1, 0),
                new Vertex(-1, 1, -1, 0),
                new Vertex(-1, -1, 1, 0),
                new Vertex(0, 0, 0, Mathf.Sqrt(5))
            };
        }

        private static void Fill8CellData(Shape tesseract)
        {
            InitVerticesPermutation(tesseract.Vertices, new[] {1f, 1f, 1f, 1f}, 0, false, 16);
        }

        private static void Fill16CellData(Shape hexadecachoron)
        {
            InitVerticesPermutation(hexadecachoron.Vertices, new[] {1f, 0f, 0f, 0f}, 0, false, 8);
        }

        private static void Fill24CellData(Shape icositetrachoron)
        {
            InitVerticesPermutation(icositetrachoron.Vertices, new[] {1f, 1f, 1f, 1f}, 0, false, 16);
            InitVerticesPermutation(icositetrachoron.Vertices, new[] {2f, 0f, 0f, 0f}, 16, false, 8);
        }

        private static void Fill120CellData(Shape hecatonicosachoron)
        {
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {0f, 0f, 2f, 2f}, 0, false, 24);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {1f, 1f, 1f, Mathf.Sqrt(5)}, 24, false, 64);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {F, F, F, 1/(F*F)}, 88, false, 64);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {1/F, 1/F, 1/F, F*F}, 152, false, 64);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {0f, 1f, 1/(F*F), F*F}, 216, true, 96);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {0f, 1/F, F, Mathf.Sqrt(5)}, 312, true, 96);
            InitVerticesPermutation(hecatonicosachoron.Vertices, new[] {1f, 2f, 1/F, F}, 408, true, 192);
        }

        private static void Fill600CellData(Shape hexacosichoron)
        {
            InitVerticesPermutation(hexacosichoron.Vertices, new[] {1f, 1f, 1f, 1f}, 0, false, 16);
            InitVerticesPermutation(hexacosichoron.Vertices, new[] {2f, 0f, 0f, 0f}, 16, false, 8);
            InitVerticesPermutation(hexacosichoron.Vertices, new[] {0f, 1f, F, 1/F}, 16 + 8, true, 96);
        }
        
        private static void InitVerticesPermutation(Vertex[] vertices, float[] numbers, int startIndex, bool even,
            int expectedNumberOfVertices, float multiplier = 1f)
        {    
            HashSet<float[]> unsignedPermutations = new HashSet<float[]>(new FloatArrayComparer());
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
                unsignedPermutations.Add(currentPermutation);
            }

            int addIndex = startIndex;
            
            foreach (var unsignedPermutation in unsignedPermutations)
            {
                List<float[]> signedPermutations = new List<float[]> { unsignedPermutation.ToArray() };

                for (int i = 0; i < numbers.Length; i++)
                {
                    if (unsignedPermutation[i] == 0f) continue;
                    
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
    }
}