namespace Domain.ValueObjects.Facturas;

public sealed record ValoresFactura
{
    public decimal ManoDeObra { get; }
    public decimal CostoRepuestos { get; }
    public decimal Descuento { get; }
    public decimal ImpuestoPct { get; }
    public decimal Subtotal { get; }
    public decimal Total { get; }

    private ValoresFactura(decimal manoDeObra, decimal costoRepuestos, decimal descuento, decimal impuestoPct, decimal subtotal, decimal total)
    {
        ManoDeObra = manoDeObra;
        CostoRepuestos = costoRepuestos;
        Descuento = descuento;
        ImpuestoPct = impuestoPct;
        Subtotal = subtotal;
        Total = total;
    }

    public static ValoresFactura Create(decimal manoDeObra, decimal costoRepuestos, decimal descuento, decimal impuestoPct, decimal subtotal, decimal total)
    {
        if (manoDeObra < 0) throw new ArgumentException("La mano de obra no puede ser negativa.");
        if (costoRepuestos < 0) throw new ArgumentException("El costo de repuestos no puede ser negativo.");
        if (descuento < 0) throw new ArgumentException("El descuento no puede ser negativo.");
        if (impuestoPct < 0 || impuestoPct > 100) throw new ArgumentException("El porcentaje de impuesto debe estar entre 0 y 100.");
        if (subtotal < 0) throw new ArgumentException("El subtotal no puede ser negativo.");
        if (total < 0) throw new ArgumentException("El total no puede ser negativo.");

        return new ValoresFactura(manoDeObra, costoRepuestos, descuento, impuestoPct, subtotal, total);
    }
}
