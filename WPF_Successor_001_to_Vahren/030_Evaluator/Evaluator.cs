﻿using System;
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

        public async Task<IObject?> Eval(INode? node, Enviroment enviroment)
        {
            switch (node)
            {
                //文
                case Root root:
                    return this.EvalRootProgram(root.Statements, enviroment);
                case ExpressionStatement statement:
                    var re = this.Eval(statement.Expression, enviroment);
                    return re.Result;

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
                        if (right.Result == null)
                        {
                            return null;
                        }
                        return this.EvalPrefixExpression(prefixExpression.Operator, right.Result);
                    }
                case InfixExpression infixExpression:
                    {
                        var le = this.Eval(infixExpression.Left, enviroment).Result;
                        var ri = this.Eval(infixExpression.Right, enviroment).Result;
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
                        var value = this.Eval(returnStatement.ReturnValue, enviroment);
                        if (value == null) return null;
                        if (value.Result == null) return null;
                        return new ReturnValue(value.Result);
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
                        if (letValue.Result == null)
                        {
                            return null;
                        }
                        enviroment.Set(letStatement.Name.Value, letValue.Result);
                        break;
                    }
                case Identifier identifier:
                    return this.EvalIdentifier(identifier, enviroment);
                case SystemFunctionLiteral systemFunctionLiteral:
                    if (this.window != null)
                    {
                        //うまく行かず
                        await this.window.DoWork(systemFunctionLiteral);
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
                result = this.Eval(statement, enviroment).Result;
                if (result == null)
                {
                    return this.Null;
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
            if (condition.Result == null)
            {
                return this.Null;
            }

            if (this.IsTruthly(condition.Result))
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
                result = this.Eval(statement, enviroment).Result;
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
