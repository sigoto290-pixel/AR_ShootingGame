using System.Collections;
using TMPro;
using UnityEngine;
public class judgeText : MonoBehaviour
{
    TextMeshProUGUI _textMeshProUGUI;
    RectTransform _rectTransform;
    [SerializeField]float _addScaleTime = 0.5f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
        StartCoroutine("FadeOut");
    }
    IEnumerator  FadeOut()
    {
        Color vertexColor = _textMeshProUGUI.color;
        for (float a = 1; a >= 0; a -= Time.deltaTime * (1 / _addScaleTime))
        {
            vertexColor.a = a;
            _textMeshProUGUI.color = vertexColor;
            _rectTransform.position += Vector3.up * Time.deltaTime * 2 * 20;              
            yield return null;
        }
        vertexColor.a = 0;
        _textMeshProUGUI.color = vertexColor;
        Destroy(gameObject);
    }
    // Update is called once per frame
}
