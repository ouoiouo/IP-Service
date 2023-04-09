namespace MyService.Frp;

public partial class ServiceIpInfo
{
    public int Id { get; set; }

    public DateTime CreateTime { get; set; }

    public string? Ipv6Value { get; set; }

    public string? Ipv4Value { get; set; }

    public long? IsDelete { get; set; }
}
