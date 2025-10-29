using System.Data.Common;
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

    public async Task<long> AddPatient(AddPatientRequest request)
    {
        const string sql = """
                               INSERT INTO Patient(name, surname, birthday)
                               VALUES (@name, @surname, @birthday)
                               returning id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@name", request.Name));
        command.Parameters.Add(new NpgsqlParameter("@surname", request.Surname));
        command.Parameters.Add(new NpgsqlParameter("@birthday", request.DateOfBirth));

        await using DbDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync()) return reader.GetInt64(0);
        throw new InvalidOperationException();
    }

    public async IAsyncEnumerable<Patient> GetAllPatients()
    {
        const string sql = """
                           select *
                           from patient
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(sql, connection);

        await using DbDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            yield return new Patient(
                Id: reader.GetInt64(0),
                Name: reader.GetString(1),
                Surname: reader.GetString(2),
                BirthDate: DateOnly.FromDateTime(reader.GetDateTime(3)),
                Status: reader.GetFieldValue<PatientStatus>(4));
        }
    }

    public async Task<Patient> GetPatientById(long id)
    {
        const string sql = """ SELECT * FROM Patient WHERE id = @id; """;

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
                BirthDate: DateOnly.FromDateTime(reader.GetDateTime(3)),
                Status: reader.GetFieldValue<PatientStatus>(4));
        }

        throw new Exception($"Patient with id {id} not found");
    }

    public async IAsyncEnumerable<Patient> GetPatientBySearchRequest(SearchPatientRequest request)
    {
        const string sql = """
                            SELECT *
                            FROM Patient
                            WHERE name = @name AND surname = @surname;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@name", request.Name));
        command.Parameters.Add(new NpgsqlParameter("@surname", request.Surname));

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            yield return new Patient(
                Id: reader.GetInt64(0),
                Name: reader.GetString(1),
                Surname: reader.GetString(2),
                BirthDate: DateOnly.FromDateTime(reader.GetDateTime(3)),
                Status: reader.GetFieldValue<PatientStatus>(4));
        }
    }

    public async Task DeletePatientById(long id)
    {
        const string sql = """ DELETE FROM Patient WHERE id = @id; """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@id", id));

        await command.ExecuteNonQueryAsync();
    }

    public async Task PatchPatientStatus(long id, PatientStatus status)
    {
        const string sql = """
                           UPDATE Patient
                           SET patient_status = @status
                           WHERE id = @id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@id", id));
        var statusParam = new NpgsqlParameter("@status", status)
        {
            DataTypeName = "patient_status"
        };
        command.Parameters.Add(statusParam);

        await command.ExecuteNonQueryAsync();
    }
}