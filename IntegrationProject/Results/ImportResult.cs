using IntegrationProject.Enums;

namespace IntegrationProject.Results
{
    internal class ImportResult
    {
        internal string StepName { get; set; }
        internal string Message { get; set; }
        internal ImportStatus Status { get; set; }
    }
}
