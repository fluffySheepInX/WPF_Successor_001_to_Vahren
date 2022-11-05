using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WPF_Successor_001_to_Vahren._005_Class;
using WPF_Successor_001_to_Vahren._020_AST;
using System.Threading;
using System.Windows.Automation;
using WPF_Successor_001_to_Vahren._015_Lexer;

namespace WPF_Successor_001_to_Vahren._030_Evaluator
{
    public class Evaluator
    {
        public BooleanObject True = new BooleanObject(true);
        public BooleanObject False = new BooleanObject(false);
        public NullObject Null = new NullObject();
        public MainWindow? window = null;
        public ClassGameStatus ClassGameStatus { get; set; } = new ClassGameStatus();

        public IObject? Eval(INode? node, Enviroment enviroment)
        {
            switch (node)
            {
                //文
                case Root root:
                    return this.EvalRootProgram(root.Statements, enviroment);
                case ExpressionStatement statement:
                    {
                        var re = this.Eval(statement.Expression, enviroment);
                        return re;
                    }
                // 式
                case IntegerLiteral integerLiteral:
                    return new IntegerObject(integerLiteral.Value);
                case BooleanLiteral booleanLiteral:
                    return booleanLiteral.Value ? this.True : this.False;
                case PrefixExpression prefixExpression:
                    {
                        var right = this.Eval(prefixExpression.Right, enviroment);
                        if (right == null)
                        {
                            return null;
                        }
                        if (right == null)
                        {
                            return null;
                        }
                        return this.EvalPrefixExpression(prefixExpression.Operator, right);
                    }
                case InfixExpression infixExpression:
                    {
                        var le = this.Eval(infixExpression.Left, enviroment);
                        var ri = this.Eval(infixExpression.Right, enviroment);
                        if (le == null || ri == null)
                        {
                            return null;
                        }

                        return this.EvalInfixExpression(
                            infixExpression.Operator,
                            le,
                            ri
                        );
                    }
                case BlockStatement blockStatement:
                    {
                        if (blockStatement.Statements == null)
                        {
                            return null;
                        }
                        return this.EvalBlockStatement(blockStatement, enviroment);
                    }
                case IfExpression ifExpression:
                    return this.EvalIfExpression(ifExpression, enviroment);
                case ReturnStatement returnStatement:
                    {
                        //var value = this.Eval(returnStatement.ReturnValue, enviroment);
                        //if (value == null) return null;
                        return new ReturnValue(new IntegerObject(-1));
                    }
                case LetStatement letStatement:
                    {
                        var letValue = this.Eval(letStatement.Value, enviroment);
                        if (letValue == null)
                        {
                            return null;
                        }
                        if (letStatement.Name == null)
                        {
                            return null;
                        }
                        enviroment.Set(letStatement.Name.Value, letValue);
                        break;
                    }
                case Identifier identifier:
                    return this.EvalIdentifier(identifier, enviroment);
                case SystemFunctionLiteral systemFunctionLiteral:
                    if (systemFunctionLiteral.Token.Type == TokenType.TALK ||
                        systemFunctionLiteral.Token.Type == TokenType.MSG)
                    {
                        if (this.window != null)
                        {
                            this.window.DoWork(systemFunctionLiteral);
                        }
                    }
                    else if (systemFunctionLiteral.Token.Type == TokenType.PUSHTURN)
                    {
                        if (this.window != null)
                        {
                            enviroment.Set(systemFunctionLiteral.Parameters[0].Value,
                                            new IntegerObject(this.window.ClassGameStatus.NowTurn));
                        }
                    }
                    else if (systemFunctionLiteral.Token.Type == TokenType.STOREPLAYERPOWER)
                    {
                        if (this.window != null)
                        {
                            enviroment.Set(systemFunctionLiteral.Parameters[0].Value,
                                            new IntegerObject(this.window.ClassGameStatus.SelectionPowerAndCity.ClassPower.Index));
                        }
                    }
                    else if (systemFunctionLiteral.Token.Type == TokenType.PUSHCOUNTPOWER)
                    {
                        if (this.window != null)
                        {
                            enviroment.Set(systemFunctionLiteral.Parameters[0].Value,
                                            new IntegerObject(this.window.ClassGameStatus.NowCountPower));
                        }
                    }
                    else if (systemFunctionLiteral.Token.Type == TokenType.PUSHSPOT)
                    {
                        if (this.window != null)
                        {
                            var re = enviroment.Get(systemFunctionLiteral.Parameters[0].Value);
                            var intRe = re.Item1 as IntegerObject;
                            if (intRe == null) return null;

                            var getPo = this.window.ClassGameStatus.ListPower.Where(x => x.Index == intRe.Value).FirstOrDefault();
                            if (getPo == null) return null;
                            enviroment.Set(systemFunctionLiteral.Parameters[1].Value,
                                            new IntegerObject(getPo.ListNowMember.Count));
                        }
                    }
                    else if (systemFunctionLiteral.Token.Type == TokenType.YET)
                    {
                        if (this.window != null)
                        {
                            string nameEvent = systemFunctionLiteral.Parameters[0].Value;
                            if (nameEvent == string.Empty) return this.False;

                            var ev = this.ClassGameStatus.ListEvent
                                        .Where(x => x.Name == nameEvent)
                                        .FirstOrDefault();
                            if (ev == null)
                            {
                                return this.False;
                            }
                            if (ev.Yet == true)
                            {
                                return this.True;
                            }
                            else
                            {
                                return this.False;
                            }
                        }
                    }
                    else if (systemFunctionLiteral.Token.Type == TokenType.ISALIVE)
                    {
                        if (this.window != null)
                        {
                            var re = enviroment.Get(systemFunctionLiteral.Parameters[0].Value);
                            var intRe = re.Item1 as IntegerObject;
                            if (intRe == null) return null;

                            int powerIndex = intRe.Value;
                            if (powerIndex == -1) return this.False;

                            var ev = this.ClassGameStatus.ListPower
                                        .Where(x => x.Index == powerIndex)
                                        .FirstOrDefault();
                            if (ev == null)
                            {
                                return this.False;
                            }
                            if (ev.IsDownfall == true)
                            {
                                return this.False;
                            }
                            else
                            {
                                return this.True;
                            }
                        }
                    }
                    else if (systemFunctionLiteral.Token.Type == TokenType.DISPLAYGAMERESULT)
                    {
                        if (this.window != null)
                        {
                            Uri uri = new Uri("/Page030_GameResult.xaml", UriKind.Relative);
                            Frame frame = new Frame();
                            frame.Source = uri;
                            frame.Margin = new Thickness(0, 0, 0, 0);
                            frame.Name = StringName.windowGameResult;
                            this.window.canvasMain.Children.Add(frame);
                            Application.Current.Properties["window"] = this.window;
                        }
                    }

                    return null;
                case DialogLiteral dialogLiteral:
                    if (dialogLiteral.Token.Type == TokenType.DIALOG)
                    {
                        MessageBox.Show(dialogLiteral.Parameters[0].Value.Replace("@@", System.Environment.NewLine));
                    }
                    return null;
                case ChoiceLiteral choiceLiteral:
                    if (choiceLiteral.Token.Type == TokenType.CHOICE)
                    {
                        Win005_Choice dlg = new Win005_Choice(choiceLiteral.Parameters.Select(x => x.Value).ToList());
                        dlg.ShowDialog();
                        enviroment.Set(choiceLiteral.VaName, new IntegerObject(dlg.ChoiceNumber));
                    }
                    return null;
                case DialogSelectLiteral dialogSelectLiteral:
                    if (dialogSelectLiteral.Token.Type == TokenType.SELECT)
                    {
                        MessageBoxResult result = MessageBox.Show(dialogSelectLiteral.Parameters[1].Value.Replace("@@", System.Environment.NewLine), "スキップ時はNoを選択", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            enviroment.Set(dialogSelectLiteral.Parameters[0].Value, new IntegerObject(1));
                        }
                    }
                    return null;
                case EventLiteral eventLiteral:
                    if (this.window == null)
                    {
                        return null;
                    }
                    {
                        var ev = this.ClassGameStatus.ListEvent
                            .Where(x => x.Name == eventLiteral.Parameters[0].Value)
                            .FirstOrDefault();
                        if (ev == null)
                        {
                            return null;
                        }

                        var evaluator = new Evaluator();
                        evaluator.ClassGameStatus = this.ClassGameStatus;
                        evaluator.window = this.window;
                        evaluator.Eval(ev.Root, enviroment);
                        ev.Yet = false;
                    }

                    return null;

            }
            return null;
        }

