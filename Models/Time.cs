namespace blogsite.Models;

public abstract class Time
{
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    public Time()
    {
        CreatedOn = DateTime.UtcNow;
        ModifiedOn = DateTime.UtcNow;
    }
}
