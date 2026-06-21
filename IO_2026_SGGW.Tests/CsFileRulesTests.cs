using System;
using System.IO;
using IO_2026_SGGW.Core;
using Xunit;
public class CsFileRulesTests : IDisposable
{
    private readonly string _dir;
    public CsFileRulesTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "t1_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
    }
    public void Dispose() { try { Directory.Delete(_dir, true); } catch { } }
    private string MakeFile(string name)
    {
        var p = Path.Combine(_dir, name);
        File.WriteAllText(p, "public class X {}");
        return p;
    }
    [Fact] // [OK]
    public void PlikCs_Akceptowany()
    {
        var f = MakeFile("Student.cs");
        Assert.True(CsFileRules.IsAcceptableCsFile(f, new string[0]));
    }
    [Fact] // [OK]
    public void PlikTxt_Odrzucony()
    {
        var f = MakeFile("notatka.txt");
        Assert.False(CsFileRules.IsAcceptableCsFile(f, new string[0]));
    }
    [Fact] // [OK]
    public void Duplikat_Odrzucony()
    {
        var f = MakeFile("Student.cs");
        Assert.False(CsFileRules.IsAcceptableCsFile(f, new[] { f }));
    }
    [Fact] // [OK] (weryfikuje T1-11) - folder z ".cs" w nazwie musi być odrzucony
    public void FolderZ_cs_wNazwie_Odrzucony()
    {
        var folder = Path.Combine(_dir, "Testy.CS");
        Directory.CreateDirectory(folder);
        Assert.False(CsFileRules.IsAcceptableCsFile(folder, new string[0]));
    }
}