/**
HandWaver, developed at the Maine IMRE Lab at the University of Maine's College of Education and Human Development
(C) University of Maine
See license info in readme.md.
www.imrelab.org
**/

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script does ___.
/// The main contributor(s) to this script is __
/// Status: ???
/// </summary>
namespace IMRE.HandWaver
{

    abstract class AbstractArc : MasterGeoObj
    {
		internal override Vector3 ClosestSystemPosition(Vector3 abstractPosition)
		{
			Debug.LogWarning("This FIG TYPE DOESN'T SUPPORT CLOSEST SYS POS : " + figType);

			throw new NotImplementedException();
		}
	}
}