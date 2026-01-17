namespace NotificationService.Application;

public class UncoveredService
{
    public void DoSomethingComplex(int x)
    {
        if (x > 0)
        {
            System.Console.WriteLine("Positive");
        }
        else
        {
            System.Console.WriteLine("Negative");
        }
    }
}
