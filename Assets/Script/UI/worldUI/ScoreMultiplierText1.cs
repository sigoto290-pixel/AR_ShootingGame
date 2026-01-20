using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class ScoreMultiplierText1 : MonoBehaviour
{
    public float ScoreMultiplier { private get; set; }
    public float ShowAnimDuration = 0.3f; 
    public float HideAnimDuration = 0.2f;
    [SerializeField]TextMeshPro _textMeshPro;
    [SerializeField]MeshRenderer _TMP_meshRenderer;
    float _startDistance;
    Vector3 _startPosDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startPosDirection  = transform.position.normalized;
        _startDistance = transform.position.magnitude;
        _textMeshPro.text = "x" + ScoreMultiplier.ToString();
        StartCoroutine(Anim());
        IEnumerator  Anim()
        {
            for (float i = 0; i <= 1; i += Time.deltaTime * (1 / ShowAnimDuration))
            {
                transform.position = _startPosDirection * _startDistance * i;
                yield return null;
            }
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            Color outlineColor = _TMP_meshRenderer.sharedMaterials[0].GetColor("_OutlineColor");
            Color glowColor = _TMP_meshRenderer.sharedMaterials[0].GetColor("_GlowColor");
            //これはTMP_SDF.shaderによるマテリアルのインスペクター上ではThicknessとして書かれている。
            float startOutlineWidth = _TMP_meshRenderer.sharedMaterials[0].GetFloat("_OutlineWidth");
            float startGlowOuter = _TMP_meshRenderer.sharedMaterials[0].GetFloat("_GlowOuter");
            
            float outlineWidth = 0; float glowOuter = 0;
            for (float i = 1; i >= 0; i -= Time.deltaTime * (1 / HideAnimDuration))
            {
                outlineColor.a = i;
                glowColor.a = i;
                outlineWidth = startOutlineWidth * i;
                glowOuter = startGlowOuter * i;
                propBlock.SetColor("_OutlineColor",outlineColor);
                propBlock.SetColor("_GlowColor",glowColor);
                propBlock.SetFloat("_OutlineWidth",outlineWidth);
                propBlock.SetFloat("_GlowOuter",glowOuter);
                _TMP_meshRenderer.SetPropertyBlock(propBlock);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
