namespace Domain.ValueObjects.ModeloVehiculos;

public sealed record RangoAnio
{
    public short AnioDesde { get; }
    public short AnioHasta { get; }

    private RangoAnio(short anioDesde, short anioHasta)
    {
        AnioDesde = anioDesde;
        AnioHasta = anioHasta;
    }

    public static RangoAnio Create(short anioDesde, short anioHasta)
    {
        if (anioDesde < 1900 || anioDesde > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("El año desde no es válido.", nameof(anioDesde));

        if (anioHasta < 1900 || anioHasta > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("El año hasta no es válido.", nameof(anioHasta));

        if (anioDesde > anioHasta)
            throw new ArgumentException("El año desde no puede ser mayor al año hasta.");

        return new RangoAnio(anioDesde, anioHasta);
    }

    public override string ToString() => $"{AnioDesde} - {AnioHasta}";
}
