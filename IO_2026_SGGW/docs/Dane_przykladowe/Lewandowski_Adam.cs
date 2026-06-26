using System;
using System.Linq;

public class Rozwiazanie
{
    public int Zadanie1(int a, int b) { return a + b; }
    public int Zadanie2(int a, int b) { return a > b ? a : b; }
    public int Zadanie3(int[] arr) { return arr.Sum(); }
    public int Zadanie4(int[] arr)
    {
        // TIMEOUT: nieskonczona petla
        while (true) { }
    }
    public double Zadanie5(int[] arr) { return arr.Average(); }
    public int Zadanie6(int n)
    {
        // WYJATEK: dzielenie przez zero
        return n / 0;
    }
    public int Zadanie7(int n)
    {
        if (n <= 1) return n;
        int a = 0, b = 1;
        for (int i = 2; i <= n; i++) { int c = a + b; a = b; b = c; }
        return b;
    }
    public bool Zadanie8(string s)
    {
        // WYJATEK: NullReferenceException
        string x = null;
        return x.Equals(s);
    }
    public string Zadanie9(string s)
    {
        var arr = s.ToCharArray();
        Array.Reverse(arr);
        return new string(arr);
    }
    public int Zadanie10(int[] arr) { return arr.Count(x => x % 2 == 0); }
}
