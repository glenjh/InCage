using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour
{
    public void PlaySound(string clipName)
    {
        SoundManager.Instance.PlaySound3D(clipName,transform,0,false,SoundType.EFFECT,false,0,10);
    }
    
    public void PlayUISound(string clipName)
    {
        SoundManager.Instance.PlaySound2D(clipName);
    }
}
