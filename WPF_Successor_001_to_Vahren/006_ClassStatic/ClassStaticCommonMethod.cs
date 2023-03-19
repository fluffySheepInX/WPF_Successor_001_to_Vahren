using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using WPF_Successor_001_to_Vahren._005_Class;
using WPF_Successor_001_to_Vahren._010_Enum;
using WPF_Successor_001_to_Vahren._015_Lexer;
using WPF_Successor_001_to_Vahren._025_Parser;

namespace WPF_Successor_001_to_Vahren._006_ClassStatic
{
    public static class ClassStaticCommonMethod
    {
        #region FindAncestors
        /// <summary>
        /// 親要素取得のメソッド
        /// </summary>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> FindAncestors(this DependencyObject depObj)
        {
            // yield returnを使っているのでyield break
            if (depObj == null) { yield break; }
            // 親取得
            depObj = VisualTreeHelper.GetParent(depObj);
            while (depObj != null)
            {
                // 親を返す
                yield return depObj;
                // 中断再開、さらに親を返す
                depObj = VisualTreeHelper.GetParent(depObj);
            }
        }
        #endregion

        #region 構造体読み込み
        public static ClassMapBattle GetClassMapBattle(string value)
        {
            ClassMapBattle classMapBattle = new ClassMapBattle();

            // コメント行を取り除く
            {
                string[] line = value.Split(Environment.NewLine).ToArray();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Contains("//") == true)
                    {
                        var data = line[i].Split("//");
                        line[i] = String.Concat(data[0], Environment.NewLine);
                    }
                }
                value = String.Join(Environment.NewLine, line);
            }

            int eleNumber = 0;
            Dictionary<string, string> map = new Dictionary<string, string>();
            while (true)
            {
                {
                    var ele =
                        new Regex(ClassStaticCommonMethod.GetPat("ele" + eleNumber), RegexOptions.IgnoreCase)
                        .Matches(value);
                    var first = ClassStaticCommonMethod.CheckMatchElement(ele);
                    if (first == null)
                    {
                        break;
                    }
                    else
                    {
                        map.Add("ele" + eleNumber, first.Value);
                    }
                }
                eleNumber++;
            }

