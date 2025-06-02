using System.Data;
using TagStorage.Library.Entities;
using TagStorage.Library.Helper;

namespace TagStorage.Library.Repository;

public class TagRepository(DatabaseConnection connection) : BaseRepository<TagEntity>(connection)
{
    protected override string TableName => "tags";

    protected override TagEntity MapEntity(IDataReader reader) =>
        new TagEntity
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Color = reader.IsDBNull(2) ? null : reader.GetString(2),
        };

    public IEnumerable<TagEntity> Get(string name)
    {
        return Get().Where(t => t.Name.FuzzyMatch(name));
    }

    public IEnumerable<TagEntity> GetNestedChildTags(int id)
    {
        //language=SQL
        return Connection.ExecuteQuery($"""
                                WITH CTE
                                AS(
                                  SELECT child.*
                                  FROM tag_children child
                                  WHERE child.parent = {id}

                                  UNION

                                  SELECT parent.*
                                  FROM CTE nextOne
                                  JOIN tag_children parent ON parent.parent = nextOne.child
                                )
                                SELECT tags.*
                                FROM CTE LEFT JOIN tags ON CTE.child = tags.id
                                """, MapEntity);
    }

    public IEnumerable<TagEntity> GetChildTags(int id)
    {
        return Connection.ExecuteQuery($"""
                                        SELECT tags.*
                                        FROM tag_children child
                                        LEFT JOIN tags ON child.child = tags.id
                                        WHERE child.parent = {id}
                                        """, MapEntity);
    }
}
