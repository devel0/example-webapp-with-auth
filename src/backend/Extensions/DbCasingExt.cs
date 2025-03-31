namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    public static void SnakeCaseMode(this IMutableEntityType entity)
    {
        var tableNameOrig = entity.GetTableName();

        if (tableNameOrig is not null)
            entity.SetTableName(tableNameOrig.ToSnakeCase());

        foreach (var property in entity.GetProperties())
        {
            property.SetColumnName(property.Name.ToSnakeCase());
        }

        foreach (var key in entity.GetKeys())
        {
            var name = key.GetName();
            if (name is not null)
            {
                var newname = name.ToSnakeCase();

                if (newname.StartsWith("pk_")) newname = "PK_" + newname.Substring(3);

                key.SetName(newname);
            }
        }

        foreach (var key in entity.GetForeignKeys())
        {
            var name = key.GetConstraintName();
            if (name is not null)
            {
                var newname = name.ToSnakeCase();

                if (newname.StartsWith("fk_")) newname = "FK_" + newname.Substring(3);

                key.SetConstraintName(newname);
            }
        }

    }

}