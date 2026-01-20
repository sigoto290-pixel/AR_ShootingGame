using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Indicator2 : MonoBehaviour
{
    public  TextMeshProUGUI TargetNameText;
    public Transform TargetTr;
    [SerializeField]Camera _camera;
    [SerializeField]GameObject _mainObj;
    [SerializeField]RectTransform _arrowRectTr;
    [SerializeField]Image _innerArrowImage;
    [SerializeField]Image _ArrowOutlineImage;

    RectTransform _rectTr;

    Vector3 _targetViewportPos;
    Vector3 _targetScreenPos;

    Vector3 _targetViewportPosAsCenter;


    Vector2 _clampedTargetScreenPos;

    bool _isInCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rectTr = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(TargetTr == null) return;
        _targetViewportPos = _camera.WorldToViewportPoint(TargetTr.position);
        _targetScreenPos = _camera.WorldToScreenPoint(TargetTr.position);
        _clampedTargetScreenPos = new Vector2(Mathf.Clamp(_targetScreenPos.x, 0, Screen.width), Mathf.Clamp(_targetScreenPos.y, 0, Screen.height));
        _targetViewportPosAsCenter = new Vector2(Mathf.Clamp01(_targetViewportPos.x), Mathf.Clamp01(_targetViewportPos.y));
        _targetViewportPosAsCenter = ((_targetViewportPosAsCenter - new Vector3(0.5f, 0.5f)) * 2).normalized;



        //インディケーターの位置や向きと描画の処理
        _isInCamera = _targetViewportPos.x >= 0 & _targetViewportPos.x <= 1 & _targetViewportPos.y >= 0 & _targetViewportPos.y <= 1;
        if (_targetViewportPos.z >= 0)///カメラの前側
        {
            if (_isInCamera)
            {
                //Debug.Log("視野内です");
                _mainObj.SetActive(false);
            }
            else
            {
                //Debug.Log("視野外です");
                _mainObj.SetActive(true);
                _rectTr.pivot = new Vector2(Mathf.Clamp01(_targetViewportPos.x),Mathf.Clamp01(_targetViewportPos.y));
                _rectTr.position = _clampedTargetScreenPos;
                _arrowRectTr.rotation = Quaternion.LookRotation(_arrowRectTr.forward, _targetViewportPosAsCenter);
            }

        }
        else///カメラの後ろ側
        {
            if (_isInCamera)
            {
                //Debug.Log("カメラの真後ろ側です");
                _mainObj.SetActive(true);
                _rectTr.pivot = new Vector2(1 - (_targetViewportPosAsCenter.x + 1)/2,1 - (_targetViewportPosAsCenter.y + 1)/2);
                //_rectTr.position = new Vector2(Screen.width,Screen.height) - _clampedTargetScreenPos;
                _rectTr.position = new Vector2(Screen.width/2,Screen.height/2) - new Vector2(_targetViewportPosAsCenter.x * Screen.width / 2,_targetViewportPosAsCenter.y * Screen.height / 2);
                _arrowRectTr.rotation = Quaternion.LookRotation(_arrowRectTr.forward, -_targetViewportPosAsCenter);
            }
            else
            {
                //Debug.Log("カメラの後ろ側面です。");
                _mainObj.SetActive(true);
                _rectTr.pivot = new Vector2(1 - Mathf.Clamp01(_targetViewportPos.x),1 - Mathf.Clamp01(_targetViewportPos.y));
                _rectTr.position = new Vector2(Screen.width,Screen.height) - _clampedTargetScreenPos;
                _arrowRectTr.rotation = Quaternion.LookRotation(_arrowRectTr.forward, -_targetViewportPosAsCenter);

            }
        }
    }
    public void SetLifeTimeAnim(float playBack,Color innerArrowRgb,Color arrowOutlineRgb)
    {
        innerArrowRgb.a = playBack;
        _innerArrowImage.color = innerArrowRgb;
        _ArrowOutlineImage.color = arrowOutlineRgb;
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }


}
