using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._030_Evaluator
{
    public class IntegerObject : IObject
    {
        public int Value { get; set; }

        public IntegerObject(int value) => this.Value = value;
        public string Inspect() => this.Value.ToString();
        public ObjectType Type() => ObjectType.INTEGER;
    }
    public class ReturnValue : IObject
    {
        public IObject Value { get; set; }

        public ReturnValue(IObject value) => this.Value = value;
        public string Inspect() => this.Value.Inspect();
        public ObjectType Type() => ObjectType.RETURN_VALUE;
    }

    public class BooleanObject : IObject
    {
        public bool Value { get; set; }

        public BooleanObject(bool value) => this.Value = value;
        public string Inspect() => this.Value ? "true" : "false";
        public ObjectType Type() => ObjectType.BOOLEAN;
    }
    public class NullObject : IObject
    {
        public string Inspect() => "null";
        public ObjectType Type() => ObjectType.NULL;
    }
}
