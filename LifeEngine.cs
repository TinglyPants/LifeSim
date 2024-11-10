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

        private Kernel _outerKernel = new Kernel("kernelOuter.png");
        private Kernel _innerKernel = new Kernel("kernelInner.png");

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
            // Convolve and normalize outer kernel
            float outerKernelResult = 0;
            for (int j = -8; j < 9; j++){
                for (int i = -8; i < 9; i++)
                {
                    outerKernelResult += GetAliveness(MCT(x + i, _width - 1), MCT(y + j, _height - 1)) * _outerKernel.GetKernelValue(i, j);
                }
            }
            var outerKernelResultNormalized = outerKernelResult / _outerKernel.MaxValue;

            // Convolve and normalize inner kernel
            float innerKernelResult = 0;
            for (int j = -1; j < 2; j++){
                for (int i = -1; i < 2; i++)
                {
                    innerKernelResult += GetAliveness(MCT(x + i, _width - 1), MCT(y + j, _height - 1)) * _innerKernel.GetKernelValue(i, j);
                }
            }
            var innerKernelResultNormalized = innerKernelResult / _innerKernel.MaxValue;

            return GetGrowth(innerKernelResultNormalized, outerKernelResultNormalized);
        }

        private float GetGrowth(float innerKernel, float outerKernel)
        {   
            if (innerKernel >= 0.5){
                if (outerKernel >= 0.26 && outerKernel <= 0.46)
                {
                    return 1f;
                }
                else
                {
                    return 0f;
                }
            }
            else if (innerKernel < 0.5){
                if (outerKernel >= 0.27 && outerKernel <= 0.36)
                {
                    return 1f;
                }
                else
                {
                    return 0f;
                }
            }
            else 
            {
                return 0f;
            }
        }

        public void RandomizeGame()
        {
            var rand = new Random();

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _prevGeneration[y, x] = rand.Next(0, 2);
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