using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerInputService : MonoBehaviour
{
    public event Action OnTapEvent;
    public event Action OnReleaseEvent;

    public event Action OnUnrestrictedTapEvent;

    public bool IsInputActive { get; private set; }

    [SerializeField]
    private GraphicRaycaster _graphicRaycaster;
    [SerializeField]
    private EventSystem _eventSystem;

    private int _layerIndex;
    private PointerEventData _blockerEventData;
    private List<RaycastResult> _blockerResults;

    private void Start()
    {
        _blockerEventData = new PointerEventData(_eventSystem);
        _layerIndex = LayerMask.NameToLayer(Constants.ClickBloCkerUiLayerName);
        _blockerResults = new List<RaycastResult>();
    }

    private void Update()
    {
        if (IsInputActive)
        {
            if(Input.GetMouseButtonDown(0) && CheckUIBlockers())
                OnTapEvent?.Invoke();
            
            if(Input.GetMouseButtonUp(0) && CheckUIBlockers())
                OnReleaseEvent?.Invoke();
        }

        if (Input.GetMouseButtonUp(0))
            OnUnrestrictedTapEvent?.Invoke();
    }

    private bool CheckUIBlockers()
    {
        _blockerResults.Clear();

        _blockerEventData.position = Input.mousePosition;

        _graphicRaycaster.Raycast(_blockerEventData, _blockerResults);

        foreach (var item in _blockerResults)
            if (item.gameObject.layer == _layerIndex)
                return false;

        return true;
    }

    public void SetInputActive(bool isActiveState)
    {
        IsInputActive = isActiveState;
    }
}
