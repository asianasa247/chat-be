namespace ManageEmployee.DataTransferObject.Response;

public class ShoppingCartProductInfoResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public double Total { get; set; }
    public string? Code { get; set; }
    public double Vat { get; set; }
    public double TotalPrice => Price + TaxVat;
    public double PriceDiscount { get; set; }
    public double TaxVat { get; set; }
    public IEnumerable<string>? Images { get; set; }
}