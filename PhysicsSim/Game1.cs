using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PhysicsSim
{
    internal enum state
    { noMove, moved, firstMove }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Dictionary<Vector2Int, int> dictionary = new Dictionary<Vector2Int, int>();
        private Dictionary<int[], int> dictionaryNew = new Dictionary<int[], int>();
        private Stopwatch framerateWatch = new Stopwatch();
        private Stopwatch framerateWatchDraw = new Stopwatch();
        private Stopwatch timeForParticlesWatch = new Stopwatch();

        //private DataTable dt = new DataTable();
        private List<Particle> particles = new List<Particle>();

        private Random rng = new Random();

        public static Texture2D square;
        private SpriteFont font;
        private bool debugging = false;
        private float framerate = 0;
        private float timePerParticle;
        private float timeForParticles = 0;
        private bool moved = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.SynchronizeWithVerticalRetrace = false;
            Window.AllowUserResizing = true;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //dt.Columns.Add("Index", typeof(int));
            //dt.Columns.Add("Material", typeof(int));
            //dt.Columns.Add("Xpos", typeof(int));
            //dt.Columns.Add("Ypos", typeof(int));
            //DataColumn[] keys = new DataColumn[1];
            //keys[0] = dt.Columns[0];
            //dt.PrimaryKey = keys;
            framerateWatch.Start();
            framerateWatchDraw.Start();
            timeForParticlesWatch.Start();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            square = Content.Load<Texture2D>("Square4");
            font = Content.Load<SpriteFont>("font");
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                Input.GetState();
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
                framerate = 1 / (float)framerateWatch.Elapsed.TotalSeconds;
                framerateWatch.Restart();
                timeForParticlesWatch.Restart();
                moved = false;
                int i;
                for (i = 0; i < particles.Count; i++)
                {
                    int x = particles[i].rectangle.X;
                    int y = particles[i].rectangle.Y;
                    //int[] pos = new int[] { particles[i].rectangle.X, particles[i].rectangle.Y };
                    particles[i].updateParticle(Window, particles, dictionaryNew);
                    //if (pos != new int[] { particles[i].rectangle.X, particles[i].rectangle.Y })
                    if (x != particles[i].rectangle.X || y != particles[i].rectangle.Y)
                    {
                        moved = true;
                        break;
                    }
                    //dictionary.Keys.ToArray()[i].X = particles[i].rectangle.X / 10;
                    //dictionary.Keys.ToArray()[i].Y = particles[i].rectangle.Y / 10;
                    //dictionaryNew.Keys.ToArray()[i] = new int[] { particles[i].rectangle.X / 10, particles[i].rectangle.Y / 10 };
                }
                if (moved)
                {
                    for (i++; i < particles.Count; i++)
                    {
                        particles[i].updateParticle(Window, particles, dictionaryNew);
                        //dictionary.Keys.ToArray()[i].X = particles[i].rectangle.X / 10;
                        //dictionary.Keys.ToArray()[i].Y = particles[i].rectangle.Y / 10;
                        //dictionaryNew.Keys.ToArray()[i] = new int[] { particles[i].rectangle.X / 10, particles[i].rectangle.Y / 10 };
                    }
                }
                timeForParticles = timeForParticlesWatch.Elapsed.Ticks;
                timePerParticle = timeForParticles / (float)particles.Count;
                if (Input.GetMouseButton(0))
                {
                    int[] mousePos = Input.MousePos();
                    if ((mousePos[0] > 0 && mousePos[0] < Window.ClientBounds.Width && mousePos[1] > 0 && mousePos[1] < Window.ClientBounds.Height))
                    {
                        mousePos = new int[] { round(mousePos[0]), round(mousePos[1]) };
                        if (!particles.Any(a => a.rectangle.X == mousePos[0] && a.rectangle.Y == mousePos[1]))
                        {
                            particles.Add(new Sand(new Rectangle(mousePos[0], mousePos[1], 10, 10), particles, Color.Yellow));
                            //dictionary.Add(new Vector2Int(particles[particles.Count - 1].rectangle.X / 10, particles[particles.Count - 1].rectangle.Y / 10), particles.Count - 1);
                            //dictionaryNew.Add(new int[] { particles[particles.Count - 1].rectangle.X / 10, particles[particles.Count - 1].rectangle.Y / 10 }, particles.Count - 1);
                        }
                        //DataRow tempRow = dt.NewRow();
                        //    tempRow[0] = particles.Count-1;
                        //    tempRow[1] = (int)particles[particles.Count-1].myMaterial;
                        //    tempRow[2] = mousePos[0];
                        //    tempRow[3] = mousePos[1];
                        //    dt.Rows.Add(tempRow);
                        //    dt.Rows[25].ItemArray[1] = 20;
                    }
                }
                if (Input.GetButtonDown(Keys.PrintScreen) || Input.GetButtonDown(Buttons.X))
                {
                    debugging = !debugging;
                    Debug.WriteLine("Started debugging");
                    if (debugging && _graphics.IsFullScreen)
                    {
                        _graphics.ToggleFullScreen();
                        _graphics.ApplyChanges();
                    }
                }
            }
            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (moved || Input.GetMouseButton(0))
            {
                GraphicsDevice.Reset();
                GraphicsDevice.Clear(Color.CornflowerBlue);
                _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
                for (int i = 0; i < particles.Count; i++)
                {
                    particles[i].Draw(_spriteBatch);
                }
                if (debugging)
                {
                    _spriteBatch.DrawString(font, "particles: " + particles.Count, new Vector2(5, 5), Color.White);
                    _spriteBatch.DrawString(font, "fps: " + framerate.ToString("F1"), new Vector2(5, 25), Color.White);
                    _spriteBatch.DrawString(font, "draw fps: " + (1 / (float)framerateWatchDraw.Elapsed.TotalSeconds).ToString("F1"), new Vector2(5, 45), Color.White);
                    framerateWatchDraw.Restart();
                    if (timePerParticle < float.MaxValue)
                    {
                        _spriteBatch.DrawString(font, "tpp: " + timePerParticle.ToString("F1"), new Vector2(5, 65), Color.White);
                    }
                    else
                    {
                        _spriteBatch.DrawString(font, "tpp: big", new Vector2(5, 65), Color.White);
                    }
                    _spriteBatch.DrawString(font, "tfap: " + timeForParticles.ToString("F1"), new Vector2(5, 85), Color.White);
                    Point s = new Point();
                    s.X = (int)rng.Next(0, 4);
                    s.Y = (int)rng.Next(0, 4);
                    Point d = new Point();
                    d.X = (int)rng.Next(0, Window.ClientBounds.Width - s.X);
                    d.Y = (int)rng.Next(0, Window.ClientBounds.Height - s.Y);

                    Rectangle destinationRectangle = new Rectangle();
                    destinationRectangle.X = s.X;
                    destinationRectangle.Y = s.Y;
                    destinationRectangle.Width = d.X;
                    destinationRectangle.Height = d.Y;

                    float transparency = (float)(rng.Next(1, 80) / 255d);
                    _spriteBatch.Draw(square, new Vector2(d.X, d.Y), new Color(255, 255, 255, 255f * transparency));
                }
                // TODO: Add your drawing code here
                _spriteBatch.End();
            }
            else
            {
                GraphicsDevice.Present();
                var a = GraphicsDevice.Textures;
            }
            base.Draw(gameTime);
        }

        private int round(double i, int v)
        {
            return (int)Math.Round(i / v) * v;
        }

        private int round(decimal i)
        {
            int v = 10;
            return (int)Math.Round(i / v) * v;
        }

        private class Particle
        {
            public Rectangle rectangle;
            public Material myMaterial;

            //public List<Particle> allTheParticles;
            private Color color;

            public enum Material
            {
                sand,
                ground,
                water,
            }

            public Particle(Rectangle _rectangle,/* List<Particle> _particles,*/ Color _color)
            {
                rectangle = _rectangle;
                myMaterial = Material.sand;
                //allTheParticles = _particles;
                color = _color;
            }

            public virtual void updateParticle(GameWindow window, List<Particle> particlesBellow, Dictionary<int[], int> dictionary)
            {
                if (myMaterial == Material.sand)
                {
                    Sand sand = (Sand)this;
                }
            }

            public void Draw(SpriteBatch _spriteBatch)
            {
                _spriteBatch.Draw(square, rectangle, color);
            }
        }

        private class Sand : Particle
        {
            public Sand(Rectangle _rectangle, List<Particle> _particles, Color _color)
                : base(_rectangle, _color)
            {
                myMaterial = Material.sand;
            }

            public override void updateParticle(GameWindow window, List<Particle> particlesBellow, Dictionary<int[], int> dictionary)
            {
                //Rectangle temp = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width, rectangle.Height);
                //bool moveDown = true;
                //List<Particle> particlesBellow = allTheParticles;
                ////List<Particle> particlesBellow = allTheParticles.FindAll(a => a.rectangle.Y >= temp.Y);
                ////List<Particle> particlesBellow = allTheParticles.FindAll(a => AdvancedMath.isDistanceLessThan(new int[] { a.rectangle.X + 5, a.rectangle.Y + 5 }, new int[] { temp.X + 5, temp.Y + 5 }, 15));

                //for (int i = 0; i < particlesBellow.Count; i++)
                //{
                //    if (particlesBellow[i].rectangle.Intersects(temp))
                //    {
                //        moveDown = false;
                //        break;
                //    }
                //}
                //if (temp.Top > window.ClientBounds.Height)
                //{
                //    return;
                //}
                //if (moveDown)
                //{
                //    rectangle.Y += 10;
                //    return;
                //}
                //bool moveRight = true;
                //temp = new Rectangle(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, rectangle.Width, rectangle.Height);
                //for (int i = 0; i < particlesBellow.Count; i++)
                //{
                //    if (particlesBellow[i].rectangle.Intersects(temp))
                //    {
                //        moveRight = false;
                //        break;
                //    }
                //}
                //if (moveRight)
                //{
                //    rectangle.Y += 10;
                //    rectangle.X += 10;
                //    return;
                //}
                //bool moveLeft = true;
                //temp = new Rectangle(rectangle.X - rectangle.Width, rectangle.Y + rectangle.Height, rectangle.Width, rectangle.Height);
                //for (int i = 0; i < particlesBellow.Count; i++)
                //{
                //    if (particlesBellow[i].rectangle.Intersects(temp))
                //    {
                //        moveLeft = false;
                //        break;
                //    }
                //}
                //if (moveLeft)
                //{
                //    rectangle.Y += 10;
                //    rectangle.X -= 10;
                //    return;
                //}

                Rectangle temp = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width, rectangle.Height);
                bool moveDown = true;

                int b;
                //if (dictionary.TryGetValue(new int[] { temp.X / 10, temp.Y / 10 }, out b))
                //{
                //}
                //if (dictionary.Any(a => a.Key == new int[] { temp.X / 10, temp.Y / 10 }))
                //{
                //}
                //List<Particle> particlesBellow = allTheParticles;
                //List<Particle> particlesBellow = allTheParticles.FindAll(a => a.rectangle.Y >= temp.Y);
                //List<Particle> particlesBellow = allTheParticles.FindAll(a => AdvancedMath.isDistanceLessThan(new int[] { a.rectangle.X + 5, a.rectangle.Y + 5 }, new int[] { temp.X + 5, temp.Y + 5 }, 15));
                /*     for (int i = 0; i < particlesBellow.Count; i++)
                     {
                         //if (particlesBellow[i].rectangle.X / 5 == temp.X / 5 && particlesBellow[i].rectangle.Y / 5 == temp.Y / 5)
                         //if (particlesBellow[i].rectangle.Intersects(temp))
                         //if (particlesBellow[i].rectangle.Location == (temp.Location))
                         if (particlesBellow[i].rectangle.X == (temp.X) && particlesBellow[i].rectangle.Y == (temp.Y))
                         {
                             moveDown = false;
                             break;
                         }
                     }*/
                if (particlesBellow.Any(a => a.rectangle.X == (temp.X) && a.rectangle.Y == (temp.Y)))
                {
                    moveDown = false;
                }
                if (temp.Top > window.ClientBounds.Height - 10)
                {
                    return;
                }
                if (moveDown)
                {
                    rectangle.Y += 10;
                    return;
                }
                bool moveRight = true;
                temp.X += rectangle.Width;

                //if (dictionary.TryGetValue(new int[] { temp.X / 10, temp.Y / 10 }, out b))
                //{
                //}

                //temp = new Rectangle(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, rectangle.Width, rectangle.Height);

                //for (int i = 0; i < particlesBellow.Count; i++)
                //{
                //    //if (particlesBellow[i].rectangle.Intersects(temp))
                //    if (particlesBellow[i].rectangle.X == (temp.X) && particlesBellow[i].rectangle.Y == (temp.Y))
                //    {
                //        moveRight = false;
                //        break;
                //    }
                //}
                if (particlesBellow.Any(a => a.rectangle.X == (temp.X) && a.rectangle.Y == (temp.Y)))
                {
                    moveRight = false;
                }
                if (moveRight)
                {
                    rectangle.Y += 10;
                    rectangle.X += 10;
                    return;
                }
                bool moveLeft = true;
                //temp = new Rectangle(rectangle.X - rectangle.Width, rectangle.Y + rectangle.Height, rectangle.Width, rectangle.Height);
                temp.X -= rectangle.Width * 2;

                //if (dictionary.TryGetValue(new int[] { temp.X / 10, temp.Y / 10 }, out b))
                //{
                //}

                //for (int i = 0; i < particlesBellow.Count; i++)
                //{
                //    //if (particlesBellow[i].rectangle.Intersects(temp))
                //    if (particlesBellow[i].rectangle.X == (temp.X) && particlesBellow[i].rectangle.Y == (temp.Y))
                //    {
                //        moveLeft = false;
                //        break;
                //    }
                //}
                if (particlesBellow.Any(a => a.rectangle.X == (temp.X) && a.rectangle.Y == (temp.Y)))
                {
                    moveLeft = false;
                }
                if (moveLeft)
                {
                    rectangle.Y += 10;
                    rectangle.X -= 10;
                    return;
                }

                //Rectangle temp = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width, rectangle.Height);
                //bool moveDown = true;
                //int value = 0;
                ////value = dictionary[new Vector2Int(temp.X / 10, temp.Y / 10)];
                //if (dictionary.ContainsKey(new Vector2Int(temp.X / 10, temp.Y / 10)) || dictionary.TryGetValue(new Vector2Int(temp.X / 10, temp.Y / 10), out value) || value != 0)
                //{
                //}
                ////List<Particle> particlesBellow = allTheParticles;
                ////List<Particle> particlesBellow = allTheParticles.FindAll(a => a.rectangle.Y >= temp.Y);
                ////List<Particle> particlesBellow = allTheParticles.FindAll(a => AdvancedMath.isDistanceLessThan(new int[] { a.rectangle.X + 5, a.rectangle.Y + 5 }, new int[] { temp.X + 5, temp.Y + 5 }, 15));

                ////if (particlesBellow[i].rectangle.X / 5 == temp.X / 5 && particlesBellow[i].rectangle.Y / 5 == temp.Y / 5)
                //if (dictionary.ContainsKey(new Vector2Int(temp.X / 10, temp.Y / 10)) || particlesBellow[value].rectangle.Intersects(temp))
                //{
                //    moveDown = false;
                //}

                //if (temp.Bottom > window.ClientBounds.Height)
                //{
                //    return;
                //}
                //if (moveDown)
                //{
                //    rectangle.Y += 10;
                //    return;
                //}
                //return;
                //bool moveRight = true;
                //temp.X += rectangle.Width;
                //dictionary.TryGetValue(new Vector2Int(temp.X / 10, temp.Y / 10), out value);
                ////temp = new Rectangle(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, rectangle.Width, rectangle.Height);
                ////for (int i = 0; i < particlesBellow.Count; i++)
                ////{
                //if (dictionary.ContainsKey(new Vector2Int(temp.X / 10, temp.Y / 10)) || particlesBellow[value].rectangle.Intersects(temp))
                //{
                //    moveRight = false;
                //}
                ////}
                //if (moveRight)
                //{
                //    rectangle.Y += 10;
                //    rectangle.X += 10;
                //    return;
                //}
                //bool moveLeft = true;
                ////temp = new Rectangle(rectangle.X - rectangle.Width, rectangle.Y + rectangle.Height, rectangle.Width, rectangle.Height);
                //temp.X -= rectangle.Width * 2;
                //dictionary.TryGetValue(new Vector2Int(temp.X / 10, temp.Y / 10), out value);
                ////for (int i = 0; i < particlesBellow.Count; i++)
                ////{
                //if (dictionary.ContainsKey(new Vector2Int(temp.X / 10, temp.Y / 10)) || particlesBellow[value].rectangle.Intersects(temp))
                //{
                //    moveLeft = false;
                //}
                ////}
                //if (moveLeft)
                //{
                //    rectangle.Y += 10;
                //    rectangle.X -= 10;
                //    return;
                //}
            }
        }

        public class Vector2Int
        {
            public int X { get; set; }

            public int Y { get; set; }

            public Vector2Int()
            {
                X = 0;
                Y = 0;
            }

            public Vector2Int(int _x)
            {
                X = _x;
                Y = 0;
            }

            public Vector2Int(int _x, int _y)
            {
                X = _x;
                Y = _y;
            }

            public static Vector2Int operator +(Vector2Int a, Vector2Int b)
            {
                return new Vector2Int(a.X + b.X, a.Y + b.Y);
            }

            public static Vector2Int operator -(Vector2Int a, Vector2Int b)
            {
                return new Vector2Int(a.X - b.X, a.Y - b.Y);
            }

            public static Vector2Int operator *(Vector2Int a, int b)
            {
                return new Vector2Int(a.X * b, a.Y * b);
            }

            public static Vector2Int operator *(Vector2Int a, float b)
            {
                return new Vector2Int((int)Math.Round(a.X * b, MidpointRounding.AwayFromZero), (int)Math.Round(a.Y * b, MidpointRounding.AwayFromZero));
            }

            public static Vector2Int operator /(Vector2Int a, int b)
            {
                return new Vector2Int(a.X / b, a.Y / b);
            }

            public static Vector2Int operator /(Vector2Int a, float b)
            {
                return new Vector2Int((int)Math.Round(a.X / b, MidpointRounding.AwayFromZero), (int)Math.Round(a.Y / b, MidpointRounding.AwayFromZero));
            }

            public static Vector2Int operator %(Vector2Int a, int b)
            {
                return new Vector2Int(a.X % b, a.Y % b);
            }

            public static Vector2Int operator %(Vector2Int a, float b)
            {
                return new Vector2Int((int)Math.Round(a.X % b, MidpointRounding.AwayFromZero), (int)Math.Round(a.Y % b, MidpointRounding.AwayFromZero));
            }

            public static readonly Vector2Int One = new Vector2Int(1, 1);
            public static readonly Vector2Int Zero = new Vector2Int(0, 0);

            public float Distance(Vector2Int b)
            {
                return AdvancedMath.Vector2Distance(new Vector2(X, Y), new Vector2(b.X, b.Y));
            }

            public Vector2Int Normalize
            {
                get
                {
                    Vector2 temp = AdvancedMath.Normalize(new Vector2(X, Y));
                    return new Vector2Int((int)Math.Round(temp.X, MidpointRounding.AwayFromZero), (int)Math.Round(temp.Y, MidpointRounding.AwayFromZero));
                }
            }
        }
    }
}