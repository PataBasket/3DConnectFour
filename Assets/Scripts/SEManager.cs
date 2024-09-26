using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    [SerializeField] private AudioSource a1;//AudioSource型の変数a1を宣言 使用するAudioSourceコンポーネントをアタッチ必要

    [SerializeField] private AudioClip b1;//AudioClip型の変数b1を宣言 使用するAudioClipをアタッチ必要
    [SerializeField] private AudioClip b2;//AudioClip型の変数b2を宣言 使用するAudioClipをアタッチ必要 
    [SerializeField] private AudioClip b3;//AudioClip型の変数b3を宣言 使用するAudioClipをアタッチ必要 

    //自作の関数1
    public void SE1()
    {
        a1.PlayOneShot(b1);//a1にアタッチしたAudioSourceの設定値でb1にアタッチした効果音を再生
    }

    //自作の関数2
    public void SE2()
    {
        a1.PlayOneShot(b2);//a2にアタッチしたAudioSourceの設定値でb2にアタッチした効果音を再生
    }

    //自作の関数3
    public void SE3()
    {
        a1.PlayOneShot(b3);//a3にアタッチしたAudioSourceの設定値でb3にアタッチした効果音を再生
    }
}