using System.Data;
using TagStorage.Library.Entities;
using TagStorage.Library.Helper;

namespace TagStorage.Library;

public class TagRepository(DatabaseConnection db)
{
    private TagEntity tagMapFunction(IDataReader reader)
    {
        return new TagEntity
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Color = reader.IsDBNull(2) ? null : reader.GetString(2),
        };
    }

    public TagEntity? Get(int id)
    {
        return db.ExecuteQuery($"SELECT * FROM tags WHERE id = {id};", tagMapFunction).FirstOrDefault();
    }

    public IEnumerable<TagEntity> Get()
    {
        return db.ExecuteQuery("SELECT * FROM tags;", tagMapFunction);
    }

    public IEnumerable<TagEntity> Get(string name)
    {
        return Get().Where(t => t.Name.FuzzyMatch(name));
    }

    public TagEntity Add(string name)
    {
        return db.ExecuteQuery($"INSERT INTO tags (name) VALUES ('{name}') RETURNING *;", tagMapFunction)
                 .First();
    }

    public void Link(int childId, int parentId)
    {
        db.ExecuteCommand($"INSERT INTO tag_parent_of (child, parent) VALUES ({childId}, {parentId});");
    }

    public IEnumerable<TagEntity> GetNestedChildTags(int id)
    {
        //language=SQL
        return db.ExecuteQuery($"""
                                WITH CTE
                                AS(
                                  SELECT child.*
                                  FROM tag_parent_of child
                                  WHERE child.parent = {id}

                                  UNION

                                  SELECT parent.*
                                  FROM CTE nextOne
                                  JOIN tag_parent_of parent ON parent.parent = nextOne.child
                                )
                                SELECT tags.*
                                FROM CTE LEFT JOIN tags ON CTE.child = tags.id
                                """, tagMapFunction);
    }

    public IEnumerable<TagEntity> GetChildTags(int id)
    {
        return db.ExecuteQuery($"""
                                SELECT tags.*
                                FROM tag_parent_of child
                                LEFT JOIN tags ON child.child = tags.id
                                WHERE child.parent = {id}
                                """, tagMapFunction);
    }
}
