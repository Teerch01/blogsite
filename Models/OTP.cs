namespace blogsite.Models;

public class OTP
{
    public OTP()
    {
        CreatedOn = DateTime.UtcNow;
    }
    public int Id { get; set; }
    public string Code { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid UserId { get; set; }
    public string Useremail { get; set; }
    public virtual User? User { get; set; }

}
