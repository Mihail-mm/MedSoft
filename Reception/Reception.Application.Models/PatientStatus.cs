using NpgsqlTypes;

namespace Reception.Application.Models;

public enum PatientStatus
{
    [PgName("not_arrived")] NotArrived = 0,
    [PgName("arrived")] Arrived = 1,
    [PgName("started")] Started = 2,
    [PgName("completed")] Completed = 3,
}