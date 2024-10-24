### Install package

`Install-Package MST.ActivityLog -Version 1.0.0`

### Program.cs
```

builder.Services.AddScoped<IActivityLogDbContext>(provider =>
        provider.GetService<ApplicationDbContext>());

builder.Services.AddActivityLogging(settings =>
{
    settings.HttpMethods.Add(HttpMethodType.Post);
    settings.HttpMethods.Add(HttpMethodType.Delete);
});

```
                             


### Add file nlog.config
```
public class ApplicationDbContext : DbContext, IActivityLogDbContext
{   
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public ApplicationDbContext()
    {
    }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
}

```


###How use controller

```
[Route("api/[controller]")]
[ApiController]
public class LogEntriesController : ControllerBase
{
    private readonly IActivityLogger _activityLogger;
    private readonly ApplicationDbContext _context;

    public LogEntriesController(IActivityLogger activityLogger, ApplicationDbContext context)
    {
        _activityLogger = activityLogger;
        _context = context;
    }

    // GET: api/LogEntries
    [HttpGet]
    public async Task<IActionResult> GetLogs()
    {
        var logs = await _context.ActivityLogs.ToListAsync();
        return Ok(logs);
    }

    // GET: api/LogEntries/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLogById(long id)
    {
        _logger.LogInformation($"Fetching log entry with ID: {id}");
        var log = await _context.ActivityLogs.FindAsync(id);

        if (log == null)
        {
            _logger.LogWarning($"Log entry with ID: {id} not found");
            return NotFound();
        }

        return Ok(log);
    }

    // POST: api/LogEntries
    [HttpPost]
    public async Task<IActionResult> AddLog([FromBody] ActivityLog activityLog)
    {
        if (activityLog == null)
        {
            return BadRequest("Log entry cannot be null");
        }

         await _activityLogger.LogAsync(activityLog);
        return CreatedAtAction(nameof(GetLogById), new { id = activityLog.Id }, activityLog);
    }

    // DELETE: api/LogEntries/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLog(long id)
    {
        var log = await _context.ActivityLogs.FindAsync(id);

        if (log == null)
        {
            return NotFound();
        }

        _context.ActivityLogs.Remove(log);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}


```
