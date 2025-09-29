using System.Globalization;
using Npgsql;
using Reception.Application.Abstraction;
using Reception.Application.Models;

namespace Reception.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PatientRepository(NpgsqlDataSource npgsqlDataSource)
    {
        _dataSource = npgsqlDataSource;
    }

    public async Task AddPatient(AddPatientRequest request)
    {
        const string sql = """
                               INSERT INTO Patient(name, surname, birthday)
                               VALUES (@name, @surname, @birthday)
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@name", request.Name));
        command.Parameters.Add(new NpgsqlParameter("@surname", request.Surname));
        command.Parameters.Add(new NpgsqlParameter("@birthday", request.DateOfBirth));

        await command.ExecuteNonQueryAsync();
    }

    public async Task<Patient> GetPatientById(long id)
    {
        const string sql = """
                                  SELECT * FROM Patient WHERE id = @id;
                           """;
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@id", id));
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Patient(
                Id: reader.GetInt64(0),
                Name: reader.GetString(1),
                Surname: reader.GetString(2),
                BirthDate: reader.GetDateTime(3));
        }

        throw new Exception($"Patient with id {id} not found");
    }

    public async Task DeletePatientById(long id)
    {
        const string sql = "DELETE FROM Patient WHERE id = @id;";
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@id", id));
        await command.ExecuteNonQueryAsync();
    }
}