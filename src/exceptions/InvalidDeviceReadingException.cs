/**
Nguyen Phuc Truong An
202419022
 */

public class InvalidDeviceReadingException : Exception
{
    public string DeviceId { get; }
    public int InvalidValue { get; }

    public InvalidDeviceReadingException(
        string deviceId,
        int invalidValue,
        string? message = null)
        : base(message ?? $"Invalid reading {invalidValue} for device {deviceId}. Value must be in range 0-100.")
    {
        DeviceId = deviceId;
        InvalidValue = invalidValue;
    }
}
