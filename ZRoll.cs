// Opening/Ending Roll for HHSAdvSDL

using System;
using System.Text;
using SDL2;

namespace HHSAdvSDL
{
    public class ZRoll
    {
        public IntPtr TextFont { get; set; } = IntPtr.Zero;
        public SDL.SDL_Color TextColor { get; set; } = new SDL.SDL_Color() { r = 192, g = 192, b = 192, a = 255 };
        public int PixelPerFrame { get; set; } = 1;
        private IntPtr renderer = IntPtr.Zero;
        private int winW, winH;
        public ZRoll(IntPtr r, int w, int h)
        {
            renderer = r;
            winW = w;
            winH = h;
        }

        public void Roll(string[] credits)
        {
            int th = 0;
            List<string> work = new List<string>();
            for (int i = 0; i < credits.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var ch in credits[i])
                {
                    sb.Append(ch);
                    SDL_ttf.TTF_SizeUTF8(TextFont, sb.ToString(), out int w, out _);
                    if (w > winW)
                    {
                        if (sb.Length > 1)
                        {
                            work.Add(sb.ToString(0, sb.Length - 1));
                            sb.Clear().Append(ch);
                        }
                    }
                }
                if (sb.Length > 0) work.Add(sb.ToString());
            }
            credits = work.ToArray();
            IntPtr[] textures = new IntPtr[credits.Length];
            SDL.SDL_Rect[] rects = new SDL.SDL_Rect[credits.Length];
            for (int i = 0; i < credits.Length; i++)
            {
                IntPtr surface = SDL_ttf.TTF_RenderUTF8_Blended(TextFont, credits[i], TextColor);
                textures[i] = SDL.SDL_CreateTextureFromSurface(renderer, surface);
                SDL.SDL_FreeSurface(surface);
                SDL.SDL_QueryTexture(textures[i], out _, out _, out int w, out int h);
                rects[i] = new SDL.SDL_Rect { x = (winW - w) / 2, y = winH + th, w = w, h = h };
                th += h + 10;
            }

            bool quit = false;
            int scrollOffset = 0;

            uint frameDelay = 1000 / 60; // 60fps
            uint frameStart;
            int frameTime;

            while (!quit)
            {
                frameStart = SDL.SDL_GetTicks();
                // 背景クリア
                SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
                SDL.SDL_RenderClear(renderer);

                // 各行をスクロールして描画
                for (int i = 0; i < credits.Length; i++)
                {
                    SDL.SDL_Rect dst = rects[i];
                    dst.y -= scrollOffset;
                    SDL.SDL_RenderCopy(renderer, textures[i], IntPtr.Zero, ref dst);
                }

                SDL.SDL_RenderPresent(renderer);

                scrollOffset += PixelPerFrame;

                // 全部スクロールし終えたら終了
                if (rects[credits.Length - 1].y - scrollOffset + rects[credits.Length - 1].h < 0)
                    quit = true;

                // フレームレート制御
                frameTime = (int)(SDL.SDL_GetTicks() - frameStart);
                if (frameDelay > frameTime)
                    SDL.SDL_Delay(frameDelay - (uint)frameTime);
            }

            // 後始末
            for (int i = 0; i < textures.Length; i++)
                SDL.SDL_DestroyTexture(textures[i]);

        }
    }
}