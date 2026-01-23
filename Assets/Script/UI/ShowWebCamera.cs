using System.Collections;
using UnityEditor.UI;
using UnityEngine;

public class ShowWebCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]Camera _playerCam;
    MeshRenderer _meshRenderer;
    enum phase{
        settingWebCam,
        fittingObjectToScreen,
    }
    [SerializeField]phase _currentPhase;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    void Update()
    {
        FitObjectToScreen();
    }
    // Update is called once per frame
    public void StartShowWebCam()
    {
        StartCoroutine(main());
        IEnumerator main()
        {
            StartCoroutine(SetWebCamTex());
            yield return new WaitWhile(()=> _currentPhase == phase.settingWebCam);
            FitObjectToScreen();
   
        }


    }
    IEnumerator SetWebCamTex()
    {
        if(GameManager.Current.WebCamTexture == null)
        {
            Debug.LogError("webCamTextureの参照渡し元が存在しません");
            _currentPhase = phase.fittingObjectToScreen;
            yield break;
        }
        yield return new WaitWhile(()=>GameManager.Current.WebCamTexture.isPlaying == false);
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        propBlock.SetTexture("_BaseMap",GameManager.Current.WebCamTexture);
        _meshRenderer.SetPropertyBlock(propBlock);
        _currentPhase = phase.fittingObjectToScreen;
    }
    void FitObjectToScreen()
    {
        // MeshFilterコンポーネントを取得
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilterコンポーネントが見つかりません。");
            return;
        }

        // オブジェクトの「元」の幅・高さを取得 (ローカル空間でのサイズ)
        float objectWidth = meshFilter.mesh.bounds.size.x;
        float objectHeight = meshFilter.mesh.bounds.size.y;

        // 幅か高さが0に近い場合は、計算不可能なので処理を中断
        if (Mathf.Approximately(objectWidth, 0) || Mathf.Approximately(objectHeight, 0))
        {
            Debug.LogError("オブジェクトのサイズが0のため、FitObjectToScreenを実行できません。");
            return;
        }
        
        // カメラからオブジェクトまでの距離を計算
        float distance = Vector3.Distance(transform.position, _playerCam.transform.position);

        // カメラの表示領域の左下と右上のワールド座標を取得
        Vector3 bottomLeft = _playerCam.ScreenToWorldPoint(new Vector3(0, 0, distance));
        Vector3 topRight = _playerCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, distance));

        // ワールド座標系でのカメラ表示領域の幅と高さを計算
        float width = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;

        // 取得した値から、オブジェクトのスケールを調整
        transform.localScale = new Vector3(width / objectWidth, height / objectHeight, 1f);
    }
}
