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
    //����֪ͨ�¼�

    public float clipLength;
    public SkillEntity skil;
    public float begin;//����״̬�Ŀ�ʼʱ��
  
    public void SetBeginTime()
    {
        begin = Time.time;
    }
}