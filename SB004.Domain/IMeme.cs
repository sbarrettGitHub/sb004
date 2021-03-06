﻿using System;
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
  public interface IMeme: ITrendFactors
  {
    string Id { get; set; }
    string RepostOfId { get; set; }
    string Title { get; set; }
    string CreatedByUserId { get; set; }
    IUser Creator { get; }   
    string SeedId { get; set; }
    byte[] ImageData { get; set; }
    string CreatedBy { get; set; }
    List<IComment> Comments { get; set; }
    string ResponseToId { get; set; }
    List<IReply> ReplyIds { get; set; }
    bool IsTopLevel { get; set;}
    double TrendScore { get; set; }
    IMeme SetCreator(IUser creator);
	List<string> HashTags { get; set; }
  }

  public interface IReply
  {
    DateTime DateCreated { get; set; }
    string Id { get; set; }
    double TrendScore { get; set; }
  }
}
