namespace Reception.Application.Models;

public record Patient(long Id, string Name, string Surname, DateOnly BirthDate);