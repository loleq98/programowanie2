using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class mi
{

    static bool spr(char q)
    {
        return (!(q >= 'a' && q <= 'z') && !(q >= '0' && q <= '9') && !(q >= 'A' && q <= 'Z'));
    }

  
    static int poi(char q)
    {
        if (q == '-' || q == '+') return 1;
        else if (q == '*' || q == '/') return 2;
        else if (q == '^') return 3;
        return 0;
    }
  
    public static void Main()
    {
        String a = String.Empty;
        String wynik=String.Empty;
        //Console.Write("prosze podać notacje zwykłą= ");
        a = "(A+B^C)*D+E^5";

        Stack<char> stos = new Stack<char>();
        Stack<String> WYNIK = new Stack<String>();

        for (int i = 0; i < a.Length; i++)
        {


            if (a[i] == '(')
            {
                stos.Push(a[i]);
            }

            else if (a[i] == ')')
            {
                while (stos.Count != 0 &&
                    stos.Peek() != '(')
                {
                    String op1 = WYNIK.Pop();
                    String op2 = WYNIK.Pop();
                    char op = stos.Pop();
                    String buff = op + op2 + op1;
                    
                    WYNIK.Push(buff);
                }
                stos.Pop();
            }

            else if (!spr(a[i]))
            {
                WYNIK.Push(a[i] + "");
            }

            else
            {
                while (stos.Count != 0 &&
                    poi(a[i]) <=
                        poi(stos.Peek()))
                {
                    String op1 = WYNIK.Pop();
                    String op2 = WYNIK.Pop();
                    char op = stos.Pop();
                    String buff = op + op2 + op1;
                   
                    WYNIK.Push(buff);
                }

                stos.Push(a[i]);
            }
        }


        while (stos.Count != 0)
        {
            String op1 = WYNIK.Pop();
            String op2 = WYNIK.Pop();
            char op = stos.Pop();
            String buff = op + op2 + op1;
            WYNIK.Push(buff);

        }
        
        foreach (var itm in WYNIK)
        {
            wynik += itm;

        }
       Console.WriteLine(wynik);
        Console.WriteLine("+*+A^BCD^E5");
      //  Console.ReadKey();
    }
}
