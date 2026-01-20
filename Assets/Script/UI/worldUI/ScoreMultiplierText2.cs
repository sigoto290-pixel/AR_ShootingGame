using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class ScoreMultiplierText2 : MonoBehaviour
{
    public float ScoreMultiplier { private get; set; }
    public float AddScaleTime = 0.3f; 
    public float FadeAnimTime = 0.2f;
    public float MaxScale = 3;
    public float MinScale = 1;
    [SerializeField]TextMeshPro _textMeshPro;
    [SerializeField]RectTransform _textRectTransform;
    [SerializeField]MeshRenderer _rippleEffectMR;
    float _maxDiffScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _maxDiffScale = MaxScale - MinScale;
        _textMeshPro.text = "x" + ScoreMultiplier.ToString();
        StartCoroutine(Anim());
        IEnumerator  Anim()
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            for (float i = 0; i <= 1; i += Time.deltaTime * (1 / AddScaleTime))
            {
                _textRectTransform.localScale = Vector3.one * (MinScale + i * _maxDiffScale);
                propBlock.SetFloat("_Playback",i);
                _rippleEffectMR.SetPropertyBlock(propBlock);
                yield return null;
            }
            _textRectTransform.localScale = Vector3.one * MaxScale;
            propBlock.SetFloat("_Playback",1);
            _rippleEffectMR.SetPropertyBlock(propBlock);

            Color startTextColor = _textMeshPro.color;
            Color startRippleColor = _rippleEffectMR.sharedMaterials[0].GetColor("_BaseColor");
            float startThickness = _rippleEffectMR.sharedMaterials[0].GetFloat("_Thickness");
            Debug.Log(startRippleColor);
            for (float i = 1; i >= 0; i -= Time.deltaTime * (1 / FadeAnimTime))
            {
                _textMeshPro.color = startTextColor * new Color(1,1,1,i);
                propBlock.SetFloat("_Thickness",startThickness * i);
                propBlock.SetColor("_BaseColor",startRippleColor * new Color(1,1,1,i));
                Debug.Log(startRippleColor * new Color(1,1,1,i));
                _rippleEffectMR.SetPropertyBlock(propBlock);
                yield return null;
            }
            Destroy(gameObject);
        }

    }
    IEnumerator Test()
    {
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        Color startTextColor = _textMeshPro.color;
        Color startRippleColor = _rippleEffectMR.sharedMaterials[0].GetColor("_BaseColor");
        for (float i = 1; i >= 0; i -= Time.deltaTime * (1 / FadeAnimTime))
        {
            _textMeshPro.color = startTextColor * new Color(1,1,1,i);
            propBlock.SetColor("_BaseColor",startRippleColor * new Color(1,1,1,i));
            Debug.Log(startRippleColor * new Color(1,1,1,i));
            _rippleEffectMR.SetPropertyBlock(propBlock);
            yield return null;
        }
        Destroy(gameObject);

    }
}
