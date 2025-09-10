// High high School Adventure SDL -- System 
using System;
using SDL2;
using System.Runtime.InteropServices;
using System.Text;
using System.Data;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace HHSAdvSDL
{
    public class ZSystem
    {
        private enum GameStatus { Title = 0, Play = 1, GameOver = 2 };
        private GameStatus status = GameStatus.Title;
        private IntPtr window = IntPtr.Zero;
        private IntPtr renderer = IntPtr.Zero;
        private IntPtr font = IntPtr.Zero;

        private const int PADDING = 6;
        private const int BITMAP_W = 320;
        private const int BITMAP_H = 240;
        private const int LOG_AREA_H = 100;

        private int winW, winH;
        private string dataFolder = string.Empty;
        private Canvas? canvas = null;
        private TextInputArea? inputArea = null;
        private ScrollView? logArea = null;
        private ZModalDialog? dialog = null;
        private ZMap? map = null;
        private ZObjects? obj = null;
        private ZWords? dict = null;
        private ZMessage? msgs = null;
        private ZRules? rules = null;
        private ZAudio? audio = null;

        private string[] credits = {
            @"High High School Adventure",
            @"",
            @"PalmOS version: hiro © 2002-2004",
            @"Android version: hiro © 2011-2025",
            @"M5 version: hiro © 2023-2025",
            @"Qt version: hiro © 2024",
            @"PicoCalc version: hiro © 2025",
            @"SDL version: hiro © 2025",
            @"",
            @"- Project ZOBPlus -",
            @"Hayami <hayami@zob.jp>",
            @"Exit <exit@zob.jp>",
            @"ezumi <ezumi@zob.jp>",
            @"Ogu <ogu@zob.jp>",
            @"neopara <neopara@zob.jp>",
            @"hiro <hiro@zob.jp>",
            @"",
            @"--- Original Staff ---",
            @"",
            @"- Director -",
            @"HIRONOBU NAKAGUCHI",
            @"",
            @"- Graphic Designers -",
            @"",
            @"NOBUKO YANAGITA",
            @"YUMIKO HOSONO",
            @"HIRONOBU NAKAGUCHI",
            @"TOSHIHIKO YANAGITA",
            @"TOHRU OHYAMA",
            @"",
            @"MASANORI ISHII",
            @"YASUSHI SHIGEHARA",
            @"HIDETOSHI SUZUKI",
            @"TATSUYA UCHIBORI",
            @"MASAKI NOZAWA",
            @"",
            @"TOMOKO OHKAWA",
            @"FUMIKAZU SHIRATSUCHI",
            @"YASUNORI YAMADA",
            @"MUNENORI TAKIMOTO",
            @"",
            @"- Message Converters -",
            @"TATSUYA UCHIBORI",
            @"HIDETOSHI SUZUKI",
            @"YASUSHI SHIGEHARA",
            @"YASUNORI YAMADA",
            @"",
            @"- Floppy Disk Converters -",
            @"HIRONOBU NAKAGUCHI",
            @"",
            @"- Music -",
            @"MASAO MIZOBE",
            @"",
            @"- Special Thanks To -",
            @"HIROSHI YAMAMOTO",
            @"TAKAYOSHI KASHIWAGI",
            @"",
            @"- Cooperate with -",
            @"Furniture KASHIWAGI",
            @"",
            @"ZAMA HIGH SCHOOL",
            @"MICRO COMPUTER CIRCLE",
        };

        private string[] opening = {
            @"ストーリー",
            @"",
            @"2019年神奈山県立ハイ高等学校は地盤が弱く校舎の老朽化も進んだため、とうとう廃校にする以外方法がなくなってしまった。",
            @"ところで大変な情報を手に入れた。",
            @"",
            @"それは、",
            @"",
            @"「ハイ高校にＡＴＯＭＩＣ ＢＯＭＢが仕掛けられている。」",
            @"",
            @"と、いうものだ。",
            @"",
            @"どうやらハイ高が廃校になった時、気が狂った理科の先生がＡＴＯＭＩＣ ＢＯＭＢを、学校のどこかに仕掛けてしまったらしい。",
            @"お願いだ。我が母校のコナゴナになった姿を見たくはない。",
            @"早くＡＴＯＭＩＣ ＢＯＭＢを取り除いてくれ……！！",
            @"",
            @"行動は英語で、",
            @"<動詞>",
            @"或いは、",
            @"<動詞>+<目的語>",
            @"のように入れていただきたい。",
            @"例えば、",
            @"look room",
            @"と入れれば部屋の様子を見ることが出来るという訳だ。",
            @"",
            @"それでは Ｇｏｏｄ Ｌｕｃｋ！！！............",
        };
        private ZSystem()
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO|SDL.SDL_INIT_AUDIO);
            if (SDL_ttf.TTF_Init() != 0)
            {
                Console.WriteLine(SDL.SDL_GetError());
                SDL.SDL_Quit();
                return;
            }

            font = SDL_ttf.TTF_OpenFont(@"C:\Windows\Fonts\YuGothR.ttc", 14);
            if (font == IntPtr.Zero)
            {
                Console.WriteLine(SDL.SDL_GetError());
                SDL_ttf.TTF_Quit();
                SDL.SDL_Quit();
                return;
            }
            SDL_ttf.TTF_SizeUTF8(font, "Mg", out _, out int lineHeight);
            int inputAreaH = lineHeight + PADDING * 2;
            int windowH = BITMAP_H + inputAreaH + LOG_AREA_H + PADDING;

            SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG);

            winW = BITMAP_W;
            winH = windowH;
            window = SDL.SDL_CreateWindow("ハイハイスクールアドベンチャー",
                                            SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
                                            BITMAP_W, windowH, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            if (window == IntPtr.Zero)
            {
                Console.WriteLine("Error creating window: " + SDL.SDL_GetError());
                throw new Exception("SDL_CreateWindow failed");
            }
            if (window == IntPtr.Zero)
            {
                Console.WriteLine("Error creating window: " + SDL.SDL_GetError());
                throw new Exception("SDL_CreateWindow failed");
            }
            renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine("Error creating renderer: " + SDL.SDL_GetError());
                SDL.SDL_DestroyWindow(window);
                throw new Exception("SDL_CreateRenderer failed");
            }

            canvas = new Canvas(renderer);
            inputArea = new TextInputArea(renderer, PADDING, BITMAP_H, BITMAP_W - 2 * PADDING, inputAreaH);
            logArea = new ScrollView(renderer, PADDING, BITMAP_H + inputAreaH, BITMAP_W - 2 * PADDING, LOG_AREA_H);
            inputArea.TextFont = font;
            inputArea.TextColor = canvas.GetPalleteColor(7);
            logArea.TextFont = font;
            logArea.TextColor = canvas.GetPalleteColor(5);
            dialog = new ZModalDialog(renderer, PADDING, 0, BITMAP_W - 2 * PADDING, BITMAP_H);
            dialog.TextFont = font;
            // data folder
            dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HHSAdvSDL");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            // load icon
            string iconFile = Path.Combine(dataFolder, @"icon.png");
            if (File.Exists(iconFile))
            {
                IntPtr iconSurface = SDL_image.IMG_Load(iconFile);
                if (iconSurface != IntPtr.Zero)
                {
                    SDL.SDL_SetWindowIcon(window, iconSurface);
                    SDL.SDL_FreeSurface(iconSurface);
                }
            }
            audio = new ZAudio(dataFolder);
            rules = new ZRules(Path.Combine(dataFolder, @"rule.dat"));
            map = new ZMap(Path.Combine(dataFolder, @"map.dat"));
            obj = new ZObjects(Path.Combine(dataFolder, @"thin.dat"));
            dict = new ZWords(Path.Combine(dataFolder, @"highds.com"));
            msgs = new ZMessage(Path.Combine(dataFolder, @"msg.dat"));
            ZUserData.Instance.load(Path.Combine(dataFolder, @"data.dat"));

        }
        private static ZSystem? instance = null;
        public static ZSystem Instance
        {
            get
            {
                if (instance == null) instance = new ZSystem();
                return instance;
            }
        }

        public bool Init()
        {
            if (canvas == null || logArea == null || inputArea == null) return false;
            canvas.Cls(canvas.GetPalleteColor(1));
            canvas.Invalidate();
            logArea.Draw();
            return true;
        }

        public bool Quit()
        {
            if (renderer != IntPtr.Zero) SDL.SDL_DestroyRenderer(renderer);
            if (window != IntPtr.Zero) SDL.SDL_DestroyWindow(window);
            if (font != IntPtr.Zero) SDL_ttf.TTF_CloseFont(font);
            SDL_ttf.TTF_Quit();
            SDL.SDL_Quit();
            return true;
        }
        private static string[] title = new string[] {
            @"ハイハイスクールアドベンチャー",
            @"Copyright(c)1995-2025",
            @"ZOBplus",
            @"hiro",
        };
        private void Title()
        {
            logArea.Clear();
            foreach (var s in title)
            {
                logArea.Add(s);
            }
            logArea.TextColor = new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 };
            inputArea.Visible = false;
            inputArea.HitAnyKey = true;
            map.Cursor = 76;
            canvas.ColorFilterType = Canvas.FilterType.None;
            map.Draw(canvas);
            logArea.Draw();
        }
        private void DrawScreen(bool with_msg)
        {
            IsDark();
            map.Draw(canvas);
            if (with_msg && status != GameStatus.GameOver)
            {
                string s = map.Message;
                if (map.IsBlank)
                {
                    logArea.Add(msgs.GetMessage(0xcc));
                }
                if (!string.IsNullOrEmpty(s))
                {
                    logArea.Add(s);
                }
            }
            ZUserData user = ZUserData.Instance;
            for (int i = 0; i < ZUserData.Items; i++)
            {
                if (user.getPlace(i) == map.Cursor)
                {
                    bool shift = false;
                    if (i == 1 && user.getFact(0) != 1)
                    {
                        shift = true;
                    }
                    obj.Id = i;
                    obj.Draw(canvas, shift);
                    if (with_msg && status != GameStatus.GameOver)
                    {
                        logArea.Add(msgs.GetMessage(0x96 + i));
                    }
                }
            }
            if (user.getFact(1) == map.Cursor)
            {
                obj.Id = 14;
                obj.Draw(canvas);
                if (with_msg && status != GameStatus.GameOver)
                {
                    logArea.Add(msgs.GetMessage(0xb4));
                }
            }
            canvas.colorFilter();
            canvas.Invalidate();
            inputArea.Draw();
            logArea.Draw();
            SDL.SDL_RenderPresent(renderer);
        }
        private void GameOver()
        {
            status = GameStatus.GameOver;
            inputArea.Visible = false;
            inputArea.HitAnyKey = true;
        }

        private bool GameEventHandler(SDL.SDL_Event e)
        {
            switch (e.type)
            {
                case SDL.SDL_EventType.SDL_KEYDOWN:
                    if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_RETURN)
                    {
                        string input = inputArea.InputText.Trim();
                        if (string.IsNullOrEmpty(input)) return false;
                        inputArea.InputText = string.Empty; // clear
                        StringBuilder log = new StringBuilder();
                        log.Append(">> ").Append(input);
                        logArea.Add(log.ToString());
                        string[] r = input.Split(' ');
                        ZCore.Instance.CmdId = (byte)dict.findVerb(r[0].Trim());
                        ZCore.Instance.ObjId = (byte)((r.Length > 1) ? dict.findObj(r[1].Trim()) : -1);
                        TimeElapsed();
                        if (status == GameStatus.GameOver) return true;
                        ExecRules();
                        if (status == GameStatus.GameOver) return true;
                        CheckTeacher();
                        DrawScreen(true);
                    }
                    break;
            }
            return false;
        }
        private void ExecRules()
        {
            bool okay = false;
            ZCore core = ZCore.Instance;
            ZUserData user = ZUserData.Instance;
            foreach (var rule in rules.Rules)
            {
                if (rule.Evaluate())
                {
                    map.Cursor = core.MapId;
                    ZCore.ZCommand c = new ZCore.ZCommand();
                    while ((c = core.pop()).Cmd != ZCore.ZCommand.Command.Nop)
                    {
                        byte o = c.Operand;
                        switch (c.Cmd)
                        {
                            case ZCore.ZCommand.Command.Nop:
                                break;
                            case ZCore.ZCommand.Command.Message:
                                string s = msgs.GetMessage(o);
                                if ((o & 0x80) == 0)
                                {
                                    s = map.Find(core.CmdId, core.ObjId);
                                }
                                logArea.Add(s);
                                break;
                            case ZCore.ZCommand.Command.Sound:
                                audio.Play(o);
                                break;
                            case ZCore.ZCommand.Command.Dialog:
                                switch (o)
                                {
                                    case 0: // boy or girl
                                        user.setFact(0, 1); // boy
                                        dialog.Id = 0;
                                        dialog.Open(msgs.GetMessage(0xe7), new string[] { "男子", "女子", string.Empty });
                                        break;
                                    case 1:
                                        dialog.Id = o;
                                        string[] labels = new string[] { "1", "2", "3" };
                                        if (core.CmdId != 0x0f)
                                        {
                                            int n = labels.Length;
                                            if (!File.Exists(Path.Combine(dataFolder, "1.dat")))
                                            {
                                                --n;
                                                labels[0] = string.Empty;
                                            }
                                            if (!File.Exists(Path.Combine(dataFolder, "2.dat")))
                                            {
                                                --n;
                                                labels[1] = string.Empty;
                                            }
                                            if (!File.Exists(Path.Combine(dataFolder, "3.dat")))
                                            {
                                                --n;
                                                labels[2] = string.Empty;
                                            }
                                            if (n == 0)
                                            {
                                                dialog.Id = -1;
                                                dialog.Open("セーブデータが存在していません。", new string[] { string.Empty, "OK", string.Empty });
                                                break;
                                            }
                                        }
                                        dialog.Open(msgs.GetMessage(0xe8), labels);
                                        break;
                                    case 2:
                                        dialog.Id = o;
                                        dialog.Open(user.getItemList(), new string[] { string.Empty, "OK", string.Empty });
                                        break;
                                    case 3:
                                        dialog.Id = o;
                                        dialog.Open(msgs.GetMessage(0xe9), new string[] { "黄", "赤", string.Empty });
                                        break;
                                }
                                break;
                            case ZCore.ZCommand.Command.GameOver:
                                switch (o)
                                {
                                    case 1: // teacher
                                        canvas.ColorFilterType = Canvas.FilterType.Sepia;
                                        DrawScreen(false);
                                        break;
                                    case 2: // explosion
                                        canvas.ColorFilterType = Canvas.FilterType.Red;
                                        DrawScreen(false);
                                        break;
                                    case 3: // clear
                                        audio.Play(3);
                                        ZRoll endroll = new ZRoll(renderer, winW, winH);
                                        endroll.TextFont = font;
                                        endroll.Roll(credits);
                                        DrawScreen(true);
                                        break;
                                }
                                GameOver();
                                break;
                        }
                    }
                    if (status == GameStatus.GameOver) return;
                    logArea.Add(msgs.GetMessage(0xed)); // Ｏ．Ｋ．
                    okay = true;
                    break;
                }
            }
            map.Cursor = core.MapId;
            if (!okay)
            {
                string s = map.Find(core.CmdId, core.ObjId);
                if (string.IsNullOrEmpty(s))
                {
                    s = msgs.GetMessage(0xec); // ダメ
                }
                logArea.Add(s);
            }
            if (map.Cursor == 74)
            {
                int msg_id = 0;
                user.setFact(13, (byte)(user.getFact(13) + 1));
                switch (user.getFact(13))
                {
                    case 4: msg_id = 0xe2; break;
                    case 6: msg_id = 0xe3; break;
                    case 10: msg_id = 0xe4; break;
                }
                if (msg_id != 0)
                {
                    logArea.Add(msgs.GetMessage(msg_id));
                }
            }
        }
        private void CheckTeacher()
        {
            ZUserData user = ZUserData.Instance;
            ZCore core = ZCore.Instance;
            if (status == GameStatus.GameOver || user.getFact(1) == core.MapId) return;
            int rd = 100 + core.MapId + ((user.getFact(1) > 0) ? 1000 : 0);
            int rz = new Random().Next(3000);
            user.setFact(1, (byte)((rd < rz) ? 0 : core.MapId));
            switch (core.MapId)
            {
                case 1:
                case 48:
                case 50:
                case 51:
                case 52:
                case 53:
                case 61:
                case 64:
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 83:
                case 86:
                    user.setFact(1, 0);
                break;
            }
        }
        private bool IsDark()
        {
            bool dim = false;
            ZCore core = ZCore.Instance;
            ZUserData user = ZUserData.Instance;
            switch (core.MapId)
            {
                case 47:
                case 48:
                case 49:
                case 61:
                case 64:
                case 65:
                case 67:
                case 68:
                case 69:
                case 71:
                case 74:
                case 75:
                case 77:
                    if (user.getFact(7) != 0)
                    {
                        if (user.getFact(6) != 0)
                        {
                            // dark mode. (color in blue)
                            canvas.ColorFilterType = Canvas.FilterType.Blue;
                            dim = true;
                        }
                    }
                    else
                    {
                        // bblack out
                        core.MapViewId = core.MapId;
                        map.Cursor = 84;
                    }
                    break;
                default:
                    if (user.getFact(6) != 0)
                    {
                        // color back to mormal (remove color filter)
                        canvas.ColorFilterType = Canvas.FilterType.None;
                        dim = false;
                    }
                    break;
                
            }
            return dim;
        }
        private void TimeElapsed()
        {
            ZUserData user = ZUserData.Instance;
            if (user.getFact(3) > 0 && user.getFact(7) == 1)
            {
                // Light is ON
                user.setFact(3, (byte)(user.getFact(3) - 1));
                if (user.getFact(3) < 8 && user.getFact(3) > 0)
                {
                    // battery LOW
                    user.setFact(6, 1); // dim mode
                    logArea.Add(msgs.GetMessage(0xd9));
                }
                else if (user.getFact(3) == 0)
                {
                    // battery ware out
                    user.setFact(7, 0); // light off
                    logArea.Add(msgs.GetMessage(0xc0));
                }
            }
            if (user.getFact(11) > 0)
            {
                user.setFact(11, (byte)(user.getFact(11) - 1));
                if (user.getFact(11) == 0)
                {
                    logArea.Add(msgs.GetMessage(0xd8));
                    if (user.getPlace(7) == 48)
                    {
                        user.getLink(75 - 1).N = 77;
                        user.getLink(68 - 1).W = 77;
                        logArea.Add(msgs.GetMessage(0xda));
                    }
                    else if (user.getPlace(7) == 255 || user.getPlace(7) == map.Cursor)
                    {
                        // suicide explosion
                        // set screen color to red
                        canvas.ColorFilterType = Canvas.FilterType.Red;
                        logArea.Add(msgs.GetMessage(0xcf));
                        logArea.Add(msgs.GetMessage(0xcb));
                        GameOver();
                    }
                    else
                    {
                        user.setPlace(7, 0);
                    }
                }
            }
        }
        private bool TitleEventHandler(SDL.SDL_Event e)
        {
            if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
            {
                if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_PAGEUP
                    || e.key.keysym.sym == SDL.SDL_Keycode.SDLK_PAGEDOWN
                    || e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                    return false;

                map.Cursor = 1;
                inputArea.InputText = string.Empty;
                inputArea.Visible = true;
                logArea.Clear();
                status = GameStatus.Play;
                SDL.SDL_FlushEvent(SDL.SDL_EventType.SDL_KEYDOWN);
                SDL.SDL_FlushEvent(SDL.SDL_EventType.SDL_TEXTINPUT);
                ZRoll openingRoll = new ZRoll(renderer, winW, winH);
                openingRoll.TextFont = font;
                openingRoll.Roll(opening);
                DrawScreen(true);
                return true;
            }
            return false;
        }
        private bool GameOverEventHandler(SDL.SDL_Event e)
        {
            if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
            {
                if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_PAGEUP
                    || e.key.keysym.sym == SDL.SDL_Keycode.SDLK_PAGEDOWN
                    || e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                    return false;

                ZUserData.Instance.load(Path.Combine(dataFolder, @"data.dat"));
                Title();
                status = GameStatus.Title;
                SDL.SDL_FlushEvent(SDL.SDL_EventType.SDL_KEYDOWN);
                SDL.SDL_FlushEvent(SDL.SDL_EventType.SDL_TEXTINPUT);
                return true;
            }
            return false;
        }
        private void SaveGame(int index)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(index).Append(".dat");
            string fileName = Path.Combine(dataFolder, sb.ToString());
            ZCore core = ZCore.Instance;
            ZUserData user = ZUserData.Instance;
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (var br = new BinaryWriter(fs))
                {
                    br.Write(core.pack());
                    br.Write(user.pack());
                }
            }
        }
        private void LoadGame(int index)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(index).Append(".dat");
            string fileName = Path.Combine(dataFolder, sb.ToString());
            ZCore core = ZCore.Instance;
            ZUserData user = ZUserData.Instance;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    core.unpack(br.ReadBytes(core.packedSize));
                    user.unpack(br.ReadBytes(user.packedSize));
                }
            }
            map.Cursor = core.MapId;
        }
        public void Loop()
        {
            bool quit = false;
            if (inputArea == null || logArea == null || canvas == null || dialog == null || map == null || obj == null || dict == null) return;
            inputArea.Draw();

            Title();
            SDL.SDL_StartTextInput();
            while (!quit)
            {
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
                {
                    var et = e.type;

                    if (et == SDL.SDL_EventType.SDL_QUIT) { quit = true; break; }

                    if (dialog.Visible)
                    {
                        if (dialog.HandleEvent(e, et))
                        {
                            if (!dialog.Visible)
                            {
                                if (dialog.ResultIndex >= 0)
                                {
                                    ZCore core = ZCore.Instance;
                                    ZUserData user = ZUserData.Instance;
                                    switch (dialog.Id)
                                    {
                                        case 0:
                                            user.setFact(0, (byte)(dialog.ResultIndex + 1));
                                            map.Cursor = 3;

                                            break;
                                        case 1:
                                            if (core.CmdId == 0x0f)
                                            {
                                                SaveGame(dialog.ResultIndex + 1);
                                            }
                                            else
                                            {
                                                LoadGame(dialog.ResultIndex + 1);
                                            }
                                            break;
                                        case 2:
                                            // nothing to do
                                            break;
                                        case 3:
                                            if (user.getPlace(11) != 0xff)
                                            {
                                                logArea.Add(msgs.GetMessage(0xe0));
                                            }
                                            if (dialog.ResultIndex == 0 || user.getPlace(11) != 0xff)
                                            {
                                                // Game Over
                                                canvas.ColorFilterType = Canvas.FilterType.Red;
                                                logArea.Add(msgs.GetMessage(0xc7));
                                                logArea.Add(msgs.GetMessage(0xee));
                                                GameOver();
                                                break;
                                            }
                                            user.setPlace(11, 0);
                                            map.Cursor = 74;
                                            //audio.Play(3);
                                            break;
                                    }
                                }
                                DrawScreen(true);
                            }
                            continue;
                        }
                    }
                    switch (status)
                    {
                        case GameStatus.Title:
                            if (TitleEventHandler(e)) continue;
                            break;
                        case GameStatus.Play:
                            if (GameEventHandler(e)) continue;
                            break;
                        case GameStatus.GameOver:
                            if (GameOverEventHandler(e)) continue;
                            break;
                    }

                    switch (et)
                    {
                        case SDL.SDL_EventType.SDL_TEXTINPUT:
                            unsafe
                            {
                                inputArea.Append(e.text.text);
                            }
                            inputArea.Draw();
                            break;

                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_BACKSPACE && inputArea.InputText.Length > 0)
                                inputArea.InputText = inputArea.InputText[..^1];
                            else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_RETURN && inputArea.InputText.Length > 0)
                            {

                                logArea.Add(inputArea.InputText);
                                inputArea.InputText = "";
                                logArea.TopPtr = 0;
                            }
                            else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_PAGEUP) logArea.Scroll(+1);
                            else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_PAGEDOWN) logArea.Scroll(-1);
                            else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                                quit = true;
                            inputArea.Draw();
                            logArea.Draw();
                            break;

                        case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                            if (e.wheel.y > 0) logArea.Scroll(+1);
                            else if (e.wheel.y < 0) logArea.Scroll(-1);
                            logArea.Draw();
                            break;
                    }
                }
                canvas.Draw();
                if (dialog.Visible) dialog.Draw();
                SDL.SDL_RenderPresent(renderer);
                SDL.SDL_Delay(16); // ~60fps
            }
        }

        private class TextInputArea
        {
            private SDL.SDL_Rect viewport = new SDL.SDL_Rect();
            public string InputText { get; set; } = string.Empty;
            public string Prompt { get; set; } = @"どうする?";
            private string HitAnyKeyMessage = @"何かキーを押してください";

            private bool hitanykey = false;
            public bool HitAnyKey
            {
                get
                {
                    return hitanykey;
                }
                set
                {
                    hitanykey = value;
                    if (value)
                    {
                        visible = false;
                    }
                    Draw();
                }
            }
            public SDL.SDL_Color TextColor { get; set; } = new SDL.SDL_Color() { r = 255, g = 255, b = 255, a = 255 };
            public IntPtr TextFont { get; set; } = IntPtr.Zero;
            private IntPtr renderer = IntPtr.Zero;
            private bool visible = true;
            public bool Visible {
                get
                {
                    return visible;
                }
                set
                {
                    visible = value;
                    if (value)
                    {
                        hitanykey = false;
                    }
                    Draw();
                }
            }

            public TextInputArea(IntPtr r, int x, int y, int w, int h)
            {
                renderer = r;
                viewport.x = x;
                viewport.y = y;
                viewport.w = w;
                viewport.h = h;
            }

            public void Draw()
            {
                if (!Visible && !HitAnyKey)
                {
                    SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
                    SDL.SDL_RenderFillRect(renderer, ref viewport);
                    return;
                }
                string text = InputText;
                SDL.SDL_Color c = TextColor;
                if (string.IsNullOrEmpty(InputText))
                {
                    text = Prompt;
                    c = new SDL.SDL_Color { r = 0, g = 255, b = 255, a = 255 };
                }
                if (HitAnyKey)
                {
                    text = HitAnyKeyMessage;
                    c = new SDL.SDL_Color { r = 255, g = 255, b = 0, a = 255 };
                }
                SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
                SDL.SDL_RenderFillRect(renderer, ref viewport);
                IntPtr surf = SDL_ttf.TTF_RenderUTF8_Blended(TextFont, text, c);
                IntPtr tex = SDL.SDL_CreateTextureFromSurface(renderer, surf);
                SDL.SDL_QueryTexture(tex, out _, out _, out int w, out int h);
                var dst = new SDL.SDL_Rect { x = viewport.x, y = viewport.y, w = w, h = h };
                SDL.SDL_RenderCopy(renderer, tex, IntPtr.Zero, ref dst);
                SDL.SDL_DestroyTexture(tex);
                SDL.SDL_FreeSurface(surf);
            }

            unsafe public void Append(byte* buffer)
            {
                // バッファの最大長を定義（SDL_TEXTINPUTEVENT_TEXT_SIZE は 32）
                const int SDL_TEXTINPUTEVENT_TEXT_SIZE = 32;

                // バッファを byte[] にコピー
                byte[] byteArray = new byte[SDL_TEXTINPUTEVENT_TEXT_SIZE];
                Marshal.Copy((IntPtr)buffer, byteArray, 0, SDL_TEXTINPUTEVENT_TEXT_SIZE);

                // null 終端文字の位置を探す
                int len = Array.IndexOf(byteArray, (byte)0);
                if (len < 0) len = SDL_TEXTINPUTEVENT_TEXT_SIZE; // null 終端が見つからない場合

                // UTF-8 文字列に変換
                InputText += Encoding.UTF8.GetString(byteArray, 0, len);
            }
        }

        private class ScrollView
        {
            private SDL.SDL_Rect viewport = new SDL.SDL_Rect();
            public SDL.SDL_Color TextColor { get; set; } = new SDL.SDL_Color() { r = 255, g = 255, b = 255, a = 255 };
            private IntPtr textFont = IntPtr.Zero;
            private IntPtr renderer = IntPtr.Zero;
            public IntPtr TextFont
            {
                get { return textFont; }
                set
                {
                    textFont = value;
                    if (value != IntPtr.Zero)
                    {
                        SDL_ttf.TTF_SizeUTF8(value, "Mg", out _, out lineHeight);
                        maxLines = viewport.h / lineHeight;
                    }
                }
            }

            private int lineHeight = 16;
            private int maxLines = 0;
            private List<string> log = new List<string>();

            public int TopPtr { get;  set; } = 0;

            public ScrollView(IntPtr r, int x, int y, int w, int h)
            {
                renderer = r;
                viewport.x = x;
                viewport.y = y;
                viewport.w = w;
                viewport.h = h;
            }
            public void Add(string s)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var ch in s)
                {
                    sb.Append(ch);
                    SDL_ttf.TTF_SizeUTF8(TextFont, sb.ToString(), out int w, out _);
                    if (w > viewport.w)
                    {
                        if (sb.Length > 1)
                        {
                            log.Add(sb.ToString(0, sb.Length - 1));
                            sb.Clear().Append(ch);
                        }
                    }
                }
                if (sb.Length > 0) log.Add(sb.ToString());
            }

            public void Clear()
            {
                log.Clear();
                TopPtr = 0;
            }

            public int Scroll(int delta)
            {
                int maxScroll = Math.Max(0, log.Count - maxLines);
                TopPtr = Math.Clamp(TopPtr + (delta > 0 ? +1 : -1), 0, maxScroll);
                return TopPtr;
            }
            public void Draw()
            {
                SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
                SDL.SDL_RenderFillRect(renderer, ref viewport);
                int visibleLines = Math.Max(1, viewport.h / lineHeight);
                int start = Math.Max(0, log.Count - visibleLines - TopPtr);
                int end = Math.Min(log.Count, start + visibleLines);

                int y = viewport.y + PADDING;
                for (int i = start; i < end; i++)
                {
                    string text = log[i];
                    if (string.IsNullOrEmpty(text)) continue;
                    IntPtr surf = SDL_ttf.TTF_RenderUTF8_Blended(TextFont, text, TextColor);
                    IntPtr tex = SDL.SDL_CreateTextureFromSurface(renderer, surf);
                    SDL.SDL_QueryTexture(tex, out _, out _, out int w, out int h);
                    var dst = new SDL.SDL_Rect { x = viewport.x, y = y, w = w, h = h };
                    SDL.SDL_RenderCopy(renderer, tex, IntPtr.Zero, ref dst);
                    SDL.SDL_DestroyTexture(tex);
                    SDL.SDL_FreeSurface(surf);
                    y += lineHeight;
                }
                //SDL.SDL_RenderPresent(renderer);
            }
        }
        
    }
}