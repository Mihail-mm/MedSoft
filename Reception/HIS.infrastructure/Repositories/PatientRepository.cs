using System.Data.Common;
using HIS.Application.Abstractions;
using HIS.Application.Models;
using Npgsql;

namespace HIS.infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PatientRepository(NpgsqlDataSource npgsqlDataSource)
    {
        _dataSource = npgsqlDataSource;
    }

    public async Task AddPatient(Patient patient)
    {
        const string sql = """
                           INSERT INTO patients(id, name, surname, birthday)
                           VALUES (@id, @name, @surname, @birthday)
                           """;
        
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", patient.Id);
        command.Parameters.Add(new NpgsqlParameter("@name", patient.Name));
        command.Parameters.Add(new NpgsqlParameter("@surname", patient.Surname));
        command.Parameters.Add(new NpgsqlParameter("@birthday", patient.BirthDate));

        await command.ExecuteNonQueryAsync();
    }

    public async IAsyncEnumerable<Patient> GetAll()
    {
        const string sql = """
                           select *
                           from patients
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

    public async Task DeletePatient(long patientId)
    {
        const string sql = """ DELETE FROM Patients WHERE id = @id; """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@id", patientId));

        await command.ExecuteNonQueryAsync();
    }

    public async Task PatchPatientStatus(long patientId, PatientStatus status)
    {
        const string sql = """
                           UPDATE Patients
                           SET patient_status = @status
                           WHERE id = @patientId;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("@patientId", patientId));
        var statusParam = new NpgsqlParameter("@status", status)
        {
            DataTypeName = "patient_status"
        };
        command.Parameters.Add(statusParam);

        await command.ExecuteNonQueryAsync();
    }
}