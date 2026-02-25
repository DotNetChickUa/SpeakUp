namespace GoogleDriveExtensions;

public class GoogleFile
{
	public string? Id { get; set; }
	public string? Name { get; set; }
	public string? MimeType { get; set; }
	public DateTime? CreatedTime { get; set; }
	public DateTime? ModifiedTime { get; set; }
	public long? Size { get; set; }
}