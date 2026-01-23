using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShowWebCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int fps;
    MeshRenderer _meshRenderer;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    public void SetWebCamTexture()
    {
        StartCoroutine(OneShotCol());
        IEnumerator OneShotCol()
        {
            yield return new WaitWhile(()=>GameManager.Current.WebCamTexture.isPlaying == false);
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            propBlock.SetTexture("_BaseMap",GameManager.Current.WebCamTexture);
            _meshRenderer.SetPropertyBlock(propBlock);
        }
    }
}
