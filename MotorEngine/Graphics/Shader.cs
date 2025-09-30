using OpenTK.Graphics.OpenGL4;

public class Shader
{
  public int Handle { get; private set; }

  public Shader(string vertexShaderSource, string fragmentShaderSource)
  {
    int vertexShader = GL.CreateShader(ShaderType.VertexShader);
    GL.ShaderSource(vertexShader, vertexShaderSource);
    GL.CompileShader(vertexShader);
    CheckShaderCompile(vertexShader);

    int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
    GL.ShaderSource(fragmentShader, fragmentShaderSource);
    GL.CompileShader(fragmentShader);
    CheckShaderCompile(fragmentShader);

    Handle = GL.CreateProgram();
    GL.AttachShader(Handle, vertexShader);
    GL.AttachShader(Handle, fragmentShader);
    GL.LinkProgram(Handle);

    GL.DetachShader(Handle, vertexShader);
    GL.DetachShader(Handle, fragmentShader);
    GL.DeleteShader(vertexShader);
    GL.DeleteShader(fragmentShader);
  }

  private void CheckShaderCompile(int shader)
  {
    GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
    if (success == 0)
    {
      string infoLog = GL.GetShaderInfoLog(shader);
      throw new System.Exception($"Erro ao compilar shader: {infoLog}");
    }
  }

  public void Use() => GL.UseProgram(Handle);
}
