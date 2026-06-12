namespace Application.Common.Exceptions;

public sealed class StockInsuficienteException : InvalidOperationException
{
    public StockInsuficienteException(IReadOnlyList<StockInsuficienteItem> items)
        : base("Stock insuficiente para uno o mas repuestos.")
    {
        Items = items;
    }

    public IReadOnlyList<StockInsuficienteItem> Items { get; }
}

public sealed record StockInsuficienteItem(int RepuestoId, int Solicitado, int Disponible);
