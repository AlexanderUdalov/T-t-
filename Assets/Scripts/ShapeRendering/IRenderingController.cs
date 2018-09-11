using FourDimensionalSpace;

namespace ShapeRendering
{
    public interface IRenderingController
    {
        IRenderingController AddRenderer(IShapeRenderer shapeRenderer);
        
        void SetShapeData(Shape shape);
        void BuildShapeView();
        void ModifyShapeView();
    }
}