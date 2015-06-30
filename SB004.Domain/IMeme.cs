using System;
namespace SB004.Domain
{
  using System.Collections.Generic;

  public interface IPositionRef
    {
        string Align { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        float Padding { get; set; }

    }
    public interface IComment
    {
        int Id { get; set; }
        IPositionRef Position { get; set; }
        string Text { get; set; }
        string Color { get; set; }
        string BackgroundColor { get; set; }
        string FontFamily { get; set; }
        string FontSize { get; set; }
        string FontWeight { get; set; }
        string TextDecoration { get; set; }
        string FontStyle { get; set; }
        string TextAlign { get; set; }
        string TextShadow { get; set; }

    }
    public interface IMeme
    {
        string Id { get; set; }
        string Title { get; set; }
        string CreatedByUserId { get; set; }
        string SeedId { get; set; }
        byte[] ImageData { get; set; }
        DateTime DateCreated { get; set; }
        string CreatedBy { get; set; }
        List<IComment> Comments { get; set; }
        string ResponseToId { get; set; }
        List<string> ReplyIds { get; set; }
        int Likes { get; set; }
        int Dislikes { get; set; }
        int Favourites { get; set; }
        int Shares { get; set; }
        int Views { get; set; }
    }
}
