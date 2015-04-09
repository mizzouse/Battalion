using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SkinnedModel;


using PhysicEditor;

namespace PhysicEditor.Models
{
    public class GameModel
    {
        private bool Loaded;

        public int ID;
        public Model model;
        public HashSet<PSphere> Spheres;
        public int SphereCount;
        public int ModelID = 0;
        public string name = "GameModel";
        public ModelTypes type = ModelTypes.Player;
        public string resourcePath = "/Models/bob";
        public Matrix World;
        private Matrix[] Bones;
        public Vector3 Position;

        //Animation
        private SkinningData SkinningData;
        private AnimationPlayer AnimationPlayer;
        private Dictionary<AnimationType, AnimationClip> Clips;
        private AnimationType CurrentAnimation;
        private AnimationType PreviousAnimation;


        public float Scale = 30.0f;
        public float RotationY = 0f;
        public float RotationZ = 0f;
        public float RotationX = 0f;

        public Matrix[] boneTransforms;

        public GameModel()
        {
            //Animations
            Clips = new Dictionary<AnimationType, AnimationClip>();
            CurrentAnimation = AnimationType.Fall;
            PreviousAnimation = AnimationType.Fall;

            Spheres = new HashSet<PSphere>();
        }

        public void Init()
        {
            SkinningData = model.Tag as SkinningData;
            AnimationPlayer = new AnimationPlayer(SkinningData);

            LoadAnimations();

            AnimationPlayer.StartClip(Clips[CurrentAnimation]);

        }

        private void LoadAnimations()
        {
            Clips.Clear();

            List<AnimationType> clips = GetClips();

            foreach (AnimationType type in clips)
            {
                Clips.Add(type, SkinningData.AnimationClips[type.ToString()]);
            }

        }

        public void SetCurrentAnimation(string name)
        {
            CurrentAnimation = (AnimationType)Enum.Parse(typeof(AnimationType), name);
        }

        public List<string> GetClipNames()
        {
            return SkinningData.AnimationClips.Keys.ToList();
        }

        public PSphere GetSphere(int ID)
        {
            foreach (PSphere sphere in Spheres)
            {
                if (sphere.ID == ID)
                {
                    return sphere;
                }
            }

            return null;
        }

        public PSphere AddSphere()
        {
            PSphere newSphere = new PSphere();
            newSphere.ParentModel = this;
            int i = 0;

            foreach (PSphere sphere in Spheres)
            {
                if (sphere.ID == i)
                {
                    i++;
                    continue;
                }
                i++;
            }
            newSphere.ID = i;
            Spheres.Add(newSphere);

            return newSphere;
        }

        public void RemoveSphere(int index)
        {
            foreach (PSphere sphere in Spheres)
            {
                if (sphere.ID == ID)
                {
                    Spheres.Remove(sphere);
                    return;
                }
            }
        }

        public int GetSphereCount()
        {
            return Spheres.Count();
        }

        public void Update(GameTime gameTime)
        {
            if (model != null && !Loaded)
            {
                Init();
                Loaded = true;
            }

            if (!Loaded)
            {
                return;
            }

            //Animations
            AnimationPlayer.UpdateBoneTransforms(gameTime.ElapsedGameTime, true);

            if (boneTransforms != null)
            {
                AnimationPlayer.GetBoneTransforms().CopyTo(boneTransforms, 0);
                AnimationPlayer.UpdateWorldTransforms(World, boneTransforms);
            }

            AnimationPlayer.UpdateSkinTransforms();

            UpdateSpheres();

            if (PreviousAnimation != CurrentAnimation)
            {
                PreviousAnimation = CurrentAnimation;
                AnimationPlayer.StartClip(Clips[CurrentAnimation]);
            }

            if (Fields.Fields.AnimationRunning)
            {
                double r = gameTime.ElapsedGameTime.TotalSeconds / 3;
                TimeSpan span = gameTime.ElapsedGameTime.Subtract(TimeSpan.FromSeconds(r));
                AnimationPlayer.Update(span, true, Matrix.Identity);
            }
            else
            {
                AnimationPlayer.Update(gameTime.ElapsedGameTime, false, Matrix.Identity);
            }
        }

        public void UpdateSpheres()
        {
            Matrix[] worldTransforms = AnimationPlayer.GetWorldTransforms();

            foreach (PSphere sphere in Spheres)
            {
                int boneIndex = SkinningData.BoneIndices[sphere.BoneName];
                sphere.Update(worldTransforms[boneIndex]);
            }
        }

        public void ResetRotations()
        {
            RotationX = 0f;
            RotationY = 0f;
            RotationZ = 0f;
        }

        public void Draw(GameTime gameTime)
        {
            //S * R * T
            World =
                    Matrix.CreateScale(Scale) *
                    Matrix.CreateRotationX(RotationX) *
                    Matrix.CreateRotationY(RotationY) *
                    Matrix.CreateRotationZ(RotationZ) *
                    Matrix.CreateTranslation(Position);

            //Calculate bones needed for drawing for animations
            if (model == null)
            {
                // Console.WriteLine("Unloaded model for path " + resourcePath);
                return;
            }
            Bones = AnimationPlayer.GetSkinTransforms();
            boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                if (ID == 1)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = boneTransforms[mesh.ParentBone.Index] * World;
                        effect.View = Camera.View;
                        effect.Projection = Camera.Projection;

                        effect.TextureEnabled = false;
                    }
                }
                else
                {
                    foreach (SkinnedEffect effect in mesh.Effects)
                    {
                        effect.SetBoneTransforms(Bones);

                        effect.World = boneTransforms[mesh.ParentBone.Index] * World;
                        effect.View = Camera.View;
                        effect.Projection = Camera.Projection;
                        effect.EnableDefaultLighting();

                        effect.SpecularColor = new Vector3(0.25f);
                        effect.SpecularPower = 16;
                    }
                }

                mesh.Draw();
            }
        }

        public List<AnimationType> GetClips()
        {
            List<AnimationType> clips = new List<AnimationType>();

            switch (type)
            {
                case ModelTypes.Vehicle:
                    break;

                case ModelTypes.Building:
                    break;

                case ModelTypes.Player:
                    clips.Add(AnimationType.Walk);
                    clips.Add(AnimationType.Fall);
                    clips.Add(AnimationType.Kick);
                    clips.Add(AnimationType.Reload);
                    break;

                case ModelTypes.Scenery:
                    break;
            }
            return clips;
        }
    }

    public enum AnimationType
    {
        //   Stand,
        //     Run,
        Walk,
        Fall,
        Kick,
        Reload,
    }

    public enum ModelTypes
    {
        Player,
        Vehicle,
        Scenery,
        Building,
        Projectile,
    }


}
