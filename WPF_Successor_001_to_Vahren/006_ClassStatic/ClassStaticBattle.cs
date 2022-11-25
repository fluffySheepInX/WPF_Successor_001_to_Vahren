using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Successor_001_to_Vahren._005_Class;

namespace WPF_Successor_001_to_Vahren._006_ClassStatic
{
    public static class ClassStaticBattle
    {
        public static void AddBuilding(ClassGameStatus? classGameStatus)
        {
            if (classGameStatus == null) return;
            if (classGameStatus.ClassBattleUnits.ClassMapBattle == null) return;

            //建築物設定

            List<(string, int, int)> bui = new List<(string, int, int)>();
            foreach (var battle in classGameStatus.ClassBattleUnits.ClassMapBattle.MapData.Select((value, index) => (value, index)))
            {
                foreach (var item in battle.value.Select((value, index) => (value, index)))
                {
                    if (item.value.Building != string.Empty)
                    {
                        bui.Add(new(item.value.Building, battle.index, item.index));
                    }
                }
            }

            List<ClassUnit> uni = new List<ClassUnit>();
            ClassHorizontalUnit classHorizontalUnit = new ClassHorizontalUnit();
            classHorizontalUnit.FlagBuilding = true;
            foreach (var item in bui)
            {
                var re = classGameStatus.ListObject.Where(x => x.NameTag == item.Item1 && x.Type == _010_Enum.MapTipObjectType.GATE).FirstOrDefault();
                if (re == null) continue;
                long id = classGameStatus.IDCount;
                classGameStatus.SetIDCount();

                uni.Add(new ClassUnitBuilding()
                {
                    ID = id
                    ,
                    Hp = re.Castle
                    ,
                    X = item.Item2
                    ,
                    Y = item.Item3
                    ,
                    Defense = re.CastleDefense
                    ,
                    MagDef = re.CastleMagdef
                });
            }
            classHorizontalUnit.ListClassUnit = uni;
            classGameStatus.ClassBattleUnits.DefUnitGroup.Add(classHorizontalUnit);
        }
    }
}
