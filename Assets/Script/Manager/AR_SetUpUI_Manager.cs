
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using AttitudeSensor = UnityEngine.InputSystem.AttitudeSensor;

public class AR_SetUpUI_Manager : MonoBehaviour
{
    public static AR_SetUpUI_Manager Current;
    void Awake()
    {
        if (Current == null)
        {
            Current = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    [Header("Title1UI")]
    
    [SerializeField]GameObject _startAR_GameButtonObj;
    [SerializeField]GameObject _loadTitle2ButtonObj;

    [SerializeField]TextMeshProUGUI _notifierTextToUser;


    [Header("WebCamSettingUI")]
    [SerializeField]ShowWebCamera _showWebCamera;
    [SerializeField]TextMeshProUGUI _webCumNumberText;

    [SerializeField]GameObject _changeWebCamsObj ;  

    [SerializeField]phase _currentPhase;
    enum phase
    {
        idle,
        waitingForWebCameraPermission,
        enablingAttitudeSensor,
        waitingForVertical,
        gettingWebCamTexOfBackCamera,
        completed 
    }
    Coroutine _currentSetUpAR;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void StartAR_GameButton()
    {
        //一度しかコールチンが走らないようにする。
        if(_currentSetUpAR != null) return;
        _startAR_GameButtonObj.GetComponent<Button>().enabled = false;
        _currentSetUpAR = StartCoroutine(SetUpAR());
        IEnumerator SetUpAR()
        {
            _currentPhase = phase.waitingForWebCameraPermission;
            StartCoroutine(GetForWebCameraPermission());
            yield return new WaitWhile(()=>_currentPhase == phase.waitingForWebCameraPermission);
            StartCoroutine(EnableAttitudeSensor());
            yield return new WaitWhile(() => _currentPhase == phase.enablingAttitudeSensor);
            StartCoroutine(RequestVerticalOrientation());
            yield return new WaitWhile(() => _currentPhase == phase.waitingForVertical);
            StartCoroutine(GetWebCamTexOfBackCamera());
            yield return new WaitWhile(() => _currentPhase == phase.gettingWebCamTexOfBackCamera);
            GameManager.Current.ResetRotation();
            GameManager.Current.LoadTitle2();


        }
        IEnumerator GetForWebCameraPermission()
        {
            while(Application.HasUserAuthorization(UserAuthorization.WebCam) == false)
            {
                _notifierTextToUser.text = "カメラの許可をしてください";
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
                _notifierTextToUser.text = "";
            }
            _currentPhase = phase.enablingAttitudeSensor;

        }
        IEnumerator EnableAttitudeSensor()
        {
            if (GameManager.Current.IsRunningInEditor){
                _currentPhase = phase.waitingForVertical;
                yield break;
            }
            
            if(AttitudeSensor.current != null)
            {
                _notifierTextToUser.text = "姿勢センサーへのアクセス許可してください。";
                InputSystem.EnableDevice(AttitudeSensor.current);
                yield return new WaitWhile(() => AttitudeSensor.current.enabled == false);
                _notifierTextToUser.text = "姿勢センサーを有効化しています。";
                yield return new WaitForSecondsRealtime(1f);
                _currentPhase = phase.waitingForVertical;

            }
            else
            {
                _notifierTextToUser.text = "姿勢センサーを検出できませんでした。";
            }
        }
        IEnumerator RequestVerticalOrientation()
        {
            if (GameManager.Current.IsRunningInEditor){
                _currentPhase = phase.gettingWebCamTexOfBackCamera;
                yield break;
            }

            //スマホが縦の時
            if(Screen.height > Screen.width)
            {
                _currentPhase = phase.gettingWebCamTexOfBackCamera;
            }//スマホが横の時
            else
            {
                _notifierTextToUser.text = "スマホを縦向きにしてください";
                yield return new WaitWhile(()=>!(Screen.height > Screen.width));
                _currentPhase = phase.gettingWebCamTexOfBackCamera;
            }
        }

        IEnumerator GetWebCamTexOfBackCamera()
        {
            _notifierTextToUser.text = "背面カメラを取得中";
            yield return new WaitForSeconds(1f);
            DebugUI_manager.DebugUI_Manager.UpdateCameraDeviceLength(WebCamTexture.devices.Length.ToString());
            foreach (WebCamDevice webCamDevice in WebCamTexture.devices)
            {
                if (webCamDevice.isFrontFacing == false)
                {
                    GameManager.Current.WebCamTexture = new WebCamTexture(webCamDevice.name);
                    break;
                }
                GameManager.Current.CurrentWebCamIndex++;
            }
            //背面カメラが見つからないとき
            if (GameManager.Current.WebCamTexture == null)
            {
                GameManager.Current.CurrentWebCamIndex = 0;
                int currentWebCamIndex = GameManager.Current.CurrentWebCamIndex;
                currentWebCamIndex = (int)Mathf.Repeat(1, WebCamTexture.devices.Length);
                GameManager.Current.WebCamTexture = new WebCamTexture(WebCamTexture.devices[currentWebCamIndex].name);
                GameManager.Current.CurrentWebCamIndex = currentWebCamIndex;
                _showWebCamera.StartShowWebCam();

                _notifierTextToUser.text = "背面カメラを検出出来ず、代わりに<br>１のカメラを表示しました。背面<br>カメラでないなら変更を押してください";
                _startAR_GameButtonObj.SetActive(false);
                _loadTitle2ButtonObj.SetActive(true);
                _changeWebCamsObj.SetActive(true);
            }//背面カメラを検出できた時
            else
            {
                _notifierTextToUser.text = "背面カメラの取得に成功、ゲームを開始します";
                yield return new WaitForSeconds(1f);
                _currentPhase = phase.completed;

            }

        }
    }
    //ここから下の関数はuiのボタンにより呼ばれる関数であり、目的は背面カメラを自動検出できなかったとき、手動で設定するための場所です。
    public void LoadTitle2Button()
    {
        StartCoroutine(OneShot());
        IEnumerator OneShot()
        {
            _notifierTextToUser.text = "これを背面カメラとして扱い、ゲームを開始します。";
            yield return new WaitForSeconds(2f);
            _currentPhase = phase.completed;
  
        }
    }


    public void ChangeWebCamIndexButton()
    {
        int currentWebCamIndex = GameManager.Current.CurrentWebCamIndex;
        currentWebCamIndex++;
        currentWebCamIndex = (int)Mathf.Repeat(currentWebCamIndex, WebCamTexture.devices.Length);
        if (GameManager.Current.WebCamTexture != null)
        {
            GameManager.Current.WebCamTexture.Stop();
            Destroy(GameManager.Current.WebCamTexture);
        }
        GameManager.Current.WebCamTexture = new WebCamTexture(WebCamTexture.devices[currentWebCamIndex].name, Screen.width, Screen.height, 30);
        _showWebCamera.StartShowWebCam();
        _webCumNumberText.text = GameManager.Current.CurrentWebCamIndex.ToString();
        GameManager.Current.CurrentWebCamIndex = currentWebCamIndex;
        _notifierTextToUser.text = "";
    }

}
