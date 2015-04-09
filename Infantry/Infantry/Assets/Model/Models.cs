using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SkinnedModel;

namespace Infantry.Assets
{
    public class ModelManager
    {
        public Dictionary<int, Models> _models = new Dictionary<int, Models>();
        int i = 1;

        public void loadModels()
        {
            Models model = new Models();
            model._id = i++;
            model._path = "Models/bob";
            _models.Add(i - 1, model);                                          //Basic infantry man used in debugging

            model = new Models();
            model._id = i++;
            model._path = "Models/gun";
            _models.Add(i - 1, model);                                           //bullet

            model = new Models();
            model._id = i++;                                                //gun
            model._path = "Models/bullet";
            _models.Add(i - 1, model);
        }

        public void Add(Model model)
        {
            Models newModel = new Models();

            newModel._id = i++;
            newModel._model = model;

            newModel.setSkinningData();
            
            _models.Add(i - 1, newModel);
        }
    }

    public class Models
    {
        public Model _model;
        public int _id;
        public string _path;
        public string _modelName;

        public SkinningData _skinningData;

        public AnimationPlayer _animation;                      
        public Dictionary<string, AnimationClip> _clips = new Dictionary<string,AnimationClip>();       

        //Rewrite to use some sort of file to get all paths and IDs
        //These IDs are the numbers a developer will need when choosing the modelID number on vehicles/weapons/items

        public void setSkinningData()
        {
            _skinningData = _model.Tag as SkinningData;

            _animation = new AnimationPlayer(_skinningData);

            AnimationClip clip = _skinningData.AnimationClips["Kick"];

            _clips.Add("Kick", clip);

            //_animation.Start();
        }
    }    
}
