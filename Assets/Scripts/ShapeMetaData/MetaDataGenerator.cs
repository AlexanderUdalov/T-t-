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

        private static readonly Dictionary<ShapeType, int> VerticesOnFace = new Dictionary<ShapeType, int>
        {
            [ShapeType.Cell5]   = 3,
            [ShapeType.Cell8]   = 4,
            [ShapeType.Cell16]  = 3,
            [ShapeType.Cell24]  = 3,
            [ShapeType.Cell120] = 5,
            [ShapeType.Cell600] = 3
        };

        private static readonly Dictionary<ShapeType, int> FacesOnCell = new Dictionary<ShapeType, int>
        {
            [ShapeType.Cell5]   = 4,
            [ShapeType.Cell8]   = 6,
            [ShapeType.Cell16]  = 4,
            [ShapeType.Cell24]  = 8,
            [ShapeType.Cell120] = 12,
            [ShapeType.Cell600] = 4
        };
            
        
        public static void GenerateDataFile(ShapeType shapeType)
        {    
            Shape shape = CreationFuncs[shapeType]();
            CalculateAdjacencyList(shape, shape.AdjacencyList.Capacity);
            CalculateFaces(shape, VerticesOnFace[shapeType], shape.Faces.Capacity);
            CalculateCells(shape, FacesOnCell[shapeType], shape.Cells.Capacity);

            Debug.Log("Все чётко!");
            string data = JsonConvert.SerializeObject(shape);
            string writePath = Path.Combine(Application.streamingAssetsPath, "ShapeMetaData", shapeType + ".json");
                        
            File.WriteAllText(writePath, data);
        }

        private static Shape Create5CellData()
        {
            Shape pentachoron = new Shape(5, 10, 10, 5)
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
            Shape tesseract = new Shape(16, 32, 24, 8);
            InitVerticesPermutation(tesseract.Vertices, new[] {1f, 1f, 1f, 1f}, 0, false, 16);
            return tesseract;
        }

        private static Shape Create16CellData()
        {
            Shape hexadecachoron = new Shape(8, 24, 32, 16);
            InitVerticesPermutation(hexadecachoron.Vertices, new[] {1f, 0f, 0f, 0f}, 0, false, 8);
            return hexadecachoron;
        }

        private static Shape Create24CellData()
        {
            Shape icositetrachoron = new Shape(24, 96, 96, 24);
            InitVerticesPermutation(icositetrachoron.Vertices, new[] {1f, 1f, 1f, 1f}, 0, false, 16);
            InitVerticesPermutation(icositetrachoron.Vertices, new[] {2f, 0f, 0f, 0f}, 16, false, 8);
            return icositetrachoron;
        }

        private static Shape Create120CellData()
        {
            Shape hecatonicosachoron = new Shape(600, 1200, 720, 120);
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
            Shape hexacosichoron = new Shape(120, 720, 1200, 600);
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

        private static void CalculateAdjacencyList(Shape shape, int expectedNumberOfEdges)
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
        
        private static void CalculateFaces(Shape shape, int limit, int expectedNumberOfFaces)
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

        private static void FacesCycleDFS(Shape shape, int u, int endV, byte[] color, int unavailableEdge, int limit,
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
                    if (cycle.Where((t, i) => t == face[i]).Count() == cycle.Count)
                        return;

                shape.Faces.Add(cycle);
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

        private static void PutMinElementInZeroPosition(List<int> cycle)
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

        /*private static void CalculateCells(Shape shape, int limit, int expectedNumberOfCells)
        {
            List<Tuple<int, int>> facesAdjacency = new List<Tuple<int, int>>();

            for (int i = 0; i < shape.Faces.Count - 1; i++)
            {
                for (int j = i + 1; j < shape.Faces.Count; j++)
                {
                    if (FacesConnected(shape.Faces[i], shape.Faces[j]))
                        facesAdjacency.Add(new Tuple<int, int>(i, j));
                }
            }
            
            byte[] used = new byte[shape.Faces.Count];
            
            for (int i = 0; i < shape.Faces.Count; i++)
            {
                if (used[i] == 1)
                    continue;
                
                byte[] color = new byte[shape.Faces.Count];
                HashSet<int> cell = new HashSet<int> {i};
                CellsCycleDFS(shape.Cells, facesAdjacency, i, i, color, -1, limit, cell, used);
            }

            for (int i = 0; i < shape.Cells.Count; i++)
                Debug.Log(i + ": " + shape.Cells[i].ToList().CycleToString());   
            
            if (shape.Cells.Count != expectedNumberOfCells)
                throw new MetaDataGenerationException(
                    $"Expected number of cells = {expectedNumberOfCells}, created = {shape.Cells.Count}");
        }*/

            
        private static void CalculateCells(Shape shape, int facesOnCellCount, int expectedNumberOfCells)
        {
            #region Init
            int preLastIndex = 0, lastIndex = 0;
            int lastFaceIndex = -1, preLastFaceIndex = 0;
            List<int> invertedFaces = new List<int>();
            HashSet<int> avaliableIndexes = new HashSet<int>();
            for (int i = 0; i < shape.Faces.Count; i++)
                avaliableIndexes.Add(i);
            #endregion

            #region Debug
            //List<int>[] facesAdjacency = new List<int>[shape.Faces.Count];
            //for (int i = 0; i < facesAdjacency.Length; i++)
            //    facesAdjacency[i] = new List<int>();

            //for (int i = 0; i < shape.Faces.Count - 1; i++)
            //    for (int j = i + 1; j < shape.Faces.Count; j++)
            //        if (FacesConnected(shape.Faces[j], shape.Faces[i]))
            //        {
            //            facesAdjacency[i].Add(j);
            //            facesAdjacency[j].Add(i);
            //        }
            //foreach (var face in shape.Faces)
            //{
            //    string toDebug = "";
            //    foreach (var i in face)
            //    {
            //        toDebug += i + ", ";
            //    }
            //    Debug.Log("Face: " + toDebug);
            //}
            //return;
            #endregion


            while (avaliableIndexes.Count > 0)
            {
                var newCell = new List<int>();

                bool findLastFace = FindSecondFaceIndex(shape, preLastFaceIndex,
                    ref lastIndex, ref lastFaceIndex, new HashSet<int>(avaliableIndexes.Except(invertedFaces)));  

                newCell.Add(preLastFaceIndex);
                newCell.Add(lastFaceIndex);
                
                FindCellRecuresive(newCell, shape, preLastFaceIndex, lastFaceIndex, avaliableIndexes);

                #region Check error
                bool selfInverted = false;
                foreach (var cell in shape.Cells)
                    if (IsSelfInverted(shape.Faces, newCell, cell))
                        selfInverted = true;

                if (newCell.Count != facesOnCellCount || selfInverted)
                {

                    if (!findLastFace) lastIndex = avaliableIndexes.Count;
                    if (lastIndex >= avaliableIndexes.Count)
                    {
                        lastIndex = 0;
                        preLastIndex++;
                        preLastFaceIndex = avaliableIndexes.ElementAt(preLastIndex);
                    }
                    continue;
                }
                #endregion
                #region if true cell
                shape.Cells.Add(newCell);
                foreach (var item in newCell)
                    avaliableIndexes.Remove(item);

                preLastIndex = 0;
                lastIndex = 0;
                if (avaliableIndexes.Count > 0)
                {
                    invertedFaces = GetInvertFaces(shape.Faces, avaliableIndexes, newCell);
                    if (invertedFaces.Count > preLastIndex)
                        preLastFaceIndex = invertedFaces[preLastIndex];
                    else preLastFaceIndex = avaliableIndexes.First();
                }
                #endregion
            }
            #region Legacy

            //List<int>[] facesAdjacency = new List<int>[shape.Faces.Count];
            //for (int i = 0; i < facesAdjacency.Length; i++)
            //    facesAdjacency[i] = new List<int>();

            //for (int i = 0; i < shape.Faces.Count - 1; i++)
            //    for (int j = i + 1; j < shape.Faces.Count; j++)
            //        if (FacesConnected(shape.Faces[j], shape.Faces[i]))
            //        {
            //            facesAdjacency[i].Add(j);
            //            facesAdjacency[j].Add(i);
            //        }



            //byte[] used = new byte[shape.Faces.Count];

            //for (int i = 0; i < shape.Faces.Count; i++)
            //{
            //    if (used[i] == 1)
            //        continue;

            //    var foundCell = FindCell(i, facesAdjacency, facesOnCellCount, shape.Faces[0].Count - 1);
            //    foreach (var faceIndex in foundCell)
            //        used[faceIndex] = 1;
            //    shape.Cells.Add(foundCell);
            //}

            //for (int i = 0; i < shape.Cells.Count; i++)
            //    Debug.Log(i + ": " + shape.Cells[i].ToList().CycleToString());

            //if (shape.Cells.Count != expectedNumberOfCells)
            //    throw new MetaDataGenerationException(
            //        $"Expected number of cells = {expectedNumberOfCells}, created = {shape.Cells.Count}");
            #endregion
        }


        public static void FindCellRecuresive(List<int> newCell, Shape shape, 
            int preLastFaceIndex, int lastFaceIndex, HashSet<int> avaliableIndexes)
        {
            var connectedFaces = new List<int>();
            foreach (var currentIndex in avaliableIndexes)
            {
                if (newCell.Contains(currentIndex)) continue;

                if (FacesConnected(shape.Faces[currentIndex], shape.Faces[preLastFaceIndex]) &&
                    FacesConnected(shape.Faces[currentIndex], shape.Faces[lastFaceIndex]))
                {
                    connectedFaces.Add(currentIndex);
                    newCell.Add(currentIndex);
                }
            }

            foreach (int faceIndex in connectedFaces)
            {
                FindCellRecuresive(newCell, shape, preLastFaceIndex, faceIndex, avaliableIndexes);
                FindCellRecuresive(newCell, shape, lastFaceIndex, faceIndex, avaliableIndexes);
            }
        }



        private static bool FindSecondFaceIndex(Shape shape, int preLastFaceIndex,
            ref int lastIndex, ref int lastFaceIndex, HashSet<int> avaliableForFirstPairIndexes)
        {
            for (int j = lastIndex; j < avaliableForFirstPairIndexes.Count; j++)
            {
                var currentIndex = avaliableForFirstPairIndexes.ElementAt(j);
                if (FacesConnected(shape.Faces[preLastFaceIndex], shape.Faces[currentIndex]))
                {
                    lastFaceIndex = currentIndex;
                    lastIndex = j + 1;
                    return true;
                }
            }
            return false;
        }



        public static List<int> GetInvertFaces(List<List<int>> faces, HashSet<int> avaliableIndexes, List<int> currentCell)
        {
            var invertFacesIndexes = new List<int>();
            foreach (var faceIndex in currentCell)
            {
                invertFacesIndexes.Add(faces.IndexOf(faces.FirstOrDefault((x) => IsInverted(x, faces[faceIndex]))));
            }
            return invertFacesIndexes.Intersect(avaliableIndexes).ToList();
        }

        public static bool IsInverted(List<int> face1, List<int> face2)
        {
            bool result = true;
            for (int i = 0; i < face1.Count; i++)
            {
                if (face1[i] != face2[face1.Count - 1 - i]) result = false;
            }

            return result;
        }

        public static bool IsSelfInverted(List<List<int>> faces, List<int> cell1, List<int> cell2)
        {
            HashSet<int> vertices1 = new HashSet<int>();
            HashSet<int> vertices2 = new HashSet<int>();
            for (int i = 0; i < cell1.Count; i++)
                for (int j = 0; j < faces[0].Count; j++)
                {
                    vertices1.Add(faces[cell1[i]][j]);
                    vertices2.Add(faces[cell2[i]][j]);
                }

            return vertices1.SetEquals(vertices2);
        }
        #region Legacy
        //private static List<int> FindCell(int startIndex, List<int>[] adjacencyList, int limit, int vertexFaceCount)
        //{
        //    int numberOfDescends = vertexFaceCount - 1;

        //    Dictionary<int, int> numberOfAdjacency = new Dictionary<int, int>();
        //    HashSet<int> activeFaces = new HashSet<int> {startIndex};
        //    HashSet<int> calculatedFaces = new HashSet<int>();

        //    for (int i = 0; i < numberOfDescends; i++)
        //    {
        //        foreach (var faceIndex in activeFaces.ToList())
        //        {
        //            foreach (var adjFace in adjacencyList[faceIndex])
        //            {
        //                if (!calculatedFaces.Contains(adjFace))
        //                    activeFaces.Add(adjFace);

        //                if (!numberOfAdjacency.ContainsKey(adjFace))
        //                    numberOfAdjacency[adjFace] = 1;
        //                else
        //                    numberOfAdjacency[adjFace]++;
        //            }

        //            activeFaces.Remove(faceIndex);
        //            calculatedFaces.Add(faceIndex);
        //        }
        //    }

        //    List<int> foundCell = numberOfAdjacency.Where(keyValue => keyValue.Value == vertexFaceCount)
        //        .Select(keyValue => keyValue.Key).ToList();

        //    return foundCell;
        //}
        #endregion

        private static void CellsCycleDFS(List<HashSet<int>> cells, List<Tuple<int, int>> facesAdjacency, int u, int endV,
            byte[] color, int unavailableEdge, int limit, HashSet<int> currentCell, byte[] used)
        {
            if (currentCell.Count > limit)
                return;
            
            if (u != endV)
                color[u] = 1;
            else if (currentCell.Count == limit)
            {
                // добавляем найденный цикл только в случае, если раньше он не встречался
                foreach (var cell in cells)
                    if (currentCell.SetEquals(cell))
                        return;
                
                cells.Add(currentCell);
                foreach (var faceIndex in currentCell)
                    used[faceIndex] = 1;
                return;
            }
            
            for (int w = 0; w < facesAdjacency.Count; w++)
            {
                if (w == unavailableEdge)
                    continue;
                if (color[facesAdjacency[w].Item2] == 0 && facesAdjacency[w].Item1 == u)
                {
                    HashSet<int> newCycle = new HashSet<int>(currentCell) { facesAdjacency[w].Item2 };
                    CellsCycleDFS(cells, facesAdjacency, facesAdjacency[w].Item2, endV, color, w, limit, newCycle, used);
                    color[facesAdjacency[w].Item2] = 0;
                }
                else if (color[facesAdjacency[w].Item1] == 0 && facesAdjacency[w].Item2 == u)
                {
                    HashSet<int> newCycle = new HashSet<int>(currentCell) { facesAdjacency[w].Item1 };
                    CellsCycleDFS(cells, facesAdjacency, facesAdjacency[w].Item1, endV, color, w, limit, newCycle, used);
                    color[facesAdjacency[w].Item1] = 0;
                }
            }
        }

        private static bool FacesConnected(List<int> f1, List<int> f2)
        {
            

            List<int> edge = new List<int>(f1.Intersect(f2).Distinct());

            if (edge.Count != 2)
                return false;

            for (int i = 0; i < f1.Count - 1; i++)
            {
                for (int j = 0; j < f2.Count - 1; j++)
                {
                    if (f1[i] == f2[j + 1] && f1[i + 1] == f2[j]) return true;
                }
            }
            return false;
            //List<int> edge = new List<int>(f1.Intersect(f2).Distinct());

            //if (edge.Count != 2)
            //    return false;

            //return f1.IndexOf(edge[0]) - f1.IndexOf(edge[1]) == f2.IndexOf(edge[1]) - f2.IndexOf(edge[0]) ||
            //       f1.IndexOf(edge[0]) == 0 && f1.IndexOf(edge[1]) == 1 && f2.IndexOf(edge[1]) == f2.Count - 2 ||
            //       f1.IndexOf(edge[1]) == 0 && f1.IndexOf(edge[0]) == 1 && f2.IndexOf(edge[0]) == f2.Count - 2 ||
            //       f2.IndexOf(edge[0]) == 0 && f2.IndexOf(edge[1]) == 1 && f1.IndexOf(edge[1]) == f1.Count - 2 ||
            //       f2.IndexOf(edge[1]) == 0 && f2.IndexOf(edge[0]) == 1 && f1.IndexOf(edge[0]) == f1.Count - 2;
        }
        
        private static string CycleToString(this List<int> list)
        {
            string r = list[0].ToString();

            for (int i = 1; i < list.Count; i++)
                r += "-" + list[i];

            return r;
        }

        private static int Factorial(this int count) =>   
            count == 0 ? 1 : Enumerable.Range(1, count).Aggregate((i, j) => i*j);   
    }
}