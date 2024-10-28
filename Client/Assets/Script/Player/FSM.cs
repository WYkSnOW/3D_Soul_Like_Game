using System;
using System.Collections;
using System.Collections.Generic;
using Game.Config;
using UnityEngine;

// 有限状态机 (FSM) 类，用于管理和控制不同状态之间的切换逻辑
public class FSM : MonoBehaviour
{
    public UnitEntity unitEntity; // 单位基础数据
    public PlayerState currentState; // 当前玩家状态
    Dictionary<int, PlayerState> stateData = new Dictionary<int, PlayerState>(); // 保存所有状态的字典，键为状态ID

    // 在Awake阶段初始化unitEntity
    private void Awake()
    {
        // 从 UnitData 中获取指定id的单位数据
        unitEntity = UnitData.Get(id);
    }

    // Start 在脚本启用时调用
    void Start()
    {
        // 此处可以初始化或调用 StateInit() 设置状态数据
    }

    // Update 每帧调用一次，用于更新逻辑
    void Update()
    {
        // 每帧的更新操作
        if (currentState == null)
        {
            DOStateEvent(currentState.id, StateEvenType.update); //状态每帧执行的事件
        }
    }

    // 初始化状态数据
    public void StateInit()
    {
        // 检查PlayerStateData中是否有状态数据
        if (PlayerStateData.all != null)
        {
            foreach (var item in PlayerStateData.all)
            {
                PlayerState p = new PlayerState();
                p.excecl_config = item.Value; // 配置状态的excel信息
                p.id = item.Key; // 设置状态ID

                stateData[item.Key] = p; // 将状态数据添加到字典中
            }
        }

        #region 设置技能数据
        // 设定不同的状态ID对应的技能数据
        stateData[1005].skill = SkillData.Get(unitEntity.ntk1); // 普攻技能 1-4
        stateData[1006].skill = SkillData.Get(unitEntity.ntk2);
        stateData[1007].skill = SkillData.Get(unitEntity.ntk3);
        stateData[1008].skill = SkillData.Get(unitEntity.ntk4);

        stateData[1009].skill = SkillData.Get(unitEntity.skill1); // 技能 1-4
        stateData[1010].skill = SkillData.Get(unitEntity.skill2);
        stateData[1011].skill = SkillData.Get(unitEntity.skill3);
        stateData[1012].skill = SkillData.Get(unitEntity.skill4);
        #endregion
    }

    /// <summary>
    /// 切换到指定的下一个状态
    /// </summary>
    /// <param name="next">状态ID，用于指定切换到的目标状态</param>
    /// <returns>如果切换成功返回true，否则返回false</returns>
    public bool ToNext(int next)
    {
        // 检查目标状态ID是否存在于状态字典中
        if (stateData.ContainsKey(next))
        {
            #region 输出日志-状态切换信息
            if (currentState != null)
            {
                Debug.Log($"{this.gameObject.name}:切换状态：{stateData[next].Info()}   当前是：{currentState.Info()}");
            }
            else
            {
                Debug.Log($"{this.gameObject.name}:切换状态：{stateData[next].Info()}");
            }
            #endregion

            if (currentState != null)
            {
                DOStateEvent(currentState.id, StateEvenType.end);
                // 可在此处执行当前状态的退出事件
            }

            // 切换至新状态
            currentState = stateData[next];
            // 调用当前状态的开始时间设置方法
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

    //存储每个状态<不同类型的事件，<同一个类型可以存在多个事件，所以用List来进行缓存>>
    //int 状态ID， Dictionary事件容器
    //事件容器StateEventType：事件类型 value（List<Action>) 代表类型对应的事件列表
    public Dictionary<int, Dictionary<StateEvenType, List<Action>>> actions = new Dictionary<int, Dictionary<StateEvenType, List<Action>>;


    /// <summary>
    /// 添加事件的接口
    /// </summary>
    /// <param name="id">状态ID</param>
    /// <param name="t">事件类型</param>
    /// <param name="action">事件</param>
    public void AddListener(int id, StateEvenType t, Action action)
    {
        if (!actions.ContainsKey(id))
        {
            action[id] = new Dictionary<StateEvenType, List<Action>>();
        }

        //如果不包含对应的事件类型
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


    // 提供快速调用不同状态不同类型的事件
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

// PlayerState 类，表示玩家的单个状态，并包含状态的相关数据和方法
public class PlayerState
{
    public int id; // 状态ID
    public PlayerStateEntity excecl_config; // Excel表中的状态配置信息

    public float clipLength; // 动画时长
    public SkillEntity skill; // 关联的技能数据
    public float begin; // 进入状态的开始时间


    /// <summary>
    /// 设置状态的开始时间
    /// </summary>
    public void SetBeginTime()
    {
        begin = Time.time; // 记录进入状态的系统时间
    }


    /// <summary>
    /// 检查状态的技能是否处于冷却状态
    /// </summary>
    /// <returns>如果技能冷却中返回true，否则返回false</returns>
    public bool IsCD()
    {
        if (skill != null) { return false; } // 如果技能为空，不进行冷却检查
        return Time.time - begin < skill.cd; // 检查当前时间是否小于冷却时间
    }


    /// <summary>
    /// 返回状态的描述信息
    /// </summary>
    /// <returns>包含状态ID和描述信息的字符串</returns>
    public string Info()
    {
        return $"状态{id}_{excecl_config.info}";
    }
}

public enum StateEvenType
{
    begin,//开始进入
    update,//每帧更新
    end,//状态退出
    onAnmEnd,//当动作结束的时候
}