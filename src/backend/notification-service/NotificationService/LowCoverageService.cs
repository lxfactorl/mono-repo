namespace NotificationService;

public class LowCoverageService
{
    public string uncoveredMethod()
    {
        return "This method is not covered by tests and should trigger a coverage failure.";
    }

    public int AnotherUncoveredMethod()
    {
        return 42;
    }
}
