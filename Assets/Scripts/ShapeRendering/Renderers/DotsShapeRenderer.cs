using System;
using System.Linq;
using Boo.Lang;
using FourDimensionalSpace;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShapeRendering
{
    public class DotsShapeRenderer : BaseShapeRenderer
    {
        protected override string GameObjectName => "DotsShapeRenderer";
        
        private GameObject _vertexPrefab;

        private List<GameObject> _vertices;

        public DotsShapeRenderer()
        {
            _vertexPrefab = Resources.Load("VertexPrefab") as GameObject;
            _vertices = new List<GameObject>();
        }

        public DotsShapeRenderer(Transform parent) : base(parent)
        {
            _vertexPrefab = Resources.Load("VertexPrefab") as GameObject;
            _vertices = new List<GameObject>();
        }

        public override void BuildShapeView()
        {
            _vertices.Clear();

            BuildVertices();
            ModifyShapeView();
        }

        public override void ModifyShapeView()
        {
            if (Shape == null) return;
            
            if (Shape.Vertices.Length != _vertices.Count)
                throw new Exception("Rendering shape doesn't match with shape parameters.");
                
            MoveVertices();
        }

        private void BuildVertices()
        {
            var vertices = Shape.Vertices.Select(vertex => Object.Instantiate(_vertexPrefab, Parent));
            _vertices.AddRange(vertices);
        }


        private void MoveVertices()
        {
            for (var i = 0; i < Shape.Vertices.Length; i++)
            {
                _vertices[i].transform.localPosition = Vertex.ToThridDimensionalSpace(Shape.Vertices[i], PointOfView);
            }
        }
    }
}