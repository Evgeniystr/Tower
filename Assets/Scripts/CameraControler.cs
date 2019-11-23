using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    public static CameraControler Instance;

    [SerializeField] CameraSettings settings;
    [SerializeField] Transform startLoockAt;
    Vector3 cameraOffset;
    Quaternion defaultRotation;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Instance.cameraOffset = Camera.main.transform.position - startLoockAt.position;
            defaultRotation = Camera.main.transform.rotation;
        }
        else
            Destroy(gameObject);
    }


    public void RestartCamera()
    {
        SetLookAt(startLoockAt.gameObject);
        Camera.main.transform.rotation = defaultRotation;
    }

    public void SetLookAt(GameObject go)
    {
        CameraMoveTo(go.transform.position);
    }

   void CameraMoveTo(Vector3 position)
    {
        StopAllCoroutines();
        StartCoroutine(MoveTo(position));
    }

    IEnumerator MoveTo(Vector3 position)
    {
        float learpTt = 0;
        while(learpTt <= 1)
        {
            yield return Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, position + cameraOffset, learpTt);
            learpTt += Time.deltaTime * settings.cameraFolowSpeed;
        }

        yield return null;
    }

    //failed element
    public void LoseCameraOwerview(GameObject failedElement)
    {
        StartCoroutine(LoseCameraMove(failedElement));
    }

    IEnumerator LoseCameraMove(GameObject failedElement)
    {
        //towerHeight
        var towerHalfHeight = failedElement.transform.position.y / 2;

        var far = settings.cameraCoffFailDistance * towerHalfHeight + settings.cameraBaseFailDistance;

        Vector3 camPosFrom = Camera.main.transform.position;
        Vector3 camPosTo = new Vector3(
                                        Camera.main.transform.position.x,
                                        towerHalfHeight,
                                        far);

        Vector3 camRotateFrom = failedElement.transform.position;
        Vector3 camRotateTo = new Vector3(failedElement.transform.position.x,
                                          towerHalfHeight, 
                                          failedElement.transform.position.z);

        var learpT = 0f;

        while (learpT < 1)
        {
            //cam move
            yield return Camera.main.transform.position = Vector3.Lerp(camPosFrom, camPosTo, learpT);

            //cam rotate
            Camera.main.transform.LookAt(Vector3.Lerp(camRotateFrom, camRotateTo, learpT));

            learpT += Time.deltaTime * settings.cameraFailSpeed;
        }

        GameManager.Instance.readyToRestart = true;
    }
}
