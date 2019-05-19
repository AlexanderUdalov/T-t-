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
        protected override string GameObjectName => "LinesShapeRenderer";
        
        private GameObject _edgePrefab;
        private List<LineRenderer> _edges;
        private ShaderHelper _shaderHelper;

        public LinesShapeRenderer(ShaderHelper helper = null)
        {
            _shaderHelper = helper;
            _edgePrefab = Resources.Load("EdgePrefab") as GameObject;
            _edges = new List<LineRenderer>();
        }

        public LinesShapeRenderer(Transform parent, ShaderHelper helper = null) : base(parent)
        {
            _shaderHelper = helper;
            _edgePrefab = Resources.Load("EdgePrefab") as GameObject;
            _edges = new List<LineRenderer>();
        }

        public override void BuildShapeView()
        {
            _edges.Clear();

            BuildEdges();            
            ModifyShapeView();
        }

        public override void ModifyShapeView()
        {
            if (Shape == null) return;
            
            if (Shape.AdjacencyList.Count != _edges.Count)
                throw new Exception("Rendering shape doesn't match with shape parameters.");
                
            MoveEdges();
        }


        private void BuildEdges()
        {
            var edges = Shape.AdjacencyList.Select(pair =>
                Object.Instantiate(_edgePrefab, Parent).GetComponent<LineRenderer>());
            _edges.AddRange(edges);
        }

        private void MoveEdges()
        {
            for (var i = 0; i < Shape.AdjacencyList.Count; i++)
            {
                var oldFirst = _edges[i].GetPosition(0);
                var oldSecond = _edges[i].GetPosition(1);

                var newFirst = Vertex.ToThridDimensionalSpace(
                    Shape.Vertices[Shape.AdjacencyList[i].Item1], PointOfView);
                var newSecond = Vertex.ToThridDimensionalSpace(
                    Shape.Vertices[Shape.AdjacencyList[i].Item2], PointOfView);

                _edges[i].SetPositions(new[] { Parent.TransformPoint(newFirst), Parent.TransformPoint(newSecond) });

                if (_shaderHelper != null) {
                    float startColorAlpha = _shaderHelper.CalculateAlpha(oldFirst, newFirst);
                    float endColorAlpha = _shaderHelper.CalculateAlpha(oldSecond, newSecond);

                    _edges[i].startColor = new Color(
                        _edges[i].startColor.r,
                        _edges[i].startColor.g,
                        _edges[i].startColor.b,
                        startColorAlpha
                        );

                    _edges[i].endColor = new Color(
                        _edges[i].endColor.r,
                        _edges[i].endColor.g,
                        _edges[i].endColor.b,
                        endColorAlpha
                        );
                }
            }
        }
    }
}