        public IObject EvalIdentifier(Identifier identifier, Enviroment enviroment)
        {
            var (value, ok) = enviroment.Get(identifier.Value);
            if (value == null) return this.Null;
            if (ok) return value;
            return this.Null;
        }

        public IObject EvalBlockStatement(BlockStatement blockStatement, Enviroment enviroment)
        {
            IObject? result = null;

            if (blockStatement.Statements == null)
            {
                return this.Null;
            }

            foreach (var statement in blockStatement.Statements)
            {
                result = this.Eval(statement, enviroment);
                if (result == null)
                {
                    continue;
                    //return this.Null;
                }
                if (result.Type() == ObjectType.RETURN_VALUE) return result;
            }
            if (result == null)
            {
                return this.Null;
            }
            return result;
        }
        public IObject EvalIfExpression(IfExpression ifExpression, Enviroment enviroment)
        {
            var condition = this.Eval(ifExpression.Condition, enviroment);

            if (condition == null)
            {
                return this.Null;
            }

            if (this.IsTruthly(condition))
            {
                if (ifExpression.Consequence == null)
                {
                    return this.Null;
                }
                var a = this.EvalBlockStatement(ifExpression.Consequence, enviroment);
                if (a == null) return this.Null;
                return a;
            }
            else if (ifExpression.Alternative != null)
            {
                if (ifExpression.Alternative == null)
                {
                    return this.Null;
                }
                var a = this.EvalBlockStatement(ifExpression.Alternative, enviroment);
                if (a == null) return this.Null;
                return a;
            }
            return this.Null;
        }

