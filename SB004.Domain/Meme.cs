﻿using System;
namespace SB004.Domain
{
  using System.Collections.Generic;

  public class PositionRef : IPositionRef
    {
        public string Align { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Padding { get; set; }
    }
    public class Comment : IComment
    {
        public int Id { get; set; }
        public IPositionRef Position { get; set; }
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
    public class Meme : IMeme
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string CreatedByUserId { get; set; }
        public string SeedId { get; set; }
        public byte[] ImageData { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public List<IComment> Comments { get; set; }
        public string ResponseToId { get; set; }
        public List<string> ReplyIds { get; set; }
    }
}
