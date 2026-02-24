namespace AbstractApiExtensions;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DrawGo.Plugin.Base;
using DrawGo.Plugin.Base.Attributes;
using DrawGo.Plugin.Base.Enums;
using DrawGo.Plugin.Base.Fields;
using DrawGo.Plugin.Base.Results;

[Export(Name = "Holidays", Category = "Lookup", Description = "https://app.abstractapi.com/api/holidays/documentation", Platform = MacroPlatform.All)]
public class HolidaysLookup : MacroCommand
{
	public HolidaysLookup()
	{
		ApiKey = AddValueInput<string>(nameof(ApiKey));
		Country = AddValueInput<string>(nameof(Country));
		Date = AddValueInput<DateTime>(nameof(Date));
		Result = AddValueOutput(nameof(Result), IsValid);
	}

	[FieldAttributes(Label = "Result", Type = DataType.Text)]
	public ValueOutput Result { get; }

	[FieldAttributes(Label = "API key", Type = DataType.Text, IsSecret = true)]
	public ValueInput ApiKey { get; }

	[FieldAttributes(Label = "Country", Type = DataType.Text)]
	public ValueInput Country { get; }

	[FieldAttributes(Label = "Date", Type = DataType.Date)]
	public ValueInput Date { get; }

	public override MacroPlatform MacroPlatform => MacroPlatform.All;
	public override string CommandName => "Holidays";
	public override string Description => "https://app.abstractapi.com/api/holidays/documentation";
	public override string Category => "Lookup";

	private async Task<Holiday[]?> IsValid(Flow flow,
		IProgress<MacroActionResult> progress,
		CancellationToken cancellationToken)
	{
		var apiKey = await flow.GetValueAsync<string>(ApiKey, progress, cancellationToken);
		var country = await flow.GetValueAsync<string>(Country, progress, cancellationToken);
		var date = await flow.GetValueAsync<DateTime>(Date, progress, cancellationToken);

		using var api = new HttpClient();
		api.BaseAddress = new Uri("https://holidays.abstractapi.com/v1/");
		var result = await api.GetAsync($"?api_key={apiKey}&country={country}&year={date.Year}&month={date.Month}&day={date.Day}", cancellationToken);

		if (result.IsSuccessStatusCode)
		{
			var options = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			};
			options.Converters.Add(new JsonStringEnumConverter());
			var content = await result.Content.ReadFromJsonAsync<Holiday[]>(options, cancellationToken);
			progress.Report(new MacroActionResult(
								this,
								new HolidaysLookupResult($"Found {content?.Length} holidays")));
			return content;
		}

		progress.Report(new MacroActionResult(
							this,
							HasErrorResult.WithError(result.ReasonPhrase ?? $"Error: {result.StatusCode}")));

		return null;
	}
}