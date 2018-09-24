using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeRendering
{
    public class ShaderHelper
    {
        private const float MaxTransparency = 0.5f;
        private Transform _camera;

        public ShaderHelper(Transform camera)
        {
            _camera = camera;
        }

        public void WriteAlpha(Mesh mesh, Vector3[] oldVertices, Vector3[] newVertices)
        {
            if (oldVertices.Length != newVertices.Length ||
                oldVertices.Length != mesh.vertices.Length)
                throw new ArgumentException($"Old count: {oldVertices.Length}; New count: {newVertices.Length}");

            var colors = new List<Color>(mesh.colors);
            for (int i = 0; i < oldVertices.Length; i++)
            {
                colors[i] = new Color(
                    colors[i].r,
                    colors[i].g,
                    colors[i].b,
                    CalculateAlpha(oldVertices[i], newVertices[i]));
            }
            mesh.SetColors(colors);
        }

        public float CalculateAlpha(Vector3 oldPosition, Vector3 newPosition)
        {
            var displacementVector = (newPosition - oldPosition).normalized;
            float projectionLength = Vector3.Dot(displacementVector, -_camera.forward);

            return CalculateVertexTransparency(projectionLength);
        }

        private float CalculateVertexTransparency(float projectionLength)
            => Mathf.Clamp01(projectionLength) * MaxTransparency;
    }
}
