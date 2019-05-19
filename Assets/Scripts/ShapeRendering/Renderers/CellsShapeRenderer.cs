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
        protected override string GameObjectName => "CellsShapeRenderer";

        private const float DefaultAlphaValue = 0.5f;
        
        private GameObject _facePrefab;

        //Cell<Face<Mesh>>
        private List<List<MeshFilter>> _filters;

        private ShaderHelper _shaderHelper;

        private List<Color> _avaliableColors = new List<Color>()
        {
             Color.red
            ,Color.green
            ,Color.blue
        };

        public CellsShapeRenderer(ShaderHelper helper = null)
        {
            _shaderHelper = helper;
            _facePrefab = Resources.Load("FacePrefab") as GameObject;
            _filters = new List<List<MeshFilter>>();
        }

        public CellsShapeRenderer(Transform parent, ShaderHelper helper = null): base(parent)
        {
            _shaderHelper = helper;
            _facePrefab = Resources.Load("FacePrefab") as GameObject;
            _filters = new List<List<MeshFilter>>();
        }

        public override void BuildShapeView()
        {
            _filters.ForEach(list => list.Clear());
            _filters.Clear();

            BuildCells();
        }

        private void BuildCells()
        {
            foreach (var cell in Shape.Cells)
            {
                BuildCell(cell);
            }
        }

        private void BuildCell(Cell cell)
        {
            var listOfFaceMeshes = new List<MeshFilter>();

            var cellParent = new GameObject("CellParent");
            cellParent.transform.SetParent(Parent);

            var color = GenerateColor();

            foreach (var faceIndex in cell.ToList())
            {
                Shape.Faces[faceIndex].ToList().Reverse();
                var instantiatedFace = Object.Instantiate(_facePrefab, cellParent.transform);

                var meshFilter = instantiatedFace.GetComponent<MeshFilter>();
                meshFilter.mesh = GenerateFaceMesh(Shape.Faces[faceIndex].Count);
                listOfFaceMeshes.Add(meshFilter);

                CalculateVertices(meshFilter.mesh, Shape.Faces[faceIndex], color);
            }

            _filters.Add(listOfFaceMeshes);
        }

        private Color GenerateColor()
        {
            return _avaliableColors[Random.Range(0, _avaliableColors.Count)];
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
                RecalculateVertices(_filters[cellIndex][i].mesh, Shape.Faces[cell.GetByIndex(i)]);
        }

        private void RecalculateVertices(Mesh mesh, Face face)
        {
            var vertices = GetVertices(face);

            _shaderHelper?.WriteAlpha(mesh, mesh.vertices, vertices);
            ModifyMesh(mesh, vertices);
        }

        private void CalculateVertices(Mesh mesh, Face face, Color faceColor)
        {
            var vertices = GetVertices(face);

            ModifyMesh(mesh, vertices);

            Color[] colors = new Color[vertices.Length];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = new Color(faceColor.r, faceColor.g, faceColor.b, DefaultAlphaValue);
            

            mesh.colors = colors;
        }
        
        private void ModifyMesh(Mesh mesh, Vector3[] vertices)
        {
            mesh.vertices = vertices;
            mesh.normals = GetNormals(vertices);
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
        }

        private Mesh GenerateFaceMesh(int verticesCount)
        {
            var triangles = GetTriangles(verticesCount);
            
            var faceMesh = new Mesh
            {
                vertices = new Vector3[triangles.Length],
                normals = new Vector3[triangles.Length],
                triangles = triangles
            };
            faceMesh.MarkDynamic();

            return faceMesh;
        }

        private Vector3[] GetVertices(Face face)
        {
            return face.ToList().Select(index => Vertex.ToThridDimensionalSpace(Shape.Vertices[index], PointOfView)).ToArray();
        }

        private Vector3[] GetNormals(Vector3[] vertices)
        {
            var normal = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]).normalized;

            var normals = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                normals[i] = normal;

            return normals;
        }

        private int[] GetTriangles(int verticesCount)
        {
            if (verticesCount < 3)
                throw new Exception(
                    $"Face with {verticesCount} passed in GenerateFaceMesh function. It must have at least 3 vertices.");

            var triangles = new int[(verticesCount - 2) * 3];
            for (int i = 0; i < verticesCount - 2; i++)
            {
                triangles[i * 3] = i + 2;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = 0;
            }

            return triangles;
        }
    }
}