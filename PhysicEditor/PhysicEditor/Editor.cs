using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PhysicEditor.Models;
using PhysicEditor.Input;

namespace PhysicEditor
{
    public partial class MainDialog : Form
    {
        public PType CurrentType;
        private int lastTick;

        public MainDialog()
        {
            InitializeComponent();
        }

        public void UpdateModel()
        {
            int ModelID = Program.CurrentModelID;
            PType Type = Program.CurrentPType;
            int SphereID = Program.CurrentSphereID;

            GameModel model = Collection.GetGameModelByID(Program.CurrentModelID);
            if (model == null)
            {
                return;
            }
            PSphere sphere = model.GetSphere(SphereID);

            if (sphere == null)
            {
                return;
            }

            textRadius.Text = sphere.Sphere.Radius.ToString();
            textCenterX.Text = sphere.AnchorPoint.X.ToString();
            textCenterY.Text = sphere.AnchorPoint.Y.ToString();
            textCenterZ.Text = sphere.AnchorPoint.Z.ToString();
        }

        //List of models -- clicking on different selection
        private void MainDialog_Load(object sender, EventArgs e)
        {
            Collection.PopulateModelList();
            Collection.ReadFile();
            listBox1.Items.Clear();

            //Refill listbox of game models
            foreach (GameModel model in Collection.Models)
            {
                listBox1.Items.Add(model.ID);
            }
            
            comboBox2.Items.Clear();

            //Refill listbox of acutal XNA models
            foreach (string s in Collection.ModelList.Keys)
            {
                comboBox2.Items.Add(s);
            }

            textBoxDrawScale.Text = "0";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.IsLoading = true;

            int index;
            Program.CurrentModelID = index = listBox1.SelectedIndex;
            GameModel model = Collection.GetGameModelByID(index);

            if (model == null || model.model == null)
            {
                return;
            }

            //Bones
            comboBoxSphereBone.Items.Clear();
            foreach (ModelBone bone in model.model.Bones)
            {
                comboBoxSphereBone.Items.Add(bone.Name);
            }

            //Animations
            comboBoxAnimations.Items.Clear();
            foreach (string s in model.GetClipNames())
            {
                comboBoxAnimations.Items.Add(s);
            }
            comboBoxAnimations.SelectedIndex = 0;

            //General Info
            textBox1.Text = model.name;
            comboBox2.SelectedItem = model.resourcePath;
            textBoxDrawScale.Text = model.Scale.ToString();

            //Fill up parts for this model's sphere collection
            switch (model.type)
            {
                case ModelTypes.Building:
                    radioBuilding.Checked = true;
                    comboParts.Items.Clear();

                    comboParts.Items.Add(PType.BuildingBase);

                    comboParts.SelectedItem = PType.BuildingBase;
                    break;
                case ModelTypes.Player:
                    comboParts.Items.Clear();

                    comboParts.Items.Add(PType.Head);
                    comboParts.Items.Add(PType.LeftArm);
                    comboParts.Items.Add(PType.LeftLeg);
                    comboParts.Items.Add(PType.RightArm);
                    comboParts.Items.Add(PType.RightLeg);
                    comboParts.Items.Add(PType.Torso);
                    
                    radioPlayer.Checked = true;

                    break;
                case ModelTypes.Projectile:
                    radioProjectile.Checked = true;
                    comboParts.Items.Clear();

                    comboParts.Items.Add(PType.Tip);

                    comboParts.SelectedItem = PType.Tip;
                    break;
                case ModelTypes.Scenery:
                    radioScenery.Checked = true;
                    comboParts.Items.Clear();

                    comboParts.Items.Add(PType.Base);

                    comboParts.SelectedItem = PType.Base;
                    break;
                case ModelTypes.Vehicle:
                    radioVehicle.Checked = true;
                    comboParts.Items.Clear();

                    comboParts.Items.Add(PType.VehicleBase);

                    comboParts.SelectedItem = PType.VehicleBase;
                    break;
            }

            if (model.GetSphereCount() != 0)
            {
                comboSpheres.Items.Clear();

                foreach (PSphere sphere in model.Spheres)
                {
                    comboSpheres.Items.Add(sphere.ID);
                }

                comboSpheres.SelectedIndex = 0;
            }
            else
            {
                textRadius.Text = "0";
                textCenterX.Text = "0";
                textCenterY.Text = "0";
                textCenterZ.Text = "0";
            }

            Program.IsLoading = false;
        }

