﻿
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    public class UnitData
    {

        static UnitData()
        {
            entityDic = new Dictionary<int, UnitEntity>(17);
             UnitEntity e0 = new UnitEntity(1001,@"玄影剑姬",0,@"Unit/1001",0,1,10011,10012,10013,10014,10015,10016,10017,10018,10019,10020,10021,10022,80,60,30,50,30,null);
            entityDic.Add(e0.id, e0);
             UnitEntity e1 = new UnitEntity(1002,@"红焰邪姬",1,@"Unit/1002",1,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20,new int[]{1001,1003,1004,1008,3010,3011,3012});
            entityDic.Add(e1.id, e1);
             UnitEntity e2 = new UnitEntity(1003,@"独目锤影",3,@"Unit/1003",1,2,30011,30012,30013,30014,30015,30016,30017,30018,30011,30011,30011,30011,80,60,30,50,20,new int[]{1003,1004,2016,1012,3008,3009,3010,3011,3012,3013});
            entityDic.Add(e2.id, e2);
             UnitEntity e3 = new UnitEntity(1004,@"小兵C",3,@"Unit/1004",1,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20,null);
            entityDic.Add(e3.id, e3);
             UnitEntity e4 = new UnitEntity(4001,@"拿匕首的女孩",3,@"Unit/BOSS/4001",1,2,40011,40012,40013,40014,40015,40016,40017,40018,20011,20011,20011,20011,0,60,30,50,20,null);
            entityDic.Add(e4.id, e4);
             UnitEntity e5 = new UnitEntity(4002,@"大胖子boss_4002",3,@"Unit/BOSS/4002",1,2,50011,50012,50013,50014,50015,50016,50017,50018,20011,20011,20011,20011,0,60,30,50,20,null);
            entityDic.Add(e5.id, e5);
             UnitEntity e6 = new UnitEntity(4003,@"双枪_boss_4003",3,@"Unit/BOSS/4003",1,2,60011,60012,60013,60014,60015,60016,60017,60018,20011,20011,20011,20011,0,60,30,50,20,null);
            entityDic.Add(e6.id, e6);
             UnitEntity e7 = new UnitEntity(4004,@"武斗师boss_4004",3,@"Unit/BOSS/4004",1,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20,null);
            entityDic.Add(e7.id, e7);
             UnitEntity e8 = new UnitEntity(5001,@"鬼面武士Boss_5001",3,@"Unit/BOSS/5001",1,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20,null);
            entityDic.Add(e8.id, e8);
             UnitEntity e9 = new UnitEntity(5002,@"长矛Boss_5002",3,@"Unit/BOSS/5002",1,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20,null);
            entityDic.Add(e9.id, e9);
             UnitEntity e10 = new UnitEntity(5003,@"Boss_5003",3,@"Unit/BOSS/5003",1,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20,null);
            entityDic.Add(e10.id, e10);
             UnitEntity e11 = new UnitEntity(5004,@"Boss_5004",3,@"Unit/BOSS/5004",1,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20,null);
            entityDic.Add(e11.id, e11);
             UnitEntity e12 = new UnitEntity(5005,@"Boss_5005",3,@"Unit/BOSS/5005",1,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20,null);
            entityDic.Add(e12.id, e12);
             UnitEntity e13 = new UnitEntity(5006,@"Boss_5006",3,@"Unit/BOSS/5006",1,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20,null);
            entityDic.Add(e13.id, e13);
             UnitEntity e14 = new UnitEntity(5007,@"Boss_5007",3,@"Unit/BOSS/5007",1,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20,null);
            entityDic.Add(e14.id, e14);
             UnitEntity e15 = new UnitEntity(5008,@"Boss_5008",3,@"Unit/BOSS/5008",1,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20,null);
            entityDic.Add(e15.id, e15);
             UnitEntity e16 = new UnitEntity(5009,@"Boss_5009",3,@"Unit/BOSS/5009",1,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20,null);
            entityDic.Add(e16.id, e16);

        }

       
        
        public static Dictionary<int, UnitEntity> all {
            get {
                return entityDic;
            }
        }
		static Dictionary<int, UnitEntity> entityDic;
		public static UnitEntity Get(int id)
		{
            if (entityDic!=null&&entityDic.TryGetValue(id,out var entity))
			{
				return entity;
			}
            return null;
		}
    }

    
    public class UnitEntity
    {
        //TemplateMember
		public int id;//单位ID
		public string info;//说明
		public int type;//类型
		public string prefab_path;//资源路径
		public int camp;//阵营
		public int att_id;//属性表ID
		public int ntk1;//技能ID_普攻1
		public int ntk2;//技能ID_普攻2
		public int ntk3;//技能ID_普攻3
		public int ntk4;//技能ID_普攻4
		public int skill1;//技能ID_技能1
		public int skill2;//技能ID_技能2
		public int skill3;//技能ID_技能3
		public int skill4;//技能ID_技能4
		public int use_prop_1;//使用暗器_炸弹
		public int use_prop_2;//使用暗器_飞镖
		public int use_prop_3;//使用暗器_飞刀
		public int use_prop_4;//使用暗器_圆轮
		public int block_probability;//格挡概率
		public int dodge_probability;//躲闪概率
		public int atk_probability;//对拼概率
		public int active_attack_probability;//主动发起攻击概率
		public int pacing_probability;//踱步概率
		public int[] drop;//掉落ID

        public UnitEntity(){}
        public UnitEntity(int id,string info,int type,string prefab_path,int camp,int att_id,int ntk1,int ntk2,int ntk3,int ntk4,int skill1,int skill2,int skill3,int skill4,int use_prop_1,int use_prop_2,int use_prop_3,int use_prop_4,int block_probability,int dodge_probability,int atk_probability,int active_attack_probability,int pacing_probability,int[] drop){
           
           this.id = id;
           this.info = info;
           this.type = type;
           this.prefab_path = prefab_path;
           this.camp = camp;
           this.att_id = att_id;
           this.ntk1 = ntk1;
           this.ntk2 = ntk2;
           this.ntk3 = ntk3;
           this.ntk4 = ntk4;
           this.skill1 = skill1;
           this.skill2 = skill2;
           this.skill3 = skill3;
           this.skill4 = skill4;
           this.use_prop_1 = use_prop_1;
           this.use_prop_2 = use_prop_2;
           this.use_prop_3 = use_prop_3;
           this.use_prop_4 = use_prop_4;
           this.block_probability = block_probability;
           this.dodge_probability = dodge_probability;
           this.atk_probability = atk_probability;
           this.active_attack_probability = active_attack_probability;
           this.pacing_probability = pacing_probability;
           this.drop = drop;

        }
    }
}
