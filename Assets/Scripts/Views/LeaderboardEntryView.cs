using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntryView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _rankTextField;
    [SerializeField]
    private TMP_Text _scoreTextField;
    [SerializeField]
    private TMP_Text _usernameTextField;
    [SerializeField]
    private Image _hilightImage;


    public void Setup(int rank, long score, string name, bool myScore)
    {
        _rankTextField.text = rank.ToString();
        _scoreTextField.text = score.ToString();
        _usernameTextField.text = name;
        _hilightImage.enabled = myScore;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
