using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPF_Successor_001_to_Vahren._006_ClassStatic;
using WPF_Successor_001_to_Vahren._010_Enum;
using WPF_Successor_001_to_Vahren._020_AST;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassAStar
    {
        public ClassAStar(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        private aStarStatus aStarStatus = aStarStatus.None;

        public aStarStatus AStarStatus
        {
            get { return aStarStatus; }
            set { aStarStatus = value; }
        }

        private int row;

        public int Row
        {
            get { return row; }
            set { row = value; }
        }

        private int col;

        public int Col
        {
            get { return col; }
            set { col = value; }
        }

        private int cost = 0;

        public int Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        private int hCost;

        public int HCost
        {
            get { return hCost; }
            set { hCost = value; }
        }

        private ClassAStar? classAStar = null;

        public ClassAStar? RefClassAStar
        {
            get { return classAStar; }
            set { classAStar = value; }
        }

        public void GetRoot(List<Point> target)
        {
            target.Add(new Point(Row, Col));
            if (RefClassAStar != null)
            {
                RefClassAStar.GetRoot(target);
            }
        }

    }

    public class ClassAStarManager
    {
        #region Pool
        private Dictionary<string, ClassAStar> pool = new Dictionary<string, ClassAStar>();
        public Dictionary<string, ClassAStar> Pool
        {
            get { return pool; }
            set { pool = value; }
        }
        #endregion

        #region ListClassAStar
        private List<ClassAStar>? listClassAStar = null;
        public List<ClassAStar>? ListClassAStar
        {
            get { return listClassAStar; }
            set { listClassAStar = value; }
        }
        #endregion
        private int endX;
        public int EndX
        {
            get { return endX; }
            set { endX = value; }
        }
        private int endY;
        public int EndY
        {
            get { return endY; }
            set { endY = value; }
        }

        public ClassAStarManager(int x, int y)
        {
            this.EndX = x;
            this.EndY = y;
            this.ListClassAStar = new List<ClassAStar>();
        }
        public ClassAStar CreateClassAStar(int x, int y)
        {
            var re = new ClassAStar(x, y);
            re.HCost = ClassStaticBattle.HeuristicMethod(x, y, EndX, EndY);
            return re;
        }

        public void RemoveClassAStar(ClassAStar classAStar)
        {
            if (this.ListClassAStar != null)
            {
                this.ListClassAStar.Remove(classAStar);
            }
        }

        public ClassAStar? OpenOne(int x, int y, int cost, ClassAStar? parent)
        {
            if (x < 0 || y < 0)
            {
                return null;
            }

            Pool.TryGetValue(x + "/" + y, out ClassAStar? value);
            if (value != null)
            {
                return value;
            }

            var getClassAStar = CreateClassAStar(x, y);
            getClassAStar.AStarStatus = aStarStatus.Open;
            getClassAStar.Cost = cost;
            getClassAStar.RefClassAStar = parent;

            if (this.ListClassAStar != null)
            {
                this.ListClassAStar.Add(getClassAStar);
                Pool.Add(getClassAStar.Row + "/" + getClassAStar.Col, getClassAStar);

            }
            else
            {
                throw new Exception();
            }

            return getClassAStar;
        }

        public void OpenAround(ClassAStar parent, List<List<MapDetail>> MapData, ClassGameStatus classGameStatus)
        {
            var x = parent.Row;
            var y = parent.Col;
            var cost = parent.Cost;
            cost += 1;
            for (int j = -1; j < 2; j++)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (x + i < 0 || y + j < 0 || x + i >= MapData.Count() || y + j >= MapData[x + i].Count())
                    {
                        continue;
                    }

                    if (MapData[x + i][y + j].Building == string.Empty)
                    {
                        OpenOne(x + i, y + j, cost, parent);
                    }
                    else
                    {
                        var ob = classGameStatus.ListObject.Where(tar => tar.NameTag == MapData[x + i][y + j].Building).FirstOrDefault();
                        if (ob != null)
                        {
                            switch (ob.Type)
                            {
                                case MapTipObjectType.WALL2:
                                    break;
                                case MapTipObjectType.GATE:
                                    OpenOne(x + i, y + j, cost, parent);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }
    }
}
