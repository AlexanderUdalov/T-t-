using FourDimensionalSpace;
using UnityEngine;

namespace ShapeRendering
{
    public abstract class BaseShapeRenderer : IShapeRenderer
    {
        protected Shape Shape;
        protected Transform Parent;
        
        protected abstract string GameObjectName { get; }

        //Если поставить w = 1 все сломается =(
        protected static readonly Vector4 PointOfView = new Vector4(0, 0, 1, 10);

        protected BaseShapeRenderer()
        {
            Parent = new GameObject(GameObjectName).transform;
        }

        public void SetShapeData(Shape shape)
        {
            Object.Destroy(Parent.gameObject);
            Parent = new GameObject(GameObjectName).transform;
            
            Shape = shape;
        }
        
        public abstract void BuildShapeView();
        public abstract void ModifyShapeView();
    }
}