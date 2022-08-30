using System;
using System.Collections.Generic;

namespace WPF_Successor_001_to_Vahren._030_Evaluator
{
    public class Enviroment
    {
        public Dictionary<string, IObject> Store { get; set; }
            = new Dictionary<string, IObject>();

        public (IObject?, bool) Get(string name)
        {
            var ok = this.Store.TryGetValue(name, out var value);
            return (value, ok);
        }

        public IObject Set(string name, IObject value)
        {
            if (this.Store.ContainsKey(name))
            {
                this.Store[name] = value;
            }
            else
            {
                this.Store.Add(name, value);
            }
            return value;
        }
    }
}
