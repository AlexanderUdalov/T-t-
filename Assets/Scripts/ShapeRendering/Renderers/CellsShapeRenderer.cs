using System;
using System.Collections.Generic;
using System.Linq;
using FourDimensionalSpace;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace ShapeRendering
{
    public class CellsShapeRenderer : BaseShapeRenderer
    {
        private GameObject _facePrefab;

        private List<List<MeshFilter>> _filters;

        public CellsShapeRenderer()
        {
            _facePrefab = Resources.Load("FacePrefab") as GameObject;
            _filters = new List<List<MeshFilter>>();
        }

        public override void BuildShapeView()
        {
            _filters.ForEach(list => list.Clear());
            _filters.Clear();

            BuildCells();
            ModifyShapeView();
        }

        private void BuildCells()
        {
            foreach (var cell in Shape.Cells)
            {
                BuildCell(cell);
            }
        }

        private void BuildCell(List<int> cell)
        {
            var listOfFaces = new List<MeshFilter>();

            var color = GenerateColor();

            foreach (var faceIndex in cell)
            {
                var instantiatedFace = Object.Instantiate(_facePrefab, Parent);
                instantiatedFace.GetComponent<MeshRenderer>().material.color = color;

                var meshFilter = instantiatedFace.GetComponent<MeshFilter>();
                meshFilter.mesh = GenerateFaceTriangles(Shape.Faces[faceIndex].Count);
                listOfFaces.Add(meshFilter);
            }
            
            _filters.Add(listOfFaces);
        }

        private Color GenerateColor()
        {
            var randomColor = Random.ColorHSV();
            randomColor.a = 0.2f;

            return randomColor;
        }

        public override void ModifyShapeView()
        {
            if (Shape == null) return;
            
            if (Shape.Cells.Count != _filters.Count)
                throw new Exception("Rendering shape doesn't match with shape parameters.");
            
            for (int i = 0; i < Shape.Cells.Count; i++)
            {
                MoveCell(i);
            }
        }

        private void MoveCell(int cellIndex)
        {
            var cell = Shape.Cells[cellIndex];
            
            for (int i = 0; i < cell.Count; i++)
            {
                var verices = GetVertices(Shape.Faces[cell[i]]);
                
                _filters[cellIndex][i].mesh.vertices = verices;
                _filters[cellIndex][i].mesh.normals  = GetNormals(verices);
            }
        }

        private Vector3[] GetVertices(List<int> face)
        {
            return face.Select(index => Vertex.ToThridDimensionalSpace(Shape.Vertices[index], PointOfView)).ToArray();
        }

        private Vector3[] GetNormals(Vector3[] vertices)
        {
            var normal = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]).normalized;

            var normals = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                normals[i] = normal;

            return normals;
        }

        private static Mesh GenerateFaceTriangles(int verticesCount)
        {
            if (verticesCount < 3)
                throw new Exception(
                    $"Face with {verticesCount} passed in GenerateFaceMesh function. It must have at least 3 vertices.");

            var triangles = new int[(verticesCount - 2) * 3];
            for (int i = 0; i < verticesCount - 2; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            var faceMesh = new Mesh
            {
                vertices = new Vector3[triangles.Length],
                normals = new Vector3[triangles.Length],
                triangles = triangles
            };

            return faceMesh;
        }
    }
}