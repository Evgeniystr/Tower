﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LoseCheckType
{
    ScaleOn,
    MoveEnd
}

public class TowerBuilder : MonoBehaviour
{
    [SerializeField] GameSettings settings;
    [SerializeField] Material loseElementMaterial;
    [SerializeField] Transform baseEement;

    bool isBuilderInputEnabled = true;
    const float YgrownStep = 0.25f;
    PoolItem currentTowerElement;
    PoolItem previousTowerElement;


    private void Update()
    {
        BuildInputHandler();
    }

    private void FixedUpdate()
    {
        ScaleHandler();
    }


    void BuildInputHandler()
    {
        if(Input.GetMouseButtonDown(0) && GameManager.Instance.readyToRestart)
            Restart();

        if (isBuilderInputEnabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnTap();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnRelease();
            }
        }
    }


    void ScaleHandler()
    {
        if (currentTowerElement != null)
        {
            ScaleUpCurrentElement();
            if (!LoseCheck(LoseCheckType.ScaleOn))
                Lose();
        }
    }


    void OnTap()
    {
        currentTowerElement = TowerPoolManager.Instance.GetNewTowerElement();
        SetCurrentElement();
        CameraControler.Instance.SetLookAt(currentTowerElement.obj);
    }

    void SetCurrentElement()
    {
        //set scale
        currentTowerElement.obj.transform.localScale = new Vector3(0, settings.heightStep, 0);

        //set position
        var baseY = baseEement? baseEement.position.y + baseEement.localScale.y - settings.heightStep : 0;

        float newYpos = settings.heightStep * (TowerPoolManager.Instance.desiredItemIndex + 1) * 2 + baseY;
        currentTowerElement.obj.transform.position = new Vector3(0, newYpos, 0);

        currentTowerElement.obj.SetActive(true);
    }

    void OnRelease()
    {
        if(LoseCheck(LoseCheckType.MoveEnd))
        {
            PerfectMoveCheck();

            previousTowerElement = currentTowerElement;
            currentTowerElement = null;
            TowerPoolManager.Instance.GoOnPoolExpandCheck();
        }
        else
            Lose();
    }

    //scale up
    void ScaleUpCurrentElement()
    {
        var scaleUpValue = settings.scaleUpSpeed * Time.fixedDeltaTime;

        var newScale = new Vector3(
            currentTowerElement.obj.transform.localScale.x + scaleUpValue,
            currentTowerElement.obj.transform.localScale.y,
            currentTowerElement.obj.transform.localScale.z + scaleUpValue);

        currentTowerElement.obj.transform.localScale = newScale;
    }


    //perfect move 
    void PerfectMoveCheck()
    {
        if (previousTowerElement == null)
            return;

        var minPerfScale = previousTowerElement.obj.transform.localScale * 0.95f;

        if (currentTowerElement.obj.transform.localScale.x >= minPerfScale.x && currentTowerElement.obj.transform.localScale.z >= minPerfScale.z)
            PerfectMovePerform();
    }

    void PerfectMovePerform()
    {
        currentTowerElement.isPerfect = true;

        var poolCount = TowerPoolManager.Instance.towerElementsPool.Count;
        int delayCount = 0;
        for (int i = poolCount - 1; i >= 0; i--)
        {
            var poolItem = TowerPoolManager.Instance.towerElementsPool[i];

            if (!poolItem.obj.activeSelf)
                continue;

            delayCount++;
            var delay = delayCount * settings.vaweDelayStep;

            Vector3 newMaxScale;
            Vector3 newFinalScale;

            //just placed item
            if (i == poolCount - 1)
            {
                newMaxScale = new Vector3(poolItem.obj.transform.localScale.x + 0.4f,
                                            poolItem.obj.transform.localScale.y,
                                            poolItem.obj.transform.localScale.z + 0.4f);

                newFinalScale = new Vector3(newMaxScale.x - 0.2f,
                                                poolItem.obj.transform.localScale.y,
                                                newMaxScale.z - 0.2f);

            }
            else
            {
                newMaxScale = new Vector3(poolItem.obj.transform.localScale.x + 0.3f,
                                        poolItem.obj.transform.localScale.y,
                                        poolItem.obj.transform.localScale.z + 0.3f);

                if (poolItem.isPerfect)
                {
                    newFinalScale = poolItem.obj.transform.localScale;
                }
                else
                {
                    newFinalScale = new Vector3(newMaxScale.x * 0.8f,
                                                newMaxScale.y,
                                                newMaxScale.z * 0.8f);
                }
            }

            poolItem.maxScaleTo = newMaxScale;
            poolItem.finalScaleTo = newFinalScale;

            StartCoroutine(Wave(poolItem, delay));
        }                                                                             
    }

    IEnumerator Wave(PoolItem poolItem, float delay)
    {
        yield return new WaitForSeconds(delay);

        float learpT = 0;
        var from = poolItem.obj.transform.localScale;
        var to = poolItem.maxScaleTo;

        while (learpT <= 1)
        {
            yield return poolItem.obj.transform.localScale = Vector3.Lerp(from, to, learpT);
            learpT += Time.fixedDeltaTime * settings.vaweScaleSpeed;
        }


        learpT = 0;
        from = poolItem.obj.transform.localScale;
        to = poolItem.finalScaleTo;

        while (learpT <= 1)
        {
            yield return poolItem.obj.transform.localScale = Vector3.Lerp(from, to, learpT);
            learpT += Time.fixedDeltaTime * settings.vaweScaleSpeed;
        }
    }

    //lose
    bool LoseCheck(LoseCheckType loseCheckType)
    {
        var currScale = currentTowerElement.obj.transform.localScale;
        Vector3 loseScale;

        switch (loseCheckType)
        {
            case LoseCheckType.ScaleOn:
                if(previousTowerElement == null)
                    loseScale = baseEement.localScale * 1.1f;
                else
                    loseScale = previousTowerElement.obj.transform.localScale * 1.1f;
                break;

            case LoseCheckType.MoveEnd:
                if (previousTowerElement == null)
                    loseScale = baseEement.localScale;
                else
                    loseScale = previousTowerElement.obj.transform.localScale;
                break;

            default:
                loseScale = Vector3.zero;
                break;
        }

        if (currScale.x >= loseScale.x || currScale.z >= loseScale.z)
        {
            //lose
            return false;
        }
        else
            //go on
            return true;
    }

    void Lose()
    {
        isBuilderInputEnabled = false;
        var failedElement = currentTowerElement;
        currentTowerElement = null;

        failedElement.obj.GetComponent<MeshRenderer>().material = loseElementMaterial;

        StartCoroutine(DestroyLoseElement(failedElement.obj));
    }

    IEnumerator DestroyLoseElement(GameObject failedElement)
    {
        yield return new WaitForSeconds(2);
        CameraControler.Instance.LoseCameraOwerview(failedElement);
        yield return new WaitForSeconds(0.1f);
        failedElement.SetActive(false);
    }

    void Restart()
    {
        GameManager.Instance.readyToRestart = false;
        TowerPoolManager.Instance.ResetPoolOff();
        CameraControler.Instance.RestartCamera();
        isBuilderInputEnabled = true;
        previousTowerElement = null;
    }

}
