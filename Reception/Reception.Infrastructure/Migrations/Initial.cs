using FluentMigrator;

namespace Reception.Infrastructure.Migrations;

[Migration(1731949849, "initial")]
public class Initial : Migration
{
    public override void Up()
    {
        Create.Table("patients")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(128).NotNullable()
            .WithColumn("surname").AsString(128).NotNullable()
            .WithColumn("birthdate").AsDate().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("patients");
    }
}