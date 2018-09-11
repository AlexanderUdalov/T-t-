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
        private GameObject _edgePrefab;
        private List<LineRenderer> _edges;

        public LinesShapeRenderer()
        {
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
                _edges[i].SetPositions(new[]
                {
                    Vertex.ToThridDimensionalSpace(Shape.Vertices[Shape.AdjacencyList[i].Item1], PointOfView),
                    Vertex.ToThridDimensionalSpace(Shape.Vertices[Shape.AdjacencyList[i].Item2], PointOfView),
                });
            }
        }
    }
}