using System;
namespace SB004.Domain
{
    public interface IPositionRef
    {
        string Align { get; set; }
        int X { get; set; }
        int Y { get; set; }
    }
    public interface IComment
    {
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
        string SeedId { get; set; }
        byte[] ImageData { get; set; }
        DateTime DateCreated { get; set; }
        string CreatedBy { get; set; }
        IComment[] Comments { get; set; }
    }
}
