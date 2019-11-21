using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LoseCheckType
{
    ScaleOn,
    MoveEnd
}

public class TowerBuilder : MonoBehaviour
{
    [SerializeField] Pool pool;
    [SerializeField] GameSettings settings;
    [SerializeField] SplashHandler splashHandler;
    [SerializeField] ScoreHandler scoreHandler;
    [SerializeField] Material loseElementMaterial;
    [SerializeField] Transform baseEement;
    [SerializeField] Material[] fruitMaterials;


    bool isBuilderInputEnabled = true;
    IpoolItem currentTowerElement;
    IpoolItem previousTowerElement;


    private void Start()
    {
        if (baseEement)
            baseEement.GetComponent<MeshRenderer>().material = GetRandomeMaterial();

        //start score resrt
        scoreHandler.Reset();
    }

    private void Update()
    {
        BuildInputHandler();
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
        currentTowerElement = pool.GetNextPoolItem();
        SetCurrentElement();
        CameraControler.Instance.SetLookAt(currentTowerElement.obj);
    }

    void SetCurrentElement()
    {
        currentTowerElement.obj.transform.localScale = new Vector3(0, currentTowerElement.obj.transform.localScale.y, 0);

        //set position
        float baseY;

        if (previousTowerElement != null)
            baseY = previousTowerElement.obj.transform.position.y;
        else if (baseEement)
            baseY = baseEement.position.y;
        else
            baseY = 0;

        float newYpos = currentTowerElement.obj.GetComponent<Renderer>().bounds.size.y + baseY;
        currentTowerElement.obj.transform.position = new Vector3(0, newYpos, 0);

        //set material
        currentTowerElement.obj.GetComponent<MeshRenderer>().material = GetRandomeMaterial();

        currentTowerElement.obj.SetActive(true);
    }

    Material GetRandomeMaterial()
    {
        int ind = Random.Range(1, fruitMaterials.Length) - 1;
        return fruitMaterials[ind];
    }

    void OnRelease()
    {
        if(LoseCheck(LoseCheckType.MoveEnd))
        {
            if (PerfectMoveCheck())
            {
                OnPerfectMove();
            }
            else
            {
                OnNormalMove();
            }

            previousTowerElement = currentTowerElement;
            currentTowerElement = null;
            pool.ExpandNeedCheck();
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

    void OnNormalMove()
    {
        if (scoreHandler)
            scoreHandler.AddNormalScore();

    }

    void OnPerfectMove()
    {
        PerfectMovePerform();

        if (splashHandler)
            splashHandler.DoSplash();

        if (scoreHandler)
            scoreHandler.AddPerfectScore();
    }


    bool PerfectMoveCheck()
    {
        if (previousTowerElement == null)
            return false;

        var minPerfScale = previousTowerElement.obj.transform.localScale * 0.95f;

        if (currentTowerElement.obj.transform.localScale.x >= minPerfScale.x && currentTowerElement.obj.transform.localScale.z >= minPerfScale.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    TowerPoolItem CastToTowerPoolItem(IpoolItem item)
    {
        return item as TowerPoolItem;
    }

    void PerfectMovePerform()
    {
        CastToTowerPoolItem(currentTowerElement).isPerfect = true;

        var poolCount = pool.GetActiveElementsCount();
        int delayCount = 0;

        IpoolItem poolItem;
        TowerPoolItem PoolItemUpcasted;

        for (int i = poolCount - 1; i >= 0; i--)//reverse cycle
        {
            poolItem = pool.GetItemByIndex(i);
            PoolItemUpcasted = CastToTowerPoolItem(poolItem);


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
            //other items
            else
            {
                newMaxScale = new Vector3(poolItem.obj.transform.localScale.x + 0.3f,
                                        poolItem.obj.transform.localScale.y,
                                        poolItem.obj.transform.localScale.z + 0.3f);

                if (PoolItemUpcasted.isPerfect)
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


            PoolItemUpcasted.SetWaveScales(ClampScales(newMaxScale), 
                                           ClampScales(newFinalScale));

            StartCoroutine(Wave(PoolItemUpcasted, delay));
        }                                                                             
    }

    Vector3 ClampScales(Vector3 scales)
    {
        if (scales.x > settings.maxScale || scales.y > settings.maxScale)
        {
            return new Vector3(Mathf.Min(scales.x, settings.maxScale),
                               Mathf.Min(scales.y, settings.maxScale),
                               scales.z);
        }
        return scales;
    }

    IEnumerator Wave(TowerPoolItem poolItem, float delay)
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


        loseElementMaterial.SetTexture("_MainTex",
            failedElement.obj.GetComponent<MeshRenderer>().material.mainTexture);
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
        pool.ResetPool();
        CameraControler.Instance.RestartCamera();
        isBuilderInputEnabled = true;
        previousTowerElement = null;

        scoreHandler.Reset();
    }

}
