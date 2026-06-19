using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using IO_2026_SGGW.Core;
using Xunit;
public class GradingServiceTests
{
    // Wymusza synchroniczne ui.Post(...) - bez tego BindingList aktualizuje się na puli wątków.
    private sealed class InlineSyncContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object state) => d(state);
        public override void Send(SendOrPostCallback d, object state) => d(state);
    }
    private static AnswerKey Key(string task, string param, string expected)
    {
        var k = new AnswerKey();
        var s = new TaskSheet { Name = task };
        s.TestCases.Add(new TestCase { ParametersRaw = param, ExpectedRaw = expected });
        k.Tasks.Add(s);
        return k;
    }
    private static StudentSolution Student(string id, string code) =>
    new StudentSolution { StudentId = id, SourceCode = code };
    private static async Task<BindingList<ResultRow>> Run(List<StudentSolution> students, AnswerKey
    key, int timeoutMs = 2000)
    {
        SynchronizationContext.SetSynchronizationContext(new InlineSyncContext());
        var results = new BindingList<ResultRow>();
        await new GradingService().RunAsync(students, key, timeoutMs, results, null);
        return results;
    }
    [Fact] // [OK] happy-path
    public async Task RunAsync_PoprawneRozwiazanie_StatusOk()
    {
        var students = new List<StudentSolution> { Student("Jan", "public class X { public int Zadanie1(int a,int b)=>a+b; }") };
        var results = await Run(students, Key("Zadanie1", "5, 10", "15"));
        Assert.Single(results);
        Assert.Equal(RunStatus.Ok, results[0].Status);
        Assert.Equal(1, results[0].Punkty);
    }
    [Fact] // [OK] blad kompilacji
    public async Task RunAsync_BladKompilacji_WszystkieWiersze()
    {
        var students = new List<StudentSolution> { Student("Jan", "public class X { broken }") };
        var results = await Run(students, Key("Zadanie1", "5, 10", "15"));
        Assert.All(results, r => Assert.Equal(RunStatus.BladKompilacji, r.Status));
    }
    [Fact] // [OK] brak metody
    public async Task RunAsync_BrakMetody()
    {
        var students = new List<StudentSolution> { Student("Jan", "public class X { public int Inna()=>1; }") };
        var results = await Run(students, Key("ZadanieX", "1", "1"));
        Assert.Equal(RunStatus.BrakMetody, results[0].Status);
    }
    [Fact] // [OK] progres do 100%
    public async Task RunAsync_Progres_DochodziDo100()
    {
        SynchronizationContext.SetSynchronizationContext(new InlineSyncContext());
        int last = -1;
        var progress = new Progress<int>(p => last = p); // tworzony PO ustawieniu kontekstu -> wywołania inline
        var results = new BindingList<ResultRow>();
        await new GradingService().RunAsync(
            new List<StudentSolution> { Student("Jan", "public class X { public int Z(int a)=>a; }") },
            Key("Z", "5", "5"), 2000, results, progress);
        Assert.Equal(100, last);
    }
    [Fact] // [OK] odpornosc - petla jednego nie blokuje drugiego
    public async Task RunAsync_PetlaNieskonczona_NieBlokujePozostalych()
    {
        var students = new List<StudentSolution>
        {
            Student("Petla", "public class X { public int Z(int a){ while(true){} } }"),
            Student("Dobry", "public class X { public int Z(int a)=>a; }")
        };
        var results = await Run(students, Key("Z", "5", "5"), 300);
        Assert.Contains(results, r => r.Student == "Petla" && r.Status == RunStatus.Timeout);
        Assert.Contains(results, r => r.Student == "Dobry" && r.Status == RunStatus.Ok);
    }
}
