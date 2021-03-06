﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class NodeEditorHandles
{
    public static Node GetHoveredNode(GraphData data, Vector2 p)
    {
        foreach (var node in data.nodes)
        {
            if (NodeEditorGUIUtility.GetNodeRect(node).Contains(p))
            {
                return node;
            }
        }
        return null;
    }
    public static NodePort GetHoveredNodePort(Node node, Vector2 p)
    {
        foreach (var port in node.ports)
        {
            if (NodeEditorGUIUtility.GetNodePortRect(port).Contains(p))
            {
                return port;
            }
        }
        return null;
    }

    public static void HandleGraphData(GraphData data)
    {
        DoMouseHandles(data);
    }
    public static bool hasMouseDragRect = false;
    public static Rect mouseDragRect = new Rect();
    static Vector2 mouseDragRectStartPoint = new Vector2();
    static GenericMenu rightClickMenu;

    public static NodePort hoveredNodePort;
    public static void DoMouseHandles(GraphData data)
    {
        Node hoveredNode = GetHoveredNode(data, Event.current.mousePosition);
        hoveredNodePort = null;
        if (hoveredNode)
            hoveredNodePort = GetHoveredNodePort(hoveredNode, Event.current.mousePosition);

        if (data.selectedNodePort != null && data.selectedNodePort.parentNode == null)
            data.selectedNodePort = null;
        Vector2 LastMousePos = Vector2.zero;
        LastMousePos = Event.current.mousePosition;
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                switch (Event.current.button)
                {
                    //L
                    case 0:
                        #region Selection
                        if (hoveredNode)
                        {
                            if (hoveredNodePort == null)
                            {
                                if (Event.current.shift)
                                {
                                    if (!data.selectedNodes.Contains(hoveredNode))
                                    {
                                        data.selectedNodes.Add(hoveredNode);
                                    }
                                }
                                else if (Event.current.control)
                                {
                                    if (data.selectedNodes.Contains(hoveredNode))
                                    {
                                        data.selectedNodes.Remove(hoveredNode);
                                    }
                                }
                                else
                                {
                                    if (data.selectedNodes.Contains(hoveredNode))
                                    {

                                    }
                                    else
                                    {
                                        data.selectedNodes.Clear();
                                        data.selectedNodes.Add(hoveredNode);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!Event.current.shift && !Event.current.control)
                            {
                                data.selectedNodes.Clear();
                            }
                            hasMouseDragRect = true;
                            mouseDragRect = new Rect();
                            mouseDragRectStartPoint = Event.current.mousePosition;
                        }
                        #endregion
                        #region ConnectionCreation
                        if (hoveredNodePort != null)
                        {
                            data.selectedNodePort = hoveredNodePort;
                        }
                        #endregion
                        break;
                    //M
                    case 2:
                        break;
                    //R
                    case 1:
                        #region Selection
                        if (hoveredNode)
                        {
                            if (hoveredNodePort == null)
                            {
                                if (Event.current.shift)
                                {
                                    if (!data.selectedNodes.Contains(hoveredNode))
                                    {
                                        data.selectedNodes.Add(hoveredNode);
                                    }
                                }
                                else if (Event.current.control)
                                {
                                    if (data.selectedNodes.Contains(hoveredNode))
                                    {
                                        data.selectedNodes.Remove(hoveredNode);
                                    }
                                }
                                else
                                {
                                    if (data.selectedNodes.Contains(hoveredNode))
                                    {

                                    }
                                    else
                                    {
                                        data.selectedNodes.Clear();
                                        data.selectedNodes.Add(hoveredNode);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!Event.current.shift && !Event.current.control)
                            {
                                data.selectedNodes.Clear();
                            }
                        }
                        #endregion

                        #region ConnectionDestroction
                        if (hoveredNodePort != null)
                        {
                            if (hoveredNodePort.connections.Count > 0)
                                data.DestroyConnection(hoveredNodePort.connections[0]);
                        }
                        #endregion
                        DropRightClickMenu(data, hoveredNode, LastMousePos);
                        break;
                }
                if (data.selectedNodes.Count > 0)
                    Selection.objects = data.selectedNodes.ToArray();
                else
                    Selection.objects = new UnityEngine.Object[] { data };
                break;
            case EventType.MouseDrag:
                switch (Event.current.button)
                {
                    //L
                    case 0:
                        if (hasMouseDragRect)
                        {
                            Vector2 mp = Event.current.mousePosition;
                            if (mp.x > mouseDragRectStartPoint.x)
                            {
                                mouseDragRect.xMin = mouseDragRectStartPoint.x;
                                mouseDragRect.xMax = mp.x;
                            }
                            else
                            {
                                mouseDragRect.xMax = mouseDragRectStartPoint.x;
                                mouseDragRect.xMin = mp.x;
                            }
                            if (mp.y > mouseDragRectStartPoint.y)
                            {
                                mouseDragRect.yMin = mouseDragRectStartPoint.y;
                                mouseDragRect.yMax = mp.y;
                            }
                            else
                            {
                                mouseDragRect.yMax = mouseDragRectStartPoint.y;
                                mouseDragRect.yMin = mp.y;
                            }
                        }
                        if (data.selectedNodes.Count > 0 && data.selectedNodePort == null && hoveredNode != null && data.selectedNodes.Contains(hoveredNode) && !hasMouseDragRect)
                        {
                            foreach (var node in data.selectedNodes)
                            {
                                node.position += Event.current.delta / data.ZoomAmm;
                            }
                        }

                        if (LastMousePos.x < 50)
                            HandleCameraPan(data, Vector2.right * Mathf.Clamp(50 - LastMousePos.x, 0, 50) / 5);
                        if (LastMousePos.y < 50)
                            HandleCameraPan(data, Vector2.up * Mathf.Clamp(50 - LastMousePos.y, 0, 50) / 5);

                        if (LastMousePos.x > Screen.width - 50)
                            HandleCameraPan(data, Vector2.left * Mathf.Clamp(LastMousePos.x - Screen.width + 50, 0, 50) / 5);
                        if (LastMousePos.y > Screen.height - 50 - 24)
                            HandleCameraPan(data, Vector2.down * Mathf.Clamp(LastMousePos.y - Screen.height + 50 + 24, 0, 50) / 5);

                        break;
                    //M
                    case 2:
                        HandleCameraPan(data, Event.current.delta);
                        break;
                    //R
                    case 1:
                        break;
                }
                break;
            case EventType.MouseUp:
                switch (Event.current.button)
                {
                    //L
                    case 0:
                        if (!Event.current.shift && !Event.current.control && hasMouseDragRect)
                        {
                            data.selectedNodes.Clear();
                        }
                        foreach (var node in data.nodes)
                        {
                            Rect nodeRect = NodeEditorGUIUtility.GetNodeRect(node);
                            if (nodeRect.Overlaps(mouseDragRect))
                            {
                                if (Event.current.shift && !data.selectedNodes.Contains(node))
                                    data.selectedNodes.Add(node);
                                else if (Event.current.control && data.selectedNodes.Contains(node))
                                    data.selectedNodes.Remove(node);
                                else if (!Event.current.shift && !Event.current.control)
                                {
                                    data.selectedNodes.Add(node);
                                }
                            }
                        }
                        mouseDragRect = new Rect(0, 0, 0, 0);
                        hasMouseDragRect = false;
                        mouseDragRectStartPoint = Vector2.zero;
                        #region ConnectionCreation
                        if (data.selectedNodePort != null && hoveredNodePort != null)
                            if (hoveredNodePort != data.selectedNodePort)
                            {
                                if (hoveredNodePort.IOType == NodePort.portType.Input && data.selectedNodePort.IOType == NodePort.portType.Output)
                                    data.TryCreateConnection(hoveredNodePort, data.selectedNodePort);
                                else if (hoveredNodePort.IOType == NodePort.portType.Output && data.selectedNodePort.IOType == NodePort.portType.Input)
                                    data.TryCreateConnection(data.selectedNodePort, hoveredNodePort);
                            }
                        data.selectedNodePort = null;
                        #endregion
                        break;
                    //M
                    case 2:
                        break;
                    //R
                    case 1:
                        break;
                }
                break;

            case EventType.ScrollWheel:
                HandleCameraZoomToPoint(data, Event.current.delta.y, 2.5f, Event.current.mousePosition);
                break;
        }
        if (data.selectedNodePort != null)
        {
            Color splineColor = Color.white;
            if (data.selectedNodePort.Type != null && !(data.selectedNodePort is StateEvent))
                splineColor = NodeEditorGUIUtility.NodePortColor(data.selectedNodePort);
            if (data.selectedNodePort.IOType == NodePort.portType.Output)
                NodeEditorGUIUtility.DrawSpline(NodeEditorGUIUtility.GetNodePortRect(data.selectedNodePort).center, Event.current.mousePosition, splineColor, 7.5f * data.ZoomAmm);
            else
                NodeEditorGUIUtility.DrawSpline(Event.current.mousePosition, NodeEditorGUIUtility.GetNodePortRect(data.selectedNodePort).center, splineColor, 7.5f * data.ZoomAmm);
        }
    }

    private static void DropRightClickMenu(GraphData data, Node hoveredNode, Vector2 LastMousePos)
    {
        rightClickMenu = new GenericMenu();
        if (hoveredNode == null)
        {
            List<NodeAttribute> nodeAttributes = new List<NodeAttribute>();
            Type[] nodeTypes = typeof(Node).Assembly.GetTypes();
            GraphDataAttribute graphAttribute = null;
            {
                var assembly = data.GetType();
                GraphDataAttribute[] GDA = (GraphDataAttribute[])assembly.GetCustomAttributes(typeof(GraphDataAttribute), false);
                if (GDA.Length > 0)
                    graphAttribute = GDA[0];
            }
            foreach (var nodeType in nodeTypes)
            {
                var attributes = (NodeAttribute[])nodeType.GetCustomAttributes(typeof(NodeAttribute), false);
                string TargetUsingID = "";
                if (graphAttribute != null)
                    TargetUsingID = graphAttribute.UsingID;
                if (attributes.Length > 0 && (attributes[0].UsingID == "" || TargetUsingID == "" || TargetUsingID == attributes[0].UsingID))
                    nodeAttributes.Add(attributes[0]);
            }
            foreach (var nodeAttribute in nodeAttributes)
            {
                rightClickMenu.AddItem(new GUIContent(nodeAttribute.creatingPath), false, () =>
                {
                    data.CreateNode(nodeAttribute.nodeType, (-data.cameraPosition + LastMousePos / data.ZoomAmm) - Screen.safeArea.size / 2 / data.ZoomAmm);

                });
            }
        }
        else
        {
            rightClickMenu.AddItem(new GUIContent("Remove"), false, () =>
            {
                foreach (var node in data.selectedNodes)
                {
                    data.DestroyNode(node);
                }
            });
            rightClickMenu.AddItem(new GUIContent("Copy"), false, () =>
            {
                foreach (var node in data.selectedNodes)
                {
                    data.CopyNode(node, node.position + new Vector2(50, 25));
                }
            });

            rightClickMenu.AddItem(new GUIContent("Reset"), false, () =>
            {
                foreach (var node in data.selectedNodes)
                {
                    data.ResetNode(node);
                }
            });
        }

        rightClickMenu.AddSeparator("");

        rightClickMenu.AddItem(new GUIContent("Prefrences"), false, () =>
        {
        });

        //Drop Down
        if (hoveredNodePort == null)
            rightClickMenu.ShowAsContext();
    }

    public static void HandleCameraPan(GraphData data, Vector2 delta)
    {
        //Pan
        data.cameraPosition += delta / data.ZoomAmm;
        if (hasMouseDragRect)
            mouseDragRectStartPoint += delta / data.ZoomAmm;
    }
    public static void HandleCameraZoomToCenter(GraphData data, float delta, float sensitivity)
    {
        //Zoom
        float d = Mathf.Clamp(delta, -0.1f, 0.1f) * -sensitivity;
        data.ZoomAmm = Mathf.Clamp(data.ZoomAmm + d, 0.8f, 1.55f);
    }
    public static void HandleCameraZoomToPoint(GraphData data, float delta, float sensitivity, Vector2 point)
    {
        //Zoom
        float df = Mathf.Clamp(delta, -0.1f, 0.1f) * -sensitivity;
        float prevZA = data.ZoomAmm;
        data.ZoomAmm = Mathf.Clamp(data.ZoomAmm + df, 0.8f, 1.55f);
        //GoToPoint
        if (data.ZoomAmm != prevZA)
        {
            Vector2 pointFC = point - new Vector2(Screen.width, Screen.height - 24) / 2;
            HandleCameraPan(data, data.cameraPosition - Vector2.Lerp(data.cameraPosition, df > 0 ? data.cameraPosition - pointFC : data.cameraPosition + pointFC, Mathf.Abs(df)));
        }
    }

}
