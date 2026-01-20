using UnityEngine;
using Audio;

public class SelectDifficultButton : MonoBehaviour
{
    public GameManager.Difficult SelectDifficult;
    public GameObject OutLineObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(SelectDifficult == GameManager.Current.CurrentDifficult)
        {
            OutLineObj.SetActive(true);
        }
        else
        {
            OutLineObj.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(SelectDifficult == GameManager.Current.CurrentDifficult)
        {
            OutLineObj.SetActive(true);
        }
        else
        {
            OutLineObj.SetActive(false);
        }
    }
    public void TouchDifficultButton()
    {
        SoundManager.Current.PlayOneShot2D_SE(OneShot.downButton,0.7f);
        switch (SelectDifficult)
        {
            case GameManager.Difficult.easy:
                GameManager.Current.CurrentDifficult = GameManager.Difficult.easy;
            break;
            case GameManager.Difficult.normal:
                GameManager.Current.CurrentDifficult = GameManager.Difficult.normal;
            break;
            case GameManager.Difficult.hard:
                GameManager.Current.CurrentDifficult = GameManager.Difficult.hard;
            break;
        }

    }
}
