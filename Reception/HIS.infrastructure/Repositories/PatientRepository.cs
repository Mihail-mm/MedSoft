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
                BirthDate: DateOnly.FromDateTime(reader.GetDateTime(3)));
        }
    }
}