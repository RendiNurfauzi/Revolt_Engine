public class DiagnosticsSystem : GameModule
{
    public override string Name => "Diagnostics";
    public override int Priority => 0; // Paling awal

    private int _frameCount = 0;
    private double _elapsedTime = 0;

    public override void OnUpdate(double dt)
    {
        _frameCount++;
        _elapsedTime += dt;

        // Tampilkan info setiap 2 detik agar tidak spamming
        if (_elapsedTime >= 2.0)
        {
            double fps = _frameCount / _elapsedTime;
            Console.WriteLine($"[Revolts Core] Heartbeat: {fps:F0} UPS | Delta: {dt:F4}s");
            _frameCount = 0;
            _elapsedTime = 0;
        }
    }
}