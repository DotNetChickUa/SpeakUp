namespace DeviceExtensions.GetLocationMacroCommand;

public class GeolocationPositionError
{
	public enum GeolocationPositionErrorCode
	{
		PermissionDenied,
		Timeout,
		PositionUnavailable
	}

	/// <summary>
	///     The error code
	/// </summary>
	/// <value></value>
	public int Code { get; set; }

	/// <summary>
	///     Easy access to error code
	/// </summary>
	/// <value></value>
	public GeolocationPositionErrorCode CodeEnum => Code switch
	{
		1 => GeolocationPositionErrorCode.PermissionDenied,
		2 => GeolocationPositionErrorCode.PositionUnavailable,
		3 => GeolocationPositionErrorCode.Timeout,
		_ => throw new NotSupportedException($"GeolocationPositionError.Code:{Code}")
	};

	/// <summary>
	///     the details of the error. Specifications note that this is primarily intended for debugging use and not to be shown
	///     directly in a user interface.
	/// </summary>
	/// <value></value>
	public string? Message { get; set; }
}