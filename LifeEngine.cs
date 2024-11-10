using System;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace LifeSim;
public class LifeEngine
    {
        private float[,] _prevGeneration;
        private float[,] _nextGeneration;
        private readonly int _width;
        private readonly int _height;

        public LifeEngine(int width, int height)
        {   
            _width = width;
            _height = height;
            _prevGeneration = new float[_height, _width];
            _nextGeneration = new float[_height, _width];
            RandomizeGame();
        }

        // Maps cartesian limited space to an equivalent toroidal space
        private int MCT(int value, int maximum)
        {   
            if (value < 0)
            {
                return maximum + value + 1;
            }
            else if (value > maximum)
            {
                return value - maximum - 1;
            }
            else
            {
                return value;
            }
        }

        private float GetAliveness(int x, int y){
            return _prevGeneration[y, x];
        }

        private float GetNextAliveState(int x, int y)
        {
            float totalAliveness = 0f;
            for (int j = -3; j < 4; j++){
                for (int i = -3; i < 4; i++)
                {
                    totalAliveness += GetAliveness(MCT(x + i, _width - 1), MCT(y + j, _height - 1));
                }
            }
            totalAliveness /= 49;

            return GetGrowth(totalAliveness);
        }

        private float GetGrowth(float neighborAliveness)
        {   
            if (neighborAliveness >= 5)
            {
                return 0f;
            }
            return (float) (Math.Cos((neighborAliveness * 2 * Math.PI / 2.5f) - Math.PI) + 1f) / 2f;
        }

        public void RandomizeGame()
        {
            var rand = new Random();

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _prevGeneration[y, x] = rand.NextSingle();
                }
            }
        }

        public void Update()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {   
                    _nextGeneration[y, x] = GetNextAliveState(x, y);
                }
            }

            // Shift contents of _nextGeneration into _prevGeneration
            Array.Copy(_nextGeneration, _prevGeneration, _nextGeneration.Length);
        }
        
        public SKBitmap GetBitmap(int renderScale)
        {
            SKBitmap outputBitmap = new SKBitmap(_width*renderScale, _height*renderScale, true);
            SKBitmap generationBitmap = new SKBitmap( _width, _height);
            
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {   
                    var aliveness = _prevGeneration[y, x];
                    var value = (byte)(aliveness * 255);
                    generationBitmap.SetPixel(x, y, new SKColor(value, value, value));
                }
            }

            generationBitmap.ScalePixels(outputBitmap, SKFilterQuality.None);

            return outputBitmap;
        }
    }