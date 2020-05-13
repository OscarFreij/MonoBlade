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
        public class PhysicsEngine
        {
            public class Core
            {
                public float GravityConstant { get; private set; }
                public float Airdensity { get; private set; }
                public float Airdensitydegrade { get; private set; }
                public int Groundlevel { get; private set; }

                public Game1 Game { get; private set; }
                public GameTime GameTime { get; private set; }

                public Core(float GravityConstant, Game1 Game)
                {
                    this.GravityConstant = GravityConstant;
                    this.Game = Game;
                }

                public Core(float GravityConstant, int Groundlevel, Game1 Game)
                {
                    this.GravityConstant = GravityConstant;
                    this.Groundlevel = Groundlevel;
                }

                public Core(float GravityConstant, int Groundlevel, float Airdensity, float Airdensitydegrade, Game1 Game)
                {
                    this.GravityConstant = GravityConstant;
                    this.Groundlevel = Groundlevel;
                    this.Airdensity = Airdensity;
                    this.Airdensitydegrade = Airdensitydegrade;
                }

                public void Tick(GameTime gameTime)
                {
                    this.GameTime = GameTime;
                }
            }

            public class Components
            {
                public class PAO //Physics Affecteble Object
                {

                }
            }

        }

        public class GameObject
        {
            public int Id { get; private set; }
            public string Name { get; set; }
            public int Team { get; private set; }
            public Game1 Game { get; private set; }
            public Components.SpriteComponent SpriteComponent { get; private set; }
            public Components.AudioComponent AudioComponent { get; private set; }
            public Components.InputManeger InputManeger { get; private set; }
            public Components.PositionComponent PositionComponent { get; private set; }
            public Components.ColliderComponent ColliderComponent { get; private set; }
            public Components.HealthComponent HealthComponent { get; private set; }
            public Components.AI AI { get; private set; }
            public GameTime GameTime { get; private set; }

            public GameObject(int id, string name, Game1 game, float X, float Y, bool AcceptInput, Vector2 ColliderDimentions, Vector2 ColliderOffset, bool ColliderIsTrigger, bool HasAI , int Team, float MaxSpeed, float Acceleration, float MaxHealth, float DamageOnTouch)
            {
                Id = id;

                Name = name;

                Game = game;

                this.Team = Team;

                InputManeger = new Components.InputManeger(this);

                PositionComponent = new Components.PositionComponent(X, Y, AcceptInput, MaxSpeed, Acceleration, this);

                ColliderComponent = new Components.ColliderComponent(ColliderDimentions, ColliderOffset, ColliderIsTrigger, this);

                SpriteComponent = new Components.SpriteComponent(this);
                
                if (MaxHealth > 0)
                {
                    HealthComponent = new Components.HealthComponent(MaxHealth, DamageOnTouch, this);
                }

                if (HasAI)
                {
                    AI = new Components.AI(Team, this);
                }
                
            }

            public void Tick(GameTime GameTime)
            {
                this.GameTime = GameTime;

                if (this.AI != null)
                {
                    this.AI.Tick();
                }

                this.InputManeger.Tick();
            
                this.ColliderComponent.Tick();

                if (this.HealthComponent != null)
                {
                    this.HealthComponent.Tick();
                }
            }

            public void Draw()
            {
                if (SpriteComponent != null)
                {
                    Game.spriteBatch.Draw(this.SpriteComponent.Sprite, this.PositionComponent.Position - this.SpriteComponent.OriginPoint, Color.White);
                }

                if (ColliderComponent != null)
                {
                    Texture2D ColliderTexture = new Texture2D(Game.GraphicsDevice,Convert.ToInt32(this.ColliderComponent.Dimensions.X + 2), Convert.ToInt32(this.ColliderComponent.Dimensions.Y + 2));
                    Color[] data = new Color[Convert.ToInt32(this.ColliderComponent.Dimensions.X + 2) * Convert.ToInt32(this.ColliderComponent.Dimensions.Y + 2)];
                    for (int i = 0; i < data.Length; ++i) data[i] = new Color(0, 280, 0, 40);
                    ColliderTexture.SetData(data);

                    Game.spriteBatch.Draw(ColliderTexture, this.ColliderComponent.SkinColliderRectangle, Color.White);
                    Game.spriteBatch.Draw(this.ColliderComponent.ColliderTexture, this.ColliderComponent.ColliderRectangle, Color.White);
                    Game.spriteBatch.Draw(this.ColliderComponent.CenterTexture, this.ColliderComponent.CenterPointRectangle, Color.White);
                }
            }
        }


        /*
         * This section contains all the GameObject Components
         */
        public class Components
        {
            public class HealthComponent
            {
                public GameObject ParrentObject { get; private set; }
                public float Health { get; private set; }
                public float MaxHealth { get; private set; }
                public float DamageOnTouch { get; private set; }

                public HealthComponent(float MaxHealth, float DamageOnTouch, GameObject ParrentObject)
                {
                    this.ParrentObject = ParrentObject;
                    this.MaxHealth = MaxHealth;
                    this.Health = this.MaxHealth;
                    this.DamageOnTouch = DamageOnTouch;
                }

                public void DealDamage(float DamageValue)
                {
                    this.Health -= DamageValue*(float)this.ParrentObject.GameTime.ElapsedGameTime.TotalSeconds;
                }

                public void Tick()
                {
                    if (Health < 0)
                    {
                        // Kill and remove this item from list
                        this.ParrentObject.Game.GameObjects.Remove(this.ParrentObject);
                    }
                }
            }

            public class AI
            {
                public GameObject Target { get; private set; }
                public int Team { get; private set; }
                public GameObject ParrentObject { get; private set; }

                public AI(int Team, GameObject ParrentObject)
                {
                    this.ParrentObject = ParrentObject;
                    this.Team = Team;
                }

                public void Tick()
                {
                    double TargetDelta;

                    if (Target == null)
                    {
                        TargetDelta = 9999999999;
                    }
                    else
                    {
                        TargetDelta = Math.Abs((this.ParrentObject.PositionComponent.Position.X - Target.PositionComponent.Position.X));
                    }

                    foreach (GameObject PotentialTarget in this.ParrentObject.Game.GameObjects)
                    {
                        if (this.ParrentObject.Team != PotentialTarget.Team && PotentialTarget.Team != 0 && PotentialTarget.Team != 3)
                        {
                            double Delta = Math.Abs((this.ParrentObject.PositionComponent.Position.X - PotentialTarget.PositionComponent.Position.X));
                            if (Delta <= TargetDelta)
                            {
                                this.Target = PotentialTarget;
                            }
                        }
                    }


                    try
                    {
                        if (this.ParrentObject.PositionComponent.Position.X > this.Target.PositionComponent.Position.X)
                        {
                            this.ParrentObject.PositionComponent.EditAxis("X", -1);
                        }
                        else if (this.ParrentObject.PositionComponent.Position.X < this.Target.PositionComponent.Position.X)
                        {
                            this.ParrentObject.PositionComponent.EditAxis("X", 1);
                        }
                        else
                        {
                            this.ParrentObject.PositionComponent.EditAxis("X", 0);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("AI ERROR: Can not get direction of target");
                    }
                }
            }

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

                    if (this.ParrentObject.PositionComponent.AcceptInput)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.A) && !Keyboard.GetState().IsKeyDown(Keys.D))
                        {
                            this.ParrentObject.PositionComponent.EditAxis("X", -1);
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


                        if (!this.ParrentObject.ColliderComponent.IsTrigger)
                        {
                            foreach (var item in this.ParrentObject.Game.GameObjects)
                            {
                                if (item.Name != this.ParrentObject.Name)
                                {
                                    this.ParrentObject.ColliderComponent.CheckColision(item);
                                }

                            }
                        }

                        Console.WriteLine($"AXIS-X = {this.ParrentObject.PositionComponent.Axis.X} | AXIS-Y = {this.ParrentObject.PositionComponent.Axis.Y}");

                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                        {
                            //this.ParrentObject.PositionComponent.HardMove(this.ParrentObject.PositionComponent.Axis, ActualSpeed * 2);
                            this.ParrentObject.PositionComponent.Tick(true);
                        }
                        else
                        {
                            //this.ParrentObject.PositionComponent.HardMove(this.ParrentObject.PositionComponent.Axis, ActualSpeed);
                            this.ParrentObject.PositionComponent.Tick(false);
                        }

                        //Console.WriteLine(this.ParrentObject.PositionComponent.Axis.X + " : " + this.ParrentObject.PositionComponent.Axis.Y);
                    }
                    else if (this.ParrentObject.AI != null)
                    {
                        if (!this.ParrentObject.ColliderComponent.IsTrigger)
                        {
                            foreach (var item in this.ParrentObject.Game.GameObjects)
                            {
                                if (item.Name != this.ParrentObject.Name)
                                {
                                    this.ParrentObject.ColliderComponent.CheckColision(item);
                                }

                            }
                        }
                        this.ParrentObject.PositionComponent.Tick(false);
                    }
                    else if (this.ParrentObject.Team == 3)
                    {
                        if (!this.ParrentObject.ColliderComponent.IsTrigger)
                        {
                            foreach (var item in this.ParrentObject.Game.GameObjects)
                            {
                                if (item.Name != this.ParrentObject.Name)
                                {
                                    this.ParrentObject.ColliderComponent.CheckColision(item);
                                }

                            }
                        }

                        this.ParrentObject.PositionComponent.Tick(false);

                    }
                    else
                    {
                        /*
                         * Do nothing.
                         * Object is static
                         */
                    }
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
                public float MaxSpeed { get; private set; }
                public Vector2 Speed { get; private set; }
                public float Acceleration { get; private set; }
                public float SprintAcceleration { get; private set; }
                public float StopForce { get; private set; }
                public float Weight { get; private set; }
                public GameObject ParrentObject { get; private set; }

                public Vector2 CurrentAccelerationForce { get; private set; }

                public PositionComponent(float X, float Y, bool AcceptInput, float MaxSpeed, float AccelerationForce, GameObject ParrenObject)
                {
                    this.ParrentObject = ParrenObject;
                    this.AcceptInput = AcceptInput;
                    Position = new Vector2(X, Y);
                    Offset = new Vector2(0, 0);
                    Axis = new Vector2(0,0);
                    Speed = new Vector2(0,0);
                    CurrentAccelerationForce = new Vector2(0,0);
                    BaseSpeed = 20.0f;
                    this.MaxSpeed = MaxSpeed;

                    Acceleration = AccelerationForce;
                    SprintAcceleration = Acceleration*3;
                    StopForce = Acceleration*40;

                    SpeedMultiplyer = 1.0f;
                    Weight = 1.5f;
                }

                public void HardMove(Vector2 Axis, float Speed)
                {
                    float subSpeed = Speed * this.SpeedMultiplyer;
                    Vector2 Movement = new Vector2(Axis.X * subSpeed, Axis.Y * subSpeed);
                    //Console.WriteLine(Position.X + " : " + Position.Y);
                    Position += Movement;
                }

                public void DirectSpeedEdit(float X, float Y)
                {
                    Speed = new Vector2(Speed.X * X, Speed.Y * Y);
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

                public void Tick(bool isRunning)
                {
                    Vector2 LocalForce = new Vector2(0, 0);

                    if (Axis.X != 0)
                    {
                        if (!isRunning)
                        {
                            LocalForce.X = (this.Acceleration * Axis.X) / this.Weight;
                        }
                        else if (isRunning)
                        {
                            LocalForce.X = (this.SprintAcceleration * Axis.X) / this.Weight;
                        }

                        if ((Speed.X > 0 && Axis.X == -1) || (Speed.X < 0 && Axis.X == 1))
                        {
                            Console.WriteLine("Moving in Oposite Direction of Disired Direction");
                            LocalForce.X = (this.Acceleration * Axis.X) / this.Weight;
                            LocalForce.X += -(this.StopForce * (Speed.X / MaxSpeed)) / Weight;
                        }

                    }
                    else
                    {
                        if (Speed.X > 0)
                        {
                            LocalForce.X = -(this.StopForce * (Speed.X / MaxSpeed)) / Weight;
                        }
                        else if (Speed.X < 0)
                        {
                            LocalForce.X = -(this.StopForce * (Speed.X / MaxSpeed)) / Weight;
                        }
                    }

                    /*
                     * Movement in the Y-Axis (No Gravity)
                     */
                    /*
                   if (Axis.Y != 0)
                   {
                       if (!isRunning)
                       {
                           LocalForce.Y = (this.Acceleration * Axis.Y) / this.Weight;
                       }
                       else if (isRunning)
                       {
                           LocalForce.Y = (this.SprintAcceleration * Axis.Y) / this.Weight;
                       }
                   }
                   else
                   {
                       if (Speed.Y > 0)
                       {
                           LocalForce.Y = -(this.StopForce * (Speed.Y / MaxSpeed)) / Weight;
                       }
                       else if (Speed.Y < 0)
                       {
                           LocalForce.Y = -(this.StopForce * (Speed.Y / MaxSpeed)) / Weight;
                       }
                   }
                   */


                    /*
                     * Movement in the Y-Axis (with Gravity and jumping)
                     */

                    if (Axis.Y < 0 && this.ParrentObject.ColliderComponent.IsOnGround)
                    {
                        LocalForce.Y = -(800.0f) / this.Weight;
                    }
                    else if (!this.ParrentObject.ColliderComponent.IsOnGround)
                    {
                        LocalForce.Y = this.ParrentObject.Game.PhysicsCore.GravityConstant * this.Weight;
                    }
                    else
                    {
                        LocalForce.Y = 0.0f;
                    }


                    CurrentAccelerationForce = new Vector2(LocalForce.X * (float)this.ParrentObject.GameTime.ElapsedGameTime.TotalSeconds, LocalForce.Y * (float)this.ParrentObject.GameTime.ElapsedGameTime.TotalSeconds);

                    Speed = new Vector2(Speed.X + CurrentAccelerationForce.X, Speed.Y + CurrentAccelerationForce.Y);
                    Console.WriteLine($"SPEED-X = {this.Speed.X} | SPEED-Y = {this.Speed.Y}");

                    if (Math.Abs(Speed.X) < 0.01f)
                    {
                        Speed = new Vector2(0, Speed.Y);
                    }

                    if (Math.Abs(Speed.Y) < 0.01f)
                    {
                        Speed = new Vector2(Speed.X, 0);
                    }


                    Position += Speed;

                }
            }

            public class SpriteComponent
            {
                public Texture2D Sprite { get; private set; }
                public Vector2 OriginPoint { get; private set; }
                public List<int> Layers { get; private set; } = new List<int>();
                public GameObject ParrentObject { get; private set; }
                public SpriteComponent(GameObject ParrentObject)
                {
                    this.ParrentObject = ParrentObject;

                    if (this.ParrentObject.Name.Contains("Player"))
                    {
                        Sprite = this.ParrentObject.Game.Content.Load<Texture2D>("sprites/Player");
                    }
                    else if (this.ParrentObject.Name.Contains("Enemy"))
                    {
                        Sprite = this.ParrentObject.Game.Content.Load<Texture2D>("sprites/Enemy");
                    }
                    else if (this.ParrentObject.Name.Contains("Bomb"))
                    {
                        Sprite = this.ParrentObject.Game.Content.Load<Texture2D>("sprites/Bomb");
                    }
                    else
                    {
                        Sprite = this.ParrentObject.Game.Content.Load<Texture2D>("random1");
                    }

                    
                    this.OriginPoint = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
                }

                public void Draw()
                {
                    this.ParrentObject.Game.spriteBatch.Draw(Sprite, this.ParrentObject.PositionComponent.Position - OriginPoint, Color.White);
                }
            }

            public class ColliderComponent : Components
            {
                public Vector2 Dimensions { get; private set; }
                public Vector2 Offset { get; private set; }
                public Vector2 CenterPoint { get; private set; }
                public Vector2 Position { get; private set; }

                public bool IsTrigger { get; private set; }
                public bool IsOnGround { get; private set; }
                public Texture2D ColliderTexture { get; private set; }
                public Texture2D CenterTexture { get; private set; }
                public Rectangle ColliderRectangle { get; private set; }
                public Rectangle SkinColliderRectangle { get; private set; }
                public Rectangle CenterPointRectangle { get; private set; }

                public List<int> Layers { get; private set; } = new List<int>();
                private GameObject ParrentObject { get; set; }
                public ColliderComponent(Vector2 Dimensions, Vector2 Offset, bool ColiderIsTrigger, GameObject ParrentObject)
                {
                    this.Dimensions = Dimensions;
                    this.Offset = Offset;
                    this.ParrentObject = ParrentObject;
                    this.IsTrigger = ColiderIsTrigger;
                    this.IsOnGround = false;
                    this.Position = ParrentObject.PositionComponent.Position;
                    this.CenterPoint = (this.Dimensions / 2);


                    GenerateColliderTexture();
                    RecalculateColliderPos();
                }

                private void RecalculateColliderPos()
                {
                    this.Position = ParrentObject.PositionComponent.Position;
                    this.ColliderRectangle = new Rectangle(Convert.ToInt32(this.Position.X - CenterPoint.X), Convert.ToInt32(this.Position.Y - CenterPoint.Y), Convert.ToInt32(this.Dimensions.X), Convert.ToInt32(this.Dimensions.Y));
                    this.SkinColliderRectangle = new Rectangle(Convert.ToInt32((this.Position.X - CenterPoint.X) - 1), Convert.ToInt32((this.Position.Y - CenterPoint.Y) - 1), Convert.ToInt32(this.Dimensions.X + 2), Convert.ToInt32(this.Dimensions.Y + 2));
                    this.CenterPointRectangle = new Rectangle(Convert.ToInt32(this.Position.X)-8, Convert.ToInt32(this.Position.Y)-8, 16, 16);
                }


                private void GenerateColliderTexture()
                {

                    this.ColliderTexture = new Texture2D(ParrentObject.Game.GraphicsDevice, Convert.ToInt32(this.Dimensions.X), Convert.ToInt32(this.Dimensions.Y));

                    Color[] data = new Color[Convert.ToInt32(this.Dimensions.X) * Convert.ToInt32(this.Dimensions.Y)];
                    for (int i = 0; i < data.Length; ++i) data[i] = new Color(150, 0, 0, 150);
                    ColliderTexture.SetData(data);

                    this.CenterTexture = new Texture2D(ParrentObject.Game.GraphicsDevice, 5, 5);

                    Color[] data2 = new Color[5 * 5];
                    for (int i = 0; i < data2.Length; ++i) data2[i] = new Color(0, 0, 150, 50);
                    CenterTexture.SetData(data2);
                }

                public void CheckColision(GameObject gameObject_2)
                {
                    /*
                     * Object Colison logic
                     */

                    float deltaX = this.Position.X - gameObject_2.ColliderComponent.Position.X;
                    float deltaY = this.Position.Y - gameObject_2.ColliderComponent.Position.Y;

                    float deltaXP = deltaX / (gameObject_2.ColliderComponent.Dimensions.X/2);
                    float deltaYP = deltaY / (gameObject_2.ColliderComponent.Dimensions.Y/2);

                    if (this.SkinColliderRectangle.Intersects(gameObject_2.ColliderComponent.SkinColliderRectangle))
                    {
                        if (this.ParrentObject.Team != 0 && gameObject_2.Team != 0)
                        {
                            Console.WriteLine($"Collition Betweene {this.ParrentObject.Name} and {gameObject_2.Name}"); 
                        }
                        
                        if (this.ParrentObject.Team != 0 && gameObject_2.Team != 0 && this.ParrentObject.Team != gameObject_2.Team)
                        {
                            gameObject_2.HealthComponent.DealDamage(this.ParrentObject.HealthComponent.DamageOnTouch);
                            this.ParrentObject.HealthComponent.DealDamage(gameObject_2.HealthComponent.DamageOnTouch);
                        }

                        //Console.WriteLine($"{this.ParrentObject.Name} <=> DELTA-X = {deltaX} | DELTA-Y = {deltaY}");
                        //Console.WriteLine($"{this.ParrentObject.Name} <=> DELTA-X-Parsed = {deltaXP} | DELTA-Y-Parsed = {deltaYP}");

                        if (gameObject_2.Name.Contains("Ground"))
                        {
                            this.IsOnGround = true;
                            Console.WriteLine($"{this.ParrentObject.Name} is touching ground!");
                        }

                        //Console.WriteLine("Skin Contact");
                        if (Math.Abs(deltaXP) >= Math.Abs(deltaYP))
                        {
                            if (deltaXP >= 0 && this.ParrentObject.PositionComponent.Axis.X < 0)
                            {
                                //Console.WriteLine("X+");
                                this.ParrentObject.PositionComponent.EditAxis("X", 0);
                                this.ParrentObject.PositionComponent.DirectSpeedEdit(0, 1);
                            }
                            else if (deltaXP < 0 && this.ParrentObject.PositionComponent.Axis.X > 0)
                            {
                                //Console.WriteLine("X-");
                                this.ParrentObject.PositionComponent.EditAxis("X", 0);
                                this.ParrentObject.PositionComponent.DirectSpeedEdit(0, 1);
                            }

                        }
                        else
                        {
                            if (deltaYP >= 0 && this.ParrentObject.PositionComponent.Axis.Y < 0)
                            {
                                //Console.WriteLine("Y+");
                                this.ParrentObject.PositionComponent.EditAxis("Y", 0);
                                this.ParrentObject.PositionComponent.DirectSpeedEdit(1, 0);
                            }
                            else if (deltaYP < 0 && this.ParrentObject.PositionComponent.Axis.Y > 0)
                            {
                                //Console.WriteLine("Y-");
                                this.ParrentObject.PositionComponent.EditAxis("Y", 0);
                                this.ParrentObject.PositionComponent.DirectSpeedEdit(1, 0);
                            }
                        }
                    }

                    if (this.ColliderRectangle.Intersects(gameObject_2.ColliderComponent.SkinColliderRectangle) && !this.IsTrigger && !gameObject_2.ColliderComponent.IsTrigger)
                    {
                        if (Math.Abs(deltaXP) >= Math.Abs(deltaYP))
                        {
                            if (deltaXP >= 0)
                            {
                                Console.WriteLine($"Collision betweene {this.ParrentObject.Name} and {gameObject_2.Name}\nLeft of {this.ParrentObject.Name}");
                                this.ParrentObject.PositionComponent.MovePrecise(new Vector2(-1,0));
                                this.ParrentObject.PositionComponent.DirectSpeedEdit(0, 1);
                            }
                            else
                            {
                                Console.WriteLine($"Collision betweene {this.ParrentObject.Name} and {gameObject_2.Name}\nRight of {this.ParrentObject.Name}");
                                this.ParrentObject.PositionComponent.MovePrecise(new Vector2(1, 0));
                                this.ParrentObject.PositionComponent.DirectSpeedEdit(0, 1);
                            }
                            
                        }
                        else
                        {
                            if (deltaYP >= 0)
                            {
                                Console.WriteLine($"Collision betweene {this.ParrentObject.Name} and {gameObject_2.Name}\nAbove {this.ParrentObject.Name}");
                                this.ParrentObject.PositionComponent.MovePrecise(new Vector2(0, -1));
                                this.ParrentObject.PositionComponent.DirectSpeedEdit(1, 0);
                            }
                            else
                            {
                                Console.WriteLine($"Collision betweene {this.ParrentObject.Name} and {gameObject_2.Name}\nBellow {this.ParrentObject.Name}");
                                this.ParrentObject.PositionComponent.MovePrecise(new Vector2(0, 1));
                                this.ParrentObject.PositionComponent.DirectSpeedEdit(1, 0);
                            }
                        }
                        //Console.WriteLine($"Collision Detected betweene {this.ParrentObject.Name} and {gameObject_2.Name}\nDistance X: {deltaX} | Distance Y: {deltaY}");
                    }
                }

                public void Tick()
                {
                    RecalculateColliderPos();
                    //Console.WriteLine(IsOnGround);
                    this.IsOnGround = false;
                }
            }

            public class Animation
            {

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