            //name
            {
                var name =
                    new Regex(ClassStaticCommonMethod.GetPat("name"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = ClassStaticCommonMethod.CheckMatchElement(name);
                if (first == null)
                {
                    classMapBattle.Name = String.Empty;
                }
                else
                {
                    classMapBattle.Name = first.Value;
                }
            }

            //tag name
            {
                var nameTag = new Regex(ClassStaticCommonMethod.GetPatTag("map"), RegexOptions.IgnoreCase)
                                .Matches(value);
                var first = ClassStaticCommonMethod.CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classMapBattle.TagName = first.Value.Replace(Environment.NewLine, "");
            }

            //data
            {
                var data =
                    new Regex(ClassStaticCommonMethod.GetPatComma("data"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = ClassStaticCommonMethod.CheckMatchElement(data);
                if (first == null)
                {
                    classMapBattle.MapData = new List<List<MapDetail>>();
                }
                else
                {
                    classMapBattle.MapData.Add(new List<MapDetail>());
                    List<string> re = first.Value.Split(",").ToList();
                    //最後の改行を消す
                    re.RemoveAt(re.Count - 1);
                    for (int i = 0; i < re.Count; i++)
                    {
                        if (re[i] == "@")
                        {
                            //改行コード時処理
                            classMapBattle.MapData.Add(new List<MapDetail>());
                            continue;
                        }
                        else
                        {
                            MapDetail mapDetail = new MapDetail();
                            var sonomama = re[i].Replace(System.Environment.NewLine, string.Empty);
                            var splitA = sonomama.Split("*");

                            //field(床画像
                            map.TryGetValue(splitA[0], out string? mapValue);
                            if (mapValue != null) mapDetail.Tip = mapValue;

                            //build(城壁や矢倉など
                            if (splitA.Length > 1)
                            {
                                var spBuild = splitA[1].Split("$");
                                foreach (var item in spBuild)
                                {
                                    map.TryGetValue(item, out string? mapValue2);
                                    if (mapValue2 != null) mapDetail.Building.Add(mapValue2);
                                }
                            }

                            //flag(部隊チップの種別
                            if (splitA.Length > 2)
                            {
                                int num = -1;
                                int temp = -1;
                                if (int.TryParse(splitA[2], out temp) != true)
                                {
                                    throw new Exception("");
                                }
                                num = temp;
                                FlagBattleMapUnit sEnum = (FlagBattleMapUnit)Enum.ToObject(typeof(FlagBattleMapUnit), num);
                                mapDetail.FlagBattleMapUnit = sEnum;
                            }

                            //部隊
                            if (splitA.Length > 3)
                            {
                                mapDetail.Unit = splitA[3];
                                if (mapDetail.Unit == "@@")
                                {
                                    mapDetail.KougekiButaiNoIti = true;
                                }
                                if (mapDetail.Unit == "@")
                                {
                                    mapDetail.BoueiButaiNoIti = true;
                                }
                            }

                            //方向
                            if (splitA.Length > 4)
                            {
                                mapDetail.Houkou = splitA[4];
                            }

                            //陣形
                            if (splitA.Length > 5)
                            {
                                mapDetail.Zinkei = splitA[5];
                            }

                            classMapBattle.MapData[classMapBattle.MapData.Count - 1].Add(mapDetail);
                        }
                    }
                }
            }

            //最後の空行を消す
            if (classMapBattle.MapData[classMapBattle.MapData.Count - 1].Count == 0)
            {
                classMapBattle.MapData.RemoveAt(classMapBattle.MapData.Count - 1);
            }

            return classMapBattle;
        }

        public static ClassObjectMapTip GetClassObjNewFormat(string value)
        {
            ClassObjectMapTip classObjectMapTip = new ClassObjectMapTip();

            // コメント行を取り除く
            {
                string[] line = value.Split(Environment.NewLine).ToArray();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Contains("#") == true)
                    {
                        var data = line[i].Split("#");
                        line[i] = String.Concat(data[0], Environment.NewLine);
                    }
                }
                value = String.Join(Environment.NewLine, line);
            }

            //nameTag
            {
                var nameTag = new Regex(GetPatTag("NewFormatObject"), RegexOptions.IgnoreCase)
                                .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classObjectMapTip.NameTag = first.Value.Replace(Environment.NewLine, "");
            }
            //type
            {
                var type =
                    new Regex(GetPat("type"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(type);
                if (first == null)
                {
                    //classObjectMapTip.Type = 0;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classObjectMapTip.Type = (MapTipObjectType)Enum.Parse(typeof(MapTipObjectType), re, true);
                }
            }
            //no_wall2
            {
                var no_wall2 =
                    new Regex(GetPat("no_wall2"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(no_wall2);
                if (first == null)
                {
                    classObjectMapTip.NoWall2 = 0;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classObjectMapTip.NoWall2 = int.Parse(re);
                }
            }
            //Castle
            {
                var Castle =
                    new Regex(GetPat("Castle"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(Castle);
                if (first == null)
                {
                    classObjectMapTip.Castle = 0;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classObjectMapTip.Castle = int.Parse(re);
                }
            }
            //CastleDefense
            {
                var CastleDefense =
                    new Regex(GetPat("CastleDefense"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(CastleDefense);
                if (first == null)
                {
                    classObjectMapTip.CastleDefense = 0;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classObjectMapTip.CastleDefense = int.Parse(re);
                }
            }
            //CastleMagdef
            {
                var CastleMagdef =
                    new Regex(GetPat("CastleMagdef"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(CastleMagdef);
                if (first == null)
                {
                    classObjectMapTip.CastleMagdef = 0;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classObjectMapTip.CastleMagdef = int.Parse(re);
                }
            }

            return classObjectMapTip;
        }

        public static ClassSkill GetClassSkillNewFormat(string value)
        {
            ClassSkill classSkill = new ClassSkill();

            // コメント行を取り除く
            {
                string[] line = value.Split(Environment.NewLine).ToArray();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Contains("#") == true)
                    {
                        var data = line[i].Split("#");
                        line[i] = String.Concat(data[0], Environment.NewLine);
                    }
                }
                value = String.Join(Environment.NewLine, line);
            }

            //nameTag
            {
                var nameTag = new Regex(GetPatTag("NewFormatSkill"), RegexOptions.IgnoreCase)
                                .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.NameTag = first.Value.Replace(Environment.NewLine, "");
            }
            //fkey
            {
                var fkey =
                    new Regex(GetPat("fkey"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(fkey);
                if (first == null)
                {
                    classSkill.FKey = (string.Empty, -1);
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "").Split("*").ToList();
                    classSkill.FKey = (re[0], int.Parse(re[1]));
                }
            }
            //sortkey
            {
                var sortkey =
                    new Regex(GetPat("sortkey"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(sortkey);
                if (first == null)
                {
                    classSkill.SortKey = 0;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classSkill.SortKey = int.Parse(re);
                }
            }
            //func
            {
                var func =
                    new Regex(GetPat("func"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(func);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Func = (SkillFunc)Enum.Parse(typeof(SkillFunc), first.Value.Replace(Environment.NewLine, ""));
            }
            //icon
            {
                var icon =
                    new Regex(GetPat("icon"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(icon);
                if (first == null)
                {
                    classSkill.Icon = new List<string>();
                }
                else
                {
                    classSkill.Icon = first.Value
                                        .Replace(Environment.NewLine, "")
                                        .Replace(" ", "")
                                        .Split(",").ToList();
                }
            }
            //name
            {
                var name =
                    new Regex(GetPat("name"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(name);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Name = first.Value.Replace(Environment.NewLine, "");
            }
            //help
            {
                var help =
                    new Regex(GetPat("help"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(help);
                if (first == null)
                {
                    classSkill.Help = string.Empty;
                }
                else
                {
                    // 改行前後の余分なスペースとタブを除去してから、$を改行に置換する
                    classSkill.Help = MoldingText(first.Value, "$");
                }
            }
            //center
            {
                var center =
                    new Regex(GetPat("center"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(center);
                if (first == null)
                {
                }
                else
                {
                    classSkill.Center = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //mp
            {
                var mp =
                    new Regex(GetPat("mp"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(mp);
                if (first == null)
                {
                }
                else
                {
                    classSkill.Mp = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //slow_per
            {
                var slow_per =
                    new Regex(GetPat("slow_per"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(slow_per);
                if (first == null)
                {

                }
                else
                {
                    classSkill.SlowPer = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //slow_time
            {
                var slow_time =
                    new Regex(GetPat("slow_time"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(slow_time);
                if (first == null)
                {

                }
                else
                {
                    classSkill.SlowTime = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //sound
            {
                var sound =
                    new Regex(GetPat("sound"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(sound);
                if (first == null)
                {
                    classSkill.Sound = new List<string>();
                }
                else
                {
                    classSkill.Sound = first.Value
                                        .Replace(Environment.NewLine, "")
                                        .Replace(" ", "")
                                        .Split(",").ToList();
                }
            }
            //image
            {
                var image =
                    new Regex(GetPat("image"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(image);
                if (first == null)
                {

                }
                else
                {
                    classSkill.Image = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //direct
            {
                var direct =
                    new Regex(GetPat("direct"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(direct);
                if (first == null)
                {

                }
                else
                {
                    classSkill.Direct = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //w
            {
                var w =
                    new Regex(GetPat("w"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(w);
                if (first == null)
                {

                }
                else
                {
                    classSkill.W = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //h
            {
                var h =
                    new Regex(GetPat("h"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(h);
                if (first == null)
                {

                }
                else
                {
                    classSkill.H = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //a
            {
                var a =
                    new Regex(GetPat("a"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(a);
                if (first == null)
                {

                }
                else
                {
                    classSkill.A = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //force_fire
            {
                var force_fire =
                    new Regex(GetPat("force_fire"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(force_fire);
                if (first == null)
                {

                }
                else
                {
                    classSkill.ForceFire = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //attr
            {
                var attr =
                    new Regex(GetPat("attr"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(attr);
                if (first == null)
                {

                }
                else
                {
                    classSkill.Attr = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //str
            {
                var str =
                    new Regex(GetPat("str"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(str);
                if (first == null)
                {
                    classSkill.Str = (string.Empty, -1);
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "").Split("*").ToList();
                    classSkill.Str = (re[0], int.Parse(re[1]));
                }
            }
            //range
            {
                var range =
                    new Regex(GetPat("range"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(range);
                if (first == null)
                {

                }
                else
                {
                    classSkill.Range = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //damage_range_adjust
            {
                var damage_range_adjust =
                    new Regex(GetPat("damage_range_adjust"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(damage_range_adjust);
                if (first == null)
                {

                }
                else
                {
                    classSkill.DamageRangeAdjust = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //range_min
            {
                var range_min =
                    new Regex(GetPat("range_min"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(range_min);
                if (first == null)
                {

                }
                else
                {
                    classSkill.RangeMin = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //speed
            {
                var speed =
                    new Regex(GetPat("speed"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(speed);
                if (first == null)
                {

                }
                else
                {
                    classSkill.Speed = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //gun_delay
            {
                var gun_delay =
                    new Regex(GetPat("gun_delay"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(gun_delay);
                if (first == null)
                {
                    classSkill.GunDelay = (string.Empty, -1);
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "").Split("*").ToList();
                    classSkill.GunDelay = (re[0], int.Parse(re[1]));
                }
            }
            //pair_next
            {
                var pair_next =
                    new Regex(GetPat("pair_next"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(pair_next);
                if (first == null)
                {

                }
                else
                {
                    classSkill.PairNext = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //next
            {
                var next =
                    new Regex(GetPat("next"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(next);
                if (first == null)
                {

                }
                else
                {
                    classSkill.Next = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //random_space
            {
                var random_space =
                    new Regex(GetPat("random_space"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(random_space);
                if (first == null)
                {

                }
                else
                {
                    classSkill.RandomSpace = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //offset
            {
                var offset =
                    new Regex(GetPat("offset"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(offset);
                if (first == null)
                {

                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                    classSkill.Offset = re;
                }
            }
            //ray
            {
                var ray =
                    new Regex(GetPat("ray"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(ray);
                if (first == null)
                {
                    classSkill.Ray = new List<int>();
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "").Split(',').Select(Int32.Parse)?.ToList();
                    classSkill.Ray = re != null ? re : new List<int>();
                }
            }
            //force_ray
            {
                var force_ray =
                    new Regex(GetPat("force_ray"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(force_ray);
                if (first == null)
                {

                }
                else
                {
                    classSkill.ForceRay = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //rush
            {
                var rush =
                    new Regex(GetPat("rush"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(rush);
                if (first == null)
                {
                    classSkill.Rush = -1;
                }
                else
                {
                    classSkill.Rush = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //rush_interval
            {
                var rush_interval =
                    new Regex(GetPat("rush_interval"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(rush_interval);
                if (first == null)
                {
                    classSkill.RushInterval = -1;
                }
                else
                {
                    classSkill.RushInterval = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //rush_random_degree
            {
                var rush_random_degree =
                    new Regex(GetPat("rush_random_degree"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(rush_random_degree);
                if (first == null)
                {
                    classSkill.RushRandomDegree = -1;
                }
                else
                {
                    classSkill.RushRandomDegree = int.Parse(first.Value.Replace(Environment.NewLine, ""));
                }
            }

            return classSkill;
        }

        public static ClassInternalAffairsDetail GetClassInternalAffairsDetail(string value)
        {
            ClassInternalAffairsDetail classInternalAffairsDetail = new ClassInternalAffairsDetail();

            // コメント行を取り除く
            {
                string[] line = value.Split(Environment.NewLine).ToArray();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Contains("#") == true)
                    {
                        var data = line[i].Split("#");
                        line[i] = String.Concat(data[0], Environment.NewLine);
                    }
                }
                value = String.Join(Environment.NewLine, line);
            }

            //nameTag
            {
                var nameTag = new Regex(GetPatTag("internalAffairsDetail"), RegexOptions.IgnoreCase)
                                .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classInternalAffairsDetail.NameTag = first.Value.Replace(Environment.NewLine, "");
            }
            //title
            {
                var title =
                    new Regex(GetPat("title"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(title);
                if (first == null)
                {
                    throw new Exception();
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classInternalAffairsDetail.Title = re;
                }
            }
            //Image
            {
                var Image =
                    new Regex(GetPat("Image"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(Image);
                if (first == null)
                {
                    throw new Exception();
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classInternalAffairsDetail.Image = re;
                }
            }
            //Help
            {
                var Help =
                    new Regex(GetPat("Help"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(Help);
                if (first == null)
                {
                    classInternalAffairsDetail.Help = "-1";
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classInternalAffairsDetail.Help = re;
                }
            }
            //Premise
            {
                var Premise =
                    new Regex(GetPat("Premise"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(Premise);
                if (first == null)
                {
                    classInternalAffairsDetail.Premise = "-1";
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classInternalAffairsDetail.Premise = re;
                }
            }
            //Cost
            {
                var Cost =
                    new Regex(GetPat("Cost"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(Cost);
                if (first == null)
                {
                    classInternalAffairsDetail.Cost = -1;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classInternalAffairsDetail.Cost = int.Parse(re);
                }
            }
            //PremiseNumber
            {
                var PremiseNumber =
                    new Regex(GetPat("PremiseNumber"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(PremiseNumber);
                if (first == null)
                {
                    classInternalAffairsDetail.PremiseNumber = -1;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classInternalAffairsDetail.PremiseNumber = int.Parse(re);
                }
            }
            //IncomeCorrection
            {
                var IncomeCorrection =
                    new Regex(GetPat("IncomeCorrection"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(IncomeCorrection);
                if (first == null)
                {
                    classInternalAffairsDetail.IncomeCorrection = 0;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classInternalAffairsDetail.IncomeCorrection = double.Parse(re);
                }
            }
            //Income
            {
                var Income =
                    new Regex(GetPat("Income"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(Income);
                if (first == null)
                {
                    classInternalAffairsDetail.Income = 0;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classInternalAffairsDetail.Income = int.Parse(re);
                }
            }
            //skill
            {
                var skill =
                    new Regex(GetPat("skill"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(skill);
                if (first == null)
                {
                    classInternalAffairsDetail.Skill = string.Empty;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    if (re == "-1")
                    {
                        classInternalAffairsDetail.Skill = string.Empty;
                    }
                    else
                    {
                        classInternalAffairsDetail.Skill = re;
                    }
                }
            }
            //unit
            {
                var unit =
                    new Regex(GetPat("unit"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(unit);
                if (first == null)
                {
                    classInternalAffairsDetail.Unit = string.Empty;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    if (re == "-1")
                    {
                        classInternalAffairsDetail.Unit = string.Empty;
                    }
                    else
                    {
                        classInternalAffairsDetail.Unit = re;
                    }
                }
            }
            //ability
            {
                var ability =
                    new Regex(GetPat("ability"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(ability);
                if (first == null)
                {
                    classInternalAffairsDetail.Ability = string.Empty;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    if (re == "-1")
                    {
                        classInternalAffairsDetail.Ability = string.Empty;
                    }
                    else
                    {
                        classInternalAffairsDetail.Ability = re;
                    }
                }
            }
            //abilityTiming
            {
                var abilityTiming =
                    new Regex(GetPat("abilityTiming"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(abilityTiming);
                if (first == null)
                {
                    classInternalAffairsDetail.AbilityTiming = string.Empty;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    if (re == "-1")
                    {
                        classInternalAffairsDetail.AbilityTiming = string.Empty;
                    }
                    else
                    {
                        classInternalAffairsDetail.AbilityTiming = re;
                    }
                }
            }
            //abilityUp
            {
                var abilityUp =
                    new Regex(GetPat("abilityUp"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(abilityUp);
                if (first == null)
                {
                    classInternalAffairsDetail.AbilityUp = 0;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classInternalAffairsDetail.AbilityUp = int.Parse(re);
                }
            }

            return classInternalAffairsDetail;
        }

        public static void GetClassEvent(string value, ClassGameStatus classGameStatus)
        {
            // コメント行を取り除く
            {
                string[] line = value.Split(Environment.NewLine).ToArray();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Contains("//") == true)
                    {
                        var data = line[i].Split("//");
                        line[i] = String.Concat(data[0], Environment.NewLine);
                    }
                }
                value = String.Join(Environment.NewLine, line);
            }

            var nameTag =
                new Regex(GetPatTagEvent("Event"), RegexOptions.IgnoreCase)
                .Matches(value);
            var first = CheckMatchElement(nameTag);
            if (first == null)
            {
                throw new Exception();
            }
            ClassEvent classEvent = new ClassEvent();
            classEvent.Name = first.Value.Replace(Environment.NewLine, "");

            var siki =
                new Regex(GetPatEvent("Event", classEvent.Name), RegexOptions.IgnoreCase)
                .Matches(value);
            var result = CheckMatchElement(siki);
            if (result == null)
            {
                throw new Exception();
            }

            var lexer = new Lexer(result.Value);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            classEvent.Root = root;
            classGameStatus.ListEvent.Add(classEvent);
        }

        public static ClassContext GetClassContextNewFormat(string value)
        {
            ClassContext classContext = new ClassContext();

            // コメント行を取り除く
            value = ReplaceComment(value);

            //title_name
            {
                var title_name =
                    new Regex(GetPat("title_name"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(title_name);
                if (first == null)
                {
                    throw new Exception();
                }
                classContext.TitleName = first.Value.Replace(Environment.NewLine, "");
            }
            //title_menu_space
            {
                var title_menu_space =
                    new Regex(GetPat("title_menu_space"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(title_menu_space);
                if (first != null)
                {
                    classContext.TitleMenuSpace = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //gain_per
            {
                var gain_per =
                    new Regex(GetPat("gain_per"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(gain_per);
                if (first != null)
                {
                    classContext.GainPer = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //neutral_max
            {
                var neutral_max =
                    new Regex(GetPat("neutral_max"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(neutral_max);
                if (first != null)
                {
                    classContext.NeutralMax = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //neutral_min
            {
                var neutral_min =
                    new Regex(GetPat("neutral_min"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(neutral_min);
                if (first != null)
                {
                    classContext.NeutralMin = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //neutral_member_max
            {
                var neutral_member_max =
                    new Regex(GetPat("neutral_member_max"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(neutral_member_max);
                if (first != null)
                {
                    classContext.NeutralMemberMax = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //neutral_member_min
            {
                var neutral_member_min =
                    new Regex(GetPat("neutral_member_min"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(neutral_member_min);
                if (first != null)
                {
                    classContext.neutralMemberMin = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //enemy_turn_skip
            {
                var enemy_turn_skip =
                    new Regex(GetPat("enemy_turn_skip"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(enemy_turn_skip);
                if (first == null)
                {
                    classContext.enemyTurnSkip = false;
                }
                else
                {
                    classContext.enemyTurnSkip = bool.Parse(first.Value);
                }
            }

            return classContext;
        }

        private static string ReplaceComment(string value)
        {
            value = value.ReplaceLineEndings();
            string[] line = value.Split(Environment.NewLine).ToArray();
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Contains("#") == true)
                {
                    var data = line[i].Split("#");
                    line[i] = String.Concat(data[0], Environment.NewLine);
                }
            }
            value = String.Join(Environment.NewLine, line);
            return value;
        }

        public static ClassSpot GetClassSpotNewFormat(string value, string fullname)
        {
            ClassSpot classSpot = new ClassSpot();

            // コメント行を取り除く
            value = ReplaceComment(value);

            //NewFormatSpot
            {
                var nameTag =
                    new Regex(GetPatTag("NewFormatSpot"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.NameTag = first.Value.Replace(Environment.NewLine, "");
            }
            //name
            {
                var name =
                    new Regex(GetPat("name"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(name);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.Name = first.Value.Replace(Environment.NewLine, "");
            }
            //image
            {
                var image =
                    new Regex(GetPat("image"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(image);
                if (first == null)
                {
                    throw new Exception();
                }

                List<string> strings = new List<string>();
                strings.Add(fullname);
                strings.Add("025_CityImage");
                strings.Add(first.Value.Replace(Environment.NewLine, ""));
                string path = System.IO.Path.Combine(strings.ToArray());
                classSpot.ImagePath = path;
            }
            //x
            {
                var x =
                    new Regex(GetPat("x"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(x);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.X = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
            }
            //y
            {
                var y =
                    new Regex(GetPat("y"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(y);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.Y = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
            }
            //Gain
            {
                var match_result =
                    new Regex(GetPat("gain"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(match_result);
                if (first != null)
                {
                    classSpot.Gain = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //Castle
            {
                var match_result =
                    new Regex(GetPat("castle"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(match_result);
                if (first != null)
                {
                    classSpot.Castle = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //Capacity
            {
                var match_result =
                    new Regex(GetPat("capacity"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(match_result);
                if (first != null)
                {
                    classSpot.Capacity = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
                }
            }
            //member
            {
                var member =
                    new Regex(GetPatComma("member"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(member);
                if (first == null)
                {
                    classSpot.ListMember = new List<(string, int)>();
                }
                else
                {
                    var tete = new List<(string, int)>();
                    foreach (var item in first.Value.Replace(Environment.NewLine, "").Split(","))
                    {
                        if (item.Contains("*") == true)
                        {
                            var tete2 = item.Split("*");
                            for (int i = 0; i < Convert.ToInt32(tete2[1]); i++)
                            {
                                tete.Add(new(tete2[0], 1));
                            }
                        }
                        else
                        {
                            tete.Add(new(item, 1));
                        }
                    }
                    classSpot.ListMember = tete;
                }
            }
            //map
            {
                var map =
                    new Regex(GetPat("map"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(map);
                if (first == null)
                {
                    classSpot.Map = String.Empty;
                }
                else
                {
                    classSpot.Map = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //wanderingMonster
            {
                var wanderingMonster =
                    new Regex(GetPat("wanderingMonster"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(wanderingMonster);
                if (first == null)
                {
                    classSpot.ListWanderingMonster = new List<(string, int)>();
                }
                else
                {
                    List<(string, int)> values = new List<(string, int)>();
                    foreach (var item in first.Value.Replace(Environment.NewLine, "").Split(",").ToList())
                    {
                        var cutResult = item.Split("*").ToList();
                        values.Add(new(cutResult[0], int.Parse(cutResult[1])));
                    }
                    classSpot.ListWanderingMonster = values;
                }
            }
            //monsterOrder
            {
                var monsterOrder =
                    new Regex(GetPat("monsterOrder"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(monsterOrder);
                if (first == null)
                {
                    classSpot.MonsterOrder = "random";
                }
                else
                {
                    classSpot.MonsterOrder = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //monster
            {
                var monster =
                    new Regex(GetPat("monster"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(monster);
                if (first == null)
                {
                    classSpot.ListMonster = new List<(string, int)>();
                }
                else
                {
                    List<(string, int)> values = new List<(string, int)>();
                    foreach (var item in first.Value.Replace(Environment.NewLine, "").Split(",").ToList())
                    {
                        var cutResult = item.Split("*").ToList();
                        if (cutResult.Count == 1)
                        {
                            values.Add(new(cutResult[0], 1));
                        }
                        else
                        {
                            values.Add(new(cutResult[0], int.Parse(cutResult[1])));
                        }
                    }
                    classSpot.ListMonster = values;
                }
            }
            //text
            {
                var text =
                    new Regex(GetPat("text"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(text);
                if (first == null)
                {
                    classSpot.Text = string.Empty;
                }
                else
                {
                    // 改行前後の余分なスペースとタブを除去してから、$を改行に置換する
                    classSpot.Text = MoldingText(first.Value, "$");
                }
            }

            return classSpot;
        }

        public static ClassSpot GetClassSpot(string value, string fullName)
        {
            ClassSpot classSpot = new ClassSpot();

            {
                var nameTag =
                    new Regex(@"(?<=spot[\s]*)([\S\n]+?)(?=[\s]|{)", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.NameTag = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var name =
                    new Regex(@"(?<=name\s*=\s*)([\S\n]+?.*(?=[\s\n]))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(name);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.Name = first.Value.Replace(System.Environment.NewLine, "").Replace("\r", "");
            }
            {
                var image =
                    new Regex(@"(?<=image\s*=\s*)([\S\n]+?.*(?=[\s\n]))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(image);
                if (first == null)
                {
                    throw new Exception();
                }
                var a = first.Value.Replace(System.Environment.NewLine, "").Replace("\r", "");

                List<string> strings = new List<string>();
                strings.Add(fullName);
                strings.Add("025_CityImage");
                //暫定でpng
                strings.Add(a + ".png");
                string path = System.IO.Path.Combine(strings.ToArray());

                classSpot.ImagePath = path;
            }
            {
                var x =
                    new Regex(@"(?<=x\s*=\s*)([\S\n]+?.*(?=[\s\n]))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(x);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.X = Convert.ToInt32(first.Value.Replace(System.Environment.NewLine, "").Replace("\r", ""));
            }
            {
                var y =
                    new Regex(@"(?<=y\s*=\s*)([\S\n]+?.*(?=[\s\n]))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(y);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.Y = Convert.ToInt32(first.Value.Replace(System.Environment.NewLine, "").Replace("\r", ""));
            }

            return classSpot;
        }

        public static ClassScenarioInfo GetClassScenario(string value)
        {
            ClassScenarioInfo classScenario = new ClassScenarioInfo();
            return classScenario;
        }

        public static ClassScenarioInfo GetClassScenarioNewFormat(string value)
        {
            ClassScenarioInfo classScenario = new ClassScenarioInfo();

            // コメント行を取り除く
            value = ReplaceComment(value);

            //scenarioName
            {
                //先読み、戻り読みの言明、Assertion
                //肯定先読み、肯定戻り読み
                var scenarioName =
                    new Regex(@"(?<=scenario_name[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(scenarioName);
                if (first == null)
                {
                    throw new Exception();
                }
                classScenario.ScenarioName = first.Value.Replace(Environment.NewLine, "");
            }
            //sortkey
            {
                var sortkey =
                    new Regex(@"(?<=sortkey[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(sortkey);
                if (first == null)
                {
                    throw new Exception();
                }
                classScenario.Sortkey = Convert.ToInt32(first.Value);
            }
            //help
            {
                var help =
                    new Regex(GetPat("help"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(help);
                if (first != null)
                {
                    classScenario.Help = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //text
            {
                var text =
                    new Regex(@"(?<=text[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(text);
                if (first == null)
                {
                    classScenario.ScenarioIntroduce = string.Empty;
                }
                else
                {
                    // 改行前後の余分なスペースとタブを除去してから、$を改行に置換する
                    classScenario.ScenarioIntroduce = MoldingText(first.Value, "$");
                }
            }
            //scenario_image_rate
            {
                var scenario_image_rate =
                    new Regex(@"(?<=scenario_image_rate[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(scenario_image_rate);
                if (first == null)
                {
                    classScenario.ScenarioImageRate = 0;
                }
                else
                {
                    classScenario.ScenarioImageRate = Convert.ToInt32(first.Value);
                    if (classScenario.ScenarioImageRate < 0)
                    {
                        classScenario.ScenarioImageRate = 0;
                    }
                    else if (classScenario.ScenarioImageRate > 100)
                    {
                        classScenario.ScenarioImageRate = 100;
                    }
                }
            }
            //scenario_image
            {
                var scenario_image =
                    new Regex(@"(?<=scenario_image[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(scenario_image);
                if (first == null)
                {
                    classScenario.ScenarioImage = String.Empty;
                }
                else
                {
                    classScenario.ScenarioImage = Convert.ToString(first.Value);
                }
            }
            //map_image_name_file
            {
                var map_image_name_file =
                    new Regex(@"(?<=map_image_name_file[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(map_image_name_file);
                if (first == null)
                {
                    classScenario.NameMapImageFile = String.Empty;
                }
                else
                {
                    classScenario.NameMapImageFile = Convert.ToString(first.Value);
                }
            }
            //buttonType
            {
                var buttonType =
                    new Regex(@"(?<=ButtonType[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(buttonType);
                if (first == null)
                {
                    classScenario.ButtonType = ButtonType.Scenario;
                }
                else
                {
                    ButtonType buttonType1;
                    Enum.TryParse(first.Value, out buttonType1);
                    classScenario.ButtonType = buttonType1;
                }
            }
            //mail
            {
                var mail =
                    new Regex(@"(?<=mail[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(mail);
                if (first == null)
                {
                    classScenario.Mail = String.Empty;
                }
                else
                {
                    classScenario.Mail = Convert.ToString(first.Value);
                }
            }
            //internet
            {
                var internet =
                    new Regex(@"(?<=internet[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(internet);
                if (first == null)
                {
                    classScenario.Internet = String.Empty;
                }
                else
                {
                    classScenario.Internet = Convert.ToString(first.Value);
                }
            }
            //spot
            {
                var spot =
                    new Regex(@"(?<=spot[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(spot);
                if (first == null)
                {
                    classScenario.DisplayListSpot = new List<string>();
                }
                else
                {
                    classScenario.DisplayListSpot = first.Value.Replace("\t", "").Replace(Environment.NewLine, "").Split(",").ToList();
                }
            }
            //power
            {
                var power =
                    new Regex(GetPat("power"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(power);
                if (first != null)
                {
                    classScenario.ListPower = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                    classScenario.InitListPower = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                }
            }
            //spot_capacity
            {
                var spot_capacity =
                    new Regex(GetPat("spot_capacity"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(spot_capacity);
                if (first == null)
                {
                    classScenario.SpotCapacity = 8;
                }
                else
                {
                    classScenario.SpotCapacity = Convert.ToInt32(first.Value);
                }
            }
            //war_capacity
            {
                var war_capacity =
                    new Regex(GetPat("war_capacity"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(war_capacity);
                if (first == null)
                {
                    classScenario.WarCapacity = 8;
                }
                else
                {
                    classScenario.WarCapacity = Convert.ToInt32(first.Value);
                }
            }
            //member_capacity
            {
                var member_capacity =
                    new Regex(GetPat("member_capacity"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(member_capacity);
                if (first == null)
                {
                    classScenario.MemberCapacity = 8;
                }
                else
                {
                    classScenario.MemberCapacity = Convert.ToInt32(first.Value);
                }
            }
            //linkSpot
            {
                var linkSpot =
                    new Regex(GetPatMethod("linkSpot"), RegexOptions.IgnoreCase)
                    .Matches(value);
                if (linkSpot == null)
                {
                    classScenario.DisplayListSpot = new List<string>();
                }
                else
                {
                    classScenario.ListLinkSpot = new List<(string, string)>();
                    foreach (var item in linkSpot)
                    {
                        var conv = Convert.ToString(item);
                        if (conv == null)
                        {
                            continue;
                        }
                        var sp = conv.Split(',');
                        classScenario.ListLinkSpot.Add((sp[0].Trim(), sp[1].Trim()));
                    }
                }
            }
            //world
            {
                var world =
                    new Regex(GetPat("world"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(world);
                if (first == null)
                {
                    classScenario.World = String.Empty;
                }
                else
                {
                    classScenario.World = Convert.ToString(first.Value);
                }
            }

            return classScenario;
        }

        public static ClassPower GetClassPowerNewFormat(string value, string fullName)
        {
            ClassPower classPower = new ClassPower();

            // コメント行を取り除く
            value = ReplaceComment(value);

            {
                var name =
                    new Regex(@"(?<=name[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(name);
                if (first == null)
                {
                    throw new Exception();
                }
                classPower.Name = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var nameTag =
                    new Regex(GetPatTag("NewFormatPower"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classPower.NameTag = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var help =
                    new Regex(GetPat("help"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(help);
                if (first != null)
                {
                    classPower.Help = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var money =
                    new Regex(@"(?<=money[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(money);
                if (first == null)
                {
                    classPower.Money = 0;
                }
                else
                {
                    classPower.Money = Convert.ToInt32(first.Value);
                }
            }
            {
                var flag =
                    new Regex(@"(?<=flag[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(flag);
                if (first == null)
                {
                    throw new Exception();
                }

                List<string> strings = new List<string>();
                strings.Add(fullName);
                strings.Add("030_FlagImage");
                strings.Add(first.Value.Replace(Environment.NewLine, ""));
                string path = System.IO.Path.Combine(strings.ToArray());

                if (File.Exists(path + ".png") == true)
                {
                    classPower.FlagPath = path + ".png";
                }
                if (File.Exists(path + ".jpg") == true)
                {
                    classPower.FlagPath = path + ".jpg";
                }
            }
            {
                var master =
                    new Regex(@"(?<=master[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(master);
                if (first == null)
                {
                    throw new Exception();
                }
                classPower.MasterTag = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var home =
                    new Regex(@"(?<=home[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(home);
                if (first == null)
                {
                    classPower.ListHome = new List<string>();
                }
                else
                {
                    classPower.ListHome = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                }
            }
            {
                var head =
                    new Regex(@"(?<=head[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(head);
                if (first == null)
                {
                    classPower.Head = String.Empty;
                }
                else
                {
                    classPower.Head = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var diff =
                    new Regex(@"(?<=diff[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(diff);
                if (first == null)
                {
                    classPower.Diff = String.Empty;
                }
                else
                {
                    classPower.Diff = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var text =
                    new Regex(GetPat("text"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(text);
                if (first == null)
                {
                    classPower.Text = string.Empty;
                }
                else
                {
                    // 改行前後の余分なスペースとタブを除去してから、$を改行に置換する
                    classPower.Text = MoldingText(first.Value, "$");
                }
            }
            {
                var enable_select =
                    new Regex(GetPat("enable_select"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(enable_select);
                if (first == null)
                {
                    classPower.EnableSelect = "on";
                }
                else
                {
                    classPower.EnableSelect = "off";
                }
            }
            {
                var member =
                    new Regex(GetPatComma("member"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(member);
                if (first == null)
                {
                    throw new Exception();
                }
                classPower.ListMember = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
            }
            {
                var commonConscription =
                    new Regex(GetPatComma("commonConscription"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(commonConscription);
                if (first == null)
                {
                    throw new Exception();
                }
                classPower.ListCommonConscription = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
            }
            {
                var image =
                    new Regex(GetPat("image"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(image);
                if (first == null)
                {
                    classPower.Image = string.Empty;
                }
                else
                {
                    List<string> strings = new List<string>();
                    strings.Add(fullName);
                    strings.Add("035_PowerImage");
                    strings.Add(first.Value.Replace(Environment.NewLine, ""));
                    string path = System.IO.Path.Combine(strings.ToArray());

                    classPower.Image = path;
                }
            }
            //fix
            {
                var fix =
                    new Regex(GetPat("fix"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(fix);
                if (first == null)
                {
                    classPower.Fix = FlagPowerFix.off;
                }
                else
                {
                    FlagPowerFix sEnum = (FlagPowerFix)Enum.Parse(typeof(FlagPowerFix), first.Value, true);
                    classPower.Fix = sEnum;
                }
            }

            return classPower;
        }

        public static ClassPower GetClassPower(string value)
        {
            ClassPower classPower = new ClassPower();
            return classPower;
        }

        public static ClassUnit GetClassUnitNewFormat(string value)
        {
            ClassUnit classUnit = new ClassUnit();

            // コメント行を取り除く
            value = ReplaceComment(value);

            {
                var nameTag =
                    new Regex(GetPatTag("NewFormatUnit"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classUnit.NameTag = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var formation =
                    new Regex(GetPat("formation"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(formation);
                if (first == null)
                {
                    classUnit.Formation = new ClassFormation()
                    {
                        Id = 0,
                        Formation = Formation.F
                    };
                }
                else
                {
                    int conv = Convert.ToInt32(first.Value);
                    switch (conv)
                    {
                        case 0:
                            classUnit.Formation = new ClassFormation()
                            {
                                Id = 0,
                                Formation = Formation.F
                            };
                            break;
                        case 1:
                            classUnit.Formation = new ClassFormation()
                            {
                                Id = 1,
                                Formation = Formation.M
                            };
                            break;
                        case 2:
                            classUnit.Formation = new ClassFormation()
                            {
                                Id = 2,
                                Formation = Formation.B
                            };
                            break;
                        default:
                            classUnit.Formation = new ClassFormation()
                            {
                                Id = 0,
                                Formation = Formation.F
                            };
                            break;
                    }
                }
            }
            {
                var name =
                    new Regex(GetPat("name"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(name);
                if (first == null)
                {
                    throw new Exception();
                }
                classUnit.Name = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var help =
                    new Regex(GetPat("help"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(help);
                if (first == null)
                {
                    classUnit.Help = string.Empty;
                }
                else
                {
                    classUnit.Help = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var race =
                    new Regex(GetPat("race"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(race);
                if (first == null)
                {
                    throw new Exception();
                }
                classUnit.Race = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var unitClass =
                    new Regex(GetPat("class"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(unitClass);
                if (first == null)
                {
                    throw new Exception();
                }
                classUnit.Class = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var image =
                    new Regex(GetPat("image"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(image);
                if (first == null)
                {
                    throw new Exception();
                }
                classUnit.Image = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var dead =
                    new Regex(GetPat("dead"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(dead);
                if (first == null)
                {
                    classUnit.Dead = String.Empty;
                }
                else
                {
                    classUnit.Dead = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var retreat =
                    new Regex(GetPat("retreat"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(retreat);
                if (first == null)
                {
                    classUnit.Retreat = String.Empty;
                }
                else
                {
                    classUnit.Retreat = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var join =
                    new Regex(GetPat("join"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(join);
                if (first == null)
                {
                    classUnit.Join = String.Empty;
                }
                else
                {
                    classUnit.Join = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var face =
                    new Regex(GetPat("face"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(face);
                if (first == null)
                {
                    classUnit.Face = string.Empty;
                }
                else
                {
                    classUnit.Face = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var voice_type =
                    new Regex(GetPat("voice_type"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(voice_type);
                if (first == null)
                {
                    classUnit.Voice_type = string.Empty;
                }
                else
                {
                    classUnit.Voice_type = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //gender
            {
                var gender =
                    new Regex(GetPat("gender"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(gender);
                if (first == null)
                {
                    classUnit.Gender = 0;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classUnit.Gender = (Gender)Enum.Parse(typeof(Gender), re, true);
                }
            }

            {
                var talent =
                    new Regex(GetPat("talent"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(talent);
                if (first == null)
                {
                    classUnit.Talent = "off";
                }
                else
                {
                    classUnit.Talent = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var friend =
                    new Regex(GetPat("friend"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(friend);
                if (first == null)
                {
                    classUnit.Friend = string.Empty;
                }
                else
                {
                    classUnit.Friend = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var merce =
                    new Regex(GetPat("merce"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(merce);
                if (first == null)
                {
                    classUnit.Merce = string.Empty;
                }
                else
                {
                    classUnit.Merce = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var staff =
                    new Regex(GetPat("staff"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(staff);
                if (first == null)
                {
                    classUnit.Staff = string.Empty;
                }
                else
                {
                    classUnit.Staff = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var initMember =
                    new Regex(GetPat("initMember"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(initMember);
                if (first == null)
                {
                    classUnit.InitMember = string.Empty;
                }
                else
                {
                    classUnit.InitMember = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var enemy =
                    new Regex(GetPat("enemy"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(enemy);
                if (first == null)
                {
                    classUnit.Enemy = string.Empty;
                }
                else
                {
                    classUnit.Enemy = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var level =
                    new Regex(GetPat("level"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(level);
                if (first == null)
                {
                    classUnit.Level = 0;
                }
                else
                {
                    classUnit.Level = Convert.ToInt32(first.Value);
                }
            }
            {
                var level_max =
                    new Regex(GetPat("level_max"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(level_max);
                if (first == null)
                {
                    classUnit.Level_max = 99999;
                }
                else
                {
                    classUnit.Level_max = Convert.ToInt32(first.Value);
                }
            }
            {
                var price =
                    new Regex(GetPat("price"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(price);
                if (first == null)
                {
                    classUnit.Price = 0;
                }
                else
                {
                    classUnit.Price = Convert.ToInt32(first.Value);
                }
            }
            {
                var cost =
                    new Regex(GetPat("cost"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(cost);
                if (first == null)
                {
                    classUnit.Cost = 0;
                }
                else
                {
                    classUnit.Cost = Convert.ToInt32(first.Value);
                }
            }
            {
                var medical =
                    new Regex(GetPat("medical"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(medical);
                if (first == null)
                {
                    classUnit.Medical = 0;
                }
                else
                {
                    classUnit.Medical = Convert.ToInt32(first.Value);
                }
            }
            {
                var hasExp =
                    new Regex(GetPat("hasExp"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(hasExp);
                if (first == null)
                {
                    classUnit.HasExp = 0;
                }
                else
                {
                    classUnit.HasExp = Convert.ToInt32(first.Value);
                }
            }
            {
                var hp =
                    new Regex(GetPat("hp"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(hp);
                if (first == null)
                {
                    classUnit.Hp = 0;
                }
                else
                {
                    classUnit.Hp = Convert.ToInt32(first.Value);
                }
            }
            {
                var mp =
                    new Regex(GetPat("mp"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(mp);
                if (first == null)
                {
                    classUnit.Mp = 0;
                }
                else
                {
                    classUnit.Mp = Convert.ToInt32(first.Value);
                }
            }
            {
                var attack =
                    new Regex(GetPat("attack"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(attack);
                if (first == null)
                {
                    classUnit.Attack = 0;
                }
                else
                {
                    classUnit.Attack = Convert.ToInt32(first.Value);
                }
            }
            {
                var defense =
                    new Regex(GetPat("defense"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(defense);
                if (first == null)
                {
                    classUnit.Defense = 0;
                }
                else
                {
                    classUnit.Defense = Convert.ToInt32(first.Value);
                }
            }
            {
                var magic =
                    new Regex(GetPat("magic"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(magic);
                if (first == null)
                {
                    classUnit.Magic = 0;
                }
                else
                {
                    classUnit.Magic = Convert.ToInt32(first.Value);
                }
            }
            {
                var magDef =
                    new Regex(GetPat("magDef"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(magDef);
                if (first == null)
                {
                    classUnit.MagDef = 0;
                }
                else
                {
                    classUnit.MagDef = Convert.ToInt32(first.Value);
                }
            }
            {
                var speed =
                    new Regex(GetPat("speed"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(speed);
                if (first == null)
                {
                    classUnit.Speed = 0;
                }
                else
                {
                    classUnit.Speed = Convert.ToDouble(first.Value);
                }
            }
            {
                var dext =
                    new Regex(GetPat("dext"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(dext);
                if (first == null)
                {
                    classUnit.Dext = 0;
                }
                else
                {
                    classUnit.Dext = Convert.ToInt32(first.Value);
                }
            }
            {
                var move =
                    new Regex(GetPat("move"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(move);
                if (first == null)
                {
                    classUnit.Move = 0;
                }
                else
                {
                    classUnit.Move = Convert.ToInt32(first.Value);
                }
            }
            {
                var hprec =
                    new Regex(GetPat("hprec"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(hprec);
                if (first == null)
                {
                    classUnit.Hprec = 0;
                }
                else
                {
                    classUnit.Hprec = Convert.ToInt32(first.Value);
                }
            }
            {
                var mprec =
                    new Regex(GetPat("mprec"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(mprec);
                if (first == null)
                {
                    classUnit.Mprec = 0;
                }
                else
                {
                    classUnit.Mprec = Convert.ToInt32(first.Value);
                }
            }
            {
                var exp =
                    new Regex(GetPat("exp"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(exp);
                if (first == null)
                {
                    classUnit.Exp = 0;
                }
                else
                {
                    classUnit.Exp = Convert.ToInt32(first.Value);
                }
            }
            {
                var exp_mul =
                    new Regex(GetPat("exp_mul"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(exp_mul);
                if (first == null)
                {
                    classUnit.Exp_mul = 0;
                }
                else
                {
                    classUnit.Exp_mul = Convert.ToInt32(first.Value);
                }
            }
            {
                var heal_max =
                    new Regex(GetPat("heal_max"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(heal_max);
                if (first == null)
                {
                    classUnit.Heal_max = 0;
                }
                else
                {
                    classUnit.Heal_max = Convert.ToInt32(first.Value);
                }
            }
            {
                var summon_max =
                    new Regex(GetPat("summon_max"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(summon_max);
                if (first == null)
                {
                    classUnit.Summon_max = 0;
                }
                else
                {
                    classUnit.Summon_max = Convert.ToInt32(first.Value);
                }
            }
            {
                var no_knock =
                    new Regex(GetPat("no_knock"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(no_knock);
                if (first == null)
                {
                    classUnit.No_knock = 0;
                }
                else
                {
                    classUnit.No_knock = Convert.ToInt32(first.Value);
                }
            }
            {
                var loyal =
                    new Regex(GetPat("loyal"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(loyal);
                if (first == null)
                {
                    classUnit.Loyal = 0;
                }
                else
                {
                    classUnit.Loyal = Convert.ToInt32(first.Value);
                }
            }
            {
                var alive_per =
                    new Regex(GetPat("alive_per"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(alive_per);
                if (first == null)
                {
                    classUnit.Alive_per = 0;
                }
                else
                {
                    classUnit.Alive_per = Convert.ToInt32(first.Value);
                }
            }
            {
                var escape_range =
                    new Regex(GetPat("escape_range"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(escape_range);
                if (first == null)
                {
                    classUnit.Escape_range = 0;
                }
                else
                {
                    classUnit.Escape_range = Convert.ToInt32(first.Value);
                }
            }
            {
                var skill =
                    new Regex(GetPat("skill"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(skill);
                if (first == null)
                {
                    classUnit.SkillName = new List<string>();
                }
                else
                {
                    classUnit.SkillName = first.Value
                                        .Replace(Environment.NewLine, "")
                                        .Replace(" ", "")
                                        .Split(",").ToList();
                }
            }

            {
                var finance =
                    new Regex(GetPat("finance"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(finance);
                if (first == null)
                {
                    classUnit.Finance = 0;
                }
                else
                {
                    classUnit.Finance = Convert.ToInt32(first.Value);
                }
            }
            {
                var movetype =
                    new Regex(GetPat("movetype"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(movetype);
                if (first == null)
                {
                    classUnit.MoveType = string.Empty;
                }
                else
                {
                    classUnit.MoveType = first.Value
                                        .Replace(Environment.NewLine, "");
                }
            }


            //ここですべきでない
            //classUnit.ID = this.ClassGameStatus.IDCount;
            //this.ClassGameStatus.SetIDCount();
            return classUnit;
        }

        public static ClassUnit GetClassUnit(string value)
        {
            ClassUnit classUnit = new ClassUnit();
            return classUnit;
        }
        public static ClassDiplomacy GetClassDiplomacy(string value)
        {
            ClassDiplomacy classDiplomacy = new ClassDiplomacy();

            // コメント行を取り除く
            value = ReplaceComment(value);

            //diplo
            {
                var diplo =
                    new Regex(GetPat("diplo"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(diplo);
                if (first != null)
                {
                    var va = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                    foreach (var item in va)
                    {
                        var eq = item.Split("=");
                        var cou = classDiplomacy.Diplo.Where(x => x.Item1 == eq[0] && x.Item2 == eq[1]).Count();
                        if (cou > 1)
                        {
                            throw new Exception("diploで同じ勢力が重複して設定されています");
                        }
                        classDiplomacy.Diplo.Add(new(eq[0], eq[1], int.Parse(eq[2])));
                    }
                }
            }
            //league
            {
                var league =
                    new Regex(GetPat("league"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(league);
                if (first != null)
                {
                    var va = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                    foreach (var item in va)
                    {
                        var eq = item.Split("=");
                        var cou = classDiplomacy.League.Where(x => x.Item1 == eq[0] && x.Item2 == eq[1]).Count();
                        if (cou > 1)
                        {
                            throw new Exception("Leagueで同じ勢力が重複して設定されています");
                        }
                        classDiplomacy.League.Add(new(eq[0], eq[1], int.Parse(eq[2])));
                    }
                }
            }
            //enemy_power
            {
                var enemy_power =
                    new Regex(GetPat("enemy_power"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(enemy_power);
                if (first != null)
                {
                    var va = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                    foreach (var item in va)
                    {
                        var eq = item.Split("=");
                        var cou = classDiplomacy.EnemyPower.Where(x => x.Item1 == eq[0] && x.Item2 == eq[1] && x.Item3 == eq[2]).Count();
                        if (cou > 1)
                        {
                            throw new Exception("EnemyPowerで同じ勢力が重複して設定されています");
                        }
                        classDiplomacy.EnemyPower.Add(new(eq[0], eq[1], eq[2], int.Parse(eq[3])));
                    }
                }
            }
            //one-way_love
            {
                var one_way_love =
                    new Regex(GetPat("one-way_love"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(one_way_love);
                if (first != null)
                {
                    var va = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                    foreach (var item in va)
                    {
                        var eq0 = item.Split("⇒");
                        var eq = eq0[1].Split("=");
                        var cou = classDiplomacy.OneWayLove.Where(x => x.Item1 == eq0[0] && x.Item2 == eq[0]).Count();
                        if (cou > 1)
                        {
                            throw new Exception("one-way_loveで同じ勢力が重複して設定されています");
                        }
                        classDiplomacy.OneWayLove.Add(new(eq0[0], eq[0], int.Parse(eq[1])));
                    }
                }
            }
            //cold
            {
                var cold =
                    new Regex(GetPat("cold"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(cold);
                if (first != null)
                {
                    var va = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                    foreach (var item in va)
                    {
                        var eq0 = item.Split("⇒");
                        var eq = eq0[1].Split("=");
                        var cou = classDiplomacy.Cold.Where(x => x.Item1 == eq0[0] && x.Item2 == eq[0]).Count();
                        if (cou > 1)
                        {
                            throw new Exception("Coldで同じ勢力が重複して設定されています");
                        }
                        classDiplomacy.Cold.Add(new(eq0[0], eq[0], int.Parse(eq[1])));
                    }
                }
            }

            return classDiplomacy;
        }

        public static Match? CheckMatchElement(MatchCollection scenarioName)
        {
            if (scenarioName == null)
            {
                return null;
            }
            if (scenarioName.Count > 1)
            {
                //タグが複数指定されています
                throw new NotImplementedException();
            }

            return scenarioName.FirstOrDefault();
        }

        /// <summary>
        /// 通常のパターン
        /// name = "test";
        /// など
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetPat(string name)
        {
            string a = @"(?<=[\s\n]+" + name + @"[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\"";))";
            return a;
        }

        /// <summary>
        /// これいる？GetPatで良い気がする
        /// member = "aaa,bbb,ccc"
        /// など
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetPatComma(string name)
        {
            return @"(?<=[\s\n]+" + name + @"[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))";
        }

        public static string GetPatTag(string name)
        {
            return @"(?<=" + name + @"[\s]*)([\S\n]+?)(?=[\s]|{)";
        }
        public static string GetPatMethod(string name)
        {
            return @"(?<=" + name + @"\()([\S\n\s]+?)(?=\);)";
        }
        public static string GetPatTagEvent(string name)
        {
            return @"(?<=" + name + @"[\s]*)([\S\n]+?)(?=[\s]+?<-)";
        }
        public static string GetPatEvent(string tag, string name)
        {
            return @"(?<=" + tag + @"[\s]*" + name + @"[\S\n\s]*<-)([\S\n\s]+?)(?=->)";
        }
        #endregion

        #region MoldingText
        /// <summary>
        /// テキスト浄化処理
        /// </summary>
        /// <param name="strTarget"></param>
        /// <param name="strStatus"></param>
        /// <returns></returns>
        public static string MoldingText(string strTarget, string strStatus)
        {
            // 改行ごとに分割 (Split) するため、改行コードを統一する。
            string strTemp = strTarget.ReplaceLineEndings();
            // Split で各行に分割した後、Trim で前後のスペースとタブを取り除く。
            char[] charsToTrim = { ' ', '\t' };
            string[] strLines = strTemp.Split(System.Environment.NewLine).Select(x => x.Trim(charsToTrim)).ToArray();
            // 各行を連結して、一つに戻す。
            strTemp = String.Join("", strLines);

            if (strStatus.Contains("$"))
            {
                // ヴァーレントゥーガのテキスト用特殊記号「$」を改行に置換する。
                strTemp = strTemp.Replace("$", System.Environment.NewLine);
            }
            if (strStatus.Contains("<double>"))
            {
                // テキスト用特殊記号「<double>」を「"」に置換する。
                strTemp = strTemp.Replace("<double>", "\"");
            }

            return strTemp;
        }
        #endregion

        #region ReturnBaseColor
        /// <summary>
        /// 基礎カラー返却
        /// </summary>
        /// <returns></returns>
        public static SolidColorBrush ReturnBaseColor()
        {
            return new SolidColorBrush(Color.FromRgb(190, 178, 175));
        }
        #endregion

        #region KeepInterval
        /// <summary>
        /// タイマーのずれを直すメソッド
        /// </summary>
        /// <param name="timer"></param>
        public static void KeepInterval(DispatcherTimer timer)
        {
            double constantInterval = 16.6;

            var now = DateTime.Now;
            var nowMilliseconds = now.TimeOfDay.TotalMilliseconds;
            // 16.6から、例えば24.9/16.6の余りである8.3を引くことで、
            // 次の実行が16.6から8.3となり結果的に1秒間60回実行が保たれる
            var timerInterval = constantInterval -
                                nowMilliseconds % constantInterval;
            timer.Interval = TimeSpan.FromMilliseconds(timerInterval);
        }
        #endregion
    }
}
