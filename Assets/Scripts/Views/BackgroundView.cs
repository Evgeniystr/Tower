using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BackgroundView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _BGPropsSR;
    [SerializeField]
    private float _moveSpeed = 1f;
    [SerializeField]
    private Transform _directionToGO;

    [Inject(Id = Constants.Camera)]
    private Transform _camTransform;

    void Start()
    {
        StartBGAnimation();
    }

    private async UniTaskVoid StartBGAnimation()
    {
        var xBorderValue = _BGPropsSR.size.x / 2;
        var yBorderValue = _BGPropsSR.size.y / 2;

        _BGPropsSR.size = _BGPropsSR.size * 2;

        var moveDirection = (new Vector3(-xBorderValue, -yBorderValue, 0) - _BGPropsSR.transform.localPosition).normalized;

        while (true)
        {
            var newPos = _BGPropsSR.transform.localPosition + moveDirection * _moveSpeed * Time.fixedDeltaTime;

            if (_BGPropsSR.transform.localPosition.x > xBorderValue && moveDirection.x > 0 ||
                _BGPropsSR.transform.localPosition.x < -xBorderValue && moveDirection.x < 0 ||
                _BGPropsSR.transform.localPosition.y > yBorderValue && moveDirection.y > 0 ||
                _BGPropsSR.transform.localPosition.y < -yBorderValue && moveDirection.y < 0)
            {
                newPos -= moveDirection * (xBorderValue * 4);
            }

            newPos.z = 0;
            _BGPropsSR.transform.localPosition = newPos;

            await UniTask.WaitForFixedUpdate();
        }
    }
}
