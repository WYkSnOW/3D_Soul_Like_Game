using System.Collections;
using System.Collections.Generic;
using Game.Config;
using UnityEngine;



public class FSM : MonoBehaviour
{ //Start is called before the first frame update
    void Start()   
    {

    }

    // Update is called once per frame
    void Upadte()
    {

    }

}

public class PlayerState
{
    public int id;
    public PlayerStateEntity excecl_config;
    //动作通知事件

    public float clipLength;
    public SkillEntity skil;
    public float begin;//进入状态的开始时间
  
    public void SetBeginTime()
    {
        begin = Time.time;
    }
}