using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MonoBlade.Core;

namespace MonoBlade
{
    class Obsolite
    {
        public class ColiderComponent : Components
        {
            public Vector2 Dimensions { get; private set; }
            public Vector2 Offset { get; private set; }
            public Vector2 CenterPoint { get; private set; }
            public Vector2 Position { get; private set; }

            public bool IsTrigger { get; private set; }
            public Texture2D ColiderTexture { get; private set; }
            public Rectangle ColiderRectangle { get; private set; }
            public Rectangle SkinColiderRectangle { get; private set; }

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
            }

            private void GenerateColiderTexture()
            {

                this.ColiderTexture = new Texture2D(ParrentObject.Game.GraphicsDevice, Convert.ToInt32(this.Dimensions.X), Convert.ToInt32(this.Dimensions.Y));

                Color[] data = new Color[Convert.ToInt32(this.Dimensions.X) * Convert.ToInt32(this.Dimensions.Y)];
                for (int i = 0; i < data.Length; ++i) data[i] = new Color(150, 0, 0, 150);
                ColiderTexture.SetData(data);
            }

            public void CheckColision(GameObject gameObject_2)
            {
                /*
                 * Object Colison logic
                 */

                float DeltaX = (this.Position.X) - (gameObject_2.ColiderComponent.Position.X) + (this.CenterPoint.X - gameObject_2.ColiderComponent.CenterPoint.X);
                float DeltaY = (this.Position.Y) - (gameObject_2.ColiderComponent.Position.Y) + (this.CenterPoint.Y - gameObject_2.ColiderComponent.CenterPoint.Y);

                float LeftDeltaX = (this.Position.X) - (gameObject_2.ColiderComponent.Position.X + (gameObject_2.ColiderComponent.CenterPoint.X) * 2);
                float RightDeltaX = -((this.Position.X + this.CenterPoint.X * 2) - (gameObject_2.ColiderComponent.Position.X));
                float UpDeltaY = (this.Position.Y) - (gameObject_2.ColiderComponent.Position.Y + (gameObject_2.ColiderComponent.CenterPoint.Y) * 2);
                float DownDeltaY = -((this.Position.Y + this.CenterPoint.Y * 2) - (gameObject_2.ColiderComponent.Position.Y));

                float FinalDeltaX = 0;
                float FinalDeltaY = 0;
                float FinalDelta = 0;

                if (LeftDeltaX > RightDeltaX)
                {
                    FinalDeltaX = LeftDeltaX;
                }
                else if (LeftDeltaX < RightDeltaX)
                {
                    FinalDeltaX = RightDeltaX;
                }

                if (UpDeltaY > DownDeltaY)
                {
                    FinalDeltaY = UpDeltaY;
                }
                else if (UpDeltaY < DownDeltaY)
                {
                    FinalDeltaY = DownDeltaY;
                }



                Console.WriteLine($"Left: {LeftDeltaX}");

                Console.WriteLine($"Right: {RightDeltaX}");

                Console.WriteLine($"Up: {UpDeltaY}");

                Console.WriteLine($"Down: {DownDeltaY}");

                Console.WriteLine(FinalDelta);

                if (this.ColiderRectangle.Intersects(gameObject_2.ColiderComponent.SkinColiderRectangle))
                {
                    // X-Axis
                    if (FinalDeltaX > FinalDeltaY)
                    {
                        if (LeftDeltaX > RightDeltaX)
                        {
                            if (this.ParrentObject.PositionComponent.Axis.X < 0)
                            {
                                this.ParrentObject.PositionComponent.EditAxis("X", 0);
                            }
                        }
                        else if (LeftDeltaX < RightDeltaX)
                        {
                            if (this.ParrentObject.PositionComponent.Axis.X > 0)
                            {
                                this.ParrentObject.PositionComponent.EditAxis("X", 0);
                            }
                        }
                    }
                    else
                    {
                        // Y-Axis
                        if (UpDeltaY > DownDeltaY)
                        {
                            if (this.ParrentObject.PositionComponent.Axis.Y < 0)
                            {
                                this.ParrentObject.PositionComponent.EditAxis("Y", 0);
                            }
                        }
                        else if (UpDeltaY < DownDeltaY)
                        {
                            if (this.ParrentObject.PositionComponent.Axis.Y > 0)
                            {
                                this.ParrentObject.PositionComponent.EditAxis("Y", 0);
                            }
                        }
                    }

                }


                if (this.ColiderRectangle.Intersects(gameObject_2.ColiderComponent.ColiderRectangle))
                {
                    //ActualSpeed *= 0.1f;

                    /*
                    float DeltaX = (gameObject_1.Colider.Position.X) - (gameObject_2.Colider.Position.X);
                    float DeltaY = (gameObject_1.Colider.Position.Y) - (gameObject_2.Colider.Position.Y);

                    Console.WriteLine(DeltaX + " : " + DeltaY);
                    */


                    if (FinalDeltaY > FinalDeltaX)
                    {
                        FinalDelta = FinalDeltaY;
                        this.ParrentObject.PositionComponent.EditAxis("Y", 0);
                        if (DeltaY > 0)
                        {
                            this.ParrentObject.PositionComponent.MovePrecise(new Vector2(0, Convert.ToInt32(FinalDelta)));
                        }
                        else
                        {
                            this.ParrentObject.PositionComponent.MovePrecise(new Vector2(0, Convert.ToInt32(-FinalDelta)));
                        }
                    }
                    else
                    {
                        FinalDelta = FinalDeltaX;
                        this.ParrentObject.PositionComponent.EditAxis("X", 0);
                        if (DeltaX > 0)
                        {
                            this.ParrentObject.PositionComponent.MovePrecise(new Vector2(Convert.ToInt32(FinalDelta), 0));
                        }
                        else
                        {
                            this.ParrentObject.PositionComponent.MovePrecise(new Vector2(Convert.ToInt32(-FinalDelta), 0));
                        }

                    }

                }
            }

            public void Tick()
            {
                RecalculateColiderPos();

            }
        }
    }
}
