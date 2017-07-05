﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void MountLoadCallback(MountLoadTask mountPart);

public class MountLoadTask : EquipLoadTask
{
    public GameObject xgo = null;
    public bool transferRef = false;
    public Renderer mainRender = null;
    private MountLoadCallback m_MountPartLoadCb = null;

    public MountLoadTask(EPartType p, MountLoadCallback mountPartLoadCb)
        : base(p)
    {
        m_MountPartLoadCb = mountPartLoadCb;
    }

    public override void Load(ref FashionPositionInfo newFpi, HashSet<string> loadedPath)
    {
        if (IsSamePart(ref newFpi))
        {
            if (m_MountPartLoadCb != null)
            {
                m_MountPartLoadCb(this);
            }
        }
        else
        {
            if (MakePath(ref newFpi, loadedPath))
            {
                xgo = Resources.Load<GameObject>(location);
                LoadFinish(xgo, this);
            }
            else
            {
                if (string.IsNullOrEmpty(location))
                {
                    processStatus = EProcessStatus.EProcessing;
                    LoadFinish(null, this);
                }
            }
        }
    }

    static void LoadFinish(GameObject gameObject, object o)
    {
        MountLoadTask mlt = o as MountLoadTask;
        if (mlt != null)
        {
            if (mlt.processStatus == EProcessStatus.EProcessing)
            {
                mlt.processStatus = EProcessStatus.EPreProcess;

                if (mlt.m_MountPartLoadCb != null)
                {
                    mlt.m_MountPartLoadCb(mlt);
                }
            }
        }
    }

    public override void Reset()
    {
        if (!transferRef && xgo != null)
        {
            GameObject.Destroy(xgo);
        }
        xgo = null;
        transferRef = false;
    }

    public void ProcessRender(int layer, bool enable, bool castShadow, int renderQueue)
    {
        if (xgo != null)
        {
            for (int i = 0; i < XCommon.tmpRender.Count; i++)
            {
                Renderer render = XCommon.tmpRender[i];
                render.enabled = enable;
                if (layer >= 0)
                {
                    render.gameObject.layer = layer;

                    bool hasUIRimMask = render.sharedMaterial.HasProperty("_UIRimMask");

                    if (hasUIRimMask)
                    {
                        render.material.SetVector("_UIRimMask", new Vector4(0, 0, 2, 0));
                    }
                }


                if (render is ParticleRenderer)
                {
                    if (renderQueue >= 0)
                    {
                        render.material.renderQueue = renderQueue;
                    }
                }
                else
                {
                    Material mat = render.sharedMaterial;
                    if (mat != null && mat.shader.renderQueue < 3000)
                    {
                        mainRender = render;
                        render.castShadows = castShadow;
                    }
                    else if (renderQueue >= 0)
                    {
                        render.material.renderQueue = renderQueue;
                    }
                }
            }
            XCommon.tmpRender.Clear();
        }
    }

    public void TransferRef(bool transfer)
    {
        transferRef = transfer;
    }
}