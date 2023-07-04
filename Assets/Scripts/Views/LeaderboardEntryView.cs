using TMPro;
using UnityEngine;

public class LeaderboardEntryView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _rankTextField;
    [SerializeField]
    private TMP_Text _scoreTextField;
    [SerializeField]
    private TMP_Text _usernameTextField;


    public void Setup(int rank, long score, string name)
    {
        _rankTextField.text = rank.ToString();
        _scoreTextField.text = score.ToString();
        _usernameTextField.text = name;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
