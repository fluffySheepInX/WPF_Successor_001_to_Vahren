using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassVec
    {
        public double X { get; set; }
        public double Y { get; set; }

        private double _speed;

        public double Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        private Point _target;

        public Point Target
        {
            get { return _target; }
            set { _target = value; }
        }

        private Point _vec;

        public Point Vec
        {
            get { return _vec; }
            set { _vec = value; }
        }

        /// <summary>
        /// 移動後の距離を試算して、標的に接近中なら false
        /// 距離が同じままか、あるいは遠ざかってるなら true を返す
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool Hit(Point current)
        {
            double current_distanceX = this.Target.X - current.X;
            double current_distanceY = this.Target.Y - current.Y;
            double next_distanceX = current_distanceX - (this.Vec.X * this.Speed);
            double next_distanceY = current_distanceY - (this.Vec.Y * this.Speed);
            if (next_distanceX * next_distanceX + next_distanceY * next_distanceY >= current_distanceX * current_distanceX + current_distanceY * current_distanceY)
            {
                return true;
            }
            return false;
        }

        public void Set()
        {
            var calc0 = ReturnVecDistance(from: new Point(this.X, this.Y), to: Target);
            this.Vec = ReturnNormalize(calc0);
        }

        public void Move()
        {
            this.X = X + (this.Vec.X * this.Speed);
            this.Y = Y + (this.Vec.Y * this.Speed);
        }

        public Point Get(Point targetFrom)
        {
            double xMove = (this.Vec.X * this.Speed);
            double yMove = (this.Vec.Y * this.Speed);
            double resultX = targetFrom.X + xMove;
            double resultY = targetFrom.Y + yMove;
            return new Point(resultX, resultY);
        }

        public Point ReturnVecDistance(Point from, Point to)
        {
            Point re = new Point();
            re.X = to.X - from.X;
            re.Y = to.Y - from.Y;
            return re;
        }
        public Point ReturnNormalize(Point target)
        {
            var result = target;
            var calc = Math.Sqrt(target.X * target.X + target.Y * target.Y);
            if (calc > 0)
            {
                var reciprocal = 1 / calc;
                result.X *= reciprocal;
                result.Y *= reciprocal;
            }
            return result;
        }
    }
}
