using System.Collections.Generic;

namespace IO_2026_SGGW.Core
{
    public class TaskSheet
    {
        public string Name { get; set; }
        public List<TestCase> TestCases { get; set; } = new List<TestCase>();
    }
}
