namespace Domain.ValueObjects.Clientes;

public sealed record NombreCompleto
{
    public string Nombres { get; }
    public string Apellidos { get; }

    private NombreCompleto(string nombres, string apellidos)
    {
        Nombres = nombres;
        Apellidos = apellidos;
    }

    public static NombreCompleto Create(string nombres, string apellidos)
    {
        if (string.IsNullOrWhiteSpace(nombres))
            throw new ArgumentException("Los nombres son obligatorios.", nameof(nombres));
            
        if (string.IsNullOrWhiteSpace(apellidos))
            throw new ArgumentException("Los apellidos son obligatorios.", nameof(apellidos));

        if (nombres.Trim().Length > 100)
            throw new ArgumentException("Los nombres no pueden exceder los 100 caracteres.", nameof(nombres));

        if (apellidos.Trim().Length > 100)
            throw new ArgumentException("Los apellidos no pueden exceder los 100 caracteres.", nameof(apellidos));

        return new NombreCompleto(nombres.Trim(), apellidos.Trim());
    }

    public override string ToString() => $"{Nombres} {Apellidos}";
}
