namespace Reception.Application.Models;

public record PatientHl7Model(long Id, string Name, string Surname, DateOnly BirthDate, string Hl7Action);