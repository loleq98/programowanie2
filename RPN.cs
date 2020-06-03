using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WEBAPI
{
    class RPN
    {        
        private string[] formula = null;
        private string[] formularpn = null;
        private string input,messege;
        private double xInput=0, xMin=0, xMax=0;
        private int nPoints=2;

        public RPN(string input)
        {
            this.input=input;
            CheckInput();            
        }
        
        public RPN (string input, double x)
        {
            this.input=input;
            CheckInput();          
            xInput = x;
        }

        public RPN(string input, double xMin, double xMax, int nPoints)
        {            
            this.input=input;
            CheckInput();            
            this.xMin = xMin;
            this.xMax = xMax;
            this.nPoints = nPoints;
        }

        public List<string> GetInfixTokenList() 
        {
            List<string> vs = new List<string>();
            foreach (var item in formula)                          
                vs.Add(item.Replace(',','.'));           
            return vs;
        }
        public List<string> GetRPNTokenList() 
        {
            List<string> vs = new List<string>();
            foreach (var item in formularpn)                          
                vs.Add(item.Replace(',','.'));           
            return vs;
        }         

        public double CalculateRPNformula()
        {                    
            if (!formularpn.Contains("x"))
            {
                if (!string.IsNullOrEmpty(formula.ToString()))
                {
                    Stack<string> stack = new Stack<string>();
                    double firstNumber, secondNumber;
                    double isDouble;
                    foreach (string item in formularpn)
                    {                        
                        if (double.TryParse(item, out isDouble))                                                   
                            stack.Push(item);                        
                        else
                        {
                            firstNumber = double.Parse(stack.Pop());
                            if (char.TryParse(item, out char isChar))
                            {                                
                                secondNumber = double.Parse(stack.Pop());
                                stack.Push(GetOperationResult(item, firstNumber, secondNumber));
                            }
                            else                                   
                                stack.Push(GetTrigonometricFunction(item, firstNumber));                            
                        }
                    }                    
                    return double.Parse(stack.Pop().Replace(',','.'));
                }
            }
            return 0;
        }

        public string CalculateValueOfGivenX()
        {            
            Stack<string> stack = new Stack<string>();
            double firstNumber, secondNumber;           
            string xValue = xInput.ToString();            
            double isDouble;

            foreach (string item in formularpn)
            {                
                if (double.TryParse(item, out isDouble) || item=="x")
                {                    
                    if (item=="x")                    
                        stack.Push(xValue);                    
                    else
                        stack.Push(item);                    
                }
                else
                {                    
                    firstNumber = double.Parse(stack.Pop());                    
                    if (char.TryParse(item, out char isChar))
                    {
                        secondNumber = double.Parse(stack.Pop());                        
                        stack.Push(GetOperationResult(item, firstNumber, secondNumber));
                    }
                    else
                        stack.Push(GetTrigonometricFunction(item, firstNumber));
                }                
            }
            
            return stack.Pop().Replace(',', '.');
        }

        public object [] CalculateValueOfGivenInterval()
        {   
            if (nPoints <= 1)
            {
                Console.WriteLine("Zla metoda");
                return null;
            }              

            double[] interval = new double[nPoints];            
            object[] xyTable = new object[nPoints];
            Stack<string> stack = new Stack<string>();
            double firstNumber, secondNumber, pointsDifference, result, isDouble;            

            pointsDifference = xMax - xMin;
            result = pointsDifference / (nPoints - 1);
            int i = 1;
            interval[0] = xMin;

            while (true)
            {
                interval[i] = interval[i - 1] + result;
                i++;
                if (i >= nPoints)
                    break;
            }            

            for (int w = 0; w < interval.Length; w++)
            {
                foreach (string item in formularpn)
                {
                    if (double.TryParse(item, out isDouble) || item == "x")
                    {
                        if (item == "x")
                            stack.Push(interval[w].ToString());
                        else
                            stack.Push(item);
                    }
                    else
                    {
                        firstNumber = double.Parse(stack.Pop());
                        if (char.TryParse(item, out char isChar))
                        {
                            secondNumber = double.Parse(stack.Pop());
                            stack.Push(GetOperationResult(item, firstNumber, secondNumber));
                        }
                        else
                            stack.Push(GetTrigonometricFunction(item, firstNumber));
                    }
                }

                xyTable[w] = new {                
                    x = interval[w].ToString().Replace(',','.') ,
                    y = stack.Peek().Replace(',','.')
                };
            }
            return xyTable;
        }             

        public bool CheckInput()
        {
            string[] operators = { "+", "-", "*", "/", "^" };
            int close = 0, open = 0;

            if (string.IsNullOrEmpty(input))
            {
                this.messege= "Wzor jest pusty";
                return false;
            }
                
            if(nPoints<=1)
                {
                    this.messege="Liczba okresow musi byc wieksza niz jeden";
                    return false;
                }
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                    open++;
                if (input[i] == ')')
                    close++;

                if (input[i] == '/')
                {
                    int j = i;
                    if (input[++j] == '0')
                        {
                            this.messege="Nie dziel przez zero";
                            return false;
                        }
                }
                else if (operators.Contains(input[i].ToString()))
                {
                    int j = i;
                    if (operators.Contains(input[++j].ToString()))
                        {
                            this.messege="zly wzor";
                            return false;
                        }
                }
            }
            if (close != open)
                {
                    this.messege="Liczba nawiasow sie nie zgadza";
                    return false;
                }

            ReturnArray(input);
            return true;
        }

        public string Message()
        {
            return messege;
        }
        private void ReturnArray(string inputFormula)
        {
            string[] tmp = new string[inputFormula.Length];
            formula = new string[inputFormula.Length];

            string nextString = null;
            int formulaLenght = 0;
            for (int i = 0; i < inputFormula.Length; i++)
            {
                if (inputFormula[i] == '-')
                {
                    int checkMinus = i;

                    if (i == 0)
                        nextString += inputFormula[i++];
                    else if (inputFormula[--checkMinus] == '(')
                        nextString += inputFormula[i++];
                }
                if (char.IsLetter(inputFormula[i]) && inputFormula[i] != 'x')
                {
                    while (char.IsLetter(inputFormula[i]))
                    {
                        nextString += inputFormula[i];
                        if (char.IsLetter(inputFormula[i + 1]) && inputFormula[i + 1] != 'x' || inputFormula[i + 2] == 'p')
                            i++;
                        else break;
                    }
                }
                else if (int.TryParse(inputFormula[i].ToString(), out int r1) || inputFormula[i] == '.')
                {
                    while (int.TryParse(inputFormula[i].ToString(), out int r2) || inputFormula[i] == '.')
                    {

                        if (inputFormula[i] == '.')
                            nextString += ",";
                        else
                            nextString += inputFormula[i];

                        if ((i + 1) == inputFormula.Length)
                            break;
                        else if (inputFormula[i + 1] == '.' || int.TryParse(inputFormula[i + 1].ToString(), out int r3))
                            i++;
                        else break;
                    }
                }
                else
                    nextString = inputFormula[i].ToString();

                tmp[formulaLenght++] = nextString;
                nextString = null;
            }

            this.formula = new string[formulaLenght];
            for (int q = 0; q < tmp.Length; q++)
            {
                if (string.IsNullOrEmpty(tmp[q]))
                    break;
                this.formula[q] = tmp[q];
            }
            ConvertInfixToRPN();
        }

        private void ConvertInfixToRPN()
        {
            Stack<string> stack = new Stack<string>();
            Queue<string> queue = new Queue<string>();
            double checkParse;

            for (int i = 0; i < formula.Length; i++)
            {
                string token = formula[i];

                if (double.TryParse(token, out checkParse) || token == "x")
                    queue.Enqueue(token);

                if (token == "(")
                    stack.Push(token);

                if (token == ")")
                {
                    while (stack.Peek() != "(")
                        queue.Enqueue(stack.Pop());

                    stack.Pop();
                }

                if (!double.TryParse(token, out checkParse) && token != "(" && token != ")" && token != "x")
                {
                    switch (token)
                    {
                        case "+":
                        case "-":
                            if (stack.Count != 0)
                            {
                                if (stack.Peek() == "(")
                                    stack.Push(token);
                                else
                                {
                                    queue.Enqueue(stack.Pop());
                                    stack.Push(token);
                                }
                            }
                            else stack.Push(token);
                            break;
                        case "*":
                        case "/":
                            if (stack.Count != 0)
                            {
                                if (stack.Peek() == "+" || stack.Peek() == "-" || stack.Peek() == "(")
                                    stack.Push(token);
                                else
                                {
                                    queue.Enqueue(stack.Pop());
                                    stack.Push(token);
                                }
                            }
                            else stack.Push(token);
                            break;
                        case "^":
                            if (stack.Count != 0)
                            {
                                if (stack.Peek() == "+" || stack.Peek() == "-" || stack.Peek() == "*" || stack.Peek() == "/" || stack.Peek() == "(")
                                    stack.Push(token);
                                else
                                {
                                    queue.Enqueue(stack.Pop());
                                    stack.Push(token);
                                }
                            }
                            else stack.Push(token);
                            break;
                        default:
                            stack.Push(token);
                            break;
                    }
                }
            }

            while (stack.Count != 0)
                queue.Enqueue(stack.Pop());

            this.formularpn = new string[queue.Count];
            int p = 0;
            while (queue.Count != 0)
                this.formularpn[p++] = queue.Dequeue();
        }

        private string GetTrigonometricFunction(string item, double number)
        {         
            string[] toRemove = { "-" };
            if (item.Contains('-'))
            {
                foreach (var c in toRemove)
                {
                    item = item.Replace(c, string.Empty);
                }
                number *= (-1);
            }
         
            double result = 0;
            switch (item)
            {                
                case "abs":
                    result = Math.Abs(number);
                    break;
                case "cos":
                    result = Math.Cos(number);
                    break;
                case "exp":
                    result = Math.Exp(number);
                    break;
                case "log":
                    result = Math.Log(number);
                    break;                
                case "sin":
                    result = Math.Sin(number);
                    break;
                case "sqrt":
                    result = Math.Sqrt(number);
                    break;
                case "tan":
                    result = Math.Tan(number);
                    break;
                case "cosh":
                    result = Math.Cosh(number);
                    break;
                case "sinh":
                    result = Math.Sinh(number);
                    break;
                case "tanh":
                    result = Math.Tanh(number);
                    break;
                case "acos":
                    result = Math.Acos(number);
                    break;
                case "asin":
                    result = Math.Asin(number);
                    break;
                case "atan":
                    result = Math.Atan(number);
                    break;
            }
            
            return result.ToString();
        }

        private string GetOperationResult(string item, double firstNumber, double secondNumber)
        {
            double result = 0;
            switch (item)
            {
                case "+":
                    result = secondNumber + firstNumber;
                    break;
                case "-":
                    result = secondNumber - firstNumber;
                    break;
                case "*":
                    result = secondNumber * firstNumber;
                    break;
                case "/":
                    if (firstNumber == 0)
                        throw new System.InvalidOperationException("Nie dziel przez zero");
                    result = secondNumber / firstNumber;                   
                    break;
                case "^":
                    result = Math.Pow(secondNumber, firstNumber);
                    break;
            }
            return result.ToString();
        }
    }
}
