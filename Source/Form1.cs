using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp24
{
    public class cActor
    {
        public int x, y, w, h, Life, dir;
    }
    public class CMultiImageActor
    {
        public int X, Y;
        public List<List<Bitmap>> animations = new List<List<Bitmap>>();
        public List<Bitmap> imgs;
        public int iFrame;
        public int xDir = 0; public int currentAnimation = 0;

    }
    public partial class Form1 : Form
    {
        Bitmap off;
        Timer tt = new Timer();
        List<CMultiImageActor> LHero = new List<CMultiImageActor>();
        List<cActor> bullet = new List<cActor>();
        List<cActor> lazer = new List<cActor>();

        int jumptype;
        bool flying = false;

        bool isLeftPressed = false;
        bool isRightPressed = false;
        bool isDownPressed = false;
        bool isShiftPressed = false;
        bool isSpacePressed = false;

        int ctTick = 0;
        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            Paint += Form1_Paint;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;

            tt.Interval = 100;
            tt.Start();
            tt.Tick += Tt_Tick;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            CreateHero();
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
        }
        private void Tt_Tick(object sender, EventArgs e)
        {
            Bullets();
            Jump();
            Gravity();
            Move();//run,walk
            Lazer();

            ctTick++;
            DrawDubb(this.CreateGraphics());
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    isLeftPressed = true;
                    break;
                case Keys.Right:
                    isRightPressed = true;
                    break;
                case Keys.Down:
                    isDownPressed = true;
                    break;
                case Keys.ShiftKey:
                    isShiftPressed = true;
                    break;
                case Keys.F:
                    createbullets();
                    break;
                case Keys.G:
                    createlazer();
                    break;
                case Keys.Space:
                    if (!flying)
                        isSpacePressed = true;
                    break;

            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) isLeftPressed = false;
            if (e.KeyCode == Keys.Right) isRightPressed = false;
            if (e.KeyCode == Keys.Space) isSpacePressed = false;
            if (e.KeyCode == Keys.Down) isDownPressed = false;
            if (e.KeyCode == Keys.ShiftKey) isShiftPressed = false;
        }



        void CreateHero()
        {
            CMultiImageActor pnn = new CMultiImageActor();
            pnn.imgs = new List<Bitmap>();
            List<Bitmap> walkLeft = new List<Bitmap>();
            List<Bitmap> walkRight = new List<Bitmap>();
            for (int i = 1; i < 5; i++)
            {
                Bitmap img = new Bitmap("Lwalk" + i + ".png");// 3shan a5od dimensions el pic
                img.MakeTransparent(img.GetPixel(0, 0));
                pnn.imgs.Add(img);

                Bitmap imgL = new Bitmap("Lwalk" + i + ".png");
                imgL.MakeTransparent(imgL.GetPixel(0, 0));
                walkLeft.Add(imgL);

                Bitmap imgR = new Bitmap("Rwalk" + i + ".png");
                imgR.MakeTransparent(imgR.GetPixel(0, 0));
                walkRight.Add(imgR);
            }
            pnn.animations.Add(walkLeft);
            pnn.animations.Add(walkRight);

            pnn.X = ClientSize.Width / 2;
            pnn.Y = ClientSize.Height / 2 - pnn.imgs[0].Height;

            LHero.Add(pnn);
        }

        void createbullets()
        {//mfrod nzbot el x,y el bttl3 mnha el bullet 3la 7sb bases fen
            cActor pnn = new cActor();
            pnn.x = LHero[0].X + LHero[0].imgs[LHero[0].iFrame].Width;
            pnn.y = LHero[0].Y + LHero[0].imgs[LHero[0].iFrame].Height / 2 + 8;
            pnn.w = 5;
            pnn.h = 0;
            pnn.Life = 1;
            if (LHero[0].xDir == 1)
            {
                pnn.dir = 1;
            }
            else
            {
                pnn.dir = 0;
            }
            bullet.Add(pnn);
        }
        void Bullets()
        {
            for (int i = 0; i < bullet.Count; i++)
            {
                if (bullet[i].Life == 1)
                {
                    if (bullet[i].dir == 1)
                    {
                        bullet[i].x += 20;
                    }
                    else
                    {
                        bullet[i].x -= 20;
                    }
                    if (bullet[i].x >= ClientSize.Width || bullet[i].x <= 0)//limit
                    {
                        bullet[i].Life = 0;//life kda mlhash lzma now bs mfrod lyha st5dam later
                        bullet.RemoveAt(i);
                    }
                }
            }
        }

        void createlazer()
        {
            int width = 200,margin=10;
            cActor pnn = new cActor();
            if (LHero[0].xDir == 1)
            {
                pnn.x = LHero[0].X + LHero[0].imgs[LHero[0].iFrame].Width;
            }
            else
            {
                pnn.x = LHero[0].X - width+ margin;
            }
            pnn.y = LHero[0].Y + LHero[0].imgs[LHero[0].iFrame].Height / 2 + 11;
            pnn.w = width;
            pnn.h = 3;
            pnn.Life = 3;

            lazer.Add(pnn);
        }
        void Lazer()
        {
            if (lazer.Count > 0)
            {
                lazer[lazer.Count - 1].Life--;
                if (lazer[lazer.Count - 1].Life <= 0)
                {
                    lazer.RemoveAt(lazer.Count - 1);
                }
            }
        }
        void Jump()
        {
            //mmkn n5lyh fluid aktr lw el jump 3la 7sb days ymen wla shmal
            //bas fyh degree of difficulty fa dk b2a (if want change ask me)
            flying = true;
            if (isSpacePressed == true)
            {
                jumptype = 0;
                LHero[0].Y -= 30;
            }
            if (isSpacePressed == true && isRightPressed == true)
            {
                jumptype = 1;
                LHero[0].Y -= 30;
                LHero[0].X += 30;
            }
            if (isSpacePressed == true && isLeftPressed == true)
            {
                jumptype = -1;
                LHero[0].Y -= 30;
                LHero[0].X -= 30;
            }
            if (isDownPressed == true && isSpacePressed == true)//super jump
            {
                jumptype = 0;
                LHero[0].Y -= 50;
            }
        }
        void Gravity()
        {
            //clientsize.height/2 for testing (htt8yr)
            if (LHero[0].Y <= ClientSize.Height / 2 - LHero[0].imgs[0].Height && jumptype == 0)//jump type:anwa3 falling
            {
                LHero[0].Y += 3;
            }
            else if (LHero[0].Y <= ClientSize.Height / 2 - LHero[0].imgs[0].Height && jumptype == 1)
            {
                LHero[0].Y += 3;
                LHero[0].X++;
            }
            else if (LHero[0].Y <= ClientSize.Height / 2 - LHero[0].imgs[0].Height && jumptype == -1)
            {
                LHero[0].Y += 3;
                LHero[0].X--;
            }
            else
            {
                flying = false;
            }
        }
        void Move()
        {
            if (isRightPressed == true)
            {
                LHero[0].X += 5;
                LHero[0].iFrame = (LHero[0].iFrame + 1) % 4;
                LHero[0].xDir = 1;
                LHero[0].currentAnimation = 1;
            }
            if (isShiftPressed == true && isRightPressed == true)
            {
                LHero[0].X += 20;
                LHero[0].iFrame = (LHero[0].iFrame + 1) % 4;
                LHero[0].xDir = 1;
                LHero[0].currentAnimation = 1;
            }

            if (isLeftPressed == true)
            {
                LHero[0].X -= 5;
                LHero[0].iFrame = (LHero[0].iFrame + 1) % 4;
                LHero[0].xDir = -1;
                LHero[0].currentAnimation = 0;
            }
            if (isShiftPressed == true && isLeftPressed == true)
            {
                LHero[0].X -= 20;
                LHero[0].iFrame = (LHero[0].iFrame + 1) % 4;
                LHero[0].xDir = -1;
                LHero[0].currentAnimation = 0;
            }
        }
        void DrawScene(Graphics g2)
        {
            g2.Clear(Color.Black);


            for (int i = 0; i < bullet.Count; i++)
            {
                Pen Pn = new Pen(Color.Yellow, 5);
                g2.DrawEllipse(Pn, bullet[i].x, bullet[i].y, 3, 3);
            }

            for (int i = 0; i < lazer.Count; i++)
            {
                Pen Pn = new Pen(Color.Yellow, 5);
                g2.FillRectangle(Brushes.Red, lazer[i].x, lazer[i].y, lazer[i].w, lazer[i].h);
            }

            for (int i = 0; i < LHero.Count; i++)
            {
                int k = LHero[i].iFrame;
                List<Bitmap> currentFrames = LHero[i].animations[LHero[i].currentAnimation];
                Bitmap frame = currentFrames[k];
                g2.DrawImage(frame, LHero[i].X, LHero[i].Y, frame.Width + 10, frame.Height + 10);
            }

            

            //test line
            g2.DrawLine(Pens.LightGray, 0, ClientSize.Height / 2, ClientSize.Width, ClientSize.Height / 2);
        }
        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);

        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }

    }
}
