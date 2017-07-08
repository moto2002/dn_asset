﻿using UnityEngine;
using System.Collections;

public class XComponent:XObject
{

    public XEntity _entity = null;

    public virtual uint ID { get { return XCommon.singleton.XHash(this.GetType().Name); } }

    public virtual void OnInitial(XEntity _entity)
    {
    }

    public virtual void OnUninit()
    {
    }


  
}