using UnityEngine;

[CreateAssetMenu(fileName = "NewCameraSettings", menuName = "GameSettings/Camera")]
public class CameraSettings : ScriptableObject
{
    public float cameraFolowSpeed;
    public float cameraFailSpeed;
    public float cameraBaseFailDistance;
    public float cameraCoffFailDistance;
}
