using System.Diagnostics;

class TouchLab
{
    const int SIZE = 512 << 20;          // 512 MB
    const int PAGE = 4096;               // page granularity

    static void Report(string phase)
    {
        var p = Process.GetCurrentProcess();
        p.Refresh();
        Console.WriteLine($"{phase,-30} Private={p.PrivateMemorySize64 >> 20,5} MB   WorkingSet={p.WorkingSet64 >> 20,5} MB");
    }

    static void Main()
    {
        Console.WriteLine($"PID {Environment.ProcessId} — attach VMMap, press F5 there to refresh. Enter to start.");
        Console.ReadLine();
        Report("0 baseline");

        // Commit 512 MB without touching it.
        // AllocateUninitializedArray skips zeroing, so the runtime
        // doesn't fault the pages in on our behalf.
        byte[] a = GC.AllocateUninitializedArray<byte>(SIZE);
        Report("1 after allocate");
        Console.WriteLine("   -> VMMap: Committed +512 MB, Working Set flat. Enter to touch...");
        Console.ReadLine();

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < SIZE; i += PAGE) a[i] = 1;   // ONE byte per page
        sw.Stop();
        Report($"2 first touch  ({sw.ElapsedMilliseconds} ms)");
        Console.WriteLine($"   -> {SIZE / PAGE:N0} soft page faults just happened. Enter for warm pass...");
        Console.ReadLine();

        sw.Restart();
        for (int i = 0; i < SIZE; i += PAGE) a[i] = 2;   // same loop, warm pages
        sw.Stop();
        Report($"3 second touch ({sw.ElapsedMilliseconds} ms)");

        GC.KeepAlive(a);
        Console.WriteLine("Done — leave it running while you inspect VMMap. Enter to exit.");
        Console.ReadLine();
    }
}
