﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace IMRE.HandWaver.ScaleStudy
{

    public interface I4D_Perspective
    {
        void SetRotation(float xy, float xz, float xw, float yz, float yw, float zw);
    }
}