// Graphics library for SDL2-CS
//

using SDL2;
using System;
using System.Collections.Generic;


namespace HHSAdvSDL
{
    public class Canvas
    {
        private class Point
        {
            public int x { get; set; }
            public int y { get; set; }
            public Point(int x, int y) { this.x = x; this.y = y; }
        }
        public IntPtr renderer { get; private set; }
        public IntPtr texture { get; private set; }
        private SDL.SDL_Rect viewport;


        private SDL.SDL_Color[] pixels = new SDL.SDL_Color[256 * 152];

        private readonly SDL.SDL_Color[] pallete = {
            new SDL.SDL_Color { r=0, g=0, b=0, a=255},       // 0 black
            new SDL.SDL_Color { r=0, g=0, b=255, a=255 },     // 1 blue
            new SDL.SDL_Color { r=255, g=0, b=0, a=255 },     // 2 red
            new SDL.SDL_Color { r=255, g=0, b=255, a=255 },   // 3 magenta
            new SDL.SDL_Color { r=0, g=255, b=0, a=255 },     // 4 green
            new SDL.SDL_Color { r=0, g=255, b=255, a=255 },   // 5 cyan
            new SDL.SDL_Color { r=255, g=255, b=0, a=255 },   // 6 yellow
            new SDL.SDL_Color { r=255, g=255, b=255, a=255 }, // 7 white
            new SDL.SDL_Color { r=192, g=192, b=192, a=255 }, // 8 light gray
            new SDL.SDL_Color { r=0, g=0, b=128, a=255 }, // 9 dark blue
            new SDL.SDL_Color { r=128, g=0, b=0, a=255 }, // 10 dark red
            new SDL.SDL_Color { r=128, g=0, b=128, a=255 }, // 11 dark magenta
            new SDL.SDL_Color { r=0, g=128, b=0, a=255 }, // 12 dark green
            new SDL.SDL_Color { r=0, g=128, b=128, a=255 }, // 13 dark cyan
            new SDL.SDL_Color { r=128, g=128, b=0, a=255 }, // 14 dark yellow
            new SDL.SDL_Color { r=128, g=128, b=128, a=255 }  // 15 dark gray
        };

        private static readonly float[] blueFilter = {
            0.0F, 0.0F, 0.1F,
            0.0F, 0.0F, 0.2F,
            0.0F, 0.0F, 0.7F,
        };

        private readonly float[] redFilter = {
            0.7F, 0.0F, 0.0F,
            0.2F, 0.0F, 0.0F,
            0.1F, 0.0F, 0.0F,
        };

        private readonly float[] sepiaFilter = {
            0.269021F, 0.527950F, 0.103030F,
            0.209238F, 0.410628F, 0.080135F,
            0.119565F, 0.234644F, 0.045791F,
        };


