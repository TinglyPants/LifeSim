using System;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace LifeSim;

class Kernel
{
    public int Width;
    public int Height;
    private float[,] _kernel;
    public float MaxValue = 0;

    public Kernel(string imagePath)
    {
        SKBitmap imageBitmap = LoadImage(imagePath);
        Width = imageBitmap.Width;
        Height = imageBitmap.Height;

        _kernel = new float[Height, Width];

        // convert pixel values to float kernel
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var pixel = imageBitmap.GetPixel(x, y);
                float value = pixel.Red + pixel.Green + pixel.Blue;
                value /= 765; // gets value of pixel from 0 to 1

                MaxValue += value;
                _kernel[y, x] = value;
            }
        }
    }

    public float GetKernelValue(int midpointOffsetX, int midpointOffsetY)
    {
        int midpointX = (int)((Width / 2) + 0.5f);
        int midpointY = (int)((Height / 2) + 0.5f);

        return _kernel[midpointOffsetY + midpointY, midpointOffsetX + midpointX];
    }

    static SKBitmap LoadImage(string imagePath)
    {
        // Read the image into a stream
        using (var stream = File.OpenRead(imagePath))
        {
            // Load the image into an SKBitmap object
            SKBitmap bitmap = SKBitmap.Decode(stream);
            return bitmap;
        }
    }
}