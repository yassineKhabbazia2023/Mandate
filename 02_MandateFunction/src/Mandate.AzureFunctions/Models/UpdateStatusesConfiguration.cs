namespace Mandate.AzureFunctions
{
    public class UpdateStatusesConfiguration
    {
        public const string SectionName = "UpdateStatuses";
        public required List<int> DailyStatusCodes { get; set; } = [40];
        public required List<int> HourlyStatusCodes { get; set; } = [10, 20, 30, 99];
    }
}
