using System;

namespace SB004.Business
{
    public class IdManager : IIdManager
    {
      /// <summary>
      /// Return a new id. prepend the date yyMMdd and type to a guid
      /// </summary>
      /// <returns></returns>
      public string NewId(IdType type)
      {
        string typeChar;
        switch (type)
        {
          case IdType.Seed:
            typeChar = "s";
            break;
          case IdType.Meme:
            typeChar = "m";
            break;
          default:
            typeChar = "u";
            break;
        }
        return DateTime.Now.ToString("yyMMdd") + typeChar + Guid.NewGuid().ToString("N");
      }

      /// <summary>
      /// For a given id extract a category
      /// Fro an id of yyMMddGuid return dd
      /// Can by used to bunch ids together e.g. as Azure table partition
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public string GetIdCategory(string id)
      {
        if (id.Length > 5)
        {
          return id.Substring(4, 2);
        }
        return "0";
      }
      /// <summary>
      /// Extract the type of id from the id supplied. Character 7 or "u" if not found
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public string GetIdTypeIndicator(string id)
      {
        string typeIndicator = "u";
        if (id.Length > 6)
        {
          typeIndicator = id.Substring(6, 1);
        }
        return typeIndicator;
      }
      /// <summary>
      /// Conver the type indicator in an id to an IdType enum 
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public IdType GetIdType(string id)
      {
        string typeIndicator = this.GetIdTypeIndicator(id);
        switch (typeIndicator)
        {
          case "s":
            return IdType.Seed;
          case "u":
            return IdType.Meme;
          default:
            return IdType.Unknown;
        }
      }

      /// <summary>
      /// Extract the prefix from the id supplied. Character 0-4
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public string GetIdPrefix(string id)
      {
        if (id.Length > 3)
        {
          return id.Substring(0, 4);
        }
        return "0000";
      }
    }
}