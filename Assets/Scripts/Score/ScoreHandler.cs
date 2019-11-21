using UnityEngine;
using TMPro;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI textMesh;
    
    int currentScore;
    int hiScore;


    void UpdateCounters()
    {
        if (currentScore > hiScore)
            hiScore = currentScore;

        textMesh.text = currentScore.ToString();
    }

    public void AddNormalScore()
    {
        currentScore++;

        UpdateCounters();
       
        animator.SetTrigger("normalScore");
    }

    public void AddPerfectScore()
    {
        currentScore++;

        UpdateCounters();

        animator.SetTrigger("perfectScore");
    }

    public void Reset()
    {
        currentScore = 0;
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
