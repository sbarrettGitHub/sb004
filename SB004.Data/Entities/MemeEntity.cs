using MongoDB.Bson;
using SB004.Domain;

namespace SB004.Data.Entities
{
  using System.Collections.Generic;
  using System.Linq;

  public class MemeEntity
  {
    public MemeEntity(IMeme meme)
    {
      this.SeedId = meme.SeedId;
      this.ImageData = meme.ImageData;
      this.DateCreated = meme.DateCreated;
      this.CreatedBy = meme.CreatedBy;
      this.Comments = meme.Comments.Select(x => new CommentEntity
      {
        BackgroundColor = x.BackgroundColor,
        Color = x.Color,
        FontFamily = x.FontFamily,
        FontSize = x.FontSize,
        FontStyle = x.FontStyle,
        FontWeight = x.FontWeight,
        Position = new PositionRefEntity
        {
          Align = x.Position.Align,
          Height = x.Position.Height,
          Padding = x.Position.Padding,
          Width = x.Position.Width,
          X = x.Position.X,
          Y= x.Position.Y
        },
        Text = x.Text,
        TextAlign = x.TextAlign,
        TextDecoration = x.TextDecoration,
        TextShadow = x.TextShadow
      }).ToList();
    }

    public ObjectId Id { get; set; }
    public string SeedId { get; set; }
    public byte[] ImageData { get; set; }
    public BsonDateTime DateCreated { get; set; }
    public string CreatedBy { get; set; }
    public List<CommentEntity> Comments { get; set; }
    
    public IMeme ToIMeme()
    {
      return new Meme
      {
        Id = this.Id.ToString(),
        Comments = this.Comments.Select(x => (IComment)new Comment
        {
          BackgroundColor = x.BackgroundColor,
          Color = x.Color,
          FontFamily = x.FontFamily,
          FontSize = x.FontSize,
          FontStyle = x.FontStyle,
          FontWeight = x.FontWeight,
          Position = new PositionRef
          {
            Align = x.Position.Align,
            Height = x.Position.Height,
            Padding = x.Position.Padding,
            Width = x.Position.Width,
            X = x.Position.X,
            Y = x.Position.Y
          },
          Text = x.Text,
          TextAlign = x.TextAlign,
          TextDecoration = x.TextDecoration,
          TextShadow = x.TextShadow
        }).ToList(),
        CreatedBy = this.CreatedBy,
        DateCreated = this.DateCreated.ToLocalTime(),
        ImageData = this.ImageData,
        SeedId = this.SeedId,
      };
    }
  }
  public class PositionRefEntity 
  {
    public string Align { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Padding { get; set; }
  }
  public class CommentEntity
  {
    public PositionRefEntity Position { get; set; }
    public string Text { get; set; }
    public string Color { get; set; }
    public string BackgroundColor { get; set; }
    public string FontFamily { get; set; }
    public string FontSize { get; set; }
    public string FontWeight { get; set; }
    public string TextDecoration { get; set; }
    public string FontStyle { get; set; }
    public string TextAlign { get; set; }
    public string TextShadow { get; set; }

  }
}
