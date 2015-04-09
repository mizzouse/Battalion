using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicEditor.Models
{
    public class RenderSphere
    {
        static VertexBuffer vertBuffer;
        static BasicEffect effect;
        static int sphereResolution;

        //
        //http://codingquirks.com/2011/01/render-boundingsphere-in-xna-4-0/
        //
        public static void InitializeGraphics(GraphicsDevice graphicsDevice, int sphereResolution)
        {
            RenderSphere.sphereResolution = sphereResolution;

            effect = new BasicEffect(graphicsDevice);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = false;

            VertexPositionColor[] verts = new VertexPositionColor[(sphereResolution + 1) * 3];

            int index = 0;

            float step = MathHelper.TwoPi / (float)sphereResolution;

            //create the loop on the XY plane first
            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
            {
                verts[index++] = new VertexPositionColor(
                    new Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f),
                    Color.White);
            }

            //next on the XZ plane
            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
            {
                verts[index++] = new VertexPositionColor(
                    new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a)),
                    Color.White);
            }

            //finally on the YZ plane
            for (float a = 0f; a <= MathHelper.TwoPi; a += step)
            {
                verts[index++] = new VertexPositionColor(
                    new Vector3(0f, (float)Math.Cos(a), (float)Math.Sin(a)),
                    Color.White);
            }

            vertBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), verts.Length, BufferUsage.None);
            vertBuffer.SetData(verts);
        }

        public static void Render(HashSet<PSphere> spheres,
           GraphicsDevice graphicsDevice,
           Color color)
        {
            foreach (PSphere sphere in spheres)
            {
                Render(sphere.Sphere, graphicsDevice, color, sphere.ParentModel.Scale);
            }
        }

        public static void Render(
           BoundingSphere sphere,
           GraphicsDevice graphicsDevice,
           Color color,
            float Scale)
        {


            if (vertBuffer == null)
                InitializeGraphics(graphicsDevice, 30);

            graphicsDevice.SetVertexBuffer(vertBuffer);
            //S * R * T
            effect.World =
                  Matrix.CreateScale(sphere.Radius + Scale) *
                  Matrix.CreateTranslation(sphere.Center);

            effect.View = Camera.View;
            effect.Projection = Camera.Projection;
            effect.DiffuseColor = color.ToVector3();

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                //render each circle individually
                graphicsDevice.DrawPrimitives(
                      PrimitiveType.LineStrip,
                      0,
                      sphereResolution);
                graphicsDevice.DrawPrimitives(
                      PrimitiveType.LineStrip,
                      sphereResolution + 1,
                      sphereResolution);
                graphicsDevice.DrawPrimitives(
                      PrimitiveType.LineStrip,
                      (sphereResolution + 1) * 2,
                      sphereResolution);

            }

        }
    }
}
