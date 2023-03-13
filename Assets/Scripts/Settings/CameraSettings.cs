using UnityEngine;

[CreateAssetMenu(fileName = "NewCameraSettings", menuName = "GameSettings/Camera")]
public class CameraSettings : ScriptableObject
{
    public float cameraFolowDuration;
    public float CamFailAnimDuration;
    public float CameraBaseFailDistance;
    public float CameraCoofFailDistance;
}
