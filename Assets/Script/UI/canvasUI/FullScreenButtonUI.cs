using UnityEngine;

public class FullScreenButtonUI : MonoBehaviour
{
    [SerializeField]GameObject _startFullScreenButtonObj;
    [SerializeField]GameObject _exitFullScreenButtonObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    bool _isStartFullScreen;
    bool _isExitFullScreen;
    // Update is called once per frame
    void Update()
    {
        
        if(Screen.fullScreen == true)
        {
            if(_isStartFullScreen == true)return;
            _isStartFullScreen = true;
            _isExitFullScreen = false;
            _startFullScreenButtonObj.SetActive(false);
            _exitFullScreenButtonObj.SetActive(true);
        }
        else
        {
            if(_isExitFullScreen == true) return;
            _isExitFullScreen = true;
            _isStartFullScreen = false;
            _exitFullScreenButtonObj.SetActive(false);
            _startFullScreenButtonObj.SetActive(true);

        }
    }
}
