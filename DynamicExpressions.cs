using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dynamic_If
{
    internal delegate object PropertySelector<T>(T data, string str);

    internal class DynamicExpressions<T>
    {
        private  PropertySelector<T> _propSelector;

        public DynamicExpressions(PropertySelector<T> ps) { _propSelector = ps; }

        public List<T> Evaluate(List<T> data, string filterExpression)
        {
            return data.Where(data => EvaluateExpression(data, filterExpression)).ToList();
        }

        private bool EvaluateExpression(T data, string expression)
        {
            expression = expression.Replace("\"", "");

            while (expression.Contains('('))
            {
                int openIndex = expression.LastIndexOf('(');
                int closeIndex = expression.IndexOf(')', openIndex);
                if (closeIndex == -1)
                    throw new ArgumentException("Mismatched parentheses");

                string subExpression = expression.Substring(openIndex + 1, closeIndex - openIndex - 1);

                bool result = ExprsByOr(data, subExpression);

                expression = expression.Substring(0, openIndex) + result.ToString() + expression.Substring(closeIndex + 1);
            }

            return ExprsByOr(data, expression);
        }

        private bool ExprsByOr(T data, string expression)
        {
            var exprs = expression.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var expr in exprs)
            {
                var trimmed = expr.Trim();
                if (ExprsByAnd(data, trimmed))
                    return true;
            }

            return false;
        }

        private bool ExprsByAnd(T data, string expression)
        {
            var exprs = expression.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var expr in exprs)
            {
                var trimmed = expr.Trim();

                string pattern = @"([\w]+)\s*(==|!=|>|>=|<|<=)\s*([-\w,]+)";
                var match = Regex.Match(trimmed, pattern, RegexOptions.IgnoreCase);

                bool res = false;
                if (!match.Success)
                {
                    pattern = @"(\w+)";
                    match = Regex.Match(trimmed, pattern, RegexOptions.IgnoreCase);
                    if (!match.Success)
                        throw new ArgumentException("Invalid expression: " + trimmed);

                    if (bool.TryParse(match.Groups[0].Value, out res))
                    {
                        if (match.Groups.Count == 2 && res == false)
                            return res;
                        else
                            continue;
                    }

                }

                var property = match.Groups[1].Value;
                var op = match.Groups[2].Value;
                var value = match.Groups[3].Value;

                object propValue = _propSelector(data, property);
                //switch (property)
                //{
                //    case "a": propValue = data.a; break;
                //    case "b": propValue = data.b; break;
                //    case "c": propValue = data.c; break;
                //    case "d": propValue = data.d; break;
                //    case "e": propValue = data.e; break;
                //    case "true": propValue = true; break;
                //    case "false": propValue = false; break;
                //    default: throw new ArgumentException("Invalid property: " + property);
                //}

                if (!EvaluateComparison(propValue, op, value))
                    return false;
            }

            return true;
        }

        private bool EvaluateComparison(object propValue, string op, string value)
        {
            switch (propValue)
            {
                case int intVal:
                    int intComp = int.Parse(value);
                    return EvaluateValue<int>(intVal, op, intComp);
                case byte byteVal:
                    byte byteComp = byte.Parse(value);
                    return EvaluateValue<byte>(byteVal, op, byteComp);
                case float floatVal:
                    float floatComp = float.Parse(value);
                    return EvaluateValue<float>(floatVal, op, floatComp);
                case UInt16 uintVal:
                    UInt16 uintComp = UInt16.Parse(value);
                    return EvaluateValue<UInt16>(uintVal, op, uintComp);
                case string strVal:
                    return EvaluateValue<string>(strVal, op, value);
                case bool boolVal:
                    bool boolComp = bool.Parse(value);
                    return EvaluateValue<bool>(boolVal, op, boolComp);
                default:
                    throw new ArgumentException("Unsupported property type");
            }
        }

        private bool EvaluateValue<T1>(T1 propValue, string op, T1 value) where T1 : IComparable<T1>
        {
            if (typeof(T1) == typeof(string) || typeof(T1) == typeof(bool))
            {
                switch (op)
                {
                    case "==": return propValue.CompareTo(value) == 0;
                    case "!=": return propValue.CompareTo(value) != 0;
                    default: throw new ArgumentException("Invalid operator: " + op);
                }
            }

            switch (op)
            {
                case "==": return propValue.CompareTo(value) == 0;
                case "!=": return propValue.CompareTo(value) != 0;
                case ">": return propValue.CompareTo(value) > 0;
                case ">=": return propValue.CompareTo(value) >= 0;
                case "<": return propValue.CompareTo(value) < 0;
                case "<=": return propValue.CompareTo(value) <= 0;
                default: throw new ArgumentException("Invalid operator: " + op);
            }
        }
    }
}
