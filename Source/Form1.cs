using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace WindowsFormsApp24
{
    public partial class Form1 : Form
    {
        Bitmap off;
        Timer tt = new Timer();
        List<CMultiImageActor> LHero = new List<CMultiImageActor>();
        List<cActor> bullet = new List<cActor>();
        List<cActor> lazer = new List<cActor>();

        int jumptype;
        bool flying = false;

        bool isUpPressed = false;
        bool isLeftPressed = false;
        bool isRightPressed = false;
        bool isDownPressed = false;
        bool isShiftPressed = false;
        bool isSpacePressed = false;
        bool isFPressed = false;
        int ctTick = 0;
        int FireRate = 0;
        int BulletSpeed = 50;
        int MoveLock = 0;
        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            Paint += Form1_Paint;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            tt.Interval = 95;
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
            Move();//run,walk
            Bullets();
            Jump();
            Gravity();
            Lazer();
            AnimateHero();
            CoolDowns();
            
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
                case Keys.Up:
                    isUpPressed = true;
                    break;
                case Keys.F:
                    isFPressed = true;
                    if (FireRate == 0)
                    {
                        createbullets();
                    }
                    break;
                case Keys.G:
                    createlazer();
                    break;
                case Keys.ShiftKey:
                    isShiftPressed = true;
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
            List<Bitmap> standLeft = new List<Bitmap>();
            List<Bitmap> standRight = new List<Bitmap>();
            Bitmap img = new Bitmap("jazz/Rstand.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            pnn.standR.Add(img);
            img = new Bitmap("jazz/Lstand.png");
            img.MakeTransparent(img.GetPixel(0, 0));
            pnn.standL.Add(img);
            for (int i = 0; i < 8; i++)
            {
                /*img = new Bitmap("Lwalk" + i + ".png");// 3shan a5od dimensions el pic
                img.MakeTransparent(img.GetPixel(0, 0));
                pnn.imgs.Add(img);*/
                img = new Bitmap("jazz/Rwalk_" + i + ".png");
                img.MakeTransparent(img.GetPixel(0, 0));
                pnn.walkR.Add(img);
                
                img = new Bitmap("jazz/Lwalk_" + i + ".png");
                img.MakeTransparent(img.GetPixel(0, 0));
                pnn.walkL.Add(img);

                if(i < 2)
                {
                    img = new Bitmap("jazz/Rshoot_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.shootR.Add(img);

                    img = new Bitmap("jazz/Lshoot_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.shootL.Add(img);
                }
            }
            /*pnn.animations.Add(standRight);
            pnn.animations.Add(standLeft);
            pnn.animations.Add(walkLeft);
            pnn.animations.Add(walkRight);
*/
            pnn.X = ClientSize.Width / 2;
            pnn.Y = ClientSize.Height / 2 - (pnn.standR[0].Height / 2);
            pnn.xDir = 1;
            pnn.currentAnimation = 0;
            LHero.Add(pnn);
        }

        void Move()
        {
            /*if(MoveLock != 0)
            {
                return;
            }
            else
            {
                MoveLock = 1;
            }*/


            // l->r works but r->l doesnt
            CMultiImageActor hero = LHero[0];
            double run = 1;
            if(isShiftPressed == true)
            {
                run = 1.5;
            }
            if (isRightPressed == true)
            {
                if(hero.xDir == -1)
                {
                    hero.iFrame = -1;
                }
                hero.xDir = 1;
                hero.X += (int)(hero.speed * run) * hero.xDir;
                //LHero[0].iFrame = (LHero[0].iFrame + 1) % 4;
                //LHero[0].currentAnimation = 2;
            }
            else if (isLeftPressed == true)
            {
                if (hero.xDir == 1)
                {
                    hero.iFrame = -1;
                }
                LHero[0].xDir = -1;
                hero.X += (int)(hero.speed * run) * hero.xDir;
                //LHero[0].iFrame = (LHero[0].iFrame + 1) % 4;
                //LHero[0].currentAnimation = 3;
            }
            
        }

        void Jump()
        {
            //mmkn n5lyh fluid aktr lw el jump 3la 7sb days ymen wla shmal
            //bas fyh degree of difficulty fa dk b2a (if want change ask me)

            //re: azdk el howa yro7 b angle ymen aw shmal?
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

        private void AnimateHero()
        {
            CMultiImageActor hero = LHero[0];
            int curr = 0;

            if (isShiftPressed == true)
            {
            }
            if (isRightPressed == true)
            {
                //LHero[0].xDir = 1;
                LHero[0].currentAnimation = 2;
            }
            else if (isLeftPressed == true)
            {
                //LHero[0].xDir = -1;
                LHero[0].currentAnimation = 3;
            }
            else
            {
                hero.iFrame = 0;
                if (hero.xDir == 1)
                {
                    LHero[0].currentAnimation = 0;
                }
                else
                {
                    hero.currentAnimation = 1;
                }
            }

            /*if (isLeftPressed == false && isRightPressed == false)
            {
                hero.iFrame = 0;
                if (hero.xDir == 1)
                {
                    hero.currentAnimation = 4;
                }
                else
                {
                    hero.currentAnimation = 5;
                }
            }*/

            hero.iFrame++;
        }

        void createbullets()
        {
            //mfrod nzbot el x,y el bttl3 mnha el bullet 3la 7sb bases fen
            FireRate = 5;

            cActor pnn = new cActor();
            //pnn.x = LHero[0].X + LHero[0].imgs[LHero[0].iFrame].Width;
            //pnn.y = LHero[0].Y + LHero[0].imgs[LHero[0].iFrame].Height / 2 + 8;

            pnn.x = LHero[0].X;
            pnn.y = LHero[0].Y;
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
                        bullet[i].x += BulletSpeed;
                    }
                    else
                    {
                        bullet[i].x -= BulletSpeed;
                    }
                    //different than the game
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

        void Gravity()
        {
            //clientsize.height/2 for testing (htt8yr)
            //rename ntmp
            int ntmp = ClientSize.Height / 2 - LHero[0].standL[0].Height;
            if (LHero[0].Y <= ntmp && jumptype == 0)//jump type:anwa3 falling
            {
                LHero[0].Y += 3;
            }
            else if (LHero[0].Y <= ntmp && jumptype == 1)
            {
                LHero[0].Y += 3;
                LHero[0].X++;
            }
            else if (LHero[0].Y <= ntmp && jumptype == -1)
            {
                LHero[0].Y += 3;
                LHero[0].X--;
            }
            else
            {
                flying = false;
            }
        }

        private void CoolDowns()
        {
            if (FireRate > 0)
            {
                FireRate--;
            }
            if (MoveLock > 0)
            {
                MoveLock--;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }


        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);

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
                CMultiImageActor hero = LHero[i];
                //List<Bitmap> currentFrames = hero.animations[LHero[i].currentAnimation];
                //Bitmap frame = currentFrames[k];
                
                Bitmap frame = hero.curr_F();
                //g2.DrawImage(frame, LHero[i].X, LHero[i].Y, frame.Width + 10, frame.Height + 10);
                g2.DrawImage(frame, hero.X - (frame.Width / 2), hero.Y - (frame.Height / 2));
            }

            //test line
            g2.DrawLine(Pens.LightGray, 0, ClientSize.Height / 2, ClientSize.Width, ClientSize.Height / 2);
        }
    }
    public class cActor
    {
        public int x, y, w, h, Life, dir;
    }
    public class CMultiImageActor
    {
        public int X, Y;
        public List<List<Bitmap>> animations = new List<List<Bitmap>>();
        //0
        public List<Bitmap> standR = new List<Bitmap>();
        //1
        public List<Bitmap> standL = new List<Bitmap>();
        //2
        public List<Bitmap> walkR = new List<Bitmap>();
        //3
        public List<Bitmap> walkL = new List<Bitmap>();
        //4
        public List<Bitmap> shootR = new List<Bitmap>();
        //5
        public List<Bitmap> shootL = new List<Bitmap>();
        //6
        public List<Bitmap> stopshootR = new List<Bitmap>();
        //7
        public List<Bitmap> stopshootL = new List<Bitmap>();
        public List<Bitmap> imgs;
        public int iFrame;
        public int xDir = 0; 
        public int currentAnimation = 0;
        public int speed = 20;
        public List<Bitmap> curr_L()
        {
            List<Bitmap> ltmp = new List<Bitmap>();
            switch (currentAnimation)
            {
                case 0:
                    ltmp = standR;
                    break;
                case 1:
                    ltmp = standL;
                    break;
                case 2:
                    ltmp = walkR;
                    break;
                case 3:
                    ltmp = walkL;
                    break;
                case 4:
                    ltmp = shootR;
            }
            return ltmp;
        }
        public Bitmap curr_F()
        {
            List<Bitmap> Ltmp = curr_L();
            return Ltmp[iFrame % (Ltmp.Count)];
        }
        public int curr_H()
        {
            Bitmap btmp = curr_F();
            return btmp.Height;
        }
        public int hitbox_up()
        {
            Bitmap btmp = curr_F();
            return X - (btmp.Height/2);
        }
    }
}
