using UnityEngine;

public class HintPanelView : MonoBehaviour
{
    [SerializeField]
    private GameObject _viewport;
    [SerializeField]
    private InteractableTrigger _trigger;


    void Start()
    {
        _trigger.OnPointerDownEvent += HideHint;

        ShowHint();
    }

    private void OnDestroy()
    {
        _trigger.OnPointerDownEvent -= HideHint;
    }

    public void HideHint()
    {
        _viewport.SetActive(false);
    }

    public void ShowHint()
    {
        _viewport.SetActive(true);
    }
}
