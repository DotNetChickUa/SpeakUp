using SQLite;
using SpeakUp.Models;

namespace SpeakUp.Services;

/// <summary>
/// Service for managing command history persistence
/// </summary>
public interface ICommandHistoryService
{
    /// <summary>
    /// Initialize the database
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Add a command to history
    /// </summary>
    Task<int> AddCommandAsync(CommandHistoryEntry entry);

    /// <summary>
    /// Get all commands for a session
    /// </summary>
    Task<List<CommandHistoryEntry>> GetSessionCommandsAsync(string sessionId);

    /// <summary>
    /// Get recent commands across all sessions
    /// </summary>
    Task<List<CommandHistoryEntry>> GetRecentCommandsAsync(int count = 50);

    /// <summary>
    /// Search commands by text
    /// </summary>
    Task<List<CommandHistoryEntry>> SearchCommandsAsync(string searchText);

    /// <summary>
    /// Delete old commands (older than specified days)
    /// </summary>
    Task<int> DeleteOldCommandsAsync(int olderThanDays = 30);

    /// <summary>
    /// Export commands to JSON
    /// </summary>
    Task<string> ExportCommandsAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Get session information
    /// </summary>
    Task<SessionInfo?> GetSessionAsync(string sessionId);

    /// <summary>
    /// Create or update session
    /// </summary>
    Task<string> SaveSessionAsync(SessionInfo session);

    /// <summary>
    /// Get all sessions
    /// </summary>
    Task<List<SessionInfo>> GetAllSessionsAsync();

    /// <summary>
    /// Delete a session and its commands
    /// </summary>
    Task DeleteSessionAsync(string sessionId);
}

/// <summary>
/// SQLite implementation of command history service
/// </summary>
internal sealed class CommandHistoryService : ICommandHistoryService
{
    private readonly SQLiteAsyncConnection _database;
    private bool _isInitialized;

    public CommandHistoryService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "commandhistory.db3");
        _database = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await _database.CreateTableAsync<CommandHistoryEntry>();
        await _database.CreateTableAsync<SessionInfo>();
        _isInitialized = true;
    }

    public async Task<int> AddCommandAsync(CommandHistoryEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        
        await InitializeAsync();
        return await _database.InsertAsync(entry);
    }

    public async Task<List<CommandHistoryEntry>> GetSessionCommandsAsync(string sessionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        
        await InitializeAsync();
        return await _database.Table<CommandHistoryEntry>()
            .Where(c => c.SessionId == sessionId)
            .OrderBy(c => c.Timestamp)
            .ToListAsync();
    }

    public async Task<List<CommandHistoryEntry>> GetRecentCommandsAsync(int count = 50)
    {
        await InitializeAsync();
        return await _database.Table<CommandHistoryEntry>()
            .OrderByDescending(c => c.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<CommandHistoryEntry>> SearchCommandsAsync(string searchText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchText);
        
        await InitializeAsync();
        var searchLower = searchText.ToLowerInvariant();
        
        return await _database.Table<CommandHistoryEntry>()
            .Where(c => c.Command.ToLower().Contains(searchLower) || 
                       (c.Result != null && c.Result.ToLower().Contains(searchLower)))
            .OrderByDescending(c => c.Timestamp)
            .ToListAsync();
    }

    public async Task<int> DeleteOldCommandsAsync(int olderThanDays = 30)
    {
        await InitializeAsync();
        var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
        
        return await _database.ExecuteAsync(
            "DELETE FROM CommandHistory WHERE Timestamp < ?", 
            cutoffDate);
    }

    public async Task<string> ExportCommandsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await InitializeAsync();
        
        var query = _database.Table<CommandHistoryEntry>();
        
        if (startDate.HasValue)
        {
            query = query.Where(c => c.Timestamp >= startDate.Value);
        }
        
        if (endDate.HasValue)
        {
            query = query.Where(c => c.Timestamp <= endDate.Value);
        }
        
        var commands = await query.OrderBy(c => c.Timestamp).ToListAsync();
        return System.Text.Json.JsonSerializer.Serialize(commands, new System.Text.Json.JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
    }

    public async Task<SessionInfo?> GetSessionAsync(string sessionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        
        await InitializeAsync();
        return await _database.Table<SessionInfo>()
            .Where(s => s.SessionId == sessionId)
            .FirstOrDefaultAsync();
    }

    public async Task<string> SaveSessionAsync(SessionInfo session)
    {
        ArgumentNullException.ThrowIfNull(session);
        
        await InitializeAsync();
        
        var existing = await GetSessionAsync(session.SessionId);
        if (existing != null)
        {
            await _database.UpdateAsync(session);
        }
        else
        {
            await _database.InsertAsync(session);
        }
        
        return session.SessionId;
    }

    public async Task<List<SessionInfo>> GetAllSessionsAsync()
    {
        await InitializeAsync();
        return await _database.Table<SessionInfo>()
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();
    }

    public async Task DeleteSessionAsync(string sessionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        
        await InitializeAsync();
        
        await _database.ExecuteAsync(
            "DELETE FROM CommandHistory WHERE SessionId = ?", 
            sessionId);
            
        await _database.ExecuteAsync(
            "DELETE FROM Sessions WHERE SessionId = ?", 
            sessionId);
    }
}
