using OpenTK.Mathematics;
using System;

struct Button
{
  public Vector2 Position;
  public Vector2 Size;
  public string Label;
  public Action OnClick;
}
