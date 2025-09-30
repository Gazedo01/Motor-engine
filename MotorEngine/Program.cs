using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using static Button; // ou apenas use `Button` normalmente


class Program
{
  static void Main()
  {
    var nativeSettings = new NativeWindowSettings()
    {
      Size = new Vector2i(800, 600),
      Title = "Minha Engine 3D"
    };

    using var game = new GameWindow(GameWindowSettings.Default, nativeSettings);
    var camera = new PlayerCamera();

    // Cursor inicial: travado e invisível
    game.CursorState = CursorState.Grabbed;

    // Para calcular offset do mouse
    bool firstMove = true;
    Vector2 lastPos = new Vector2();

    // Sensibilidade do mouse
    float mouseSensitivity = 0.2f;

    // Configuração inicial
    game.Load += () =>
    {
      GL.Enable(EnableCap.DepthTest);
      GL.ClearColor(Color4.Black);
      Renderer.Init();

      Renderer.DrawButton((1, 1), (1, 5), new Color4(0.2f, 0.2f, 0.7f, 1f));
      ;

      // Atualização da lógica
      game.UpdateFrame += (FrameEventArgs args) =>
      {
        var keyboard = game.KeyboardState;
        float deltaTime = (float)args.Time;

        // Movimentação da câmera
        if (keyboard.IsKeyDown(Keys.W)) camera.ProcessKeyboard("FORWARD", deltaTime);
        if (keyboard.IsKeyDown(Keys.S)) camera.ProcessKeyboard("BACKWARD", deltaTime);
        if (keyboard.IsKeyDown(Keys.A)) camera.ProcessKeyboard("LEFT", deltaTime);
        if (keyboard.IsKeyDown(Keys.D)) camera.ProcessKeyboard("RIGHT", deltaTime);

        // Alternar cursor travado/normal com ESC
        if (keyboard.IsKeyPressed(Keys.Escape))
        {
          if (game.CursorState == CursorState.Grabbed)
            game.CursorState = CursorState.Normal;
          else
            game.CursorState = CursorState.Grabbed;
        }
      };

      // Movimento do mouse
      game.MouseMove += (MouseMoveEventArgs e) =>
      {
        if (game.CursorState != CursorState.Grabbed) return; // só processa quando travado

        if (firstMove)
        {
          lastPos = e.Position;
          firstMove = false;
          return;
        }

        float deltaX = (float)(e.Position.X - lastPos.X) * mouseSensitivity;
        float deltaY = (float)(e.Position.Y - lastPos.Y) * mouseSensitivity;
        lastPos = e.Position;

        camera.ProcessMouse(deltaX, deltaY);
      };

      // Renderização
      double time = 0.0;
      int frames = 0;

      game.RenderFrame += (FrameEventArgs args) =>
      {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // FPS no título
        time += args.Time;
        frames++;
        if (time >= 1.0)
        {
          game.Title = $"Minha Engine 3D - FPS: {frames}";
          frames = 0;
          time = 0.0;
        }

        // Matrizes
        var projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f),
                800f / 600f,
                0.1f,
                100f
            );

        var view = camera.GetViewMatrix();
        var model = Matrix4.Identity;
        var mvp = model * view * projection;

        // Desenhar objetos
        Renderer.DrawCube(Vector3.Zero, 1f, mvp);
        Renderer.DrawPyramid(new Vector3(2, 0, 0), 1f, 1.5f, mvp);
        Renderer.DrawSphere(new Vector3(-2, 0, 0), 1f, 16, mvp);

        game.SwapBuffers();
      };

      game.Run();
    }
}
