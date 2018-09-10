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
        private GameObject _cellPrefab;

        private List<MeshFilter> _filters;
        private List<MeshRenderer> _renderers;

        public CellsShapeRenderer(Shape shape) : base(shape)
        {
            _cellPrefab = Resources.Load("CellPrefab") as GameObject;

            _filters = new List<MeshFilter>();
            _renderers = new List<MeshRenderer>();
        }

        public override void BuildShapeView()
        {
            _filters.Clear();
            _renderers.Clear();

            for (int i = 0; i < Shape.Cells.Count; i++)
            {
                var cellGameObject = Object.Instantiate(_cellPrefab, Parent);

                _filters.Add(cellGameObject.GetComponent<MeshFilter>());
                _renderers.Add(cellGameObject.GetComponent<MeshRenderer>());
            }

            foreach (var meshRenderer in _renderers)
            {
                var randomColor = Random.ColorHSV();
                randomColor.a = 0.2f;

                meshRenderer.material.color = randomColor;
            }
            
            ModifyShapeView();
        }

        public override void ModifyShapeView()
        {
            if (Shape == null) return;
            
            if (Shape.Cells.Count != _filters.Count || Shape.Cells.Count != _renderers.Count)
                throw new Exception("Rendering shape doesn't match with shape parameters.");
            
            for (int i = 0; i < Shape.Cells.Count; i++)
            {
                _filters[i].mesh = GenerateCellMesh(Shape.Cells[i]);
            }
        }

        private Mesh GenerateCellMesh(List<int> cell)
        {
            var combineMeshes = new CombineInstance[cell.Count];

            for (int i = 0; i < cell.Count; i++)
            {
                combineMeshes[i].mesh = GenerateFaceMesh(Shape.Faces[cell[i]]);
            }

            var cellMesh = new Mesh();
            cellMesh.CombineMeshes(combineMeshes, true, false);
            return cellMesh;
        }

        private Mesh GenerateFaceMesh(List<int> face)
        {
            if (face.Count < 3)
                throw new Exception(
                    $"Face with {face.Count} passed in GenerateFaceMesh function. It must have at least 3 vertices.");

            var positions3D = face.Select(index => Vertex.ToThridDimensionalSpace(Shape.Vertices[index], PointOfView)).ToArray();
            
            var normal = Vector3.Cross(positions3D[1] - positions3D[0], positions3D[2] - positions3D[0]).normalized;

            var normals = new Vector3[face.Count];
            for (int i = 0; i < face.Count; i++)
                normals[i] = normal;

            var triangles = new int[(face.Count - 2) * 3];
            for (int i = 0; i < face.Count - 2; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            var faceMesh = new Mesh
            {
                vertices = positions3D,
                normals = normals,
                triangles = triangles
            };

            return faceMesh;
        }
    }
}