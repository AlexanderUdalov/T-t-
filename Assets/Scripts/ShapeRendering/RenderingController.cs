using System.Collections.Generic;
using FourDimensionalSpace;

namespace ShapeRendering
{
    public class RenderingController : IRenderingController
    {
        private Shape _shape;
        
        private readonly List<IShapeRenderer> _renderers = new List<IShapeRenderer>();

        public RenderingController(Shape shape)
        {
            _shape = shape;
        }
            
        public IRenderingController AddRenderer(IShapeRenderer shapeRenderer)
        {
            _renderers.Add(shapeRenderer);
            shapeRenderer.SetShapeData(_shape);
            return this;
        }

        public void SetShapeData(Shape shape)
        {
            _shape = shape;
            _renderers.ForEach(renderer => renderer.SetShapeData(_shape));
        }

        public void BuildShapeView()
        {
            _renderers.ForEach(renderer => renderer.BuildShapeView());
        }

        public void ModifyShapeView()
        {
            _renderers.ForEach(renderer => renderer.ModifyShapeView());
        }
    }
}