namespace Domain.ValueObjects.Citas;

public sealed record HorarioCita
{
    public TimeOnly HoraInicio { get; }
    public TimeOnly HoraFin { get; }

    private HorarioCita(TimeOnly horaInicio, TimeOnly horaFin)
    {
        HoraInicio = horaInicio;
        HoraFin = horaFin;
    }

    public static HorarioCita Create(TimeOnly horaInicio, TimeOnly horaFin)
    {
        if (horaInicio >= horaFin)
            throw new ArgumentException("La hora de inicio debe ser anterior a la hora de fin.");

        return new HorarioCita(horaInicio, horaFin);
    }

    public override string ToString() => $"{HoraInicio} - {HoraFin}";
}
