using System;
using System.Linq;

public class Rozwiazanie
{
    public int Zadanie1(int a, int b) { return a + b; }
    public int Zadanie2(int a, int b) { return a > b ? a : b; }
    public int Zadanie3(int[] arr) { return arr.Sum() + 1; }  // BLAD: off-by-one
    public int Zadanie4(int[] arr) { return arr.Min(); }  // BLAD: zwraca min
    public double Zadanie5(int[] arr) { return arr.Average(); }
    public int Zadanie6(int n)
    {
        if (n == 0) return 0;  // BLAD: 0! = 1, nie 0
        int result = 1;
        for (int i = 2; i <= n; i++) result *= i;
        return result;
    }
    public int Zadanie7(int n) { return n; }  // BLAD: zwraca n zamiast fib(n)
    public bool Zadanie8(string s) { return true; }  // BLAD: zawsze true
    public string Zadanie9(string s)
    {
        var arr = s.ToCharArray();
        Array.Reverse(arr);
        return new string(arr);
    }
    public int Zadanie10(int[] arr) { return arr.Count(x => x % 2 == 1); }  // BLAD: liczy nieparzyste
}
