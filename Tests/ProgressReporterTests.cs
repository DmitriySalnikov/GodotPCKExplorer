using GodotPCKExplorer;
using System.Globalization;

namespace PCKBruteforcer
{
    public sealed class ProgressReporterTests(string testName) : BasicPCKProgressReporter, IPCKProgressReporter
    {
        readonly string test_name = testName;

        public override void Log(string txt)
        {
            var isFirst = true;

            if (PrintLogText != null)
            {
                PrintLogText?.Invoke(txt);
            }
            else
            {
                txt = string.Join("\n",
                    txt.Split('\n').
                    Select((t) =>
                    {
                        var res = $"[{test_name} {DateTime.Now.TimeOfDay.ToString(null, CultureInfo.InvariantCulture)}]{(isFirst ? "\t" : "-\t")}{t}";
                        isFirst = false;
                        return res;
                    }));
                Console.WriteLine(txt);
            }
        }
    }
}
