namespace Uagrm.Sw1.iRecycle.Bff.SharedKernel.Audit;

public interface IAuditableEntity
{
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
