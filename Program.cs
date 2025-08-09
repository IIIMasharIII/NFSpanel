using GameOverlay.Drawing;
using GameOverlay.Windows;
using NFSpanel;
using NAudio;
using NAudio.Wave;
using System.Diagnostics;
using NAudio.Utils;

namespace entryPoint
{
    class Program
    {

        private static GraphicsWindow _window;
        private static EApanel _panel;
        private static bool startPlaying = false;

        static void Main
            (string[] args)
        {
            //_panel = new EApanel("\"Dilated Peoples\"", "Neighborhood Watch", "\"Who\'s Who\"");

            var gfx = new Graphics()
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true
            };

            _window = new GraphicsWindow(0, 0, 1920, 1080, gfx)
            {
                FPS = 60,
                IsTopmost = true,
                IsVisible = true
            };



            _window.DestroyGraphics += _window_DestroyGraphics;
            _window.DrawGraphics += _window_DrawGraphics;
            _window.SetupGraphics += _window_SetupGraphics;

            _window.Create();

            FileInfo[] files = new DirectoryInfo("..\\..\\..\\nfsmusic").GetFiles();


            while (true)
            {
                string musicFile = files[new Random().Next(files.Length)].Name;
                string[] info = musicFile.Replace(".mp3", "").Split("_-_");

                _panel = new EApanel(info[0], info[2], $"\"{info[1]}\"");
                using (AudioFileReader audio = new AudioFileReader("..\\..\\..\\nfsmusic\\" + musicFile))
                {
                    using (WaveOutEvent wave = new WaveOutEvent())
                    {
                        wave.Init(audio);
                        wave.Play();
                        wave.Volume = 0.02f;
                        while (wave.PlaybackState == PlaybackState.Playing)
                        {

                        }
                    }
                }
            }
        }

        private static void _window_SetupGraphics
            (object? sender, SetupGraphicsEventArgs e)
        {
            EApanel.Initialize(e.Graphics);
        }

        private static void _window_DrawGraphics
            (object? sender, DrawGraphicsEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.ClearScene();
            _panel.Update();
            _panel.Draw(graphics);
        }

        private static void _window_DestroyGraphics
            (object? sender, DestroyGraphicsEventArgs e)
        {

        }
    }
}
