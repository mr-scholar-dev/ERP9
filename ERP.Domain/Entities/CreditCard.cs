namespace ERP.Domain.Entities;

public record class CreditCard
{
    public int IdCreditCard { get; set; }
    public string CreditCardName { get; set; } = null!;
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
