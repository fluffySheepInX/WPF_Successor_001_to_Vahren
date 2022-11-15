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

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
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

                        //ClassConfigGameTitle _classConfigGameTitle = new ClassConfigGameTitle();
                        //int _nowNumberGameTitle = 0;

                        // get target path.
                        List<string> strings = new List<string>();
                        strings.Add(Environment.CurrentDirectory);
                        strings.Add("001_Warehouse");
                        strings.Add("001_DefaultGame");
                        strings.Add("055_TestBattle");
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

                                    ClassTestBattle classTestBattle = new ClassTestBattle();
                                    if (kind == 0)
                                    {
                                        classTestBattle = GetClassTestBattle(getData.Value);
                                    }
                                    else
                                    {
                                        //ClassGameStatus.ListUnit.Add(GetClassUnit(getData.Value));
                                    }
                                }
                            }

                        }
                    }

                    break;
                default:
                    MessageBox.Show("未知の引数：" + e.Args[0]);
                    break;
            }
        }

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

            //map
            {
                var map =
                    new Regex(WPF_Successor_001_to_Vahren.MainWindow.GetPat("map"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = WPF_Successor_001_to_Vahren.MainWindow.CheckMatchElement(map);
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
                    new Regex(WPF_Successor_001_to_Vahren.MainWindow.GetPat("player"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = WPF_Successor_001_to_Vahren.MainWindow.CheckMatchElement(player);
                if (first == null)
                {
                    classTestBattle.Player = String.Empty;
                }
                else
                {
                    classTestBattle.Player = first.Value;
                }
            }


            return classTestBattle;
        }
    }
}
