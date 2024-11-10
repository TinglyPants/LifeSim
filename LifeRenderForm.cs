using System;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace LifeSim;

public class LifeRenderForm : Form
{
    private readonly SKControl skControl;
    private readonly System.Windows.Forms.Timer renderTimer;

    private LifeEngine _lifeEngine;
    private readonly int _scale;

    public LifeRenderForm()
    {
        // Initialize SKControl and add it to the form
        skControl = new SKControl
        {
            Dock = DockStyle.Fill
        };
        skControl.PaintSurface += OnPaintSurface;
        Controls.Add(skControl);

        // Set form properties
        Text = "Life Sim";
        Width = 800;
        Height = 800;

        _scale = 4;

        _lifeEngine = new LifeEngine(Width/_scale, Height/_scale);

        renderTimer = new System.Windows.Forms.Timer();
        renderTimer.Interval = 500; // Set interval to 16ms for ~60 FPS
        renderTimer.Tick += (sender, e) =>
        {   
            skControl.Invalidate(); // Triggers the PaintSurface event
        };
        renderTimer.Start();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        SKBitmap bitmap = _lifeEngine.GetBitmap(_scale);
        canvas.DrawBitmap(bitmap, new SKPoint(0, 0));
        _lifeEngine.Update();
    }
}