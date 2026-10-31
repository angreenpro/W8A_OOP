/**
Nguyen Phuc Truong An
202419022
 */

public class MaintenanceService
{
    private readonly List<string> requests = new();

    public IReadOnlyList<string> Requests => requests.AsReadOnly();

    public void HandleCriticalAlert(object? sender, DeviceAlertEventArgs e)
    {
        if (e.Severity != AlertSeverity.Critical)
        {
            return;
        }

        requests.Add($"Maintenance request for {e.DeviceId}: {e.Message}");
    }
}
