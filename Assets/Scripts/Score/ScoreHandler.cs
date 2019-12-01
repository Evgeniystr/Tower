using UnityEngine;
using TMPro;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI textMesh;

    [SerializeField] public int lastScore { private set; get; }
    [SerializeField] public int hiScore { private set; get; }

    private void Start()
    {
        GameManager.Instance.RestartEvent += Reset;
    }

    void UpdateCounters()
    {
        if (lastScore > hiScore)
            hiScore = lastScore;

        textMesh.text = lastScore.ToString();
    }

    public void AddNormalScore()
    {
        lastScore++;

        UpdateCounters();
       
        animator.SetTrigger("normalScore");
    }

    public void AddPerfectScore()
    {
        lastScore++;

        UpdateCounters();

        animator.SetTrigger("perfectScore");
    }

    public void Reset()
    {
        lastScore = 0;
        UpdateCounters();
    }


    //hi score ever
    //hi score day
    //hi score week

    //perfect move percent
    //hi perfect move percent ever/day/week

    //also need count minimal fruit count fo eachachivment
    //15% not bad, good
    //30% nice, good
    //50% perfect master
    //70%+ maestro! grend bellissimo!
    //90% godlike!!! unbelivable! incredible! immposible

}
