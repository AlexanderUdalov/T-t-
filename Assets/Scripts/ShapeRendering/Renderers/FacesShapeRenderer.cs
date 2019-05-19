using System;
using System.Collections.Generic;
using System.Linq;
using FourDimensionalSpace;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace ShapeRendering
{
    public class FacesShapeRenderer : BaseShapeRenderer
    {
        protected override string GameObjectName => "FacesShapeRenderer";

        private const float DefaultAlphaValue = 0.5f;
        
        private GameObject _facePrefab;

        private List<MeshFilter> _filters;

        private ShaderHelper _shaderHelper;

        private List<Color> _avaliableColors = new List<Color>()
        {
             Color.red
//            ,Color.green
//            ,Color.blue
        };

        public FacesShapeRenderer(ShaderHelper helper = null)
        {
            _shaderHelper = helper;
            _facePrefab = Resources.Load("FacePrefab") as GameObject;
            _filters = new List<MeshFilter>();
        }

        public FacesShapeRenderer(Transform parent, ShaderHelper helper = null): base(parent)
        {
            _shaderHelper = helper;
            _facePrefab = Resources.Load("FacePrefab") as GameObject;
            _filters = new List<MeshFilter>();
        }

        public override void BuildShapeView()
        {
            _filters.Clear();

            BuildFaces();
        }

        private void BuildFaces()
        {
            for (int i = 0; i < Shape.Faces.Count; i++)
            {
                BuildFace(Shape.Faces[i], i);

            }
        }

        private void BuildFace(Face face, int index)
        {
            var color = GenerateColor();

            face.ToList().Reverse();
            var instantiatedFace = Object.Instantiate(_facePrefab, Parent);
            instantiatedFace.name = "Face" + index;

            var meshFilter = instantiatedFace.GetComponent<MeshFilter>();
            meshFilter.mesh = GenerateFaceMesh(face.Count);

            CalculateVertices(meshFilter.mesh, face, color);

            _filters.Add(meshFilter);
        }

        private Color GenerateColor()
        {
            return _avaliableColors[Random.Range(0, _avaliableColors.Count)];
        }

        public override void ModifyShapeView()
        {
            if (Shape == null) return;
            
            if (Shape.Faces.Count != _filters.Count)
                throw new Exception("Rendering shape doesn't match with shape parameters.");
            
            for (int i = 0; i < Shape.Faces.Count; i++)
            {
                MoveFace(i);
            }
        }

        private void MoveFace(int faceIndex)
        {
            var face = Shape.Faces[faceIndex];
            RecalculateVertices(_filters[faceIndex].mesh, face);
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