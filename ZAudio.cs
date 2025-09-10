// ZAudio for HHSAdvSDL

using System;
using System.Dynamic;
using System.Text;
using SDL2;

namespace HHSAdvSDL
{
    public class ZAudio
    {
        private string[] soundFiles = {
            "highschool", "charumera", "explosion", string.Empty,  "in_toilet", "acid",
        };
        public string DataFolder { get; private set; }
        public ZAudio(string d)
        {
            DataFolder = d;
        }
        public void Play(int id)
        {
            if (id < 0 || id >= soundFiles.Length || string.IsNullOrEmpty(soundFiles[id])) return;
            StringBuilder mp3 = new StringBuilder(soundFiles[id]);
            mp3.Append(".mp3");
            string af = Path.Combine(DataFolder, mp3.ToString());
            if (File.Exists(af))
            {
                if (SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048) < 0)
                {
                    Console.WriteLine($"Mix_OpenAudio Error: {SDL.SDL_GetError()}");
                    return;
                }

                // MP3 読み込み
                IntPtr music = SDL_mixer.Mix_LoadMUS(af);
                if (music == IntPtr.Zero)
                {
                    Console.WriteLine($"Mix_LoadMUS Error: {SDL.SDL_GetError()}");
                    return;
                }

                // 再生（-1 はループなし、0 は1回再生）
                if (SDL_mixer.Mix_PlayMusic(music, 0) == -1)
                {
                    Console.WriteLine($"Mix_PlayMusic Error: {SDL.SDL_GetError()}");
                    return;
                }
                // 後始末
                Task.Run(() =>
                {
                    while (SDL_mixer.Mix_PlayingMusic() != 0)
                    {
                        SDL.SDL_Delay(100);
                    }
                    SDL_mixer.Mix_FreeMusic(music);
                    SDL_mixer.Mix_CloseAudio();
                });
            }
        }
    }
}