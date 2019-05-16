using System;
using System.Collections.Generic;
using System.Linq;
using FourDimensionalSpace;

namespace ShapeMetaData
{
    public class CellsGenerator : IGenerator
    {
        private static readonly Dictionary<ShapeType, int> FacesOnCell = new Dictionary<ShapeType, int>
        {
            [ShapeType.Cell5]   = 4,
            [ShapeType.Cell8]   = 6,
            [ShapeType.Cell16]  = 4,
            [ShapeType.Cell24]  = 8,
            [ShapeType.Cell120] = 12,
            [ShapeType.Cell600] = 4
        };
        
        public void Execute(Shape shape)
        {
            CalculateCells(shape, FacesOnCell[shape.ShapeType], shape.Cells.Capacity);
        }
        
        private static void CalculateCells(Shape shape, int facesOnCellCount, int expectedNumberOfCells)
        {
            int preLastIndex = 0, lastIndex = 0;
            int lastFaceIndex = -1, preLastFaceIndex = 0;
            List<int> invertedFaces = new List<int>();
            HashSet<int> avaliableIndexes = new HashSet<int>();
            for (int i = 0; i < shape.Faces.Count; i++)
                avaliableIndexes.Add(i);

            while (avaliableIndexes.Count > 0)
            {
                var newCell = new Cell();

                bool findLastFace = FindSecondFaceIndex(shape, preLastFaceIndex,
                    ref lastIndex, ref lastFaceIndex, new HashSet<int>(avaliableIndexes.Except(invertedFaces)));  

                newCell.AddFaceIndex(preLastFaceIndex);
                newCell.AddFaceIndex(lastFaceIndex);
                
                FindCellRecuresive(newCell, shape, preLastFaceIndex, lastFaceIndex, avaliableIndexes);

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
                
                shape.Cells.Add(newCell);
                foreach (var item in newCell.ToList())
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
            }

            if (shape.Cells.Count != expectedNumberOfCells)
                throw new MetaDataGenerationException(
                    $"Expected number of cells = {expectedNumberOfCells}, created = {shape.Cells.Count}");
        }


        public static void FindCellRecuresive(Cell newCell, Shape shape, 
            int preLastFaceIndex, int lastFaceIndex, HashSet<int> avaliableIndexes)
        {
            var connectedFaces = new List<int>();
            foreach (var currentIndex in avaliableIndexes)
            {
                if (newCell.ToList().Contains(currentIndex)) continue;

                if (FacesConnected(shape.Faces[currentIndex], shape.Faces[preLastFaceIndex]) &&
                    FacesConnected(shape.Faces[currentIndex], shape.Faces[lastFaceIndex]))
                {
                    connectedFaces.Add(currentIndex);
                    newCell.AddFaceIndex(currentIndex);
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

        public static List<int> GetInvertFaces(List<Face> faces, HashSet<int> avaliableIndexes, Cell currentCell)
        {
            var invertFacesIndexes = new List<int>();
            foreach (var faceIndex in currentCell.ToList())
            {
                invertFacesIndexes.Add(faces.IndexOf(faces.FirstOrDefault(x => IsInverted(x, faces[faceIndex]))));
            }
            return invertFacesIndexes.Intersect(avaliableIndexes).ToList();
        }

        public static bool IsInverted(Face face1, Face face2)
        {
            for (int i = 0; i < face1.Count; i++)
            {
                if (face1.GetByIndex(i) != face2.GetByIndex(face1.Count - 1 - i)) 
                    return false;
            }

            return true;
        }

        public static bool IsSelfInverted(List<Face> faces, Cell cell1, Cell cell2)
        {
            HashSet<int> vertices1 = new HashSet<int>();
            HashSet<int> vertices2 = new HashSet<int>();
            for (int i = 0; i < cell1.Count; i++)
                for (int j = 0; j < faces[0].Count; j++)
                {
                    vertices1.Add(faces[cell1.GetByIndex(i)].GetByIndex(j));
                    vertices2.Add(faces[cell2.GetByIndex(i)].GetByIndex(j));
                }

            return vertices1.SetEquals(vertices2);
        }

        private static bool FacesConnected(Face face1, Face face2)
        {
            List<int> edge = new List<int>(face1.ToList().Intersect(face2.ToList()).Distinct());

            if (edge.Count != 2)
                return false;

            for (int i = 0; i < face1.Count - 1; i++)
            {
                for (int j = 0; j < face2.Count - 1; j++)
                {
                    if (face1.GetByIndex(i) == face2.GetByIndex(j + 1) && 
                        face1.GetByIndex(i + 1) == face2.GetByIndex(j)) return true;
                }
            }
            return false;
        }
    }
}