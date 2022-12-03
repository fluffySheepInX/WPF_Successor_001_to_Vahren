using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace WPF_Successor_001_to_Vahren._006_ClassStatic
{
    public static class ClassStaticCommonMethod
    {
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
    }
}
