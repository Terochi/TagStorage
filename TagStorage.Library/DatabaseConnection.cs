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
            	id    integer primary key not null,
            	name  text,
            	color text
            );
            create table if not exists tag_children
            (
            	child  integer references tags on delete cascade,
            	parent integer references tags on delete cascade,
            	primary key (child, parent)
            );
            create table if not exists file_tags
            (
            	tag  integer references tags on delete cascade,
            	file integer references files on delete cascade,
            	primary key (tag, file)
            );
            create table if not exists files
            (
                id integer primary key not null
            );
            create table if not exists file_locations
            (
                id      integer primary key not null,
                file    integer references files on delete cascade,
                type    text CHECK(type IN ('F','D')) not null default 'F', -- File/Directory
            	path    text,
                machine text,
                unique (path, machine)
            );
            create table if not exists changes
            (
                id       integer primary key not null,
                location integer references file_locations on delete cascade,
                date     datetime not null,
                size     integer,
                hash     text
            );

            create table if not exists tagging_rules
            (
                id   integer primary key not null,
                name text not null unique
            );
            create table if not exists automatic_tags
            (
                id        integer primary key not null,
                rule      integer references tagging_rules on delete cascade,
                directory text not null
            );

            create table if not exists directories
            (
                id        integer primary key not null,
                type      text CHECK(type IN ('I','E')) not null default 'I', -- Included/Excluded
                directory text not null
            );

            insert or ignore into tagging_rules (name) values ('Current Names');
            """);
    }
}
