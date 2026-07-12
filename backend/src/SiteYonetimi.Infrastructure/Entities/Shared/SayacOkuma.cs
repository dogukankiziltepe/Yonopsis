namespace SiteYonetimi.Infrastructure.Entities.Shared;

public class SayacOkuma
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SiteId { get; set; }
    // Either AnaSayacId or DaireSayacId is set
    public Guid? AnaSayacId { get; set; }
    public Guid? DaireSayacId { get; set; }
    public DateTime OkumaTarihi { get; set; }
    public decimal OncekiEndeks { get; set; }
    public decimal SonEndeks { get; set; }
    public decimal Tuketim => SonEndeks - OncekiEndeks;
    public string? Aciklama { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public AnaSayac? AnaSayac { get; set; }
    public DaireSayac? DaireSayac { get; set; }
}
