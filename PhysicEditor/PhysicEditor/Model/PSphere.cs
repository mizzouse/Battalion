using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using PhysicEditor.Input;

namespace PhysicEditor.Models
{
    public class PSphere
    {
        public int ID;

        public string BoneName = "head";
        public PType Part;

        public GameModel ParentModel;

        public Vector3 AnchorPoint;
        public float Radius;

        public BoundingSphere Sphere;

        public PSphere()
        {

        }

        public PSphere(int ID, GameModel ParentModel)
        {
            this.ID = ID;
            this.ParentModel = ParentModel;
        }

        public void SetAnchorPoint(float x, float y, float z)
        {
            AnchorPoint.X = x;
            AnchorPoint.Y = y;
            AnchorPoint.Z = z;
        }

        public void SetSphere()
        {
            Sphere.Center = AnchorPoint;
            Sphere.Radius = Radius;
        }

        public void Update(Matrix BoneTransform)
        {
            Sphere.Radius = Radius;
            Sphere.Center = Vector3.Transform(AnchorPoint, BoneTransform);
        }
    }
}
