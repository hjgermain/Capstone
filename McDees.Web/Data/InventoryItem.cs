namespace McDees.Web.Data;

public sealed class InventoryItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string StoreName { get; set; } = "McDees Downtown";
    public decimal OnHandQuantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal ReorderPoint { get; set; }
    public decimal UnitCost { get; set; }
    public string StorageLocation { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
