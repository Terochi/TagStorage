using System.Data;
using Microsoft.Data.Sqlite;

namespace TagStorage.Library;

public class DatabaseConnection
{
    private readonly IDbConnection db;

    public DatabaseConnection(string path)
    {
        db = new SqliteConnection($"Data Source={path};");
        db.Open();
        createDatabaseIfNotExists();
    }

    public void ExecuteCommand(string commandText)
    {
        using (IDbCommand command = db.CreateCommand())
        {
            command.CommandText = commandText;
            command.ExecuteNonQuery();
        }
    }

    public IEnumerable<T> ExecuteQuery<T>(string queryText, Func<IDataReader, T> mapFunction)
    {
        using (IDbCommand command = db.CreateCommand())
        {
            command.CommandText = queryText;
            IDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                yield return mapFunction(reader);
            }

            reader.Close();
        }
    }

    private void createDatabaseIfNotExists()
    {
        ExecuteCommand(
            """
            create table if not exists tags
            (
            	id   integer primary key not null,
            	name text,
            	color text
            );
            create table if not exists tag_parent_of
            (
            	child  integer references tags on delete cascade,
            	parent integer references tags on delete cascade,
            	primary key (child, parent)
            );
            """);
    }
}
