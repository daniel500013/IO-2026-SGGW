using System;
using System.Reflection;
using IO_2026_SGGW.Core;
using Xunit;

public class SolutionEngineTests
{
    private readonly SolutionCompiler compiler = new SolutionCompiler();
    private readonly SolutionRunner runner = new SolutionRunner();

    private Assembly Compile(string src)
    {
        var r = compiler.Compile(src);
        Assert.True(r.Success, "Kod miał się skompilować: " + r.ErrorMessage);
        return r.Assembly;
    }

    // ---------- SolutionCompiler ----------
    [Fact] // [OK]
    public void Compile_PoprawnyKod_Sukces()
    {
        var r = compiler.Compile("public class X { public int F() => 42; }");
        Assert.True(r.Success);
        Assert.NotNull(r.Assembly);
    }

    [Fact] // [OK]
    public void Compile_BlednaSkladnia_Porazka()
    {
        var r = compiler.Compile("public class X { broken }");
        Assert.False(r.Success);
    }

    [Fact] // [OK]
    public void Compile_Linq_Dziala()
    {
        var r = compiler.Compile("using System.Linq; public class X { public int S(int[] a) => a.Sum(); }");
        Assert.True(r.Success);
    }

    [Fact] // [REGRESJA T3-16/T3-17] - bezpiecznik ma odrzucic grozne API
    public void Compile_GrozneApi_Odrzucone()
    {
        // Uwaga: Kod kompiluje się na obecnym silniku (Environment jest dostępny) -> test będzie CZERWONY do czasu wdrożenia Zad.3.
        var r = compiler.Compile("public class X { public int F(){ System.Environment.Exit(0); return 0; } }");
        Assert.False(r.Success);
    }

    // ---------- FindMethod ----------
    [Fact] // [OK]
    public void FindMethod_NormalizujeNazwe()
    {
        var asm = Compile("public class X { public int Zadanie1a(int a,int b)=>a+b; }");
        var m = runner.FindMethod(asm, "Zadanie 1a"); // spacja w nazwie zadania
        Assert.NotNull(m);
        Assert.Equal("Zadanie1a", m.Name);
    }

    [Fact] // [OK]
    public void FindMethod_BrakMetody_Null()
    {
        var asm = Compile("public class X { public int Inna()=>1; }");
        Assert.Null(runner.FindMethod(asm, "ZadanieX"));
    }

    // ---------- ParseArgs ----------
    [Fact] // [OK]
    public void ParseArgs_Skalary()
    {
        var m = Compile("public class X { public int F(int a,int b)=>0; }").GetType("X").GetMethod("F");
        var args = runner.ParseArgs("5, 10", m.GetParameters());
        Assert.Equal(new object[] { 5, 10 }, args);
    }

    [Fact] // [OK]
    public void ParseArgs_TablicaISkalar()
    {
        var m = Compile("public class X { public int F(int[] a,int b)=>0; }").GetType("X").GetMethod("F");
        var args = runner.ParseArgs("[1,2,3], 5", m.GetParameters());
        Assert.Equal(new int[] { 1, 2, 3 }, (int[])args[0]);
        Assert.Equal(5, args[1]);
    }

    // ---------- IsCorrect ----------
    [Fact] // [OK]
    public void IsCorrect_DoubleTolerancja()
    {
        Assert.True(runner.IsCorrect(1.0000001, "1.0", typeof(double)));
    }

    [Fact] // [OK]
    public void IsCorrect_Tablica()
    {
        Assert.True(runner.IsCorrect(new int[] { 1, 2, 3 }, "[1,2,3]", typeof(int[])));
    }

    [Fact] // [REGRESJA T3-15] - bool podany jako "1"
    public void IsCorrect_BoolJako1()
    {
        // Uwaga: Obecnie porównanie tekstowe -> test będzie CZERWONY do czasu poprawek w Etapie 4
        Assert.True(runner.IsCorrect(true, "1", typeof(bool)));
    }

    // ---------- InvokeWithTimeout ----------
    [Fact] // [OK]
    public void Invoke_NieskonczonaPetla_Timeout()
    {
        var asm = Compile("public class X { public int F(){ while(true){} } }");
        var m = runner.FindMethod(asm, "F");
        var res = runner.InvokeWithTimeout(m, new object[0], 200);
        Assert.Equal(RunStatus.Timeout, res.Status);
    }

    [Fact] // [OK]
    public void Invoke_RzucaWyjatek_StatusWyjatek()
    {
        var asm = Compile("public class X { public int F(){ throw new System.Exception(\"boom\"); } }");
        var m = runner.FindMethod(asm, "F");
        var res = runner.InvokeWithTimeout(m, new object[0], 1000);
        Assert.Equal(RunStatus.Wyjatek, res.Status);
    }
}