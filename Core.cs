using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace MonoBlade
{
    public class Core
    {
        public class GameObject
        {
            public int Id { get; private set; }
            public string Name { get; set; }
            public Game1 Game { get; private set; }
            public Components.SpriteComponent SpriteComponent { get; private set; }
            public Components.AudioComponent AudioComponent { get; private set; }
            public Components.InputManeger InputManeger { get; private set; }
            public Components.PositionComponent PositionComponent { get; private set; }
            public Components.ColiderComponent ColiderComponent { get; private set; }
            public GameTime GameTime { get; private set; }

            public GameObject(int id, string name, Game1 game, float X, float Y, bool AcceptInput)
            {
                Id = id;

                Name = name;

                Game = game;

                InputManeger = new Components.InputManeger(this);

                PositionComponent = new Components.PositionComponent(X, Y, AcceptInput);

                ColiderComponent = new Components.ColiderComponent(new Vector2(100,100),new Vector2(100,100),false,this);

            }

            public void Tick(GameTime GameTime)
            {
                this.GameTime = GameTime;

                if (this.PositionComponent.AcceptInput == true)
                {
                    this.InputManeger.Tick();
                }
                

                this.ColiderComponent.Tick();
            }

            public void Draw()
            {
                if (SpriteComponent != null)
                {
                    Game.spriteBatch.Draw(this.SpriteComponent.Sprite, this.PositionComponent.Position - this.SpriteComponent.OriginPoint, Color.White);
                }

                if (ColiderComponent != null)
                {
                    Game.spriteBatch.Draw(this.ColiderComponent.ColiderTexture, this.ColiderComponent.ColiderRectangle, Color.White);
                    Game.spriteBatch.Draw(this.ColiderComponent.CenterTexture, this.ColiderComponent.CenterPointRectangle, Color.White);
                }
            }
        }


        /*
         * This section contains all the GameObject Components
         */
        public class Components
        {
            public class InputManeger
            {
                private GameObject ParrentObject { get; set; }
                public InputManeger(GameObject ParrentObject)
                {
                    this.ParrentObject = ParrentObject;
                }

                public void Tick()
                {
                    float ActualSpeed = this.ParrentObject.PositionComponent.BaseSpeed * (float)this.ParrentObject.GameTime.ElapsedGameTime.TotalSeconds;

                    if (Keyboard.GetState().IsKeyDown(Keys.A) && !Keyboard.GetState().IsKeyDown(Keys.D))
                    {
                        this.ParrentObject.PositionComponent.EditAxis("X",-1);
                    }
                    else if (!Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.D))
                    {
                        this.ParrentObject.PositionComponent.EditAxis("X", 1);
                    }
                    else
                    {
                        this.ParrentObject.PositionComponent.EditAxis("X", 0);
                    }

                    // Up / Down
                    if (Keyboard.GetState().IsKeyDown(Keys.W) && !Keyboard.GetState().IsKeyDown(Keys.S))
                    {
                        this.ParrentObject.PositionComponent.EditAxis("Y", -1);
                    }
                    else if (!Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.S))
                    {
                        this.ParrentObject.PositionComponent.EditAxis("Y", 1);
                    }
                    else
                    {
                        this.ParrentObject.PositionComponent.EditAxis("Y", 0);
                    }


                    if (!this.ParrentObject.ColiderComponent.IsTrigger)
                    {
                        foreach (var item in this.ParrentObject.Game.GameObjects)
                        {
                            if (item.Name != this.ParrentObject.Name)
                            {
                                this.ParrentObject.ColiderComponent.CheckColision(item);
                            }

                        }
                    }


                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                    {
                        this.ParrentObject.PositionComponent.HardMove(this.ParrentObject.PositionComponent.Axis, ActualSpeed * 2);
                    }
                    else
                    {
                        this.ParrentObject.PositionComponent.HardMove(this.ParrentObject.PositionComponent.Axis, ActualSpeed);
                    }

                    //Console.WriteLine(this.ParrentObject.PositionComponent.Axis.X + " : " + this.ParrentObject.PositionComponent.Axis.Y);
                }
            }
            public class PositionComponent
            {
                public Vector2 Position { get; private set; }
                public Vector2 Offset { get; private set; }
                public float SpeedMultiplyer { get; private set; }
                public bool AcceptInput { get; private set; }
                public Vector2 Axis { get; private set; }
                public float BaseSpeed { get; private set; }
                public float Weight { get; private set; }

                public PositionComponent(float X, float Y, bool AcceptInput)
                {
                    this.AcceptInput = AcceptInput;
                    Position = new Vector2(X, Y);
                    Offset = new Vector2(0, 0);
                    Axis = new Vector2(0,0);
                    BaseSpeed = 50.0f;
                    SpeedMultiplyer = 1.0f;
                    Weight = 100.0f;
                }

                public void HardMove(Vector2 Axis, float Speed)
                {
                    float subSpeed = Speed * this.SpeedMultiplyer;
                    Vector2 Movement = new Vector2(Axis.X * subSpeed, Axis.Y * subSpeed);
                    //Console.WriteLine(Position.X + " : " + Position.Y);
                    Position += Movement;
                }

                public void MovePrecise(Vector2 Axis)
                {
                    this.Position -= Axis;
                }

                public void EditAxis(string axisName, float newValue)
                {
                    if (axisName.ToLower() == "x")
                    {
                        this.Axis = new Vector2(newValue, this.Axis.Y);
                    }
                    else if (axisName.ToLower() == "y")
                    {
                        this.Axis = new Vector2(this.Axis.X, newValue);
                    }
                }

                public void InflictForce(Vector2 forceVector)
                {

                }
            }

            public class SpriteComponent
            {
                public Texture2D Sprite { get; private set; }
                public Vector2 OriginPoint { get; private set; }
                public List<int> Layers { get; private set; } = new List<int>();
                public SpriteComponent()
                {

                }

                public void Draw()
                {

                }
            }

            public class ColiderComponent : Components
            {
                public Vector2 Dimensions { get; private set; }
                public Vector2 Offset { get; private set; }
                public Vector2 CenterPoint { get; private set; }
                public Vector2 Position { get; private set; }

                public bool IsTrigger { get; private set; }
                public Texture2D ColiderTexture { get; private set; }
                public Texture2D CenterTexture { get; private set; }
                public Rectangle ColiderRectangle { get; private set; }
                public Rectangle SkinColiderRectangle { get; private set; }
                public Rectangle CenterPointRectangle { get; private set; }

                public List<int> Layers { get; private set; } = new List<int>();
                private GameObject ParrentObject { get; set; }
                public ColiderComponent(Vector2 Dimensions, Vector2 Offset, bool ColiderIsTrigger, GameObject ParrentObject)
                {
                    this.Dimensions = Dimensions;
                    this.Offset = Offset;
                    this.CenterPoint = new Vector2(Convert.ToInt32((Dimensions.X / 2) - Offset.X), Convert.ToInt32((Dimensions.Y / 2) - Offset.Y));
                    this.IsTrigger = ColiderIsTrigger;

                    this.ParrentObject = ParrentObject;

                    GenerateColiderTexture();
                    RecalculateColiderPos();
                }

                private void RecalculateColiderPos()
                {
                    this.Position = ParrentObject.PositionComponent.Position - this.CenterPoint;
                    this.ColiderRectangle = new Rectangle(Convert.ToInt32(this.Position.X), Convert.ToInt32(this.Position.Y), Convert.ToInt32(this.Dimensions.X), Convert.ToInt32(this.Dimensions.Y));
                    this.SkinColiderRectangle = new Rectangle(Convert.ToInt32(this.Position.X - 1), Convert.ToInt32(this.Position.Y - 1), Convert.ToInt32(this.Dimensions.X + 2), Convert.ToInt32(this.Dimensions.Y + 2));
                    this.CenterPointRectangle = new Rectangle(Convert.ToInt32(this.Position.X - this.CenterPoint.X), Convert.ToInt32(this.Position.Y - this.CenterPoint.Y), 5, 5);
                }

                private void GenerateColiderTexture()
                {

                    this.ColiderTexture = new Texture2D(ParrentObject.Game.GraphicsDevice, Convert.ToInt32(this.Dimensions.X), Convert.ToInt32(this.Dimensions.Y));

                    Color[] data = new Color[Convert.ToInt32(this.Dimensions.X) * Convert.ToInt32(this.Dimensions.Y)];
                    for (int i = 0; i < data.Length; ++i) data[i] = new Color(150, 0, 0, 150);
                    ColiderTexture.SetData(data);

                    this.CenterTexture = new Texture2D(ParrentObject.Game.GraphicsDevice, 5, 5);

                    Color[] data2 = new Color[5 * 5];
                    for (int i = 0; i < data2.Length; ++i) data2[i] = new Color(0, 0, 150, 255);
                    CenterTexture.SetData(data2);
                }

                public void CheckColision(GameObject gameObject_2)
                {
                    /*
                     * Object Colison logic
                     */

                    if (this.ColiderRectangle.Intersects(gameObject_2.ColiderComponent.SkinColiderRectangle))
                    {
                        
                    }
                }

                public void Tick()
                {
                    RecalculateColiderPos();
                    
                }
            }

            public class AudioComponent
            {
                List<AudioClip> AudioClips = new List<AudioClip>();
                public AudioComponent()
                {

                }

                public void AddClip(string Name, SoundEffect Clip)
                {
                    AudioClips.Add(new AudioClip(Name, Clip));
                }

                public void PlayClip(string Name)
                {
                    for (int i = 0; i < AudioClips.Count; i++)
                    {
                        if (AudioClips[i].Name == Name)
                        {

                        }
                    }
                }
            }


            /*
             * This section contains sub-components that are part of the main components but
             * these sub-components shall not be initialized by themselves.
             */
            private class AudioClip
            {
                public string Name { get; private set; }
                public SoundEffect Clip { get; private set; }
                public AudioClip(string Name, SoundEffect Clip)
                {
                    this.Name = Name;
                    this.Clip = Clip;
                }
            }
        }
    }
}
