/**
Nguyen Phuc Truong An
202419022
 */

// Lop duy nhat biet ca publisher (thiet bi) va subscriber (dich vu).
// Gom toan bo += / -= vao mot cho de tranh dang ky trung hoac quen huy.
public class DeviceMonitoringCoordinator : IDisposable
{
    private readonly List<TeachingDevice> devices;
    private readonly ConsoleNotificationService notificationService;
    private readonly DeviceAuditLogger auditLogger;
    private readonly MaintenanceService maintenanceService;

    private bool isSubscribed;
    private bool isDisposed;

    public bool IsSubscribed => isSubscribed;

    public DeviceMonitoringCoordinator(
        IEnumerable<TeachingDevice> devices,
        ConsoleNotificationService notificationService,
        DeviceAuditLogger auditLogger,
        MaintenanceService maintenanceService)
    {
        if (devices == null)
        {
            throw new ArgumentNullException(nameof(devices));
        }

        // Sao chep de danh sach khong bi thay doi tu ben ngoai
        // giua Subscribe() va Unsubscribe().
        this.devices = devices.ToList();

        if (this.devices.Any(d => d == null))
        {
            throw new ArgumentException("Device list must not contain null.", nameof(devices));
        }

        this.notificationService = notificationService
            ?? throw new ArgumentNullException(nameof(notificationService));
        this.auditLogger = auditLogger
            ?? throw new ArgumentNullException(nameof(auditLogger));
        this.maintenanceService = maintenanceService
            ?? throw new ArgumentNullException(nameof(maintenanceService));
    }

    public void Subscribe()
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(DeviceMonitoringCoordinator));
        }

        // Khong dang ky trung cung handler nhieu lan.
        if (isSubscribed)
        {
            return;
        }

        foreach (TeachingDevice device in devices)
        {
            device.StatusChanged += auditLogger.HandleStatusChanged;
            device.AlertRaised += notificationService.HandleAlert;
            device.AlertRaised += maintenanceService.HandleCriticalAlert;
        }

        isSubscribed = true;
    }

    public void Unsubscribe()
    {
        if (!isSubscribed)
        {
            return;
        }

        // Huy doi xung voi Subscribe().
        foreach (TeachingDevice device in devices)
        {
            device.StatusChanged -= auditLogger.HandleStatusChanged;
            device.AlertRaised -= notificationService.HandleAlert;
            device.AlertRaised -= maintenanceService.HandleCriticalAlert;
        }

        isSubscribed = false;
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        Unsubscribe();
        isDisposed = true;
    }
}
