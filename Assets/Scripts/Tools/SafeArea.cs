using UnityEngine;

[ExecuteInEditMode]
public class SafeArea : MonoBehaviour
{
    private RectTransform _rt;
    private Rect _safeArea;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _safeArea = Screen.safeArea;
        UpdateSafeArea();
    }

    private void Update()
    {
        if (_safeArea != Screen.safeArea)
        {
            _safeArea = Screen.safeArea;
            UpdateSafeArea();
        }
    }

    private void UpdateSafeArea()
    {
        if (Screen.width == 0 || Screen.height == 0)
            return;

        Vector2 anchorMin = _safeArea.position;
        Vector2 anchorMax = _safeArea.position + _safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rt.anchorMin = anchorMin;
        _rt.anchorMax = anchorMax;
    }
}
