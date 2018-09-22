using System;
using UnityEngine;

namespace ShapeRendering
{
    public class ShaderHelper
    {
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

            for (int i = 0; i < oldVertices.Length; i++)
            {
                var displacementVector = (newVertices[i] - oldVertices[i]).normalized;
                float projectionLength = Vector3.Dot(displacementVector, -_camera.forward);

                Debug.Log(projectionLength);
            }
        }
    }
}
