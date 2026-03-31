using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Bean.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bean.Noise
{
    public class PerlinNoise
    {
        private int[] _permutation;

        public float Scale = 0.01f;

        public Vector2 Offset = Vector2.Zero;

        public int Octaves = 6;
        public float Persistence = 0.45f;

        public float Frequency = 2f;
        public float Amplitude = 0.5f;

        public float StrechX = 1f;
        public float StrechY = 1f;


        public PerlinNoise(int seed)
        {
            System.Random random = new System.Random(seed);

            this._permutation = new int[512];

            int[] p = new int[256];

            for (int i = 0; i < 256; i++)
            {
                p[i] = i;
            }

            for (int i = 0; i < 256; i++)
            {
                int swapIndex = random.Next(256);
                
                int temp = p[i];
                p[i] = p[swapIndex];
                p[swapIndex] = temp; 
            }

            for (int i = 0; i < 512; i++)
            {
                this._permutation[i] = p[i % 256];
            }
        }

        private float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private float Lerp(float a, float b, float t)
        {
            return a + t * (b - a);
        }

        private float Gradient(int hash, float x, float y) 
        {
            int h = hash & 0xF;

            float u = (h < 8) ? x : y;
            float v = (h < 4) ? y : (h == 12 || h == 14 ? x : 0);

            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        public float Noise(float fx, float fy)
        {
            fx += this.Offset.X;
            fy += this.Offset.Y;

            fx *= this.Scale;
            fy *= this.Scale;

            // Find unit grid cell containing point. converts the float to an int and ensures it stays between 0-255
            int iX = (int)MathF.Floor(fx) & 255;
            int iY = (int)MathF.Floor(fy) & 255;

            // Find relative x, y of point in cell. Removes the int from the float so 2.7 becomes just .7
            fx -= MathF.Floor(fx);
            fy -= MathF.Floor(fy);

            // Compute fade curves for x, y. Smooths edges
            float u = Fade(fx);
            float v = Fade(fy);

            // Hash coordinates of the 4 corners. adding a one moves it to the other side of the axies so iY is the top and iY + 1 is the bottom
            int aa = this._permutation[iX + this._permutation[iY]];
            int ab = this._permutation[iX + this._permutation[iY + 1]];
            int ba = this._permutation[iX + 1 + this._permutation[iY]];
            int bb = this._permutation[iX + 1 + this._permutation[iY + 1]];

            // Blend results from 4 corners
            float lerp1 = Lerp(Gradient(aa, fx, fy), Gradient(ba, fx - 1, fy), u);
            float lerp2 = Lerp(Gradient(ab, fx, fy - 1), Gradient(bb, fx - 1, fy - 1), u);

            return Lerp(lerp1, lerp2, v);
        }

        public float FractalNoise(float x, float y)
        {
            float total = 0f;
            float frequency = this.Frequency;
            float amplitude = this.Amplitude;
            float maxValue = 0f;

            for (int i = 0; i < Octaves; i++)
            {
                float n = Noise(x * frequency * StrechX, y * frequency * StrechY);
                n = n * 2f - 1f; // shift from [0,1] to [-1,1] before summing
                total += n * amplitude;
                maxValue += amplitude;

                amplitude *= Persistence;
                frequency *= 2f;
            }

            float result = total / maxValue;
            return (result + 1f) / 2f; // back to [0,1]
        }


        public Texture2D GetTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(GraphicsManager.Instance.GraphicsDevice, width, height);
            Color[] colours = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float noise = FractalNoise(x, y);

                    Color colour = new Color(noise, noise, noise);

                    int index = x + y * width;
                    colours[index] = colour;
                }

            }

            texture.SetData(colours);
            return texture;
        }

    }
}