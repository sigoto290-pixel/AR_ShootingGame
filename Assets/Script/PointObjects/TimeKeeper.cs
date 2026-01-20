using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

public class TimeKeeper : MonoBehaviour
{
    //ここはPointObjectのアクティブアニメーションや、デスポーンアニメーションの管理を行っているクラスです。具体的なアニメーションの仕方はPointObjectクラスを参照
    //またPointObjectGeneratorのインスタンスとの連携も担っています。
    [Header("インスペクター設定用")]
    public List<PointObject> TargetPointObjectList;
    [SerializeField,Range(0,1)] float _activateAnimRate = 0.2f;
    [SerializeField]float _deactivateAnimDuration = 0.2f;
    
    [SerializeField] float _perlinNoiseMagni = 1;

    [Header("表示用")]
    public float ActivationDelay;
    public TargetState CurrentTargetState = TargetState.Preparing;
    public TimingState CurrentTaimingState = TimingState.GoodTiming;
    [SerializeField]float _sumLifeTime;
    [SerializeField]float _sumNextActivationDelay;
    [SerializeField] int _nextGeneratableCount;

    public enum TargetState
    {
        //有効化される前の状態
        Preparing,
        //有効化中の状態
        Activating,
        //有効化完了の状態
        ActivationCompleted,
        //無効化中の状態
        Deactivating
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ManageLifeCycle());
        IEnumerator ManageLifeCycle()
        {
            //ポイントオブジェクトの有効化までの時間可視化
            AllAddLifeTimeGUI_Anim(ActivationDelay);
            yield return new WaitForSeconds(ActivationDelay - _activateAnimRate * ActivationDelay);
            CurrentTargetState = TargetState.Activating;
            CurrentTaimingState = TimingState.GoodTiming;
            AllActivatePointObject();
            AllPlayActivateAnim(_activateAnimRate * ActivationDelay);
            yield return new WaitForSeconds(_activateAnimRate * ActivationDelay);
            CurrentTargetState = TargetState.ActivationCompleted;
            CurrentTaimingState = TimingState.PerfectTiming;
            AllAddPointObjectCost();
            NoticeGeneratableNextPointObject();
            AllSubtractLifeTimeGUI_Anim(_sumLifeTime);
            yield return new WaitForSeconds(_sumLifeTime * 0.5f);
            CurrentTaimingState = TimingState.GreatTiming;
            yield return new WaitForSeconds(_sumLifeTime * 0.25f);
            CurrentTaimingState = TimingState.GoodTiming;
            yield return new WaitForSeconds(_sumLifeTime * 0.25f);
            CurrentTargetState = TargetState.Deactivating;
            //デスポーンアニメーションの開始
            AllDeactivatePointObject();
            AllPlayDeactivateAnim(_deactivateAnimDuration);
            yield return new WaitForSeconds(_deactivateAnimDuration);
            Destroy(gameObject);
        }

        void AllAddLifeTimeGUI_Anim(float activationDelay){
            foreach(PointObject targetPointObject in TargetPointObjectList){
                if(targetPointObject == null) continue;
                StartCoroutine(targetPointObject.AddLifeTimeGUI_Anim(ActivationDelay));
            }
        }
        void AllActivatePointObject(){
            _nextGeneratableCount =TargetPointObjectList[0].nextGeneratableCount;
            foreach(PointObject targetPointObject in TargetPointObjectList){
                if(targetPointObject == null) continue;
                targetPointObject.TargetTimeKeeper = this;
                (float nextActivationDelay,float lifeTime) = targetPointObject.Initialize();
                _sumNextActivationDelay += nextActivationDelay;
                _sumLifeTime += lifeTime;
            }

        }
        void AllPlayActivateAnim(float activateAnimDuration){
            foreach(PointObject targetPointObject in TargetPointObjectList){
                if(targetPointObject == null) continue;
                targetPointObject.PlayActivateAnim(activateAnimDuration);
            }
        }
        void AllAddPointObjectCost(){
            foreach(PointObject targetPointObject in TargetPointObjectList){
                if(targetPointObject == null) continue;
                PointObjectGenerater2.CurrentPointObjectGenerater2.AddSumPointObjectCost(targetPointObject.PointObjectCost);
            }
        }
        void AllSubtractLifeTimeGUI_Anim(float sumLifeTime){
            foreach(PointObject targetPointObject in TargetPointObjectList){
                if(targetPointObject == null) continue;
                StartCoroutine(targetPointObject.SubtractLifeTimeGUI_Anim(sumLifeTime));
            }
        }
        void AllDeactivatePointObject(){
            foreach(PointObject targetPointObject in TargetPointObjectList){
                if(targetPointObject == null) continue;
                targetPointObject.TimeOver();
            }
        }
        void AllPlayDeactivateAnim(float deactivateAnimDuration){
            foreach(PointObject targetPointObject in TargetPointObjectList){
                if(targetPointObject == null) continue;
                targetPointObject.PlayTimeOverAnim(deactivateAnimDuration);
            }
        }
    }
    //PointObjectGeneratorに次の生成を指定するメンバ
    public void NoticeGeneratableNextPointObject()
    {
        PointObjectGenerater2.CurrentPointObjectGenerater2.NoticeGeneratable(_sumNextActivationDelay,0,_perlinNoiseMagni,_nextGeneratableCount);
    }
    //PointObjectがTimeKeeperの管理から外れる為の関数メンバ
    public void NoticeDestruction(PointObject pointObject)
    {
        TargetPointObjectList.Remove(pointObject);
        pointObject.TargetIndicator2.Destroy();
    }


    // Update is called once per frame
}
