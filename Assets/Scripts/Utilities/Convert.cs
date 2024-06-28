using UnityEngine;

namespace Utilities
{
    public static class Convert
    {
        public static Vector3 ScreenToWorld(Camera camera, Vector3 position)
        {
            position.z = camera.nearClipPlane;
            return camera.ScreenToViewportPoint(position);
        }
    }
}
