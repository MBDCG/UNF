﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
public class NodeEditorWindow : EditorWindow
{
    public GraphData data;
    public void OnEnable()
    {
        NodeEditorGUIUtility.Init();
        if (data != null && data.nodes != null)
            foreach (var node in data.nodes)
            {
                if (node != null)
                    node.Init();
            }
    }
    public static NodeEditorWindow GetWindow(GraphData data)
    {
        NodeEditorWindow window = CreateInstance<NodeEditorWindow>();
        window.data = data;
        window.OnEnable();
        window.Show();
        return window;
    }
    public void OnGUI()
    {
        if (data)
        {
            NodeEditorGUIUtility.DrawGraphData(data);
            NodeEditorHandles.HandleGraphData(data);

            Repaint();
        }
    }
}
