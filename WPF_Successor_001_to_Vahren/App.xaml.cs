using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using WPF_Successor_001_to_Vahren._005_Class;
using WPF_Successor_001_to_Vahren._006_ClassStatic;
using WPF_Successor_001_to_Vahren._010_Enum;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Application_Startup
        /// <summary>
        /// 最初に実行される
        /// デバッグ用に作成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                return;
            }

            //「プロセスにアタッチ」機能でデバッグ可能
            switch (e.Args[0])
            {
                case "/debug":

                    MessageBox.Show(e.Args[0]);

                    if (e.Args[1] == "/battle")
                    {
                        MessageBox.Show(e.Args[1]);

                        ClassConfigGameTitle _classConfigGameTitle = new ClassConfigGameTitle();
                        //int _nowNumberGameTitle = 0;

                        // get target path.
                        List<string> strings = new List<string>();
                        strings.Add(Environment.CurrentDirectory);
                        strings.Add("001_Warehouse");
                        strings.Add("001_DefaultGame");

                        var b = System.IO.Directory.CreateDirectory(System.IO.Path.Combine(strings.ToArray()));
                        _classConfigGameTitle.DirectoryGameTitle.Add(b);

                        strings.Add("070_Scenario");
                        string path = System.IO.Path.Combine(strings.ToArray());

                        // get file.
                        var files = System.IO.Directory.EnumerateFiles(
                            path,
                            "*.txt",
                            System.IO.SearchOption.AllDirectories
                            );

                        //check
                        {
                            if (files.Count() < 1)
                            {
                                // ファイルがない！
                                throw new Exception();
                            }
                        }

                        //ファイル毎に繰り返し
                        List<ClassTestBattle> classTestBattle = new List<ClassTestBattle>();
                        ClassGameStatus classGameStatus = new ClassGameStatus();

                        foreach (var item in files)
                        {
                            string readAllLines;
                            readAllLines = File.ReadAllText(item);

                            if (readAllLines.Length == 0)
                            {
                                continue;
                            }

                            // 大文字かっこは許しまへんで
                            {
                                var ch = readAllLines.Length - readAllLines.Replace("{", "").Replace("}", "").Length;
                                if (ch % 2 != 0 || readAllLines.Length - ch == 0)
                                {
                                    throw new Exception();
                                }
                            }

                            //TestBattle
                            {

                                string targetString = "TestBattle";
                                // 大文字かっこも入るが、上でチェックしている
                                // \sは空行や改行など
                                var newFormatScenarioMatches = new Regex(targetString + @"[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase)
                                                                        .Matches(readAllLines);
                                var listMatches = newFormatScenarioMatches
                                                    .Where(x => x != null)
                                                    .ToList();
                                if (listMatches == null)
                                {
                                    // データがない！
                                    throw new Exception();
                                }
                                if (listMatches.Count < 1)
                                {
                                    // データがないので次
                                }
                                else
                                {
                                    foreach (var getData in listMatches)
                                    {
                                        //enumを使うべき？
                                        int kind = 0;
                                        {
                                            //このコードだとNewFormatUnitTest等が通るのでよくない
                                            string join = string.Join(String.Empty, getData.Value.Take(targetString.Length));
                                            if (String.Compare(join, targetString, true) == 0)
                                            {
                                                kind = 0;
                                            }
                                            else
                                            {
                                                kind = 1;
                                            }
                                        }

                                        if (kind == 0)
                                        {
                                            classTestBattle.Add(GetClassTestBattle(getData.Value));
                                        }
                                        else
                                        {
                                            //ClassGameStatus.ListUnit.Add(GetClassUnit(getData.Value));
                                        }
                                    }
                                }
                            }

                            // Map
                            {
                                string targetString = "map";
                                // 大文字かっこも入るが、上でチェックしている←本当か？
                                // \sは空行や改行など
                                var mapMatches = new Regex(targetString + @"[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase)
                                                    .Matches(readAllLines);

                                var listMatches = mapMatches.Where(x => x != null).ToList();
                                if (listMatches == null)
                                {
                                    // データがない！
                                    throw new Exception();
                                }
                                if (listMatches.Count < 1)
                                {
                                    // データがないので次
                                }
                                else
                                {
                                    foreach (var getData in listMatches)
                                    {
                                        classGameStatus.ListClassMapBattle.Add(ClassStaticCommonMethod.GetClassMapBattle(getData.Value));
                                    }
                                }

                                // Map 終わり
                            }

                            // Unit
                            {
                                string targetString = "NewFormatUnit";
                                // 大文字かっこも入るが、上でチェックしている
                                // \sは空行や改行など
                                var newFormatScenarioMatches = new Regex(targetString + @"[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase).Matches(readAllLines);
                                var scenarioMatches = new Regex(@"Unit[\s]+?.*[\s]+?\{([\s\S\n]+?)\}").Matches(readAllLines);

                                var listMatches = newFormatScenarioMatches.Where(x => x != null).ToList();
                                listMatches.AddRange(scenarioMatches.Where(x => x != null).ToList());

                                if (listMatches == null)
                                {
                                    // データがない！
                                    throw new Exception();
                                }
                                if (listMatches.Count < 1)
                                {
                                    // データがないので次
                                }
                                else
                                {
                                    foreach (var getData in listMatches)
                                    {
                                        //enumを使うべき？
                                        int kind = 0;
                                        {
                                            //このコードだとNewFormatUnitTest等が通るのでよくない
                                            string join = string.Join(String.Empty, getData.Value.Take(targetString.Length));
                                            if (String.Compare(join, targetString, true) == 0)
                                            {
                                                kind = 0;
                                            }
                                            else
                                            {
                                                kind = 1;
                                            }
                                        }

                                        if (kind == 0)
                                        {
                                            classGameStatus.ListUnit.Add(ClassStaticCommonMethod.GetClassUnitNewFormat(getData.Value));
                                        }
                                        else
                                        {
                                            //ClassGameStatus.ListUnit.Add(GetClassUnit(getData.Value));
                                        }
                                    }
                                }
                            }
                            // Unit 終わり

                            // Skill
                            {
                                string targetString = "NewFormatSkill";
                                // 大文字かっこも入るが、上でチェックしている
                                // \sは空行や改行など
                                var newFormatScenarioMatches = new Regex(targetString + @"[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase).Matches(readAllLines);
                                var scenarioMatches = new Regex(@"Skill[\s]+?.*[\s]+?\{([\s\S\n]+?)\}").Matches(readAllLines);

                                var listMatches = newFormatScenarioMatches.Where(x => x != null).ToList();
                                listMatches.AddRange(scenarioMatches.Where(x => x != null).ToList());

                                if (listMatches == null)
                                {
                                    // データがない！
                                    throw new Exception();
                                }
                                if (listMatches.Count < 1)
                                {
                                    // データがないので次
                                }
                                else
                                {
                                    foreach (var getData in listMatches)
                                    {
                                        //enumを使うべき？
                                        int kind = 0;
                                        {
                                            //このコードだとNewFormatUnitTest等が通るのでよくない
                                            string join = string.Join(String.Empty, getData.Value.Take(targetString.Length));
                                            if (String.Compare(join, targetString, true) == 0)
                                            {
                                                kind = 0;
                                            }
                                            else
                                            {
                                                kind = 1;
                                            }
                                        }

                                        if (kind == 0)
                                        {
                                            classGameStatus.ListSkill.Add(ClassStaticCommonMethod.GetClassSkillNewFormat(getData.Value));
                                        }
                                        else
                                        {
                                            //ClassGameStatus.ListUnit.Add(GetClassUnit(getData.Value));
                                        }
                                    }
                                }
                            }
                            // Skill 終わり

                            //object
                            {
                                string targetString = "NewFormatObject";
                                // 大文字かっこも入るが、上でチェックしている
                                // \sは空行や改行など
                                var newFormatScenarioMatches = new Regex(targetString + @"[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase).Matches(readAllLines);
                                var scenarioMatches = new Regex(@"Object[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase).Matches(readAllLines);

                                var listMatches = newFormatScenarioMatches.Where(x => x != null).ToList();
                                listMatches.AddRange(scenarioMatches.Where(x => x != null).ToList());

                                if (listMatches != null)
                                {
                                    if (listMatches.Count < 1)
                                    {
                                        // データがないので次
                                    }
                                    else
                                    {
                                        foreach (var getData in listMatches)
                                        {
                                            //enumを使うべき？
                                            int kind = 0;
                                            {
                                                //このコードだとNewFormatObjectTest等が通るのでよくない
                                                string join = string.Join(String.Empty, getData.Value.Take(targetString.Length));
                                                if (String.Compare(join, targetString, true) == 0)
                                                {
                                                    kind = 0;
                                                }
                                                else
                                                {
                                                    kind = 1;
                                                }
                                            }

                                            if (kind == 0)
                                            {
                                                classGameStatus.ListObject.Add(ClassStaticCommonMethod.GetClassObjNewFormat(getData.Value));
                                            }
                                            else
                                            {
                                                //ClassGameStatus.ListObject.Add(GetClassObj(getData.Value));
                                            }
                                        }
                                    }
                                }
                            }
                            //object 終わり
                        }

                        // unitのスキル名からスキルクラスを探し、unitに格納
                        foreach (var itemUnit in classGameStatus.ListUnit)
                        {
                            foreach (var itemSkillName in itemUnit.SkillName)
                            {
                                var x = classGameStatus.ListSkill
                                        .Where(x => x.NameTag == itemSkillName)
                                        .FirstOrDefault();
                                if (x == null)
                                {
                                    continue;
                                }

                                itemUnit.Skill.Add(x);
                            }
                        }


                        foreach (var item in classTestBattle)
                        {
                            if (string.Join("", e.Args[2].Skip(1).ToList()) == item.NameTag)
                            {
                                classGameStatus.ClassBattle.BattleWhichIsThePlayer =
                                    item.Player;
                                CreateMap(item, _classConfigGameTitle, classGameStatus);
                                return;
                            }
                        }

                        MessageBox.Show("/Battle end");
                    }

                    break;
                default:
                    MessageBox.Show("未知の引数：" + e.Args[0]);
                    break;
            }

            return;
        }
        #endregion
        #region GetClassTestBattle
        private ClassTestBattle GetClassTestBattle(string value)
        {
            ClassTestBattle classTestBattle = new ClassTestBattle();

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

            //tag name
            {
                var nameTag = new Regex(ClassStaticCommonMethod.GetPatTag("TestBattle"), RegexOptions.IgnoreCase)
                                .Matches(value);
                var first = ClassStaticCommonMethod.CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classTestBattle.NameTag = first.Value.Replace(Environment.NewLine, "");
            }

            //map
            {
                var map =
                    new Regex(ClassStaticCommonMethod.GetPat("map"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = ClassStaticCommonMethod.CheckMatchElement(map);
                if (first == null)
                {
                    classTestBattle.Map = String.Empty;
                }
                else
                {
                    classTestBattle.Map = first.Value;
                }
            }
            //player
            {
                var player =
                    new Regex(ClassStaticCommonMethod.GetPat("player"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = ClassStaticCommonMethod.CheckMatchElement(player);
                if (first == null)
                {
                    classTestBattle.Player = _010_Enum.BattleWhichIsThePlayer.Sortie;
                }
                else
                {
                    classTestBattle.Player = (_010_Enum.BattleWhichIsThePlayer)Enum.Parse(typeof(_010_Enum.BattleWhichIsThePlayer), first.Value, true);
                }
            }
            //memberKougeki
            {
                var member =
                    new Regex(ClassStaticCommonMethod.GetPatComma("memberKougeki"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = ClassStaticCommonMethod.CheckMatchElement(member);
                if (first == null)
                {
                    classTestBattle.ListMember = new List<(string, int)>();
                }
                else
                {
                    var tete = new List<(string, int)>();
                    foreach (var item in first.Value.Replace(Environment.NewLine, "").Split(","))
                    {
                        if (item.Contains("*") == true)
                        {
                            var tete2 = item.Split("*");
                            tete.Add(new(tete2[0], Convert.ToInt32(tete2[1])));
                        }
                        else
                        {
                            tete.Add(new(item, 1));
                        }
                    }
                    classTestBattle.ListMember = tete;
                }
            }
            //memberBouei
            {
                var member =
                    new Regex(ClassStaticCommonMethod.GetPatComma("memberBouei"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = ClassStaticCommonMethod.CheckMatchElement(member);
                if (first == null)
                {
                    classTestBattle.ListMemberBouei = new List<(string, int)>();
                }
                else
                {
                    var tete = new List<(string, int)>();
                    foreach (var item in first.Value.Replace(Environment.NewLine, "").Split(","))
                    {
                        if (item.Contains("*") == true)
                        {
                            var tete2 = item.Split("*");
                            tete.Add(new(tete2[0], Convert.ToInt32(tete2[1])));
                        }
                        else
                        {
                            tete.Add(new(item, 1));
                        }
                    }
                    classTestBattle.ListMemberBouei = tete;
                }
            }


            return classTestBattle;
        }
        #endregion

        #region Map生成
        private static void CreateMap(ClassTestBattle classTestBattle, ClassConfigGameTitle _classConfigGameTitle, ClassGameStatus classGameStatus)
        {
            Win010_TestBattle window = new Win010_TestBattle(classTestBattle, _classConfigGameTitle, classGameStatus);

            window.ShowDialog();
        }
        #endregion
    }
}
