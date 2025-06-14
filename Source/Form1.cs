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
        List<CMultiImageActor> plats = new List<CMultiImageActor>();
        List<CMultiImageActor> ladder = new List<CMultiImageActor>();
        List<cActor> bullet = new List<cActor>();
        List<cActor> lazer = new List<cActor>();

        int jumptype;
        bool flying = false;
        bool gravity = false;

        bool isUpPressed = false;
        bool isLeftPressed = false;
        bool isRightPressed = false;
        bool isDownPressed = false;
        bool isShiftPressed = false;
        bool isSpacePressed = false;
        bool isFPressed = false;
        bool isOnPlatform = false;
        bool isOnLadder = false;

        int ctTick = 0;
        int FireRate = 0;
        int BulletSpeed = 50;
        int MoveLock = 0;
        int stopFire = 0;
        int stopCrouch = 0;
        int runordash = 0;
        int jump_ct = 0;
        int jump_ct_max = 0;
        int max_jump = 2 * 2;
        int min_jump = 3;
        int jump_pow = 20;
        int jump_cd = 0;
        int jump_cd_reset = 20;
        int gravity_speed = 10;
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
            platforms();
            ladders();

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
            platmove();
            platchecker();
            ladderschecker();
            CoolDowns();

            ctTick++;
            DrawDubb(this.CreateGraphics());
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            CMultiImageActor hero = LHero[0];
            switch (e.KeyCode)
            {
                case Keys.Right:
                    isRightPressed = true;
                    break;
                case Keys.Left:
                    isLeftPressed = true;
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
                    isSpacePressed = true;
                    if (jump_cd == 0)
                    {
                        if (gravity == false)
                        {
                            flying = true;
                            if (jump_ct_max < max_jump)
                            {
                                jump_ct_max++;
                            }
                            else
                            {
                                jump_ct_max /= 2;
                                jump_cd = jump_cd_reset;
                            }
                        }
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            CMultiImageActor hero = LHero[0];
            switch (e.KeyCode)
            {
                case Keys.Right:
                    isRightPressed = false;
                    break;
                case Keys.Left:
                    isLeftPressed = false;
                    break;
                case Keys.Down:
                    isDownPressed = false;
                    if (hero.currentAnimation == 14 || hero.currentAnimation == 15)
                    {
                        stopFire = hero.s_stopcrouchL.Count;
                    }
                    break;
                case Keys.Up:
                    isUpPressed = false;
                    break;
                case Keys.F:
                    isFPressed = false;
                    if (hero.currentAnimation == 4 || hero.currentAnimation == 5)
                    {
                        stopFire = hero.s_stopshootL.Count;
                    }
                    break;
                case Keys.Space:
                    isSpacePressed = false;
                    if(jump_ct_max > 0)
                    {
                        jump_ct_max /= 2;
                        jump_cd = jump_cd_reset;
                    }
                    break;
                case Keys.ShiftKey:
                    isShiftPressed = false;
                    runordash = 0;
                    break;
            }

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

            int j;

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

                if(i < 5)
                {
                    for (j = 0; j < 1; j++)
                    {
                        img = new Bitmap("jazz/Sjump_" + i + ".png");
                        img.MakeTransparent(img.GetPixel(0, 0));
                        pnn.jumpS.Add(img);
                    }
                }
                if (i < 4)
                {
                    img = new Bitmap("jazz/Rstopshoot_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.s_stopshootR.Add(img);

                    img = new Bitmap("jazz/Lstopshoot_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.s_stopshootL.Add(img);

                    img = new Bitmap("jazz/Rdash_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.dashR.Add(img);

                    img = new Bitmap("jazz/Ldash_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.dashL.Add(img);

                    img = new Bitmap("jazz/Rrun_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.runR.Add(img);

                    img = new Bitmap("jazz/Lrun_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.runL.Add(img);
                }

                if (i < 3)
                {
                    img = new Bitmap("jazz/Sfall_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.fallS.Add(img);
                }

                if (i < 2)
                {
                    for (j = 0; j < 3; j++)
                    {
                        img = new Bitmap("jazz/Rshoot_" + i + ".png");
                        img.MakeTransparent(img.GetPixel(0, 0));
                        pnn.shootR.Add(img);

                        img = new Bitmap("jazz/Lshoot_" + i + ".png");
                        img.MakeTransparent(img.GetPixel(0, 0));
                        pnn.shootL.Add(img);

                        img = new Bitmap("jazz/Rushoot_" + i + ".png");
                        img.MakeTransparent(img.GetPixel(0, 0));
                        pnn.shootupR.Add(img);

                        img = new Bitmap("jazz/Lushoot_" + i + ".png");
                        img.MakeTransparent(img.GetPixel(0, 0));
                        pnn.shootupL.Add(img);
                    }

                    img = new Bitmap("jazz/Rlookup_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.lookupR.Add(img);

                    img = new Bitmap("jazz/Llookup_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.lookupL.Add(img);

                    img = new Bitmap("jazz/Rcrouch_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.crouchR.Add(img);

                    img = new Bitmap("jazz/Lcrouch_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.crouchL.Add(img);
                }

                if (i == 0)
                {
                    img = new Bitmap("jazz/Rstopcrouch_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.s_stopcrouchR.Add(img);

                    img = new Bitmap("jazz/Lstopcrouch_" + i + ".png");
                    img.MakeTransparent(img.GetPixel(0, 0));
                    pnn.s_stopcrouchL.Add(img);
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
            if (isShiftPressed == true)
            {
                run = 2;
            }
            if (isRightPressed == true)
            {
                if (hero.xDir == -1)
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

            if (LHero[0].yDir == -1)//elevator related
            {
                LHero[0].Y--;
            }
            if (LHero[0].yDir == 1)
            {
                LHero[0].Y++;
            }

        }

        void Jump()
        {
            //mmkn n5lyh fluid aktr lw el jump 3la 7sb days ymen wla shmal
            //bas fyh degree of difficulty fa dk b2a (if want change ask me)

            //re: azdk el howa yro7 b angle ymen aw shmal?
            if (jump_ct < jump_ct_max + min_jump && flying == true)
            {
                jumptype = 0;
                LHero[0].Y -= jump_pow;
                /*if (jump_ct > 0 && isRightPressed == true)
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
                }*/
                /*if (isDownPressed == true && isSpacePressed == true)//super jump
                {
                    jumptype = 0;
                    LHero[0].Y -= 50;
                }*/
                jump_ct++;
            }
            else
            {
                jump_ct = 0;
                jump_ct_max = 0;
                flying = false;
            }
        }

        private void AnimateHero()
        {
            CMultiImageActor hero = LHero[0];
            int old = hero.currentAnimation;

            if (isRightPressed == true)
            {
                if (isShiftPressed == true)
                {
                    runordash++;
                    if (runordash < hero.runR.Count)
                    {
                        hero.currentAnimation = 12;
                    }
                    else
                    {
                        hero.currentAnimation = 10;
                    }
                }
                else
                {
                    LHero[0].currentAnimation = 2;
                }
            }
            else if (isLeftPressed == true)
            {
                if (isShiftPressed == true)
                {
                    runordash++;
                    if (runordash < hero.runR.Count)
                    {
                        hero.currentAnimation = 13;
                    }
                    else
                    {
                        hero.currentAnimation = 11;
                    }
                }
                else
                {
                    LHero[0].currentAnimation = 3;
                }
            }
            else if (isDownPressed == true)
            {
                if (hero.xDir == 1)
                {
                    /*if (isFPressed == true)
                    {
                        hero.currentAnimation = 14;
                    }
                    else
                    {*/
                    /*if (stopFire > 0)
                    {
                        hero.currentAnimation = 6;
                    }
                    else
                    {*/
                    if (hero.iFrame == 1)
                    {
                        hero.iFrame = 0;
                    }
                    hero.currentAnimation = 14;
                    //}
                    //}
                }
                else
                {
                    /*if (isFPressed == true)
                    {
                        hero.currentAnimation = 14;
                    }
                    else
                    {*/
                    /*if (stopFire > 0)
                    {
                        hero.currentAnimation = 6;
                    }
                    else
                    {*/
                    if (hero.iFrame == 1)
                    {
                        hero.iFrame = 0;
                    }
                    hero.currentAnimation = 15;
                    //}
                    //}
                }
            }
            else if (isUpPressed == true)
            {

                if (hero.xDir == 1)
                {
                    if (hero.xDir == 1)
                    {
                        if (isFPressed == true)
                        {
                            hero.currentAnimation = 20;
                        }
                        else
                        {
                            if (stopFire > 0)
                            {
                                hero.currentAnimation = 6;
                            }
                            else
                            {
                                if (flying == true)
                                {
                                    hero.currentAnimation = 18;
                                }
                                else if (gravity == true)
                                {
                                    hero.currentAnimation = 19;
                                }
                                else
                                {
                                    if (hero.iFrame == 1)
                                    {
                                        hero.iFrame = 0;
                                    }
                                    hero.currentAnimation = 8;
                                }
                                
                            }
                        }
                    }
                }
                else
                {
                    if (isFPressed == true)
                    {
                        hero.currentAnimation = 21;
                    }
                    else
                    {
                        if (stopFire > 0)
                        {
                            hero.currentAnimation = 7;
                        }
                        else
                        {
                            if (flying == true)
                            {
                                hero.currentAnimation = 18;
                            }
                            else if (gravity == true)
                            {
                                hero.currentAnimation = 19;
                            }
                            else
                            {
                                if (hero.iFrame == 1)
                                {
                                    hero.iFrame = 0;
                                }
                                hero.currentAnimation = 9;
                            }
                        }
                    }
                }
            }
            else
            {
                if (hero.xDir == 1)
                {

                    if (isFPressed == true)
                    {
                        hero.currentAnimation = 4;
                    }
                    else
                    {
                        if (stopFire > 0)
                        {
                            hero.currentAnimation = 6;
                        }
                        else
                        {
                            if (flying == true)
                            {
                                hero.currentAnimation = 18;
                            }
                            else if(gravity == true)
                            {
                                hero.currentAnimation = 19;
                            }
                            else
                            {
                                hero.currentAnimation = 0;
                            }
                        }
                    }
                }
                else
                {
                    if (isFPressed == true)
                    {
                        hero.currentAnimation = 5;
                    }
                    else
                    {
                        if (stopFire > 0)
                        {
                            hero.currentAnimation = 7;
                        }
                        else
                        {
                            if (flying == true)
                            {
                                hero.currentAnimation = 18;
                            }
                            else if (gravity == true)
                            {
                                hero.currentAnimation = 19;
                            }
                            else
                            {
                                hero.currentAnimation = 1;
                            }
                        }
                    }
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

            if (hero.currentAnimation != old)
            {
                hero.iFrame = 0;
                if (old == 6 || old == 7)
                {
                    stopFire = 0;
                }
                if (old == 14 && hero.currentAnimation == 0)
                {
                    hero.currentAnimation = 16;
                }
                /*if((old != 12 || hero.currentAnimation == 10 && (old != 13 || hero.currentAnimation != 11))
                {
                    runordash = 0;
                }*/
            }
            else
            {
                hero.iFrame++;
            }
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
            int width = 200, margin = 10;
            cActor pnn = new cActor();
            if (LHero[0].xDir == 1)
            {
                pnn.x = LHero[0].X + LHero[0].imgs[LHero[0].iFrame].Width;
            }
            else
            {
                pnn.x = LHero[0].X - width + margin;
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
            int ntmp = ClientSize.Height / 2 - LHero[0].standL[0].Height + 16;
            if (isOnPlatform == true || isOnLadder == true || flying == true || (LHero[0].Y >= ntmp))
            {
                gravity = false;
                if(flying == false)
                {
                    jump_cd = 0;
                }
            }
            else
            {
                gravity = true;
            }
            if (gravity == true)
            {
                //clientsize.height/2 for testing (htt8yr)
                //rename ntmp
                if (LHero[0].Y <= ntmp && jumptype == 0)//jump type:anwa3 falling
                {
                    LHero[0].Y += gravity_speed;
                }
                else if (LHero[0].Y <= ntmp && jumptype == 1)
                {
                    LHero[0].Y += gravity_speed;
                    LHero[0].X++;
                }
                else if (LHero[0].Y <= ntmp && jumptype == -1)
                {
                    LHero[0].Y += gravity_speed;
                    LHero[0].X--;
                }
                else
                {
                    //
                    //flying = false;
                }
            }
        }

        void platforms()
        {
            int x=850,y=350;
            for(int i = 0; i < 2; i++)
            {   
            CMultiImageActor pnn = new CMultiImageActor();
            pnn.img = new Bitmap("jazz/platf.png");
            pnn.img.MakeTransparent(pnn.img.GetPixel(0, 0));
            pnn.X=x;
            pnn.Y=y;
            pnn.yDir=1;
            plats.Add(pnn);
                x+=100;
                y-=50;
            }
        }
        void platmove()
        {
            //elevator
            for(int i = 0; i < plats.Count; i++)
            {
                if (plats[i].yDir == -1)
                {
                plats[i].Y--;
                }
                if (plats[i].yDir == 1)
                {
                plats[i].Y++;
                }

                if (plats[i].Y <=320 )
                {
                plats[i].yDir=1;
                }
                if (plats[i].Y >=360 )
                {
                plats[i].yDir=-1;
                }
            }
        }
        void platchecker()
        {
            int heroBottom = LHero[0].Y + LHero[0].standL[0].Height/2;
            for(int i = 0; i < plats.Count; i++)
            {
                if ((heroBottom <= plats[i].Y+5 && heroBottom>=plats[i].Y)&&(LHero[0].X>=plats[i].X
                    &&LHero[0].X<=plats[i].X+plats[i].img.Width))
                {
                    flying=false;
                    isOnPlatform = true;
                    LHero[0].yDir=plats[i].yDir;
                    break;
                }
                else
                {
                    isOnPlatform = false;
                    LHero[0].yDir=0;
                }
            }
        }

        void ladders()
        {
            int x=550,y=250;
              
            CMultiImageActor pnn = new CMultiImageActor();
            pnn.img = new Bitmap("jazz/ladder.png");
            pnn.img.MakeTransparent(pnn.img.GetPixel(0, 0));
            pnn.X=x;
            pnn.Y=y;
            pnn.yDir=1;
            ladder.Add(pnn);
                
            
        }

        void ladderschecker()
        {
            int ct = 0;
            int heroBottom = LHero[0].Y + LHero[0].standL[0].Height/2;
            for(int i = 0; i < ladder.Count; i++)
            {
                if ((heroBottom <= ladder[i].Y+ladder[i].img.Height && heroBottom>=ladder[i].Y)&&(LHero[0].X>=ladder[i].X
                    &&LHero[0].X<=ladder[i].X+ladder[i].img.Width))
                {
                    ct++;
                    flying=false;
                    isOnLadder = true;
                    if (isUpPressed == true)
                    {
                        LHero[0].Y-=5;
                    }
                    if (isDownPressed == true)
                    {
                        LHero[0].Y+=5;
                    }
                    break;
                }
            }
            if(ct == 0)
            {
                isOnLadder = false;
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
            if (stopFire > 0)
            {
                stopFire--;
            }
            if (jump_cd > 0)
            {
                jump_cd--;
            }
            /*if(runordash > 0)
            {
                runordash--;
            }*/
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

            for (int i = 0; i < plats.Count; i++)
            {
                g2.DrawImage(plats[i].img,plats[i].X,plats[i].Y);
            }

            for (int i = 0; i < ladder.Count; i++)
            {
                g2.DrawImage(ladder[i].img,ladder[i].X,ladder[i].Y);
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
        public List<Bitmap> s_stopshootR = new List<Bitmap>();
        //7
        public List<Bitmap> s_stopshootL = new List<Bitmap>();
        //8
        public List<Bitmap> lookupR = new List<Bitmap>();
        //9
        public List<Bitmap> lookupL = new List<Bitmap>();
        //10
        public List<Bitmap> dashR = new List<Bitmap>();
        //11
        public List<Bitmap> dashL = new List<Bitmap>();
        //12
        public List<Bitmap> runR = new List<Bitmap>();
        //13
        public List<Bitmap> runL = new List<Bitmap>();
        //14
        public List<Bitmap> crouchR = new List<Bitmap>();
        //15
        public List<Bitmap> crouchL = new List<Bitmap>();
        //16
        public List<Bitmap> s_stopcrouchR = new List<Bitmap>();
        //17
        public List<Bitmap> s_stopcrouchL = new List<Bitmap>();
        //18
        public List<Bitmap> jumpS = new List<Bitmap>();
        //19
        public List<Bitmap> fallS = new List<Bitmap>();
        //20
        public List<Bitmap> shootupR = new List<Bitmap>();
        //21
        public List<Bitmap> shootupL = new List<Bitmap>();
        public Bitmap img;
        public List<Bitmap> imgs;
        public int iFrame;
        public int xDir = 0; 
        public int yDir = 0; 
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
                    break;
                case 5:
                    ltmp = shootL;
                    break;
                case 6:
                    ltmp = s_stopshootR;
                    break;
                case 7:
                    ltmp = s_stopshootL;
                    break;
                case 8:
                    ltmp = lookupR;
                    break;
                case 9:
                    ltmp = lookupL;
                    break;
                case 10:
                    ltmp = dashR;
                    break;
                case 11:
                    ltmp = dashL;
                    break;
                case 12:
                    ltmp = runR;
                    break;
                case 13:
                    ltmp = runL;
                    break;
                case 14:
                    ltmp = crouchR;
                    break;
                case 15:
                    ltmp = crouchL;
                    break;
                case 16:
                    ltmp = s_stopcrouchR;
                    break;
                case 17:
                    ltmp = s_stopcrouchL;
                    break;
                case 18:
                    ltmp = jumpS;
                    break;
                case 19:
                    ltmp = fallS;
                    break;
                case 20:
                    ltmp = shootupR;
                    break;
                case 21:
                    ltmp = shootupL;
                    break;
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
