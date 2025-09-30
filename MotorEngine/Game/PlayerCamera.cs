using OpenTK.Mathematics;

public class PlayerCamera
{
  public Vector3 Position { get; private set; } = new Vector3(0, 1, 3); // posição inicial
  public float Yaw { get; private set; } = -90f; // olhando para frente (eixo -Z)
  public float Pitch { get; private set; } = 0f;
  public float Speed { get; set; } = 2.5f; // velocidade do jogador
  public float Sensitivity { get; set; } = 0.1f; // sensibilidade do mouse

  public Vector3 Front { get; private set; } = -Vector3.UnitZ;
  public Vector3 Up { get; private set; } = Vector3.UnitY;
  public Vector3 Right { get; private set; } = Vector3.UnitX;

  public Matrix4 GetViewMatrix()
  {
    return Matrix4.LookAt(Position, Position + Front, Up);
  }

  public void ProcessKeyboard(string direction, float deltaTime)
  {
    float velocity = Speed * deltaTime;
    if (direction == "FORWARD")
      Position += Front * velocity;
    if (direction == "BACKWARD")
      Position -= Front * velocity;
    if (direction == "LEFT")
      Position -= Right * velocity;
    if (direction == "RIGHT")
      Position += Right * velocity;
  }

  public void ProcessMouse(float xOffset, float yOffset)
  {
    xOffset *= Sensitivity;
    yOffset *= Sensitivity;

    Yaw += xOffset;
    Pitch -= yOffset; // invertido (mouse para cima = olhar para cima)

    // limitar pitch para não virar a câmera de ponta cabeça
    if (Pitch > 89.0f) Pitch = 89.0f;
    if (Pitch < -89.0f) Pitch = -89.0f;

    UpdateVectors();
  }

  private void UpdateVectors()
  {
    Vector3 front;
    front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
    front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
    front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
    Front = Vector3.Normalize(front);

    Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
    Up = Vector3.Normalize(Vector3.Cross(Right, Front));
  }
}
