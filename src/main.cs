/**
Nguyen Phuc Truong An
202419022
 */

public static class Program
{
    public static void Main()
    {
        var notificationService = new ConsoleNotificationService();
        var auditLogger = new DeviceAuditLogger();
        var maintenanceService = new MaintenanceService();

        var projector = new ProjectorRemote("PRJ-01", "Projector remote A101");
        var airConditioner = new AirConditionerRemote("AC-01", "AC remote", "A101");
        var microphone = new WirelessMicrophone("MIC-01", "Wireless mic A101");

        var devices = new List<TeachingDevice> { projector, airConditioner, microphone };

        // Subscriber dung delegate AlertFilter + Action (muc 8).
        var criticalOnly = new FilteredAlertSubscriber(
            alert => alert.Severity == AlertSeverity.Critical,
            alert => Console.WriteLine(
                $"  >> CRITICAL: {alert.DeviceId} - {alert.Message}"));

        using (var coordinator = new DeviceMonitoringCoordinator(
            devices, notificationService, auditLogger, maintenanceService))
        {
            Section("1. Subscribe (goi 2 lan, khong dang ky trung)");
            coordinator.Subscribe();
            coordinator.Subscribe();
            foreach (TeachingDevice device in devices)
            {
                device.AlertRaised += criticalOnly.HandleAlert;
            }
            Console.WriteLine("Subscribed.");

            Section("2. Thay doi trang thai");
            projector.ChangeStatus(DeviceStatus.Borrowed);
            projector.ChangeStatus(DeviceStatus.Borrowed); // trung -> khong co event
            microphone.ChangeStatus(DeviceStatus.Borrowed);
            microphone.ChangeStatus(DeviceStatus.Available);
            PrintList("Audit log", auditLogger.Entries);

            Section("3. ProjectorRemote: pin 100 -> 50 -> 19 -> 10 -> 0 -> 80");
            foreach (int level in new[] { 50, 19, 10, 0, 80 })
            {
                Console.WriteLine($"- Battery = {level}");
                projector.UpdateBatteryLevel(level);
            }

            Section("4. AirConditionerRemote: mat phan hoi 2 lan, roi phuc hoi");
            airConditioner.UpdateResponseState(false);
            airConditioner.UpdateResponseState(false); // lap -> khong canh bao
            airConditioner.UpdateResponseState(true);

            Section("5. WirelessMicrophone: pin 25, tin hieu 35, tin hieu 0");
            microphone.UpdateBatteryLevel(25);
            microphone.UpdateSignalQuality(35);
            microphone.UpdateSignalQuality(0);

            Section("6. Gia tri khong hop le");
            TryUpdate(() => projector.UpdateBatteryLevel(150));
            TryUpdate(() => microphone.UpdateSignalQuality(-5));
            Console.WriteLine(
                $"State unchanged: projector battery = {projector.BatteryLevel}, " +
                $"mic signal = {microphone.SignalQuality}");

            PrintList("Maintenance requests", maintenanceService.Requests);
        } // Dispose() -> Unsubscribe()

        foreach (TeachingDevice device in devices)
        {
            device.AlertRaised -= criticalOnly.HandleAlert;
        }

        Section("7. Sau Dispose: khong subscriber nao nhan event");
        int auditCount = auditLogger.Entries.Count;
        int requestCount = maintenanceService.Requests.Count;

        projector.ChangeStatus(DeviceStatus.UnderMaintenance);
        airConditioner.UpdateResponseState(false);

        Console.WriteLine(
            $"Audit entries: {auditCount} -> {auditLogger.Entries.Count}, " +
            $"maintenance requests: {requestCount} -> {maintenanceService.Requests.Count}");
    }

    private static void TryUpdate(Action update)
    {
        try
        {
            update();
        }
        catch (InvalidDeviceReadingException ex)
        {
            Console.WriteLine(
                $"Error [{ex.DeviceId}, value={ex.InvalidValue}]: {ex.Message}");
        }
    }

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {title} ===");
    }

    private static void PrintList(string title, IReadOnlyList<string> items)
    {
        Console.WriteLine($"{title} ({items.Count}):");
        foreach (string item in items)
        {
            Console.WriteLine($"  {item}");
        }
    }
}
