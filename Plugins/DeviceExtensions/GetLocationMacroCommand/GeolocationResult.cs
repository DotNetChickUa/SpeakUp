namespace DeviceExtensions.GetLocationMacroCommand;

using System.Diagnostics.CodeAnalysis;

public class GeolocationResult
{
	/// <summary>
	///     Current user location from his browser
	/// </summary>
	/// <value></value>
	public GeolocationPosition? Location { get; set; }

	/// <summary>
	///     Error received when getting the current location
	/// </summary>
	/// <value></value>
	[MemberNotNull(nameof(Location))]
	public GeolocationPositionError? Error { get; set; }
}