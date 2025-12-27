using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Revolt.Core;
using Revolt.Graphics;
using Revolt.Engine.Resources; // Resource Types
using Revolt.Engine.Systems;  // ResourceSystem
using System.Numerics;

namespace RevoltEngine.Graphics.OpenGL;

public class OpenGLSystem : EngineModule, IGraphicsSystem
{
    public override string Name => "OpenGL_Backend";
    public override int Priority => 0; // Tetap 0 agar inisialisasi hardware paling awal
    public bool IsHeadless => false;
    public string ApiName => "OpenGL 4.5";

    private IWindow? _window;
    private GL? _gl;
    private uint _vao, _vbo, _ebo;
    private uint _activeShaderProgram;

    // Tambahkan referensi ke CoreEngine untuk ambil ResourceSystem
    private readonly CoreEngine _engine;
    public OpenGLSystem(CoreEngine engine) => _engine = engine;

    public override void OnAwake()
    {
        // 1. Setup Window
        var options = WindowOptions.Default;
        options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
        options.Title = "RevoltEngine - Resource Managed";
        
        _window = Window.Create(options);
        _window.Initialize();
        _gl = _window.CreateOpenGL();
        
        // 2. Setup Hardware Buffers
        SetupBatchResources();

        // 3. Setup Shaders via ResourceSystem (MENGGANTI InitDefaultShaders lama)
        LoadInitialResources();

        Console.WriteLine($"[Graphics] {ApiName} Initialized.");
    }

    private void LoadInitialResources()
    {
        var resSystem = _engine.GetSystem<ResourceSystem>();

        // Kita minta ResourceSystem untuk membuat/mengambil shader
        var shaderRes = resSystem.GetOrCreate("DefaultShader", () => 
        {
            // Pindahkan logika compile ke sini sebagai "Factory"
            uint handle = CreateShader(GetDefaultVertexSource(), GetDefaultFragmentSource());
            return new ShaderResource { Name = "DefaultShader", ProgramHandle = handle };
        });

        _activeShaderProgram = shaderRes.ProgramHandle;
        _gl!.UseProgram(_activeShaderProgram);

        // Set Projection Matrix
        UpdateProjectionMatrix();
    }

    private void UpdateProjectionMatrix()
    {
        var projection = Matrix4x4.CreateOrthographicOffCenter(0, 800, 600, 0, -1f, 1f);
        unsafe {
            int loc = _gl!.GetUniformLocation(_activeShaderProgram, "uProjection");
            if (loc != -1) _gl.UniformMatrix4(loc, 1, false, (float*)&projection);
        }
    }

    public IWindow GetWindow() => _window!;

    public override void OnUpdate(double dt)
    {
        _window?.DoEvents();
        BeginFrame();
    }

    public void BeginFrame() => _gl?.Clear((uint)ClearBufferMask.ColorBufferBit | (uint)ClearBufferMask.DepthBufferBit);
    public void EndFrame() => _window?.SwapBuffers();
    public bool ShouldClose() => _window?.IsClosing ?? true;

    // --- LOGIKA HELPER (TETAP ADA TAPI PRIVATE) ---

    private uint CreateShader(string vertexSource, string fragmentSource)
    {
        uint vertex = _gl!.CreateShader(ShaderType.VertexShader);
        _gl.ShaderSource(vertex, vertexSource);
        _gl.CompileShader(vertex);

        uint fragment = _gl.CreateShader(ShaderType.FragmentShader);
        _gl.ShaderSource(fragment, fragmentSource);
        _gl.CompileShader(fragment);

        uint program = _gl.CreateProgram();
        _gl.AttachShader(program, vertex);
        _gl.AttachShader(program, fragment);
        _gl.LinkProgram(program);

        _gl.DeleteShader(vertex);
        _gl.DeleteShader(fragment);
        return program;
    }

    private unsafe void SetupBatchResources()
    {
        _vao = _gl!.GenVertexArray();
        _vbo = _gl.GenBuffer();
        _ebo = _gl.GenBuffer();
        _gl.BindVertexArray(_vao);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        _gl.BufferData(BufferTargetARB.ArrayBuffer, 1024 * 1024, null, BufferUsageARB.DynamicDraw);
        
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        _gl.BufferData(BufferTargetARB.ElementArrayBuffer, 1024 * 1024, null, BufferUsageARB.DynamicDraw);

        uint stride = 7 * sizeof(float);
        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, (void*)0);
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, stride, (void*)(3 * sizeof(float)));
        _gl.EnableVertexAttribArray(1);
    }

    public unsafe void SubmitBatch(float[] vertexData, uint[] indexData, int vCount, int iCount)
    {
        if (_gl == null || iCount == 0) return;
        _gl.UseProgram(_activeShaderProgram);
        _gl.BindVertexArray(_vao);

        fixed (float* v = vertexData)
            _gl.BufferSubData(BufferTargetARB.ArrayBuffer, 0, (uint)(vCount * sizeof(float)), v);

        fixed (uint* i = indexData)
            _gl.BufferSubData(BufferTargetARB.ElementArrayBuffer, 0, (uint)(iCount * sizeof(uint)), i);

        _gl.DrawElements(PrimitiveType.Triangles, (uint)iCount, DrawElementsType.UnsignedInt, (void*)0);
    }

    public void ClearColor(float r, float g, float b, float a) => _gl?.ClearColor(r, g, b, a);
    public void SetShader(string name) { /* Kedepannya tinggal ambil dari ResourceSystem */ }

    public override void OnShutdown()
    {
        _gl?.DeleteVertexArray(_vao);
        _gl?.DeleteBuffer(_vbo);
        _gl?.DeleteBuffer(_ebo);
        // _activeShaderProgram tidak dihapus di sini, tapi di ResourceSystem.OnShutdown()!
        _gl?.Dispose();
        _window?.Dispose();
    }

    // Default Sources (Hanya fallback jika file tidak ditemukan)
    private string GetDefaultVertexSource() => @"#version 450 core
        layout (location = 0) in vec3 aPos;
        layout (location = 1) in vec4 aColor;
        out vec4 vColor;
        uniform mat4 uProjection;
        void main() { gl_Position = uProjection * vec4(aPos, 1.0); vColor = aColor; }";

    private string GetDefaultFragmentSource() => @"#version 450 core
        in vec4 vColor;
        out vec4 FragColor;
        void main() { FragColor = vColor; }";
}