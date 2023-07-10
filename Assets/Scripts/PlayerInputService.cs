using UnityEngine;
using System;

public class PlayerInputService : MonoBehaviour
{
    public event Action OnTapEvent;
    public event Action OnReleaseEvent;

    public bool IsInputActive { get; private set; }


    private void Update()
    {
        if (IsInputActive)
        {
            if(Input.GetMouseButtonDown(0))
                OnTapEvent?.Invoke();
            
            if(Input.GetMouseButtonUp(0))
                OnReleaseEvent?.Invoke();
        }
    }

    public void SetInputActive(bool isActiveState)
    {
        IsInputActive = isActiveState;
    }
}
