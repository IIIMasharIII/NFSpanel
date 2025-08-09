using GameOverlay.Drawing;
using NAudio;
using NAudio.Wave;
using System.Diagnostics;

namespace NFSpanel
{
    class EApanel
    {
        private Point position = new Point(20, 20);
        private static Image EAlogo;
        private static Image musicPanel;
        private static SolidBrush debugBrush;
        private static SolidBrush musicBrush;
        private static Font font;
        private static Font artistsFont;
        private Thread _closeThread;
        private int logoWIDTH = 150;
        private int startLogoWIDTH = 1;
        private int logoHEIGHT = 150;
        private int startLogoHEIGHT = 1;
        private int logoX = 275;
        private int logoY = 875;

        private int panelWIDTH = 800;
        private int startPanelWIDTH = 1;
        private int panelHEIGHT = 130;
        private int panelX = 250;
        private int panelY = 810;

        private bool open = true;
        private bool needFont = false;
        private bool needClose = false;
        private bool needPanel = false;


        private bool panelNoRender = false;
        private bool logoNoRender = false;

        public string Artist = "";
        public string AlbumName = "";
        public string MusicName = "";

        public EApanel(string artist, string albumname, string musicname)
        {
            Artist = artist;
            AlbumName = albumname;
            MusicName = musicname;
        }

        public static void Initialize
            (Graphics graphics)
        {
            if (EAlogo == null) EAlogo = graphics.CreateImage("..\\..\\..\\1.png");
            if (musicPanel == null) musicPanel = graphics.CreateImage("..\\..\\..\\2.png");
            if (musicBrush == null)
            {
                musicBrush = new SolidBrush(graphics.GetRenderTarget(), Color.FromARGB(System.Drawing.Color.Snow.ToArgb()));
                musicBrush.Brush.Opacity = 0;
            }
            if (debugBrush == null)
            {
                debugBrush = new SolidBrush(graphics.GetRenderTarget(), Color.Red);
            }
            if (font == null) font = graphics.CreateFont("Arial", 24f, false, true, true);
            if (artistsFont == null) artistsFont = graphics.CreateFont("Arial", 34f, true, true, true);
        }

        private static float SmoothMove
           (float a, float b, float speed)
        {
            return (b - a) / speed;
        }

        public void Update()
        {
            if (open)
            {
                int widthDiff = (int)SmoothMove(startLogoWIDTH, logoWIDTH, 4);
                startLogoWIDTH += widthDiff + widthDiff;
                logoX -= widthDiff;
                int heightDiff = (int)SmoothMove(startLogoHEIGHT, logoHEIGHT, 4);
                startLogoHEIGHT += heightDiff + heightDiff;
                logoY -= heightDiff;

                if (widthDiff == 0) needPanel = true;
                //else
                //{
                //    int widthDiff = (int)SmoothMove(startLogoWIDTH, 0, 7);
                //    startLogoWIDTH += widthDiff + widthDiff;
                //    logoX -= widthDiff;
                //    int heightDiff = (int)SmoothMove(startLogoHEIGHT, 0, 7);
                //    startLogoHEIGHT += heightDiff + heightDiff;
                //    logoY -= heightDiff;
                //}

                if (!needPanel) return;


                int panel = (int)SmoothMove(startPanelWIDTH, panelWIDTH, 7);
                if (panel == 0)
                {
                    needFont = true;
                }
                startPanelWIDTH += panel;
                //else
                //{
                //    int panel = (int)SmoothMove(startPanelWIDTH, 0, 7);
                //    if (panel == 0)
                //    {
                //        //open = true;
                //    }
                //    startPanelWIDTH += panel;
                //}
            }

            if (needFont && musicBrush != null)
            {
                musicBrush.Brush.Opacity += 0.1f;
                needClose = musicBrush.Brush.Opacity >= 1 ? true : false;
            }

            if (needClose)
            {
                needFont = false;
                if (_closeThread == null && musicBrush.Brush.Opacity >= 1f)
                {
                    _closeThread = new Thread(() =>
                    {
                        Thread.Sleep(3000);
                        open = false;
                        while (musicBrush.Brush.Opacity >= 0f)
                        {
                            musicBrush.Brush.Opacity -= 0.1f;
                            Thread.Sleep(16);
                        }

                        int panel = 1;
                        while (panel != 0)
                        {
                            panel = (int)SmoothMove(startPanelWIDTH, 0, 7);
                            startPanelWIDTH += panel;
                            Thread.Sleep(16);
                        }
                        panelNoRender = true;

                        int widthDiff = 1;
                        int heightDiff = 1;
                        while (widthDiff != 0)
                        {
                            widthDiff = (int)SmoothMove(startLogoWIDTH, 0, 7);
                            heightDiff = (int)SmoothMove(startLogoHEIGHT, 0, 7);
                            startLogoWIDTH += widthDiff + widthDiff;
                            logoX -= widthDiff;
                            startLogoHEIGHT += heightDiff + heightDiff;
                            logoY -= heightDiff;
                            Thread.Sleep(16);
                        }
                        logoNoRender = true;
                    });
                    _closeThread.Start();
                }
            }
        }

        public void Draw(Graphics graphics)
        {
            Rectangle panelRec = new Rectangle(panelX, panelY, panelX + startPanelWIDTH, panelY + panelHEIGHT);
            if (!panelNoRender) graphics.DrawImage(musicPanel, panelRec);
            graphics.DrawText(artistsFont, musicBrush, new Point(400, 820), Artist);
            graphics.DrawText(font, musicBrush, new Point(400, 860), MusicName);
            graphics.DrawText(font, musicBrush, new Point(400, 890), AlbumName);
            Rectangle logoRec = new Rectangle(logoX, logoY, logoX + startLogoWIDTH, logoY + startLogoHEIGHT);
            if (!logoNoRender) graphics.DrawImage(EAlogo, logoRec);

#if DEBUG
            //graphics.DrawRectangle(debugBrush, rectangle1, 1f);
            //graphics.DrawRectangle(debugBrush, rectangle2, 1f);
#endif
        }
    }
}
