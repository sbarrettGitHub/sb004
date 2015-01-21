namespace SB004.Business
{
  using SB004.Domain;

  public enum IdType {Unknown, Seed, Meme};

  public interface IIdManager
  {
    string NewId(IdType type);

    string GetIdCategory(string id);

    string GetIdTypeIndicator(string id);

    string GetIdPrefix(string id);

    IdType GetIdType(string id);
  }
}
