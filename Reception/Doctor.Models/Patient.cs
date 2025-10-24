namespace Doctor.Models;

public record Patient(long Id, string Name, string Surname, DateOnly BirthDate, PatientStatus Status);