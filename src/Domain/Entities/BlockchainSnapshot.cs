namespace ICMarketWebAPI.Domain.Entities;
public class BlockchainSnapshot : BaseAuditableEntity
{
    public BlockchainNetwork Network { get; set; }

    public string RawJson { get; set; } = default!;

    //time requested from the API endpoint
    public DateTime CreatedAt { get; set; }
}
