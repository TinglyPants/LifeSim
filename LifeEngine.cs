using System;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace LifeSim;
public class LifeEngine
    {
        private SKBitmap _bitmapPrev;
        private SKBitmap _bitmapNext;
        private readonly int _width;
        private readonly int _height;
        private readonly SKColor _white = new SKColor(255, 255, 255);
        private readonly SKColor _black = new SKColor(0, 0, 0);

        public LifeEngine(int width, int height)
        {   
            _bitmapPrev = new SKBitmap(width, height, true);
            _bitmapNext = new SKBitmap(width, height, true);
            _width = width;
            _height = height;

            // default life start state
            _bitmapPrev.Erase(_black);
            _bitmapNext.Erase(_black);

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

        private bool CheckIsAliveNext(SKBitmap bitmap, int x, int y)
        {
            SKColor self = bitmap.GetPixel(x, y);

            int aliveNeighbors = 0;
            for (int j = -1; j < 2; j++)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (i == 0 && j == 0)
                    {
                        // skip self
                        continue;
                    }
                    SKColor neighborCell = bitmap.GetPixel(MCT(x + i, _width-1), MCT(y + j, _height-1));
                    if (neighborCell == _white)
                    {
                        aliveNeighbors += 1;
                    }
                }
            }

            if (self == _white)
            {
                if (aliveNeighbors == 2 || aliveNeighbors == 3)
                {
                    return true;
                }
                else return false;
            }

            else
            {
                if (aliveNeighbors == 3)
                {
                    return true;
                }
                else return false;
            }
        }

        public void RandomizeGame()
        {
            var rand = new Random();

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    var index = rand.Next(0, 2);
                    _bitmapPrev.SetPixel(x, y, new[] {_white, _black}[index]);
                }
            }
        }

        public void Update()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {   
                    if (CheckIsAliveNext(_bitmapPrev, x, y) == true)
                    {
                        _bitmapNext.SetPixel(x, y, _white);
                    }
                    else {
                        _bitmapNext.SetPixel(x, y, _black);
                    }
                }
            }

            // shift frames back one ready for next update
            _bitmapNext.CopyTo(_bitmapPrev);
            _bitmapNext.Erase(_black);
        }
        
        public SKBitmap GetBitmap(int renderScale)
        {
            SKBitmap outputBitmap = new SKBitmap(_width*renderScale, _height*renderScale, true);

            SKFilterQuality quality = SKFilterQuality.None; // remains pixelated.
            _bitmapPrev.ScalePixels(outputBitmap, quality); // modifies outputBitmap.

            return outputBitmap;
        }
    }