using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

public static class Renderer
{
  private static Shader shader;
  private static int vao, vbo;

  private static string vertexShaderSrc = @"
        #version 330 core
        layout(location = 0) in vec3 aPosition;
        uniform mat4 uMVP;

        void main()
        {
            gl_Position = uMVP * vec4(aPosition, 1.0);
        }
    ";

  private static string fragmentShaderSrc = @"
        #version 330 core
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(1.0, 1.0, 1.0, 1.0);
        }
    ";

  public static void Init()
  {
    shader = new Shader(vertexShaderSrc, fragmentShaderSrc);

    vao = GL.GenVertexArray();
    vbo = GL.GenBuffer();
  }

  private static void Draw(float[] vertices, PrimitiveType type, Matrix4 mvp)
  {
    shader.Use();

    int loc = GL.GetUniformLocation(shader.Handle, "uMVP");
    GL.UniformMatrix4(loc, false, ref mvp);

    GL.BindVertexArray(vao);
    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

    GL.EnableVertexAttribArray(0);
    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

    GL.DrawArrays(type, 0, vertices.Length / 3);

    GL.BindVertexArray(0);
  }

  // 📦 Cubo
  public static void DrawCube(Vector3 center, float size, Matrix4 mvp)
  {
    float half = size / 2f;

    Vector3[] points =
    {
            new(center.X - half, center.Y - half, center.Z - half),
            new(center.X + half, center.Y - half, center.Z - half),
            new(center.X + half, center.Y + half, center.Z - half),
            new(center.X - half, center.Y + half, center.Z - half),

            new(center.X - half, center.Y - half, center.Z + half),
            new(center.X + half, center.Y - half, center.Z + half),
            new(center.X + half, center.Y + half, center.Z + half),
            new(center.X - half, center.Y + half, center.Z + half),
        };

    int[] edges =
    {
            0,1, 1,2, 2,3, 3,0,
            4,5, 5,6, 6,7, 7,4,
            0,4, 1,5, 2,6, 3,7
        };

    List<float> vertices = new List<float>();
    for (int i = 0; i < edges.Length; i += 2)
    {
      Vector3 p1 = points[edges[i]];
      Vector3 p2 = points[edges[i + 1]];
      vertices.AddRange(new float[] { p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z });
    }

    Draw(vertices.ToArray(), PrimitiveType.Lines, mvp);
  }

  // 🔺 Pirâmide
  public static void DrawPyramid(Vector3 center, float baseSize, float height, Matrix4 mvp)
  {
    float half = baseSize / 2f;

    Vector3 top = new(center.X, center.Y + height, center.Z);

    Vector3[] basePts =
    {
            new(center.X - half, center.Y, center.Z - half),
            new(center.X + half, center.Y, center.Z - half),
            new(center.X + half, center.Y, center.Z + half),
            new(center.X - half, center.Y, center.Z + half),
        };

    int[] edges =
    {
            0,1, 1,2, 2,3, 3,0, // base
            0,4, 1,4, 2,4, 3,4  // lados
        };

    List<float> vertices = new List<float>();
    for (int i = 0; i < 4; i++) // base
    {
      Vector3 p1 = basePts[i];
      Vector3 p2 = basePts[(i + 1) % 4];
      vertices.AddRange(new float[] { p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z });
    }
    for (int i = 0; i < 4; i++) // lados
    {
      Vector3 p1 = basePts[i];
      vertices.AddRange(new float[] { p1.X, p1.Y, p1.Z, top.X, top.Y, top.Z });
    }

    Draw(vertices.ToArray(), PrimitiveType.Lines, mvp);
  }

  // ⚽ Esfera em wireframe
  public static void DrawSphere(Vector3 center, float radius, int segments, Matrix4 mvp)
  {
    List<float> vertices = new List<float>();

    for (int i = 0; i < segments; i++)
    {
      float lat0 = MathF.PI * (-0.5f + (float)(i) / segments);
      float z0 = MathF.Sin(lat0);
      float zr0 = MathF.Cos(lat0);

      float lat1 = MathF.PI * (-0.5f + (float)(i + 1) / segments);
      float z1 = MathF.Sin(lat1);
      float zr1 = MathF.Cos(lat1);

      for (int j = 0; j <= segments; j++)
      {
        float lng = 2 * MathF.PI * (float)(j - 1) / segments;
        float x = MathF.Cos(lng);
        float y = MathF.Sin(lng);

        Vector3 p1 = new(center.X + radius * x * zr0, center.Y + radius * y * zr0, center.Z + radius * z0);
        Vector3 p2 = new(center.X + radius * x * zr1, center.Y + radius * y * zr1, center.Z + radius * z1);

        vertices.AddRange(new float[] { p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z });
      }
    }

    Draw(vertices.ToArray(), PrimitiveType.Lines, mvp);
  }

  // 🔲 Desenha um botão 2D na tela
  public static void DrawButton(Vector2 position, Vector2 size, Color4 color)
  {
    // Desenha o quadrado 2D usando OpenGL moderno (sem funções de matriz fixa)
    float x = position.X;
    float y = position.Y;
    float w = size.X;
    float h = size.Y;

    float[] vertices = {
        x,     y,     0f,
        x + w, y,     0f,
        x + w, y + h, 0f,
        x,     y + h, 0f
    };

    int quadVao = GL.GenVertexArray();
    int quadVbo = GL.GenBuffer();

    GL.BindVertexArray(quadVao);
    GL.BindBuffer(BufferTarget.ArrayBuffer, quadVbo);
    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
    GL.EnableVertexAttribArray(0);
    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

    // Use a simple shader for 2D color (you may need to adapt your shader to accept color)
    shader.Use();
    // Set color uniform if your shader supports it, otherwise set color in fragment shader

    Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, 800, 600, 0, -1, 1);
    int loc = GL.GetUniformLocation(shader.Handle, "uMVP");
    GL.UniformMatrix4(loc, false, ref ortho);

    GL.DrawArrays(PrimitiveType.Quads, 0, 4);

    GL.BindVertexArray(0);
    GL.DeleteBuffer(quadVbo);
    GL.DeleteVertexArray(quadVao);
  }

}
