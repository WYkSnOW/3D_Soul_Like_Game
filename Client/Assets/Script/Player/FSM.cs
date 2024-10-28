using System;
using System.Collections;
using System.Collections.Generic;
using Game.Config;
using UnityEngine;

// ����״̬�� (FSM) �࣬���ڹ���Ϳ��Ʋ�ͬ״̬֮����л��߼�
public class FSM : MonoBehaviour
{
    public UnitEntity unitEntity; // ��λ��������
    public PlayerState currentState; // ��ǰ���״̬
    Dictionary<int, PlayerState> stateData = new Dictionary<int, PlayerState>(); // ��������״̬���ֵ䣬��Ϊ״̬ID

    // ��Awake�׶γ�ʼ��unitEntity
    private void Awake()
    {
        // �� UnitData �л�ȡָ��id�ĵ�λ����
        unitEntity = UnitData.Get(id);
    }

    // Start �ڽű�����ʱ����
    void Start()
    {
        // �˴����Գ�ʼ������� StateInit() ����״̬����
    }

    // Update ÿ֡����һ�Σ����ڸ����߼�
    void Update()
    {
        // ÿ֡�ĸ��²���
        if (currentState == null)
        {
            DOStateEvent(currentState.id, StateEvenType.update); //״̬ÿִ֡�е��¼�
        }
    }

    // ��ʼ��״̬����
    public void StateInit()
    {
        // ���PlayerStateData���Ƿ���״̬����
        if (PlayerStateData.all != null)
        {
            foreach (var item in PlayerStateData.all)
            {
                PlayerState p = new PlayerState();
                p.excecl_config = item.Value; // ����״̬��excel��Ϣ
                p.id = item.Key; // ����״̬ID

                stateData[item.Key] = p; // ��״̬������ӵ��ֵ���
            }
        }

        #region ���ü�������
        // �趨��ͬ��״̬ID��Ӧ�ļ�������
        stateData[1005].skill = SkillData.Get(unitEntity.ntk1); // �չ����� 1-4
        stateData[1006].skill = SkillData.Get(unitEntity.ntk2);
        stateData[1007].skill = SkillData.Get(unitEntity.ntk3);
        stateData[1008].skill = SkillData.Get(unitEntity.ntk4);

        stateData[1009].skill = SkillData.Get(unitEntity.skill1); // ���� 1-4
        stateData[1010].skill = SkillData.Get(unitEntity.skill2);
        stateData[1011].skill = SkillData.Get(unitEntity.skill3);
        stateData[1012].skill = SkillData.Get(unitEntity.skill4);
        #endregion
    }

    /// <summary>
    /// �л���ָ������һ��״̬
    /// </summary>
    /// <param name="next">״̬ID������ָ���л�����Ŀ��״̬</param>
    /// <returns>����л��ɹ�����true�����򷵻�false</returns>
    public bool ToNext(int next)
    {
        // ���Ŀ��״̬ID�Ƿ������״̬�ֵ���
        if (stateData.ContainsKey(next))
        {
            #region �����־-״̬�л���Ϣ
            if (currentState != null)
            {
                Debug.Log($"{this.gameObject.name}:�л�״̬��{stateData[next].Info()}   ��ǰ�ǣ�{currentState.Info()}");
            }
            else
            {
                Debug.Log($"{this.gameObject.name}:�л�״̬��{stateData[next].Info()}");
            }
            #endregion

            if (currentState != null)
            {
                DOStateEvent(currentState.id, StateEvenType.end);
                // ���ڴ˴�ִ�е�ǰ״̬���˳��¼�
            }

            // �л�����״̬
            currentState = stateData[next];
            // ���õ�ǰ״̬�Ŀ�ʼʱ�����÷���
            currentState.SetBeginTime();
            DOStateEvent(currentState.id, StateEvenType.begin);
            return true;
        }
        return false;
    }

    public void AnimationOnPlayEnd()
    {
        DOStateEvent(currentState.id, StateEvenType.onAnmEnd);
    }

    //�洢ÿ��״̬<��ͬ���͵��¼���<ͬһ�����Ϳ��Դ��ڶ���¼���������List�����л���>>
    //int ״̬ID�� Dictionary�¼�����
    //�¼�����StateEventType���¼����� value��List<Action>) �������Ͷ�Ӧ���¼��б�
    public Dictionary<int, Dictionary<StateEvenType, List<Action>>> actions = new Dictionary<int, Dictionary<StateEvenType, List<Action>>;


    /// <summary>
    /// ����¼��Ľӿ�
    /// </summary>
    /// <param name="id">״̬ID</param>
    /// <param name="t">�¼�����</param>
    /// <param name="action">�¼�</param>
    public void AddListener(int id, StateEvenType t, Action action)
    {
        if (!actions.ContainsKey(id))
        {
            action[id] = new Dictionary<StateEvenType, List<Action>>();
        }

        //�����������Ӧ���¼�����
        if (actions[id].ContainsKey(t) == false)
        {
            action[id] = new Dictionary<StateEvenType, List<Action>>();
            List<Action> list = new List<Action>();
            list.Add(action);
            actions[id][t] = list;
        }
        else
        {
            actions[id][t].Add(action);
        }
    }


    // �ṩ���ٵ��ò�ͬ״̬��ͬ���͵��¼�
    publie void DOStateEvent (int id, StateEvenType t)
    {
        if (actions.TryGetValue(id, out var v))
        {
            if (v.TryGetValue(t, out var 1st))
            {
                for (int i = 0; i < 1st.Count; i++)
                {
                    1st[i].Invoke();
                }
            }
        }
    }

}

// PlayerState �࣬��ʾ��ҵĵ���״̬��������״̬��������ݺͷ���
public class PlayerState
{
    public int id; // ״̬ID
    public PlayerStateEntity excecl_config; // Excel���е�״̬������Ϣ

    public float clipLength; // ����ʱ��
    public SkillEntity skill; // �����ļ�������
    public float begin; // ����״̬�Ŀ�ʼʱ��


    /// <summary>
    /// ����״̬�Ŀ�ʼʱ��
    /// </summary>
    public void SetBeginTime()
    {
        begin = Time.time; // ��¼����״̬��ϵͳʱ��
    }


    /// <summary>
    /// ���״̬�ļ����Ƿ�����ȴ״̬
    /// </summary>
    /// <returns>���������ȴ�з���true�����򷵻�false</returns>
    public bool IsCD()
    {
        if (skill != null) { return false; } // �������Ϊ�գ���������ȴ���
        return Time.time - begin < skill.cd; // ��鵱ǰʱ���Ƿ�С����ȴʱ��
    }


    /// <summary>
    /// ����״̬��������Ϣ
    /// </summary>
    /// <returns>����״̬ID��������Ϣ���ַ���</returns>
    public string Info()
    {
        return $"״̬{id}_{excecl_config.info}";
    }
}

public enum StateEvenType
{
    begin,//��ʼ����
    update,//ÿ֡����
    end,//״̬�˳�
    onAnmEnd,//������������ʱ��
}