        //Save changes to model
        private void button4_Click(object sender, EventArgs e)
        {
            int index = Program.CurrentModelID;

            GameModel changes = Collection.GetGameModelByID(index);

            if (changes == null)
            {
                Console.WriteLine("null model -- not saving");
                return;
            }

            //Info
            changes.name = textBox1.Text;
            changes.resourcePath = comboBox2.SelectedItem.ToString();

            //Game info
            changes.Scale = Convert.ToSingle(textBoxDrawScale.Text);
            
            PSphere currentSphere = changes.GetSphere(Program.CurrentSphereID);

            if (currentSphere != null)
            {
                currentSphere.Sphere.Radius = Convert.ToSingle(textRadius.Text);
                currentSphere.AnchorPoint.X = Convert.ToSingle(textCenterX.Text);
                currentSphere.AnchorPoint.Y = Convert.ToSingle(textCenterY.Text);
                currentSphere.AnchorPoint.Z = Convert.ToSingle(textCenterZ.Text);
                currentSphere.Part = (PType)comboParts.SelectedItem;

            }
            Collection.WriteFile();
        }

        //Changing associated part for selected sphere
        private void comboParts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboParts.SelectedText != "")
            {
                PSphere currentSphere = Collection.GetGameModelByID(Program.CurrentModelID).GetSphere(Program.CurrentSphereID);

                currentSphere.Part = (PType)Enum.Parse(typeof(PType), comboParts.SelectedText);
            }
        }

        private void buttonRotateX_Click(object sender, EventArgs e)
        {
            Collection.GetGameModelByID(Program.CurrentModelID).RotationX += 0.01f;
        }

        private void buttonRotateY_Click(object sender, EventArgs e)
        {
            Collection.GetGameModelByID(Program.CurrentModelID).RotationY += 0.01f;
        }

        private void buttonRotateZ_Click(object sender, EventArgs e)
        {
            Collection.GetGameModelByID(Program.CurrentModelID).RotationZ += 0.01f;
        }

        private void buttonZoomOut_Click(object sender, EventArgs e)
        {
            Camera.ZoomControl(false);
        }

        private void buttonZoomIn_Click(object sender, EventArgs e)
        {
            Camera.ZoomControl(true);
        }
        
        private void buttonAdjustRadius_Click(object sender, EventArgs e)
        {
            Fields.Fields.SetAllToFalse();
            Fields.Fields.AdjustingSphere = true;
            Fields.Fields.Spheres = Fields.SphereMode.Radius;
        }


        private void comboSpheres_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.CurrentSphereID = (int)comboSpheres.SelectedItem;

            PSphere currentSphere = Collection.GetGameModelByID(Program.CurrentModelID).GetSphere(Program.CurrentSphereID);

            textRadius.Text = currentSphere.Sphere.Radius.ToString();
            textCenterX.Text = currentSphere.AnchorPoint.X.ToString();
            textCenterY.Text = currentSphere.AnchorPoint.Y.ToString();
            textCenterZ.Text = currentSphere.AnchorPoint.Z.ToString();

            comboBoxSphereBone.SelectedItem = currentSphere.BoneName;
            comboParts.SelectedItem = currentSphere.Part;
        }

        //Adds a sphere
        private void buttonAddSphere_Click(object sender, EventArgs e)
        {
            GameModel model = Collection.GetGameModelByID(Program.CurrentModelID);

            PSphere newSphere = model.AddSphere();

            comboSpheres.Items.Add(newSphere.ID);

            Program.CurrentSphereID = newSphere.ID;

            textCenterX.Text = "0";
            textCenterY.Text = "0";
            textCenterZ.Text = "0";
            textRadius.Text = "0";

            Collection.WriteFile();
        }

        private void buttonRemoveSphere_Click(object sender, EventArgs e)
        {
            foreach (int i in comboSpheres.Items)
            {
                if (i == Program.CurrentSphereID)
                {
                    comboSpheres.Items.Remove(i);
                    Collection.GetGameModelByID(Program.CurrentModelID).RemoveSphere(Program.CurrentSphereID);
                    Program.CurrentSphereID = comboSpheres.SelectedIndex = 0;
                    break;
                }
            }
            Collection.WriteFile();
        }

        //Add game model
        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(Collection.AddGameModel().ID);
        }
        
        private void radioVehicle_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.IsLoading)
            {
                return;
            }

            GameModel model = Collection.Models[Program.CurrentModelID];
            model.type = ModelTypes.Vehicle;

            comboParts.Items.Clear();
            comboSpheres.Items.Clear();

            Program.CurrentPType = PType.VehicleBase;

            comboParts.Items.Add(PType.VehicleBase);

            comboParts.SelectedIndex = 0;

            textCenterX.Text = "0";
            textCenterY.Text = "0";
            textCenterZ.Text = "0";
            textRadius.Text = "0";
        }

        private void radioProjectile_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.IsLoading)
            {
                return;
            }

            GameModel model = Collection.Models[Program.CurrentModelID];
            model.type = ModelTypes.Projectile;

            comboParts.Items.Clear();
            comboSpheres.Items.Clear();

            Program.CurrentPType = PType.Tip;

            comboParts.Items.Add(PType.Tip);

            comboParts.SelectedIndex = 0;

            textCenterX.Text = "0";
            textCenterY.Text = "0";
            textCenterZ.Text = "0";
            textRadius.Text = "0";
        }

        private void radioPlayer_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.IsLoading)
            {
                return;
            }

            GameModel model = Collection.Models[Program.CurrentModelID];
            model.type = ModelTypes.Player;

            comboParts.Items.Clear();
            comboSpheres.Items.Clear();

            Program.CurrentPType = PType.Head;

            comboParts.Items.Add(PType.Head);
            comboParts.Items.Add(PType.LeftArm);
            comboParts.Items.Add(PType.LeftLeg);
            comboParts.Items.Add(PType.RightArm);
            comboParts.Items.Add(PType.RightLeg);
            comboParts.Items.Add(PType.Torso);

            comboParts.SelectedIndex = 0;

            textCenterX.Text = "0";
            textCenterY.Text = "0";
            textCenterZ.Text = "0";
            textRadius.Text = "0";
        }

        private void radioScenery_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.IsLoading)
            {
                return;
            }

            GameModel model = Collection.Models[Program.CurrentModelID];
            model.type = ModelTypes.Scenery;

            comboParts.Items.Clear();
            comboSpheres.Items.Clear();

            Program.CurrentPType = PType.Base;

            comboParts.Items.Add(PType.Base);

            comboParts.SelectedIndex = 0;

            textCenterX.Text = "0";
            textCenterY.Text = "0";
            textCenterZ.Text = "0";
            textRadius.Text = "0";
        }

        private void radioBuilding_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.IsLoading)
            {
                return;
            }

            GameModel model = Collection.Models[Program.CurrentModelID];
            model.type = ModelTypes.Building;

            comboParts.Items.Clear();
            comboSpheres.Items.Clear();

            Program.CurrentPType = PType.BuildingBase;

            comboParts.Items.Add(PType.BuildingBase);

            comboParts.SelectedIndex = 0;

            textCenterX.Text = "0";
            textCenterY.Text = "0";
            textCenterZ.Text = "0";
            textRadius.Text = "0";
        }
        
        private void checkBoxWireMode_CheckedChanged(object sender, EventArgs e)
        {
            Program.IsWireMode = checkBoxWireMode.Checked;
        }

        private void comboBoxSphereBone_SelectedIndexChanged(object sender, EventArgs e)
        {
            PSphere sphere = Collection.GetGameModelByID(Program.CurrentModelID).GetSphere(Program.CurrentSphereID);

            if (sphere == null)
            {
                return;
            }

            sphere.BoneName = comboBoxSphereBone.Text;            
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            Fields.Fields.AnimationRunning = !Fields.Fields.AnimationRunning;
        }

        private void comboBoxAnimations_SelectedIndexChanged(object sender, EventArgs e)
        {

            GameModel model = Collection.GetGameModelByID(Program.CurrentModelID);

            model.SetCurrentAnimation(comboBoxAnimations.SelectedItem.ToString());
        }

        public void GoToNextAnimation()
        {
            int now = Environment.TickCount;

            if (now - lastTick < 500)
            {
                return;
            }
            lastTick = now;
            int i = comboBoxAnimations.SelectedIndex;
          
            if (comboBoxAnimations.Items.Count > i + 1)
            {
                comboBoxAnimations.SelectedIndex++;
            }
            else
            {
                comboBoxAnimations.SelectedIndex = 0;
            }
        }

        private void buttonAdjustCenter_Click(object sender, EventArgs e)
        {
            Fields.Fields.SetAllToFalse();
            Fields.Fields.AdjustingSphere = true;
            Fields.Fields.Spheres = Fields.SphereMode.Anchor;
        }

        //Saving a default set
        private void buttonSetDefault_Click(object sender, EventArgs e)
        {
            GameModel model = Collection.GetGameModelByID(Program.CurrentModelID);

            if (!Collection.DefaultPhysics.ContainsKey(comboBoxDefault.SelectedItem.ToString()))
            {
                Collection.DefaultPhysics.Add(comboBoxDefault.SelectedItem.ToString(), new Collection.DefaultSpheres());
            }

            Collection.DefaultPhysics[comboBoxDefault.SelectedItem.ToString()].Spheres.Clear();

            Collection.DefaultSpheres spheres = new Collection.DefaultSpheres();

            foreach (PSphere sphere in model.Spheres)
            {              
                spheres.Spheres.Add(sphere);
            }

            Collection.DefaultPhysics[comboBoxDefault.SelectedItem.ToString()] = spheres;
        }

        private void SetDefaults(string name)
        {
            //Reset all spheres
            if (!Collection.DefaultPhysics.ContainsKey(name))
            {
                Collection.DefaultPhysics.Add(name, new Collection.DefaultSpheres());
            }

            GameModel model = Collection.GetGameModelByID(Program.CurrentModelID);

            comboSpheres.Items.Clear();
            model.Spheres.Clear();

            foreach (PSphere sphere in Collection.DefaultPhysics[name].Spheres)
            {
                sphere.ParentModel = model;
                comboSpheres.Items.Add(sphere.ID);
                model.Spheres.Add(sphere);
            }
        }

        private void buttonHuman1_Click(object sender, EventArgs e)
        {
            SetDefaults("Human 1");
        }

        private void buttonHuman2_Click(object sender, EventArgs e)
        {
            SetDefaults("Human 2");
        }

        private void buttonHuman3_Click(object sender, EventArgs e)
        {
            SetDefaults("Human 3");
        }

        private void buttonHuman4_Click(object sender, EventArgs e)
        {
            SetDefaults("Human 4");
        }

        private void buttonHuman5_Click(object sender, EventArgs e)
        {
            SetDefaults("Human 5");
        }

        private void buttonAlien1_Click(object sender, EventArgs e)
        {
            SetDefaults("Alien 1");
        }

        private void buttonAlien2_Click(object sender, EventArgs e)
        {
            SetDefaults("Alien 2");
        }

        private void buttonAlien3_Click(object sender, EventArgs e)
        {
            SetDefaults("Alien 3");
        }

        private void buttonAlien4_Click(object sender, EventArgs e)
        {
            SetDefaults("Alien 4");
        }

        private void buttonAlien5_Click(object sender, EventArgs e)
        {
            SetDefaults("Alien 5");
        }

        private void buttonVehicle1_Click(object sender, EventArgs e)
        {
            SetDefaults("Vehicle 1");
        }

        private void buttonVehicle2_Click(object sender, EventArgs e)
        {
            SetDefaults("Vehicle 2");
        }

        private void buttonVehicle3_Click(object sender, EventArgs e)
        {
            SetDefaults("Vehicle 3");
        }

        private void buttonVehicle4_Click(object sender, EventArgs e)
        {
            SetDefaults("Vehicle 4");
        }

        private void comboBoxDefault_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
