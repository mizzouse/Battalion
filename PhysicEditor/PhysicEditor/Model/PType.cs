using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysicEditor.Models
{
    public enum PType
    {
        //No Collisions
        Null,

        //Human
        Head,
        LeftLeg,
        RightLeg,
        LeftArm,
        RightArm,
        Torso,

        //Scenery
        Base,

        //Projectile
        Tip,

        //Building
        BuildingBase,

        //Vehicle
        VehicleBase,
    }
}