        public Canvas(IntPtr r)
        {
            renderer = r;
            if (renderer == IntPtr.Zero) throw new Exception("Failed to create renderer");
            SDL.SDL_SetRenderDrawBlendMode(renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
            viewport = new SDL.SDL_Rect { x = 32, y = 44, w = 256, h = 152 };
            texture = SDL.SDL_CreateTexture(renderer, SDL.SDL_PIXELFORMAT_ARGB8888,
                                          (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                                          viewport.w, viewport.h);
        }
        public void Cls(SDL.SDL_Color c)
        {
            //SDL.SDL_SetRenderDrawColor(renderer, c.r, c.g, c.b, c.a);
            //SDL.SDL_RenderClear(renderer);
            for (int i = 0; i < pixels.Length; i++) pixels[i] = c;
            this.Draw();
        }

        private int GetColorIndex(SDL.SDL_Color c) =>
            ((c.b == 255) ? 1 : 0) + ((c.r == 255) ? 2 : 0) + ((c.g == 255) ? 4 : 0);
        public void Draw()
        {
            SDL.SDL_LockTexture(texture, IntPtr.Zero, out IntPtr vbuf, out int pitch);
            unsafe
            {
                for (int y = 0; y < viewport.h; y++)
                {
                    byte* row = (byte*)vbuf + y * pitch;
                    for (int x = 0; x < viewport.w; x++)
                    {
                        row[x * 4 + 0] = pixels[y * viewport.w + x].b;
                        row[x * 4 + 1] = pixels[y * viewport.w + x].g;
                        row[x * 4 + 2] = pixels[y * viewport.w + x].r;
                        row[x * 4 + 3] = pixels[y * viewport.w + x].a;
                    }
                }
            }
            SDL.SDL_UnlockTexture(texture);
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref viewport);
            //SDL.SDL_RenderPresent(renderer);
        }
        public void Cls()
        {
            Cls(new SDL.SDL_Color { r = 0, g = 0, b = 0, a = 255 });
        }
        public void Invalidate() => SDL.SDL_RenderPresent(renderer);

        public void pset(int x, int y, SDL.SDL_Color c)
        {
            if (x >= viewport.w || y >= viewport.h || x < 0 || y < 0) return;
            SDL.SDL_SetRenderDrawColor(renderer, c.r, c.g, c.b, c.a);
            SDL.SDL_RenderDrawPoint(renderer, viewport.x + x, viewport.y + y);
            pixels[y * viewport.w + x] = c;
        }
        public void pset(int x, int y, int col) => pset(x, y, pallete[col]);

        public SDL.SDL_Color pget(int x, int y)
        {
            if (x >= viewport.w || y >= viewport.h || x < 0 || y < 0) return new SDL.SDL_Color { r = 0, g = 0, b = 0, a = 255 };
            return pixels[y * viewport.w + x];
        }
        public void line(int x1, int y1, int x2, int y2, SDL.SDL_Color col)
        {
            int dx, ddx, dy, ddy;
            int wx, wy;
            int x, y;
            dy = y2 - y1;
            ddy = 1;
            if (dy < 0)
            {
                dy = -dy;
                ddy = -1;
            }
            wy = dy / 2;
            dx = x2 - x1;
            ddx = 1;
            if (dx < 0)
            {
                dx = -dx;
                ddx = -1;
            }
            wx = dx / 2;
            if (dx == 0 && dy == 0)
            {
                pset(x1, y1, col);
                return;
            }
            if (dy == 0)
            {
                for (x = x1; x != x2; x += ddx) pset(x, y1, col);
                pset(x2, y1, col);
                return;
            }
            if (dx == 0)
            {
                for (y = y1; y != y2; y += ddy) pset(x1, y, col);
                pset(x1, y2, col);
                return;
            }
            pset(x1, y1, col);
            if (dx > dy)
            {
                y = y1;
                for (x = x1; x != x2; x += ddx)
                {
                    pset(x, y, col);
                    wx -= dy;
                    if (wx < 0)
                    {
                        wx += dx;
                        y += ddy;
                    }
                }
            }
            else
            {
                x = x1;
                for (y = y1; y != y2; y += ddy)
                {
                    pset(x, y, col);
                    wy -= dx;
                    if (wy < 0)
                    {
                        wy += dy;
                        x += ddx;
                    }
                }
            }
            pset(x2, y2, col);
        }

        public void line(int x1, int y1, int x2, int y2, int col) => line(x1, y1, x2, y2, pallete[col]);

        public void paint(int x, int y, SDL.SDL_Color f, SDL.SDL_Color b)
        {
            int l, r;
            int wx;
            Queue<Point> q = new Queue<Point>();
            SDL.SDL_Color c = pget(x, y);
            if (c.Equals(f) || c.Equals(b))
            {
                return;
            }
            q.Enqueue(new Point(x, y));
            while (q.Count > 0)
            {
                Point p = q.Dequeue();
                c = pget(p.x, p.y);
                if (c.Equals(f) || c.Equals(b)) continue;
                for (l = p.x - 1; l >= 0; l--)
                {
                    c = pget(l, p.y);
                    if (c.Equals(f) || c.Equals(b)) break;
                }
                ++l;
                for (r = p.x + 1; r < viewport.w; r++)
                {
                    c = pget(r, p.y);
                    if (c.Equals(f) || c.Equals(b)) break;
                }
                --r;
                line(l, p.y, r, p.y, f);
                for (wx = l; wx <= r; wx++)
                {
                    int uy = p.y - 1;
                    if (uy >= 0)
                    {
                        c = pget(wx, uy);
                        if (!c.Equals(f) && !c.Equals(b))
                        {
                            if (wx == r)
                            {
                                q.Enqueue(new Point(wx, uy));
                            }
                            else
                            {
                                c = pget(wx + 1, uy);
                                if (c.Equals(f) || c.Equals(b)) q.Enqueue(new Point(wx, uy));
                            }
                        }
                    }
                    int ly = p.y + 1;
                    if (ly < viewport.h)
                    {
                        c = pget(wx, ly);
                        if (!c.Equals(f) && !c.Equals(b))
                        {
                            if (wx == r)
                            {
                                q.Enqueue(new Point(wx, ly));
                            }
                            else
                            {
                                c = pget(wx + 1, ly);
                                if (c.Equals(f) || c.Equals(b)) q.Enqueue(new Point(wx, ly));
                            }
                        }
                    }
                }
            }
        }
        public void paint(int x, int y, int fc, int bc)
        {
            paint(x, y, pallete[fc], pallete[bc]);
        }
        public void tonePaint(byte[] tone, bool tiling = false)
        {
            SDL.SDL_Color[] pat = {
                new SDL.SDL_Color { r=0, g=0, b=0, a=255},       // 0 black
                new SDL.SDL_Color { r=0, g=0, b=255, a=255 },     // 1 blue
                new SDL.SDL_Color { r=255, g=0, b=0, a=255 },     // 2 red
                new SDL.SDL_Color { r=255, g=0, b=255, a=255 },   // 3 magenta
                new SDL.SDL_Color { r=0, g=255, b=0, a=255 },     // 4 green
                new SDL.SDL_Color { r=0, g=255, b=255, a=255 },   // 5 cyan
                new SDL.SDL_Color { r=255, g=255, b=0, a=255 },   // 6 yellow
                new SDL.SDL_Color { r=255, g=255, b=255, a=255 }, // 7 white
            };
            SDL.SDL_Color[] col = new SDL.SDL_Color[pat.Length];
            Array.Copy(pat, col, pat.Length);
            int p = 0;
            int n = (int)tone[p++];
            for (int i = 1; i <= n; i++)
            {
                pat[i].b = tone[p++];
                pat[i].r = tone[p++];
                pat[i].g = tone[p++];
                pat[i].a = 255;
                col[i].r = 0;
                col[i].g = 0;
                col[i].b = 0;
                col[i].a = 255;
                for (int bit = 0; bit < 8; bit++)
                {
                    byte mask = (byte)(1 << bit);
                    if ((pat[i].r & mask) != 0) col[i].r++;
                    if ((pat[i].g & mask) != 0) col[i].g++;
                    if ((pat[i].b & mask) != 0) col[i].b++;
                }
                if (col[i].r > 0) col[i].r = (byte)(col[i].r * 32 - 1);
                if (col[i].g > 0) col[i].g = (byte)(col[i].g * 32 - 1);
                if (col[i].b > 0) col[i].b = (byte)(col[i].b * 32 - 1);
            }
            for (int wy = 0; wy < viewport.h; wy++)
            {
                for (int wx = 0; wx < viewport.w; wx++)
                {
                    SDL.SDL_Color c = pget(wx, wy);
                    int ci = GetColorIndex(c);
                    if (ci > 0 && ci <= n)
                    {
                        SDL.SDL_Color nc = col[ci];
                        if (tiling)
                        {
                            byte bits = (byte)(7 - wx % 8);
                            if (((pat[ci].r >> bits) & 1) != 0) nc.r = 255; else nc.r = 0;
                            if (((pat[ci].g >> bits) & 1) != 0) nc.g = 255; else nc.g = 0;
                            if (((pat[ci].b >> bits) & 1) != 0) nc.b = 255; else nc.b = 0;
                        }
                        pset(wx, wy, nc);
                    }
                }
            }
        }
        public enum FilterType
        {
            None,
            Blue,
            Red,
            Sepia
        };

        public FilterType ColorFilterType { get; set; } = FilterType.None;
        public void colorFilter()
        {
            float[] f;
            switch (ColorFilterType)
            {
                case FilterType.Blue:
                    f = blueFilter;
                    break;
                case FilterType.Red:
                    f = redFilter;
                    break;
                case FilterType.Sepia:
                    f = sepiaFilter;
                    break;
                default:
                    return;
            }
            SDL.SDL_LockTexture(texture, IntPtr.Zero, out IntPtr vbuf, out int pitch);
            unsafe
            {
                for (int y = 0; y < viewport.h; y++)
                {
                    byte* row = (byte*)vbuf + y * pitch;
                    for (int x = 0; x < viewport.w; x++)
                    {
                        int o = y * viewport.w + x;
                        pixels[o] = applyFilter(pixels[o], f);
                        row[x * 4 + 0] = pixels[o].b;
                        row[x * 4 + 1] = pixels[o].g;
                        row[x * 4 + 2] = pixels[o].r;
                        row[x * 4 + 3] = pixels[o].a;
                    }
                }
            }
            SDL.SDL_UnlockTexture(texture);
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref viewport);
        }
        private SDL.SDL_Color applyFilter(SDL.SDL_Color c, float[] f)
        {
            int r = (int)(c.r * f[0] + c.g * f[1] + c.b * f[2]);
            int g = (int)(c.r * f[3] + c.g * f[4] + c.b * f[5]);
            int b = (int)(c.r * f[6] + c.g * f[7] + c.b * f[8]);
            if (r > 255) r = 255;
            if (g > 255) g = 255;
            if (b > 255) b = 255;
            return new SDL.SDL_Color { r = (byte)r, g = (byte)g, b = (byte)b, a = c.a };
        }
        public void drawRect(int x0, int y0, int x1, int y1, SDL.SDL_Color c)
        {
            line(x0, y0, x1, y0, c);
            line(x1, y0, x1, y1, c);
            line(x0, y1, x1, y1, c);
            line(x0, y1, x0, y0, c);
        }

        public void fillRect(int x0, int y0, int x1, int y1, SDL.SDL_Color c)
        {
            if (y0 > y1)
            {
                int y = y0;
                y0 = y1;
                y1 = y;
            }
            for (int y = y0; y <= y1; y++)
            {
                line(x0, y, x1, y, c);
            }
        }
        
        public SDL.SDL_Color GetPalleteColor(int i) => pallete[i];
    }
}