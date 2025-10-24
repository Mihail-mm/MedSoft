using FluentMigrator;

namespace HIS.infrastructure.Migrations;

[Migration(1731949849, "initial")]
public class Initial : Migration
{
    public override void Up()
    {
        Execute.Sql("CREATE TYPE patient_status AS ENUM ('not-arrived', 'arrived', 'started', 'completed');");

        Create.Table("patients")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(128).NotNullable()
            .WithColumn("surname").AsString(128).NotNullable()
            .WithColumn("birthdate").AsDate().NotNullable()
            .WithColumn("patient_status").AsCustom("patient_status").NotNullable().WithDefaultValue("not-arrived");
    }

    public override void Down()
    {
        Delete.Table("patients");
        Execute.Sql("DROP TYPE schedule_status;");
    }
}