using System;
using System.Linq;
using Boo.Lang;
using FourDimensionalSpace;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShapeRendering
{
    public class LinesShapeRenderer : BaseShapeRenderer
    {
        private GameObject _vertexPrefab;
        private GameObject _edgePrefab;

        private List<GameObject> _vertices;
        private List<LineRenderer> _edges;

        public LinesShapeRenderer(Shape shape) : base(shape)
        {
            _vertexPrefab = Resources.Load<GameObject>("VertexPrefab");
            _edgePrefab = Resources.Load<GameObject>("EdgePrefab");
            
            _vertices = new List<GameObject>();
            _edges = new List<LineRenderer>();
        }

        public override void BuildShapeView()
        {
            _vertices.Clear();
            _edges.Clear();

            BuildVertices();
            BuildEdges();
            
            ModifyShapeView();
        }

        public override void ModifyShapeView()
        {
            if (Shape == null) return;
            
            if (Shape.Vertices.Length != _vertices.Count || Shape.AdjacencyList.Count != _edges.Count)
                throw new Exception("Rendering shape doesn't match with shape parameters.");
                
            MoveVertices();
            MoveEdges();
        }

        private void BuildVertices()
        {
            var vertices = Shape.Vertices.Select(vertex => Object.Instantiate(_vertexPrefab, Parent));
            _vertices.AddRange(vertices);
        }

        private void BuildEdges()
        {
            var edges = Shape.AdjacencyList.Select(pair =>
                Object.Instantiate(_edgePrefab, Parent).GetComponent<LineRenderer>());
            _edges.AddRange(edges);
        }

        private void MoveVertices()
        {
            for (var i = 0; i < Shape.Vertices.Length; i++)
            {
                _vertices[i].transform.position = Vertex.ToThridDimensionalSpace(Shape.Vertices[i], PointOfView);
            }
        }

        private void MoveEdges()
        {
            for (var i = 0; i < Shape.AdjacencyList.Count; i++)
            {
                _edges[i].SetPositions(new[]
                {
                    Vertex.ToThridDimensionalSpace(Shape.Vertices[Shape.AdjacencyList[i].Item1], PointOfView),
                    Vertex.ToThridDimensionalSpace(Shape.Vertices[Shape.AdjacencyList[i].Item2], PointOfView),
                });
            }
        }
    }
}