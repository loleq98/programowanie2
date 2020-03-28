using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace MathText
{
    public enum TokenType
    {
        None,
        Number,
        Constant,
        Plus,
        Minus,
        Multiply,
        Divide,
        Exponent,
        UnaryMinus,
        Sine,
        Cosine,
        Tangent,
        LeftParenthesis,
        RightParenthesis
    }

    public struct ReversePolishNotationToken
    {
        public string TokenValue;
        public TokenType TokenValueType;
    }

    public class ReversePolishNotation
    {
        private Queue output;
        private Stack ops;

        private string sOriginalExpression;
        public string OriginalExpression
        {
            get { return sOriginalExpression; }
        }

        private string sTransitionExpression;
        public string TransitionExpression
        {
            get { return sTransitionExpression; }
        }

        private string sPostfixExpression;
        public string PostfixExpression
        {
            get { return sPostfixExpression; }
        }

        public ReversePolishNotation()
        {
            sOriginalExpression = string.Empty;
            sTransitionExpression = string.Empty;
            sPostfixExpression = string.Empty;
        }

        public void Parse(string Expression)
        {
            output = new Queue();
            ops = new Stack();

            sOriginalExpression = Expression;

            string sBuffer = Expression.ToLower();
            sBuffer = Regex.Replace(sBuffer, @"(?<number>\d+(\.\d+)?)", " ${number} ");
            sBuffer = Regex.Replace(sBuffer, @"(?<ops>[+\-*/^()])", " ${ops} ");
            sBuffer = Regex.Replace(sBuffer, "(?<alpha>(pi|e|sin|cos|tan))", " ${alpha} ");
            sBuffer = Regex.Replace(sBuffer, @"\s+", " ").Trim();

            sBuffer = Regex.Replace(sBuffer, "-", "MINUS");
            sBuffer = Regex.Replace(sBuffer, @"(?<number>(pi|e|(\d+(\.\d+)?)))\s+MINUS", "${number} -");
            sBuffer = Regex.Replace(sBuffer, "MINUS", "~");

            sTransitionExpression = sBuffer;

            string[] saParsed = sBuffer.Split(" ".ToCharArray());
            int i = 0;
            double tokenvalue;
            ReversePolishNotationToken token, opstoken;
            for (i = 0; i < saParsed.Length; ++i)
            {
                token = new ReversePolishNotationToken();
                token.TokenValue = saParsed[i];
                token.TokenValueType = TokenType.None;

                try
                {
                    tokenvalue = double.Parse(saParsed[i]);
                    token.TokenValueType = TokenType.Number;
                    output.Enqueue(token);
                }
                catch
                {
                    switch (saParsed[i])
                    {
                        case "+":
                            token.TokenValueType = TokenType.Plus;
                            if (ops.Count > 0)
                            {
                                opstoken = (ReversePolishNotationToken)ops.Peek();
                                while (IsOperatorToken(opstoken.TokenValueType))
                                {
                                    output.Enqueue(ops.Pop());
                                    if (ops.Count > 0)
                                    {
                                        opstoken = (ReversePolishNotationToken)ops.Peek();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            ops.Push(token);
                            break;
                        case "-":
                            token.TokenValueType = TokenType.Minus;
                            if (ops.Count > 0)
                            {
                                opstoken = (ReversePolishNotationToken)ops.Peek();
                                while (IsOperatorToken(opstoken.TokenValueType))
                                {
                                    output.Enqueue(ops.Pop());
                                    if (ops.Count > 0)
                                    {
                                        opstoken = (ReversePolishNotationToken)ops.Peek();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            ops.Push(token);
                            break;
                        case "*":
                            token.TokenValueType = TokenType.Multiply;
                            if (ops.Count > 0)
                            {
                                opstoken = (ReversePolishNotationToken)ops.Peek();
                                while (IsOperatorToken(opstoken.TokenValueType))
                                {
                                    if (opstoken.TokenValueType == TokenType.Plus || opstoken.TokenValueType == TokenType.Minus)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        output.Enqueue(ops.Pop());
                                        if (ops.Count > 0)
                                        {
                                            opstoken = (ReversePolishNotationToken)ops.Peek();
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            ops.Push(token);
                            break;
                        case "/":
                            token.TokenValueType = TokenType.Divide;
                            if (ops.Count > 0)
                            {
                                opstoken = (ReversePolishNotationToken)ops.Peek();
                                while (IsOperatorToken(opstoken.TokenValueType))
                                {
                                    if (opstoken.TokenValueType == TokenType.Plus || opstoken.TokenValueType == TokenType.Minus)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        output.Enqueue(ops.Pop());
                                        if (ops.Count > 0)
                                        {
                                            opstoken = (ReversePolishNotationToken)ops.Peek();
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            ops.Push(token);
                            break;
                        case "^":
                            token.TokenValueType = TokenType.Exponent;
                            ops.Push(token);
                            break;
                        case "~":
                            token.TokenValueType = TokenType.UnaryMinus;
                            ops.Push(token);
                            break;
                        case "(":
                            token.TokenValueType = TokenType.LeftParenthesis;
                            ops.Push(token);
                            break;
                        case ")":
                            token.TokenValueType = TokenType.RightParenthesis;
                            if (ops.Count > 0)
                            {
                                opstoken = (ReversePolishNotationToken)ops.Peek();
                                while (opstoken.TokenValueType != TokenType.LeftParenthesis)
                                {
                                    output.Enqueue(ops.Pop());
                                    if (ops.Count > 0)
                                    {
                                        opstoken = (ReversePolishNotationToken)ops.Peek();
                                    }
                                    else
                                    {
                                        throw new Exception("Unbalanced parenthesis!");
                                    }
                                    
                                }
                                ops.Pop();
                            }

                            if (ops.Count > 0)
                            {
                                opstoken = (ReversePolishNotationToken)ops.Peek();
                                if (IsFunctionToken(opstoken.TokenValueType))
                                {
                                    output.Enqueue(ops.Pop());
                                }
                            }
                            break;
                        case "pi":
                            token.TokenValueType = TokenType.Constant;
                            output.Enqueue(token);
                            break;
                        case "e":
                            token.TokenValueType = TokenType.Constant;
                            output.Enqueue(token);
                            break;
                        case "sin":
                            token.TokenValueType = TokenType.Sine;
                            ops.Push(token);
                            break;
                        case "cos":
                            token.TokenValueType = TokenType.Cosine;
                            ops.Push(token);
                            break;
                        case "tan":
                            token.TokenValueType = TokenType.Tangent;
                            ops.Push(token);
                            break;
                    }
                }
            }

            while (ops.Count != 0)
            {
                opstoken = (ReversePolishNotationToken)ops.Pop();
                if (opstoken.TokenValueType == TokenType.LeftParenthesis)
                {
                    throw new Exception("Unbalanced parenthesis!");
                }
                else
                {
                    output.Enqueue(opstoken);
                }
            }

            sPostfixExpression = string.Empty;
            foreach (object obj in output)
            {
                opstoken = (ReversePolishNotationToken)obj;
                sPostfixExpression += string.Format("{0} ", opstoken.TokenValue);
            }
        }

        public double Evaluate()
        {
            Stack result = new Stack();
            double oper1 = 0.0, oper2 = 0.0;
            ReversePolishNotationToken token = new ReversePolishNotationToken();
            foreach (object obj in output)
            {
                token = (ReversePolishNotationToken)obj;
                switch (token.TokenValueType)
                {
                    case TokenType.Number:
                        result.Push(double.Parse(token.TokenValue));
                        break;
                    case TokenType.Constant:
                        result.Push(EvaluateConstant(token.TokenValue));
                        break;
                    case TokenType.Plus:
                        if (result.Count >= 2)
                        {
                            oper2 = (double)result.Pop();
                            oper1 = (double)result.Pop();
                            result.Push(oper1 + oper2);
                        }
                        else
                        {
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Minus:
                        if (result.Count >= 2)
                        {
                            oper2 = (double)result.Pop();
                            oper1 = (double)result.Pop();
                            result.Push(oper1 - oper2);
                        }
                        else
                        {
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Multiply:
                        if (result.Count >= 2)
                        {
                            oper2 = (double)result.Pop();
                            oper1 = (double)result.Pop();
                            result.Push(oper1 * oper2);
                        }
                        else
                        {
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Divide:
                        if (result.Count >= 2)
                        {
                            oper2 = (double)result.Pop();
                            oper1 = (double)result.Pop();
                            result.Push(oper1 / oper2);
                        }
                        else
                        {
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Exponent:
                        if (result.Count >= 2)
                        {
                            oper2 = (double)result.Pop();
                            oper1 = (double)result.Pop();
                            result.Push(Math.Pow(oper1, oper2));
                        }
                        else
                        {
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.UnaryMinus:
                        if (result.Count >= 1)
                        {
                            oper1 = (double)result.Pop();
                            result.Push(-oper1);
                        }
                        else
                        {
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Sine:
                        if (result.Count >= 1)
                        {
                            oper1 = (double)result.Pop();
                            result.Push(Math.Sin(oper1));
                        }
                        else
                        {
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Cosine:
                        if (result.Count >= 1)
                        {
                            oper1 = (double)result.Pop();
                            result.Push(Math.Cos(oper1));
                        }
                        else
                        {
                            throw new Exception("Evaluation error!");
                        }
                        break;
                    case TokenType.Tangent:
                        if (result.Count >= 1)
                        {
                            oper1 = (double)result.Pop();
                            result.Push(Math.Tan(oper1));
                        }
                        else
                        {
                            throw new Exception("Evaluation error!");
                        }
                        break;
                }
            }

            if (result.Count == 1)
            {
                return (double)result.Pop();
            }
            else
            {
                throw new Exception("Evaluation error!");
            }
        }

        private bool IsOperatorToken(TokenType t)
        {
            bool result = false;
            switch (t)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Multiply:
                case TokenType.Divide:
                case TokenType.Exponent:
                case TokenType.UnaryMinus:
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }

        private bool IsFunctionToken(TokenType t)
        {
            bool result = false;
            switch (t)
            {
                case TokenType.Sine:
                case TokenType.Cosine:
                case TokenType.Tangent:
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }

        private double EvaluateConstant(string TokenValue)
        {
            double result = 0.0;
            switch (TokenValue)
            {
                case "pi":
                    result = Math.PI;
                    break;
                case "e":
                    result = Math.E;
                    break;
            }
            return result;
        }
    }
}