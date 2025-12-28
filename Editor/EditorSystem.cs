using System.Numerics;
using Revolt.Core;
using Revolt.Core.Input; 
using Revolt.Editor;
using Revolt.Editor.Systems;
using Revolt.Graphics;

namespace Revolt.Editor.Systems;

public class EditorSystem : EditorModule
{
    public override string Name => "Internal_UI_Core";
    public override int Priority => 900;

    private readonly CoreEngine _engine;
    private Vector2 _mousePos;
    private bool _isMouseDown;

    public EditorSystem(CoreEngine engine) => _engine = engine;

    public override void OnUpdate(double dt)
{
    var graphics = _engine.GetSystem<IGraphicsSystem>();
    var uiBatcher = _engine.GetSystem<UIBatchRendererSystem>();
    var input = _engine.GetSystem<IInputSystem>();

    // 1. Gambar Game Scene di area "Scene View"
    // Kita sisihkan 250px untuk sidebar kiri
    int sidebarWidth = 250;
    int sceneWidth = 800 - sidebarWidth; // Asumsi window 800px

    // Set Viewport agar rendering 3D masuk ke kotak scene saja
    graphics.SetViewport(sidebarWidth, 0, sceneWidth, 800);
    graphics.SetRenderMode(RenderMode.World3D);
    
    // (Render 3D otomatis berjalan di sistem lain seperti BatchRendererSystem)

    // 2. Gambar Editor UI (Overlay)
    // Kembalikan viewport ke full screen untuk UI
    graphics.SetViewport(0, 0, 800, 800);
    graphics.SetRenderMode(RenderMode.UI2D);
    
    uiBatcher.StartFrame();
    
    // Sidebar
    uiBatcher.DrawRect(0, 0, sidebarWidth, 800, new Vector4(0.1f, 0.1f, 0.1f, 1.0f));
    
    // Toolbar atas
    uiBatcher.DrawRect(sidebarWidth, 0, sceneWidth, 40, new Vector4(0.15f, 0.15f, 0.15f, 1.0f));

    if (Button("Play Game", sidebarWidth + 10, 5, 100, 30)) { /* Logic */ }

    uiBatcher.EndFrame();
}

    /// <summary>
    /// Helper untuk membuat tombol Instant Mode GUI (IMGUI)
    /// </summary>
    public bool Button(string label, float x, float y, float w, float h)
    {
        var uiBatcher = _engine.GetSystem<UIBatchRendererSystem>();

        // AABB Collision Check
        bool hovered = _mousePos.X >= x && _mousePos.X <= x + w &&
                       _mousePos.Y >= y && _mousePos.Y <= y + h;

        // Logic warna berdasarkan state
        Vector4 color;
        if (hovered)
        {
            // Biru saat diklik, Abu terang saat hover
            color = _isMouseDown ? new Vector4(0.1f, 0.45f, 0.8f, 1.0f) : new Vector4(0.35f, 0.35f, 0.35f, 1.0f);
        }
        else
        {
            // Warna default
            color = new Vector4(0.22f, 0.22f, 0.22f, 1.0f);
        }

        // Gambar fisik tombol
        uiBatcher.DrawRect(x, y, w, h, color);

        // Berikan feedback true hanya pada frame saat mouse dilepas atau ditekan (tergantung selera)
        // Di sini kita pakai "Is Down" agar terasa responsif
        return hovered && _isMouseDown;
    }
}