        public bool IsTruthly(IObject obj)
        {
            if (obj == this.True) return true;
            if (obj == this.False) return false;
            if (obj == this.Null) return false;
            return true;
        }

        public IObject EvalInfixExpression(string op, IObject left, IObject right)
        {
            if (left == null || right == null)
            {
                return this.Null;
            }

            if (left is IntegerObject leftIntegerObject
                && right is IntegerObject rightIntegerObject)
            {
                return this.EvalIntegerInfixExpression(op, leftIntegerObject, rightIntegerObject);
            }

            switch (op)
            {
                case "||":
                    if (left is BooleanObject leftBooleanObject && right is BooleanObject rightBooleanObject)
                    {
                        if (leftBooleanObject.Value == true || rightBooleanObject.Value == true)
                        {
                            return ToBooleanObject(true);
                        }
                    }
                    return ToBooleanObject(false);
                case "&&":
                case "==":
                    return ToBooleanObject(left == right);
                case "!=":
                    return ToBooleanObject(left != right);
            }

            return this.Null;
        }

        public IObject EvalIntegerInfixExpression(string op, IntegerObject left, IntegerObject right)
        {
            var leftValue = left.Value;
            var rightValue = right.Value;

            switch (op)
            {
                case "+":
                    return new IntegerObject(leftValue + rightValue);
                case "-":
                    return new IntegerObject(leftValue - rightValue);
                case "*":
                    return new IntegerObject(leftValue * rightValue);
                case "/":
                    return new IntegerObject(leftValue / rightValue);
                case "<":
                    return this.ToBooleanObject(leftValue < rightValue);
                case ">":
                    return this.ToBooleanObject(leftValue > rightValue);
                case "==":
                    return this.ToBooleanObject(leftValue == rightValue);
                case "!=":
                    return this.ToBooleanObject(leftValue != rightValue);
            }
            return this.Null;
        }

        public BooleanObject ToBooleanObject(bool value) => value ? this.True : this.False;

        public IObject? EvalRootProgram(List<IStatement> statements, Enviroment enviroment)
        {
            IObject? result = null;
            foreach (var statement in statements)
            {
                result = this.Eval(statement, enviroment);
                if (result is ReturnValue returnValue)
                {
                    return returnValue.Value;
                }
            }
            return result;
        }

        public IObject EvalPrefixExpression(string op, IObject right)
        {
            switch (op)
            {
                case "!":
                    return this.EvalBangOperator(right);
                case "-":
                    return this.EvalMinusPrefixOperatorExpression(right);
            }
            return this.Null;
        }

        public IObject EvalBangOperator(IObject right)
        {
            if (right == this.True) return this.False;
            if (right == this.False) return this.True;
            if (right == this.Null) return this.True;
            return this.False;
        }
        public IObject EvalMinusPrefixOperatorExpression(IObject right)
        {
            if (right.Type() != ObjectType.INTEGER) return this.Null;
            var conv = (right as IntegerObject);
            if (conv == null)
            {
                return this.Null;
            }
            var value = conv.Value;
            return new IntegerObject(-value);
        }


    }
}
