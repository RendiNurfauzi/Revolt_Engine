using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Revolt.Core;
using Revolt.Graphics;
using Revolt.Engine.Resources;
using Revolt.Engine.Systems;
using System.Numerics;

namespace RevoltEngine.Graphics.OpenGL;

public class OpenGLSystem : GraphicsModule
{
    // --- IMPLEMENTASI PROPERTI INTERFACE ---
    public override string Name => "OpenGL_Backend";
    public override int Priority => 0;
    public override bool IsHeadless => false;
    public override string ApiName => "OpenGL 4.5";

    private IWindow? _window;
    private GL? _gl;
    private uint _vao, _vbo, _ebo;
    private uint _activeShaderProgram;

    private int _windowWidth = 800;
    private int _windowHeight = 600;

    private readonly CoreEngine _engine;
    public OpenGLSystem(CoreEngine engine) => _engine = engine;

    public override void OnAwake()
    {
        var options = WindowOptions.Default;
        options.Size = new Silk.NET.Maths.Vector2D<int>(_windowWidth, _windowHeight);
        options.Title = "RevoltEngine - Core Integrated";

        _window = Window.Create(options);
        _window.Initialize();

        _window.Resize += (size) =>
        {
            _windowWidth = size.X;
            _windowHeight = size.Y;
            _gl?.Viewport(0, 0, (uint)_windowWidth, (uint)_windowHeight);
        };

        _gl = _window.CreateOpenGL();
        _gl!.Enable(EnableCap.DepthTest);
        _gl.DepthFunc(DepthFunction.Lequal);

        SetupBatchResources();
        LoadInitialResources();
    }

    private void LoadInitialResources()
    {
        var resSystem = _engine.GetSystem<ResourceSystem>();
        var shaderRes = resSystem.GetOrCreate("DefaultShader", () =>
        {
            uint handle = CreateShader(GetDefaultVertexSource(), GetDefaultFragmentSource());
            return new ShaderResource { Name = "DefaultShader", ProgramHandle = handle };
        });

        _activeShaderProgram = shaderRes.ProgramHandle;
        _gl!.UseProgram(_activeShaderProgram);

        SetRenderMode(RenderMode.World3D);
    }

    // --- IMPLEMENTASI METHOD INTERFACE (THE FIX) ---

    public override void ClearColor(float r, float g, float b, float a) => _gl?.ClearColor(r, g, b, a);

    public override void BeginFrame() => _gl?.Clear((uint)ClearBufferMask.ColorBufferBit | (uint)ClearBufferMask.DepthBufferBit);

    public override void EndFrame() => _window?.SwapBuffers();

    public override bool ShouldClose() => _window?.IsClosing ?? true;

    public override void SetShader(string shaderName)
    {
        // Untuk sekarang kita tetap pakai default, 
        // kedepannya bisa ambil dari ResourceSystem berdasarkan string name
    }

    public override void SetRenderMode(RenderMode mode)
    {
        if (_gl == null) return;

        if (mode == RenderMode.UI2D)
        {
            _gl.Disable(EnableCap.DepthTest);
            var projection = Matrix4x4.CreateOrthographicOffCenter(0, _windowWidth, _windowHeight, 0, -1, 1);
            var view = Matrix4x4.Identity;
            ApplyMatrices(projection, view);
        }
        else
        {
            _gl.Enable(EnableCap.DepthTest);
            float fov = 60.0f * (MathF.PI / 180.0f);
            float aspect = (float)_windowWidth / _windowHeight;
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(fov, aspect, 0.1f, 20000.0f);
            var view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 1000f), Vector3.Zero, Vector3.UnitY);
            ApplyMatrices(projection, view);
        }
    }

    private unsafe void ApplyMatrices(Matrix4x4 projection, Matrix4x4 view)
    {
        if (_gl == null) return;
        _gl.UseProgram(_activeShaderProgram);
        int projLoc = _gl.GetUniformLocation(_activeShaderProgram, "uProjection");
        if (projLoc != -1) _gl.UniformMatrix4(projLoc, 1, false, (float*)&projection);
        int viewLoc = _gl.GetUniformLocation(_activeShaderProgram, "uView");
        if (viewLoc != -1) _gl.UniformMatrix4(viewLoc, 1, false, (float*)&view);
    }

    public unsafe override void SubmitBatch(float[] vertexData, uint[] indexData, int vertexCount, int indexCount)
    {
        if (_gl == null || indexCount == 0) return;
        _gl.UseProgram(_activeShaderProgram);
        _gl.BindVertexArray(_vao);
        fixed (float* v = vertexData)
            _gl.BufferSubData(BufferTargetARB.ArrayBuffer, 0, (uint)(vertexCount * sizeof(float)), v);
        fixed (uint* i = indexData)
            _gl.BufferSubData(BufferTargetARB.ElementArrayBuffer, 0, (uint)(indexCount * sizeof(uint)), i);
        _gl.DrawElements(PrimitiveType.Triangles, (uint)indexCount, DrawElementsType.UnsignedInt, (void*)0);
    }

    // --- ENGINE CORE OVERRIDES ---
    public override void SetViewport(int x, int y, int width, int height)
{
    _gl?.Viewport(x, y, (uint)width, (uint)height);
    // Simpan ukuran viewport untuk kalkulasi matrix Aspect Ratio nanti
    _windowWidth = width; 
    _windowHeight = height;
}

    public override void OnUpdate(double dt)
    {
        _window?.DoEvents();
        BeginFrame(); // Setiap frame kita clear layarnya
    }
    public override void UpdateViewMatrix(Vector3 position, Vector3 target)
    {
        if (_gl == null) return;

        // Hitung matriks View (Mata Kamera)
        var view = Matrix4x4.CreateLookAt(position, target, Vector3.UnitY);

        unsafe
        {
            _gl.UseProgram(_activeShaderProgram);
            int loc = _gl.GetUniformLocation(_activeShaderProgram, "uView");
            if (loc != -1)
            {
                _gl.UniformMatrix4(loc, 1, false, (float*)&view);
            }
        }
    }

    public IWindow GetWindow() => _window!;

    // --- INTERNAL HELPERS ---

    private unsafe void SetupBatchResources()
    {
        _vao = _gl!.GenVertexArray();
        _vbo = _gl.GenBuffer();
        _ebo = _gl.GenBuffer();
        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        _gl.BufferData(BufferTargetARB.ArrayBuffer, 1024 * 1024 * 4, null, BufferUsageARB.DynamicDraw);
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        _gl.BufferData(BufferTargetARB.ElementArrayBuffer, 1024 * 1024, null, BufferUsageARB.DynamicDraw);
        uint stride = 7 * sizeof(float);
        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, (void*)0);
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, stride, (void*)(3 * sizeof(float)));
        _gl.EnableVertexAttribArray(1);
    }

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

    public override void OnShutdown()
    {
        _gl?.DeleteVertexArray(_vao);
        _gl?.DeleteBuffer(_vbo);
        _gl?.DeleteBuffer(_ebo);
        _gl?.Dispose();
        _window?.Dispose();
    }

    private string GetDefaultVertexSource() => @"#version 450 core
        layout (location = 0) in vec3 aPos;
        layout (location = 1) in vec4 aColor;
        out vec4 vColor;
        uniform mat4 uProjection;
        uniform mat4 uView;
        void main() { 
            gl_Position = uProjection * uView * vec4(aPos, 1.0); 
            vColor = aColor; 
        }";

    private string GetDefaultFragmentSource() => @"#version 450 core
        in vec4 vColor;
        out vec4 FragColor;
        void main() { FragColor = vColor; }";
}