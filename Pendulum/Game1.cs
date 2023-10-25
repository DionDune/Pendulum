using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pendulum
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        List<Keys> PrevPresses = new List<Keys>();
        bool PrevClicking_Right;


        List<Pendulum> Pendulums = new List<Pendulum>();
        int PendulumSelectedIndex;
        bool PendulemFixedLength;

        float GravityForce;
        bool GamePaused;

        Texture2D Color_White;



        #region Initialize

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 1000;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            GamePaused = false;
            GravityForce = 1;
            PendulemFixedLength = false;

            PendulumSelectedIndex = -1;
            PrevClicking_Right = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Color_White = Content.Load<Texture2D>("Colour_White");
        }

        #endregion

        #region Main

        private void Main()
        {
            foreach (Pendulum pendulum in Pendulums)
            {
                pendulum.Update(GravityForce);
            }
        }

        private void MovePendulum(int x, int y)
        {
            if (PendulumSelectedIndex >= 0 && Pendulums.Count != 0)
            {
                Pendulums[PendulumSelectedIndex].angle_Velocity = 0;

                Pendulums[PendulumSelectedIndex].pendulumLength = (float)Math.Sqrt(Math.Pow((double)x - Pendulums[PendulumSelectedIndex].CentrePoint_X, 2) +
                                                    Math.Pow((double)y - Pendulums[PendulumSelectedIndex].CentrePoint_Y, 2));

                Pendulums[PendulumSelectedIndex].angle = (float)Math.Atan2((double)x - Pendulums[PendulumSelectedIndex].CentrePoint_X,
                                            (double)y - Pendulums[PendulumSelectedIndex].CentrePoint_Y);
            }
        }
        private void ChangePendulum()
        {
            if (PendulumSelectedIndex + 1 >= Pendulums.Count)
            {
                PendulumSelectedIndex = -1;
            }
            else
            {
                PendulumSelectedIndex++;
            }
        }
        private void CreatePendulum(float PendulumLength)
        {
            if (PendulumSelectedIndex < Pendulums.Count && PendulumSelectedIndex != -1)
            {
                if (PendulemFixedLength)
                {
                    Pendulums[PendulumSelectedIndex].Children.Add(new Pendulum()
                    {
                        CentrePoint_X = Pendulums[PendulumSelectedIndex].x,
                        CentrePoint_Y = Pendulums[PendulumSelectedIndex].y,
                        pendulumLength = PendulumLength,
                        angle = (float)Math.Atan2((double)Mouse.GetState().X - Pendulums[PendulumSelectedIndex].x,
                                            (double)Mouse.GetState().Y - Pendulums[PendulumSelectedIndex].y),
                        Parent = Pendulums[PendulumSelectedIndex]
                    });
                }
                else
                {
                    Pendulums[PendulumSelectedIndex].Children.Add(new Pendulum()
                    {
                        CentrePoint_X = Pendulums[PendulumSelectedIndex].x,
                        CentrePoint_Y = Pendulums[PendulumSelectedIndex].y,
                        pendulumLength = (float)Math.Sqrt(Math.Pow((double)Mouse.GetState().X - Pendulums[PendulumSelectedIndex].x, 2) +
                                                    Math.Pow((double)Mouse.GetState().Y - Pendulums[PendulumSelectedIndex].y, 2)),
                        angle = (float)Math.Atan2((double)Mouse.GetState().X - Pendulums[PendulumSelectedIndex].x,
                                            (double)Mouse.GetState().Y - Pendulums[PendulumSelectedIndex].y),
                        Parent = Pendulums[PendulumSelectedIndex]
                    });
                }
                
                Pendulums.Add(Pendulums[PendulumSelectedIndex].Children.Last());
            }
            else if (PendulumSelectedIndex == -1)
            {
                Pendulums.Add(new Pendulum()
                {
                    CentrePoint_X = Mouse.GetState().X,
                    CentrePoint_Y = Mouse.GetState().Y,
                    pendulumLength = PendulumLength
                });
            }

            if (GamePaused)
            {
                Pendulums.Last().Update(GravityForce);
            }
        }
        private void ClearPendulums()
        {
            Pendulums.Clear();
            PendulumSelectedIndex = -1;
        }

        private void MouseClickHandler()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                MovePendulum(Mouse.GetState().X, Mouse.GetState().Y);
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed && !PrevClicking_Right)
            {
                PrevClicking_Right = true;

                CreatePendulum (50);
                
            }
            else if (Mouse.GetState().RightButton != ButtonState.Pressed)
            {
                PrevClicking_Right = false;
            }

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                CreatePendulum(100);
            }
        }
        private void KeyPressHandler()
        {
            List<Keys> CurrentPresses = new List<Keys>( Keyboard.GetState().GetPressedKeys() );

            if (CurrentPresses.Contains(Keys.Tab) && !PrevPresses.Contains(Keys.Tab))
            {
                ChangePendulum();
            }
            if (CurrentPresses.Contains(Keys.Escape) && !PrevPresses.Contains(Keys.Escape))
            {
                GamePaused = !GamePaused;
            }
            if (CurrentPresses.Contains(Keys.C))
            {
                ClearPendulums();
            }

            PrevPresses = CurrentPresses;
        }

        #endregion

        #region Fundamentals

        protected override void Update(GameTime gameTime)
        {
            if (!GamePaused)
            {
                Main();
            } 
            MouseClickHandler();
            KeyPressHandler();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!GamePaused)
            {
                GraphicsDevice.Clear(Color.White);
            }
            else
            {
                GraphicsDevice.Clear(Color.Gray);
            }



            _spriteBatch.Begin();


            //Pendulums
            foreach (Pendulum pendulum in Pendulums)
            {
                if (PendulumSelectedIndex == Pendulums.IndexOf(pendulum))
                {
                    _spriteBatch.Draw(Color_White, new Rectangle((int)pendulum.x - 6, (int)pendulum.y - 6, 12, 12), Color.Gold);
                }
                else
                {
                    _spriteBatch.Draw(Color_White, new Rectangle((int)pendulum.x - 6, (int)pendulum.y - 6, 12, 12), Color.Black);
                }

                //CentrePoints
                if (pendulum.Parent == null)
                {
                    _spriteBatch.Draw(Color_White, new Rectangle((int)pendulum.CentrePoint_X - 5, (int)pendulum.CentrePoint_Y - 5, 10, 10), Color.Red);
                }
            }


            _spriteBatch.End();



            base.Draw(gameTime);
        }

        #endregion
    }
}