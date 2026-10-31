/**
Nguyen Phuc Truong An
202419022
 */

public class DeviceAuditLogger
{
    private readonly List<string> entries = new();

    public IReadOnlyList<string> Entries => entries.AsReadOnly();

    public void HandleStatusChanged(object? sender, DeviceStatusChangedEventArgs e)
    {
        entries.Add(
            $"{e.ChangedAt}: {e.DeviceId}, " +
            $"{e.OldStatus} -> {e.NewStatus}");
    }
}
