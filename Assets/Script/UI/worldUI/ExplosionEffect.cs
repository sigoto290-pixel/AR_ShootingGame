using System.Collections;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public enum AnimPhase
    {
        NotPlayed,
        PlayingExplosion,
        Completed,
    }

    public float FadeInTime;
    public float ClippingTime;
    public Color BaseColor;
    public float Intensity;
    [Header("表示用")]public AnimPhase CurrentAnimPhase{private set;get;}

    public MeshRenderer MeshRenderer;
    MaterialPropertyBlock _propBlock;
    float _playFadeInTime;
    float _playClippingTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _propBlock = new MaterialPropertyBlock();
        StartCoroutine(OneShot());
        IEnumerator OneShot()
        {
            CurrentAnimPhase = AnimPhase.PlayingExplosion;
            _propBlock.SetColor("_BaseColor",BaseColor * Intensity);
            _propBlock.SetFloat("_clipping",0);
            while(_playFadeInTime < FadeInTime)
            {
                _propBlock.SetFloat("_fadeIn",_playFadeInTime / FadeInTime);
                MeshRenderer.SetPropertyBlock(_propBlock);
                _playFadeInTime += Time.deltaTime;
                yield return null;
            }
            _propBlock.SetFloat("_fadeIn",1);
            MeshRenderer.SetPropertyBlock(_propBlock);
            yield return null;
            while(_playClippingTime < ClippingTime)
            {
                _propBlock.SetFloat("_clipping",_playClippingTime / ClippingTime);
                MeshRenderer.SetPropertyBlock(_propBlock);
                _playClippingTime += Time.deltaTime;
                yield return null;
            }
            CurrentAnimPhase = AnimPhase.Completed;
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
