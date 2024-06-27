using UnityEngine;

public class Convert : MonoBehaviour
{
    public Vector3 ScreenToWorld(Camera camera, Vector3 position)
    {
        position.z = camera.nearClipPlane;
        return camera.ScreenToViewportPoint(position);
    }
}
