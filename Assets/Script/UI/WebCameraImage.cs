using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WebCameraImage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int fps;
    public AspectRatioFitter AspectRatioFitter;    
    RawImage _rawImage;


    void Start()
    {
        _rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AdjustWebCameraImageSize();
        //CheckWebCameraInstance();
    }
    bool isDeviceVertical;
    WebCamTexture webCamTexture;
    void AdjustWebCameraImageSize()
    {
        if (GameManager.Current.WebCamTexture == null) return;
        //デバイスが縦向きか
        isDeviceVertical = Screen.height > Screen.width;
        webCamTexture = GameManager.Current.WebCamTexture;
        AspectRatioFitter.aspectRatio = (float)webCamTexture.width / webCamTexture.height;
        return;
        //縦向きの時
        if (isDeviceVertical)
        {
            Debug.Log("縦");
            if (AspectRatioFitter.aspectRatio == (float)webCamTexture.height / webCamTexture.width) return;
            AspectRatioFitter.aspectRatio = (float)webCamTexture.height / webCamTexture.width;
        }
        else//横向きの時
        {
            Debug.Log("横");
            if (AspectRatioFitter.aspectRatio == (float)webCamTexture.width / webCamTexture.height) return;
            AspectRatioFitter.aspectRatio = (float)webCamTexture.width / webCamTexture.height;
        }
    }
    public void SetWebCamTexture()
    {
        StartCoroutine(OneShotCol());
        IEnumerator OneShotCol()
        {
            yield return new WaitWhile(()=>GameManager.Current.WebCamTexture.isPlaying == false);
            _rawImage.texture = GameManager.Current.WebCamTexture;
        }
    }
    void CheckWebCameraInstance()
    {
        if(_rawImage.texture != GameManager.Current.WebCamTexture)
        {
            _rawImage.texture = GameManager.Current.WebCamTexture;
            Debug.Log("webCamTextureとrawImageインスタンスが持つtextureが異なることを検知しました");
        }

    }
}
