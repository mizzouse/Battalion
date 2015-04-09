using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

using Microsoft.Xna.Framework.Graphics;

namespace PhysicEditor.Models
{
    public static class Collection
    {
        //List of game models used by game -- multiple game models can use the same base XNA model
        public static List<GameModel> Models = new List<GameModel>();
        //List of XNA models used by game models [one entry per content file] -- does not support temporary files
        public static Dictionary<string, Model> ModelList = new Dictionary<string, Model>();
        //List of default physics setups set and used by developers
        public static Dictionary<string, DefaultSpheres> DefaultPhysics = new Dictionary<string, DefaultSpheres>();

        public class DefaultSpheres
        {
            public List<PSphere> Spheres = new List<PSphere>();
        }

        public static GameModel AddGameModel()
        {
            int i = 0;
            while (GetGameModelByID(i) != null)
            {
                i++;
            }
            GameModel model = new GameModel();
            model.ID = i;

            Models.Add(model);

            WriteFile();

            return model;
        }

        public static GameModel GetGameModelByID(int ID)
        {
            foreach (GameModel m in Models)
            {
                if (m.ID == ID)
                {
                    return m;
                }
            }
            return null;
        }

        public static void WriteGameFiles()
        {
            WriteGamePhysicsFile();
        }

        public static void WriteGamePhysicsFile()
        {

        }

        public static void PopulateModelList()
        {
            ModelList.Clear();

            using (var file = File.OpenRead(@"sys\Models.txt"))
            {
                using (var reader = new BinaryReader(file))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string s = reader.ReadString();
                        if (!ModelList.ContainsKey(s))
                        {
                            ModelList.Add(s, Program.mainGame.LoadModelContent(s));
                        }
                    }
                }
            }
        }

        

        public static void ReadFile()
        {
            Models.Clear();

            Console.WriteLine("Reading models...");

            using (var file = File.OpenRead(@"sys\Package5.txt"))
            {
                if (file.Length == 0)
                {
                    return;
                }
                using (var reader = new BinaryReader(file))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        GameModel model = new GameModel();
                        model.ID = reader.ReadInt32();
                        model.name = reader.ReadString();
                        model.resourcePath = reader.ReadString();
                        model.ModelID = reader.ReadInt32();
                        model.Scale = reader.ReadSingle();

                        if (ModelList.ContainsKey(model.resourcePath))
                        {
                            model.model = ModelList[model.resourcePath];
                        }
                        else
                        {
                            Console.WriteLine("Attempted to load invalid model -- " + model.resourcePath);
                        }

                        int SphereCount = reader.ReadInt32();

                        for (int k = 0; k < SphereCount; k++)
                        {
                            PSphere sphere = new PSphere(reader.ReadInt32(), model);
                            sphere.Part = (PType) Enum.Parse(typeof(PType), reader.ReadString());
                            sphere.BoneName = reader.ReadString();
                            sphere.AnchorPoint.X = reader.ReadSingle();
                            sphere.AnchorPoint.Y = reader.ReadSingle();
                            sphere.AnchorPoint.Z = reader.ReadSingle();
                            sphere.Radius = reader.ReadSingle();

                            sphere.SetSphere();

                            model.Spheres.Add(sphere);
                        }
                        Models.Add(model);

                        int DefaultCount = reader.ReadInt32();

                        for (int j = 0; j < DefaultCount; j++)
                        {
                            string Name = reader.ReadString();
                            int PCount = reader.ReadInt32();
                            
                            DefaultPhysics.Add(Name, new DefaultSpheres());

                            for (int m = 0; m < PCount; m++)
                            {
                                PSphere sphere = new PSphere(reader.ReadInt32(), null);
                                sphere.Part = (PType)Enum.Parse(typeof(PType), reader.ReadString());
                                sphere.BoneName = reader.ReadString();
                                sphere.AnchorPoint.X = reader.ReadSingle();
                                sphere.AnchorPoint.Y = reader.ReadSingle();
                                sphere.AnchorPoint.Z = reader.ReadSingle();
                                sphere.Radius = reader.ReadSingle();

                                sphere.SetSphere();

                                DefaultPhysics[Name].Spheres.Add(sphere);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("---Complete!");
        }

        public static void WriteFile()
        {
            Console.WriteLine("Saving file...");
            using (var file = File.Create(@"sys\Package5.txt"))
            {
                using (var writer = new BinaryWriter(file))
                {
                    writer.Write(Models.Count);

                    foreach (GameModel model in Models)
                    {
                        writer.Write(model.ID);
                        writer.Write(model.name);
                        writer.Write(model.resourcePath);
                        writer.Write(model.ModelID);
                        writer.Write(model.Scale);

                        writer.Write(model.Spheres.Count);

                        foreach (PSphere sphere in model.Spheres)
                        {
                            writer.Write(sphere.ID);
                            writer.Write(sphere.Part.ToString());
                            writer.Write(sphere.BoneName);
                            writer.Write(sphere.AnchorPoint.X);
                            writer.Write(sphere.AnchorPoint.Y);
                            writer.Write(sphere.AnchorPoint.Z);
                            writer.Write(sphere.Radius);
                        }
                    }

                    writer.Write(DefaultPhysics.Count);

                    foreach (KeyValuePair<string, DefaultSpheres> pair in DefaultPhysics)
                    {
                        writer.Write(pair.Key);
                        writer.Write(pair.Value.Spheres.Count);

                        foreach (PSphere sph in pair.Value.Spheres)
                        {
                            writer.Write(sph.ID);
                            writer.Write(sph.Part.ToString());
                            writer.Write(sph.BoneName);
                            writer.Write(sph.AnchorPoint.X);
                            writer.Write(sph.AnchorPoint.Y);
                            writer.Write(sph.AnchorPoint.Z);
                            writer.Write(sph.Radius);
                        }
                    }
                }
            }
            Console.WriteLine("---Complete!");
        }
    }
}
