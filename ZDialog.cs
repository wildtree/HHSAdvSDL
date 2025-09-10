// Dialog class for HHSAdv SDL


using System;
using SDL2;

namespace HHSAdvSDL
{
    class ZModalDialog
    {
        public class Button
        {
            private bool visible = true;
            private SDL.SDL_Rect viewport = new SDL.SDL_Rect();
            private IntPtr renderer = IntPtr.Zero;
            public IntPtr TextFont { get; set; } = IntPtr.Zero;
            public bool Visible
            {
                get { return visible; }
                set { visible = value; }
            }
            public int Id { get; set; } = -1;
            public SDL.SDL_Rect Rect { get => viewport; }
            private string label = string.Empty;
            public string Label
            {
                get
                {
                    return label;
                }
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        Visible = false;
                    }
                    label = value;
                }
            }
            public Button(IntPtr r, int x, int y, int w, int h)
            {
                renderer = r;
                viewport.x = x;
                viewport.y = y;
                viewport.w = w;
                viewport.h = h;
            }
            public void Draw(int selectedIndex)
            {
                bool sel = Id == selectedIndex;
                byte r = (byte)(sel ? 100 : Visible ? 200 : 60);
                byte g = (byte)(sel ? 140 : Visible ? 200 : 60);
                byte b = (byte)(sel ? 200 : Visible ? 200 : 60);
                SDL.SDL_SetRenderDrawColor(renderer, r, g, b, 255);
                SDL.SDL_RenderFillRect(renderer, ref viewport);

                IntPtr surf = SDL_ttf.TTF_RenderUTF8_Blended(TextFont, Label, new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 });
                IntPtr tex = SDL.SDL_CreateTextureFromSurface(renderer, surf);
                SDL.SDL_QueryTexture(tex, out _, out _, out int tw, out int th);
                var dst = new SDL.SDL_Rect { x = viewport.x + (viewport.w - tw) / 2, y = viewport.y + (30 - th) / 2, w = tw, h = th };
                SDL.SDL_RenderCopy(renderer, tex, IntPtr.Zero, ref dst);
                SDL.SDL_DestroyTexture(tex);
                SDL.SDL_FreeSurface(surf);
            }

            public bool Clicked(int x, int y)
            {
                if (!Visible) return false;
                if (x >= viewport.x && x < viewport.x + viewport.w && y >= viewport.y && y < viewport.y + viewport.h)
                {
                    return true;
                }
                return false;
            }

            public bool Hover(int x, int y)
            {
                return Clicked(x, y);
            }
        }
        private bool visible = false;
        public bool Visible
        {
            get { return visible; }
            private set
            {
                visible = value;
                if (value)
                {
                    Draw();
                }
                else
                {
                    SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
                    SDL.SDL_RenderFillRect(renderer, ref viewport);
                    SDL.SDL_RenderPresent(renderer);
                }
            }
        }
        public int Id { get; set; } = -1;
        public string Message { get; private set; } = "";
        private string[] Labels { get; set; } = Array.Empty<string>();
        public Button[] Buttons { get; private set; } = Array.Empty<Button>();
        public int ResultIndex { get; private set; } = -1;
        private int selectedIndex = 0;

        private SDL.SDL_Rect viewport = new SDL.SDL_Rect();
        private IntPtr renderer = IntPtr.Zero;
        public IntPtr TextFont { get; set; }

        public ZModalDialog(IntPtr r, int x, int y, int w, int h)
        {
            viewport.x = x;
            viewport.y = y;
            viewport.w = w;
            viewport.h = h;
            renderer = r;
        }

        public void Open(string message, string[] labels)
        {
            Message = message;
            Labels = labels;
            ResultIndex = -1;
            Buttons = new Button[labels.Length];
            for (selectedIndex = 0; selectedIndex < labels.Length; selectedIndex++)
            {
                if (!string.IsNullOrEmpty(labels[selectedIndex]))
                {
                    break;
                }
            }
            Visible = true;
        }

        public bool HandleEvent(SDL.SDL_Event e, SDL.SDL_EventType et)
        {
            if (!Visible) return false;
            if (et == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN && e.button.button == SDL.SDL_BUTTON_LEFT)
            {
                for (int i = 0; i < Buttons.Length; i++)
                {
                    if (Buttons[i] != null && Buttons[i].Clicked(e.button.x, e.button.y))
                    {
                        ResultIndex = i;
                        Visible = false;
                        break;
                    }
                }
                return true;
            }
            if (et == SDL.SDL_EventType.SDL_MOUSEMOTION)
            {
                for (int i = 0; i < Buttons.Length; i++)
                {
                    if (Buttons[i] != null && Buttons[i].Hover(e.motion.x, e.motion.y))
                    {
                        selectedIndex = i;
                        break;
                    }
                }
                return true;
            }
            if (et == SDL.SDL_EventType.SDL_KEYDOWN)
            {
                if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_LEFT)
                {
                    while (true)
                    {
                        selectedIndex = (selectedIndex + Buttons.Length - 1) % Buttons.Length;
                        if (Buttons[selectedIndex].Visible) break;
                    }
                }
                else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_RIGHT)
                {
                    while (true)
                    {
                        selectedIndex = (selectedIndex + 1) % Buttons.Length;
                        if (Buttons[selectedIndex].Visible) break;
                    }
                }
                else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_RETURN)
                {
                    ResultIndex = selectedIndex;
                    Visible = false;
                }
                else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                {
                    ResultIndex = -1;
                    Visible = false;
                }
                else if (Buttons[0].Visible && e.key.keysym.sym == SDL.SDL_Keycode.SDLK_1)
                {
                    selectedIndex = 0;
                }
                else if (Buttons[1].Visible && e.key.keysym.sym == SDL.SDL_Keycode.SDLK_2)
                {
                    selectedIndex = 1;
                }
                else if (Buttons[2].Visible && e.key.keysym.sym == SDL.SDL_Keycode.SDLK_3)
                {
                    selectedIndex = 2;
                }
                SDL.SDL_FlushEvent(SDL.SDL_EventType.SDL_KEYDOWN);
                SDL.SDL_FlushEvent(SDL.SDL_EventType.SDL_TEXTINPUT);
                return true;
            }
            return false;
        }

        public void Draw()
        {
            SDL.SDL_SetRenderDrawBlendMode(renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 160);
            SDL.SDL_RenderFillRect(renderer, ref viewport);

            int dlgW = Math.Min(viewport.w - 80, 400);
            int dlgH = 140;
            int dlgX = (viewport.w - dlgW) / 2;
            int dlgY = (viewport.h - dlgH) / 2;
            var rcDlg = new SDL.SDL_Rect { x = dlgX, y = dlgY, w = dlgW, h = dlgH };
            SDL.SDL_SetRenderDrawColor(renderer, 50, 50, 60, 255);
            SDL.SDL_RenderFillRect(renderer, ref rcDlg);

            // メッセージ表示
            var lines = Message.Split('\n');
            int textY = dlgY + 10;
            foreach (var line in lines)
            {
                int maxWidth = dlgW - 20; // パディング分引く
                IntPtr surf = SDL_ttf.TTF_RenderUTF8_Blended_Wrapped(TextFont, line, new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 }, (uint)maxWidth);
                IntPtr tex = SDL.SDL_CreateTextureFromSurface(renderer, surf);
                SDL.SDL_QueryTexture(tex, out _, out _, out int w, out int h);
                var dst = new SDL.SDL_Rect { x = dlgX + 10, y = textY, w = w, h = h };
                SDL.SDL_RenderCopy(renderer, tex, IntPtr.Zero, ref dst);
                SDL.SDL_DestroyTexture(tex);
                SDL.SDL_FreeSurface(surf);
                textY += h + 4;
            }

            // ボタン表示（3つ想定）
            int btnSpacing = 10;
            int btnY = dlgY + dlgH - 40;
            int totalW = dlgW - btnSpacing; // 最初と最後のスペース分を引く
            int btnX = dlgX + (dlgW - totalW) / 2 + btnSpacing / 2;
            for (int i = 0; i < Buttons.Length; i++)
            {
                if (Buttons[i] == null)
                {
                    Buttons[i] = new Button(renderer, btnX, btnY, totalW / Buttons.Length - btnSpacing, 30);
                    Buttons[i].Label = Labels[i];
                    Buttons[i].TextFont = TextFont;
                    Buttons[i].Id = i;
                }
                Buttons[i].Draw(selectedIndex);
                btnX += Buttons[i].Rect.w + btnSpacing;
            }

        }
    }
}