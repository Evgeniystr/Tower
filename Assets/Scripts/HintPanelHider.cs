using UnityEngine;

public class HintPanelHider : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            gameObject.SetActive(false);
    }
}
