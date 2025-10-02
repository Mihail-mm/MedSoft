namespace Reception.Application.Models;

public record AddPatientRequest(string Name, string Surname, DateOnly DateOfBirth);