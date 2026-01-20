using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//ここは全ての的に共通する振る舞いや状態を書く抽象クラスです。
public abstract class PointObject : MonoBehaviour
{
    [Header("PointObjectの設定用プロパティ")]
    //このリストの0番地はPointObjectの具象クラスのインスタンスが持つと言えるTMPやMRのリストが入る。
    public List<TMPandMeshRenderer> FadeTargetList;
    public List<Collider> ColliderList;
    public GameObject MainObj;
    public float PointObjectCost;
    public int nextGeneratableCount;
    public MeshRenderer LifeTimeGUI_MR;
    [SerializeField]protected BreakAnimator _targetBreakAnimator;
    [Header("表示用")]
    public Vector2 PointObjectPos;
    public Vector2 DebugPointObjectPos;
    public Vector2 PointObjectPosAsCenter;
    public Vector2 NormalizePointObjectPosAsCenter;
    public TimeKeeper TargetTimeKeeper;
    public Indicator2 TargetIndicator2;

    //　インスペクターから見れん奴ら
    /// 音符
    public static float FourthNote;
    public static float EighthNote;
    public static float SixteenthNote;


    

    public abstract (float nextActivationDelay,float lifeTime) Initialize();
    public abstract void TimeOver();
    public void PlayActivateAnim(float activateAnimDuration)
    {
        float playBackActiveAnimTime = 0;
        StartCoroutine(Anim());
        IEnumerator Anim()
        {
            while(playBackActiveAnimTime < activateAnimDuration)
            {
                playBackActiveAnimTime += Time.deltaTime;
                MainObj.transform.localScale = Vector3.one * (playBackActiveAnimTime / activateAnimDuration);
                MainObj.transform.localScale = Vector3.ClampMagnitude(MainObj.transform.localScale,Vector3.one.magnitude);
                yield return null;
            }
         }
    
    }

    
    
    public void PlayTimeOverAnim(float deactivateAnimDuration)
    {
        StartCoroutine(FadeOutCoroutine());
        IEnumerator FadeOutCoroutine()
        {
            TargetIndicator2.Destroy();
            Utility.ChangeEnabledColliders(ColliderList,false);
            float playbackDeSpawnTime = 0;

            PointObjectGenerater2.CurrentPointObjectGenerater2.SubtractSumPointObjectCost(PointObjectCost);
            PointObjectGenerater2.CurrentPointObjectGenerater2.RemovePointObjectPos(PointObjectPos,2);
            while(playbackDeSpawnTime < deactivateAnimDuration)
            {
                playbackDeSpawnTime += Time.deltaTime;
                foreach(TMPandMeshRenderer fadeTarget in FadeTargetList)
                {
                    fadeTarget.SetFadeOfMeshRenderers(playbackDeSpawnTime / deactivateAnimDuration);
                    fadeTarget.SetAlphaOfTextMeshPros(1 - playbackDeSpawnTime / deactivateAnimDuration);
                }
                yield return null;
            }
            foreach(TMPandMeshRenderer tMPorMeshRenderer in FadeTargetList)
            {
                tMPorMeshRenderer.SetFadeOfMeshRenderers(1);
                tMPorMeshRenderer.SetAlphaOfTextMeshPros(0);
            }
            Destroy(gameObject);
        }

    }
    public IEnumerator SubtractLifeTimeGUI_Anim(float duration){
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        for(float playback = 0;playback < duration; playback += Time.deltaTime)
        {
            propBlock.SetFloat("_outerFrameAnim",1 - playback / duration);
            if(LifeTimeGUI_MR == null) yield break;
            if(TargetIndicator2 == null) yield break;
            TargetIndicator2.SetLifeTimeAnim(1 - playback / duration,Color.white,Color.black);
            LifeTimeGUI_MR.SetPropertyBlock(propBlock);
            yield return null;
        }
    }
    public IEnumerator AddLifeTimeGUI_Anim(float duration){
        MainObj.SetActive(false);

        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        for(float playback = 0;playback < duration;playback += Time.deltaTime)
        {
            propBlock.SetFloat("_outerFrameAnim",playback / duration);
            if(LifeTimeGUI_MR == null) yield break;
            //shadergraphで作ったshader側の都合で、Color.blackを第二引数に設定する時は、微小な量を足してください。
            TargetIndicator2.SetLifeTimeAnim(playback / duration,Color.black + new Color(0.04f,0.04f,0.04f),Color.white);
            LifeTimeGUI_MR.SetPropertyBlock(propBlock);
            yield return null;
        }
    }
    abstract protected IEnumerator BreakCoroutine();
    protected void ActivateMain()
    {
        MainObj.SetActive(true);
        this.enabled = true;
        PointObjectPosAsCenter = new Vector2(PointObjectPos.x - PointObjectGenerater2.CurrentPointObjectGenerater2.PointObjectMapLength.x / 2, PointObjectPos.y - PointObjectGenerater2.CurrentPointObjectGenerater2.PointObjectMapLength.y / 2);
        NormalizePointObjectPosAsCenter = new Vector2(PointObjectPosAsCenter.x / (PointObjectGenerater2.CurrentPointObjectGenerater2.PointObjectMapLength.x / 2), PointObjectPosAsCenter.y / (PointObjectGenerater2.CurrentPointObjectGenerater2.PointObjectMapLength.y / 2));
        foreach(TMPandMeshRenderer tMPorMeshRenderer in FadeTargetList)
        {
            tMPorMeshRenderer.Initialize();
        }
        //DebugPointObjectPos = PointObjectGenerater2.pointObjectGenerater2.pointObjectWorldPosToPointObjectPos(transform.position);
    }

}
[Serializable]
public class TMPandMeshRenderer
{
    public List<TextMeshPro> TextMeshProList = new List<TextMeshPro>();
    public List<MeshRenderer> MeshRendererList = new List<MeshRenderer>();
    MaterialPropertyBlock _propBlock;
    public TMPandMeshRenderer(TextMeshPro[] addTextMeshPros,MeshRenderer[] addMeshRenderers)
    {
        TextMeshProList.AddRange(addTextMeshPros);
        MeshRendererList.AddRange(addMeshRenderers);
        _propBlock = new MaterialPropertyBlock();
    }
    public TMPandMeshRenderer(TextMeshPro addTextMeshPro,MeshRenderer[] addMeshRenderers)
    {
        TextMeshProList.Add(addTextMeshPro);
        MeshRendererList.AddRange(addMeshRenderers);
        _propBlock = new MaterialPropertyBlock();
    }
    public void Initialize()
    {
        _propBlock = new MaterialPropertyBlock();
    }

        
    
    public void SetAlphaOfTextMeshPros(float alpha)
    {
        foreach(TextMeshPro textMeshPro in TextMeshProList)
        {
            if(textMeshPro == null)continue;
            Color color = textMeshPro.color;
            color.a = alpha;
            textMeshPro.color = color;
        }
    }
    public void SetFadeOfMeshRenderers(float dithering)
    {
        _propBlock.Clear();
        foreach(MeshRenderer meshRenderer in MeshRendererList)
        {
            if(meshRenderer == null)continue;
            _propBlock.SetFloat("_Fade",dithering);
            for(int propertyBlockIndex = 0; propertyBlockIndex < meshRenderer.sharedMaterials.Length; propertyBlockIndex++)
            {
                meshRenderer.SetPropertyBlock(_propBlock,propertyBlockIndex);
            }
        }
    }

}
