using System.Data;
using TagStorage.Library.Helper;

namespace TagStorage.Library;

public class TagRepository(DatabaseConnection db)
{
	private TagEntity fetchTagEntity(IDataReader reader)
	{
		return new TagEntity
		{
			Id = reader.GetInt32(0),
			Name = reader.GetString(1)
		};
	}

	public TagEntity? Get(int id)
	{
		return db.ExecuteQuery($"SELECT * FROM tags WHERE id = {id};", fetchTagEntity).FirstOrDefault();
	}

	public IEnumerable<TagEntity> Get()
	{
		return db.ExecuteQuery("SELECT * FROM tags;", fetchTagEntity);
	}

	public IEnumerable<TagEntity> Get(string name)
	{
		return Get().Where(t => t.Name.FuzzyMatch(name));
	}

	public TagEntity Add(string name)
	{
		return db.ExecuteQuery($"INSERT INTO tags (name) VALUES ('{name}') RETURNING id, name;", fetchTagEntity).First();
	}

	public void Link(int childId, int parentId)
	{
		db.ExecuteCommand($"INSERT INTO tag_parent_of (child, parent) VALUES ({childId}, {parentId});");
	}
}