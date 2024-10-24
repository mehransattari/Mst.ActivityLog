namespace Mst.ActivityLog;

using System.ComponentModel.DataAnnotations;

public class ActivityLog
{
    [Key]
    public long Id { get; set; }

    [MaxLength(450)]
    public string UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ActionType { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [MaxLength(50)]
    public string IpAddress { get; set; }

    [MaxLength(500)]
    public string UserAgent { get; set; }

    [MaxLength(100)]
    public string ControllerName { get; set; }

    [MaxLength(100)]
    public string ActionName { get; set; }
}
