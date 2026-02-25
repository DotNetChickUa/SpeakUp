using System.ComponentModel;
using Shared;

namespace DeviceExtensions.GetLocationMacroCommand;

[SpeakUpTool]
public class GetLocationMacroCommand
{
	[Description("Returns my geoposition location")]
	public static async Task<GeolocationResult?> GetLocation(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var result = await Geolocation.GetLocationAsync();
		if (result is null)
		{
			return null;
		}

		var geolocationResult = new GeolocationResult()
		{
			Location = new GeolocationPosition()
			{
				Coords = new GeolocationCoordinates()
				{
					Latitude = result.Latitude,
					Longitude = result.Longitude,
					Accuracy = result.Accuracy ?? 0,
					Speed = result.Speed,
					Altitude = result.Altitude
				}
			}
		};
		
		return geolocationResult;
	}
}