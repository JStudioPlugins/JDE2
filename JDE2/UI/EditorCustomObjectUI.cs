using HarmonyLib;
using JDE2.Managers;
using JDE2.SituationDependent;
using JDE2.Utils;
using SDG.Unturned;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JDE2.UI
{
    public class EditorCustomObjectUI : EditorCustomUI, IEditorDependent
    {
        public static EditorCustomObjectUI Instance { get; private set; }

        public override int ContainerOffsetX => 0;
        public override int ContainerOffsetY => 0;
        public override float ContainerPositionScaleX => 1f;
        public override int ContainerSizeOffsetX => -20;
        public override int ContainerSizeOffsetY => -20;
        public override float ContainerSizeScaleX => 1f;
        public override float ContainerSizeScaleY => 1f;

        [Element(0)]
        private static ISleekFloat32Field XField;

        [Element(1)]
        private static ISleekFloat32Field YField;

        [Element(2)]
        private static ISleekFloat32Field ZField;

        [Element(3)]
        private static SleekButtonState TransformStateButton;

        [Element(4)]
        private static ISleekButton ApplyButton;

        [Element(5)]
        [Element(6)]
        private static ISleekUInt16Field ObjectIdField;

        [Element(7)]
        private static SleekButtonState ObjectHightlightColorButton;

        [Element(8)]
        private static ISleekButton ObjectHighlightButton;

        [Element(9)]
        private static ISleekButton ObjectHighlightSelectionButton;

        [Element(10)]
        private static ISleekButton clearObjectHighlightButton;

        public Vector3 LastPosition;

        public Vector3 LastRotation;

        public Vector3 LastScale;


        /*A lot of this was ported from the original JDE. This means that much of the code is messy. JDE was my first module, so most of the code was sloppy. JDE2 was created to fix that.
        * Instead of having to go back and constantly modify Main or one of the Managers, I chose an interface approach and used reflection.
        */
        public void UpdateFields(bool updateAnyways = false)
        {
            if (FocusedLevelObject != null)
            {
                if (TransformStateButton.state == 0)
                {
                    if (LastPosition != null || updateAnyways)
                    {
                        if (FocusedLevelObject.transform.position != LastPosition)
                        {
                            LastPosition = FocusedLevelObject.transform.position;
                            XField.Value = FocusedLevelObject.transform.position.x;
                            YField.Value = FocusedLevelObject.transform.position.y;
                            ZField.Value = FocusedLevelObject.transform.position.z;
                        }
                    }
                    else
                    {
                        LastPosition = FocusedLevelObject.transform.position;
                        XField.Value = FocusedLevelObject.transform.position.x;
                        YField.Value = FocusedLevelObject.transform.position.y;
                        ZField.Value = FocusedLevelObject.transform.position.z;
                    }
                }
                else if (TransformStateButton.state == 1)
                {
                    if (LastRotation != null || updateAnyways)
                    {
                        if (FocusedLevelObject.transform.eulerAngles != LastRotation)
                        {
                            LastRotation = FocusedLevelObject.transform.eulerAngles;
                            XField.Value = FocusedLevelObject.transform.eulerAngles.x;
                            YField.Value = FocusedLevelObject.transform.eulerAngles.y;
                            ZField.Value = FocusedLevelObject.transform.eulerAngles.z;
                        }
                    }
                    else
                    {
                        LastRotation = FocusedLevelObject.transform.eulerAngles;
                        XField.Value = FocusedLevelObject.transform.eulerAngles.x;
                        YField.Value = FocusedLevelObject.transform.eulerAngles.y;
                        ZField.Value = FocusedLevelObject.transform.eulerAngles.z;
                    }
                }
                else
                {
                    if (LastScale != null || updateAnyways)
                    {
                        if (FocusedLevelObject.transform.localScale != LastScale)
                        {
                            LastScale = FocusedLevelObject.transform.localScale;
                            XField.Value = FocusedLevelObject.transform.localScale.x;
                            YField.Value = FocusedLevelObject.transform.localScale.y;
                            ZField.Value = FocusedLevelObject.transform.localScale.z;
                        }
                    }
                    else
                    {
                        LastScale = FocusedLevelObject.transform.localScale;
                        XField.Value = FocusedLevelObject.transform.localScale.x;
                        YField.Value = FocusedLevelObject.transform.localScale.y;
                        ZField.Value = FocusedLevelObject.transform.localScale.z;
                    }
                }
            }
            else
            {
                XField.Value = 0f;
                YField.Value = 0f;
                ZField.Value = 0f;
                LastPosition = new Vector3();
                LastRotation = new Vector3();
                LastScale = new Vector3();
            }
        }

        public LevelObject FocusedLevelObject
        {
            get
            {
                GameObject selectedObj = EditorObjects.GetMostRecentSelectedGameObject();
                if (selectedObj != null)
                {
                    return FindLevelObject(selectedObj);
                }
                else
                {
                    return null;
                }
            }
        }

        public List<EditorSelection> Selection
        {
            get => (List<EditorSelection>)ReflectionTool.GetField<EditorObjects>("selection").GetValue(null);

            set => ReflectionTool.GetField<EditorObjects>("selection").SetValue(null, value);
        }

        public Dictionary<Transform, Color> Highlighted;

        public Color HighlightColor;

        public void UpdateHighlights()
        {
            foreach (KeyValuePair<Transform, Color> highlight in Highlighted)
            {
                HighlighterTool.highlight(highlight.Key, highlight.Value);
            }
        }

        private static void OnClickedApplyButton(ISleekElement button)
        {
            if (Instance.Selection.Count > 0 && Instance.FocusedLevelObject != null)
            {
                LevelObjects.step++;
                foreach (EditorSelection obj in Instance.Selection)
                {
                    if (TransformStateButton.state == 0)
                    {
                        LevelObject levelObj = FindLevelObject(obj.transform.gameObject);
                        Vector3 difference = new Vector3(XField.Value - Instance.FocusedLevelObject.transform.position.x, YField.Value - Instance.FocusedLevelObject.transform.position.y, ZField.Value - Instance.FocusedLevelObject.transform.position.z);
                        Vector3 change = new Vector3(levelObj.transform.position.x + difference.x, levelObj.transform.position.y + difference.y, levelObj.transform.position.z + difference.z);
                        LevelObjects.registerTransformObject(levelObj.transform, change, levelObj.transform.rotation, levelObj.transform.localScale, levelObj.transform.position, levelObj.transform.rotation, levelObj.transform.localScale);
                        levelObj.transform.position = change;
                    }
                    else if (TransformStateButton.state == 1)
                    {
                        LevelObject levelObj = FindLevelObject(obj.transform.gameObject);
                        Vector3 original = Instance.FocusedLevelObject.transform.eulerAngles;
                        Vector3 difference = new Vector3(XField.Value - original.x, YField.Value - original.y, ZField.Value - original.z);
                        Vector3 levelObjOriginal = levelObj.transform.eulerAngles;
                        Vector3 change = new Vector3(levelObjOriginal.x + difference.x, levelObjOriginal.y + difference.y, levelObjOriginal.z + difference.z);
                        LevelObjects.registerTransformObject(levelObj.transform, levelObj.transform.position, Quaternion.Euler(change), levelObj.transform.localScale, levelObj.transform.position, levelObj.transform.rotation, levelObj.transform.localScale);
                    }
                    else
                    {
                        LevelObject levelObj = FindLevelObject(obj.transform.gameObject);
                        Vector3 difference = new Vector3(XField.Value - Instance.FocusedLevelObject.transform.localScale.x, YField.Value - Instance.FocusedLevelObject.transform.localScale.y, ZField.Value - Instance.FocusedLevelObject.transform.localScale.z);
                        Vector3 change = new Vector3(levelObj.transform.localScale.x + difference.x, levelObj.transform.localScale.y + difference.y, levelObj.transform.localScale.z + difference.z);
                        LevelObjects.registerTransformObject(levelObj.transform, levelObj.transform.localScale, levelObj.transform.rotation, change, levelObj.transform.position, levelObj.transform.rotation, levelObj.transform.localScale);
                    }
                }
            }
        }

        public static LevelObject FindLevelObject(GameObject rootGameObject)
        {
            if (rootGameObject == null)
                return null;
            Transform transform = rootGameObject.transform;
            if (Regions.tryGetCoordinate(transform.position, out byte x, out byte y))
            {
                for (int index = 0; index < LevelObjects.objects[x, y].Count; ++index)
                {
                    if (LevelObjects.objects[x, y][index].transform == transform)
                        return LevelObjects.objects[x, y][index];
                }
            }
            return null;
        }

        //ROYGBIV
        private static void OnClickedObjectHighlightColorButton(SleekButtonState button, int index)
        {
            if (index == 0)
            {
                Instance.HighlightColor = Color.red;
            }
            else if (index == 1)
            {
                Instance.HighlightColor = Palette.COLOR_O;
            }
            else if (index == 2)
            {
                Instance.HighlightColor = Color.yellow;
            }
            else if (index == 3)
            {
                Instance.HighlightColor = Color.green;
            }
            else if (index == 4)
            {
                Instance.HighlightColor = Color.blue;
            }
            else if (index == 5)
            {
                Instance.HighlightColor = new Color(75f, 0, 130f);
            }
            else if (index == 6)
            {
                Instance.HighlightColor = new Color(128f, 0, 225f);
            }
        }

        private static void OnClickedTransformStateButton(SleekButtonState button, int index)
        {
            Instance.UpdateFields(true);
        }

        private static void OnClickedObjectHighlightButton(ISleekElement button)
        {
            foreach (var list in LevelObjects.objects)
            {
                foreach (var obj in list)
                {
                    
                    if (obj.asset.id == ObjectIdField.Value)
                    {
                        HighlighterTool.highlight(obj.transform, Instance.HighlightColor);
                        HighlighterTool.highlight(obj.transform, Instance.HighlightColor);
                        if (!Instance.Highlighted.ContainsKey(obj.transform))
                        {
                            Instance.Highlighted.Add(obj.transform, Instance.HighlightColor);
                        }
                    }
                    

                }
            }
        }

        private static void OnClickedObjectHighlightSelectionButton(ISleekElement button)
        {
            foreach (EditorSelection obj in Instance.Selection)
            {
                HighlighterTool.highlight(obj.transform, Instance.HighlightColor);
                if (!Instance.Highlighted.ContainsKey(obj.transform))
                {
                    Instance.Highlighted.Add(obj.transform, Instance.HighlightColor);
                }

            }
        }

        private static void OnClickedClearObjectHighlightButton(ISleekElement button)
        {
            foreach (var obj in Instance.Highlighted)
            {
                HighlighterTool.unhighlight(obj.Key);
            }
            Instance.Highlighted.Clear();
        }


        public EditorCustomObjectUI() : base()
        {
            Instance = this;

            XField = Glazier.Get().CreateFloat32Field();
            XField.PositionOffset_X = 500;
            XField.PositionOffset_Y = -115;
            XField.PositionScale_X = 0.5f;
            XField.PositionScale_Y = 0.5f;
            XField.SizeOffset_X = 200;
            XField.SizeOffset_Y = 30;
            XField.Value = 0f;
            XField.AddLabel("X", ESleekSide.LEFT);
            Container.AddChild(XField);

            YField = Glazier.Get().CreateFloat32Field();
            YField.PositionOffset_X = -100;
            YField.PositionOffset_Y = -75;
            YField.PositionScale_X = 0.5f;
            YField.PositionScale_Y = 0.5f;
            YField.SizeOffset_X = 200;
            YField.SizeOffset_Y = 30;
            YField.Value = 0f;
            YField.AddLabel("Y", ESleekSide.LEFT);
            Container.AddChild(YField);

            ZField = Glazier.Get().CreateFloat32Field();
            ZField.PositionOffset_X = -100;
            ZField.PositionOffset_Y = -35;
            ZField.PositionScale_X = 0.5f;
            ZField.PositionScale_Y = 0.5f;
            ZField.SizeOffset_X = 200;
            ZField.SizeOffset_Y = 30;
            ZField.Value = 0f;
            ZField.AddLabel("Z", ESleekSide.LEFT);
            Container.AddChild(ZField);

            TransformStateButton = new SleekButtonState(new GUIContent("Translate"), new GUIContent("Rotate"), new GUIContent("Scale"));
            TransformStateButton.PositionOffset_X = -100;
            TransformStateButton.PositionOffset_Y = 5;
            TransformStateButton.PositionScale_X = 0.5f;
            TransformStateButton.PositionScale_Y = 0.5f;
            TransformStateButton.SizeOffset_X = 200;
            TransformStateButton.SizeOffset_Y = 30;
            TransformStateButton.AddLabel("Mode", ESleekSide.LEFT);
            TransformStateButton.tooltip = "Changes the precise mode.";
            TransformStateButton.onSwappedState += OnClickedTransformStateButton;
            Container.AddChild(TransformStateButton);

            ApplyButton = Glazier.Get().CreateButton();
            ApplyButton.PositionOffset_X = -100;
            ApplyButton.PositionOffset_Y = 45;
            ApplyButton.PositionScale_X = 0.5f;
            ApplyButton.PositionScale_Y = 0.5f;
            ApplyButton.SizeOffset_X = 200;
            ApplyButton.SizeOffset_Y = 30;
            ApplyButton.Text = "Apply";
            ApplyButton.TooltipText = "Applies all translations.";
            ApplyButton.OnClicked += OnClickedApplyButton;
            Container.AddChild(ApplyButton);

            Highlighted = new Dictionary<Transform, Color>();

            ObjectIdField = Glazier.Get().CreateUInt16Field();
            ObjectIdField.PositionOffset_X = -100;
            ObjectIdField.PositionOffset_Y = 125;
            ObjectIdField.PositionScale_X = 0.5f;
            ObjectIdField.PositionScale_Y = 0.5f;
            ObjectIdField.SizeOffset_X = 200;
            ObjectIdField.SizeOffset_Y = 30;
            ObjectIdField.TooltipText = "The unsigned 16-bit integer ID of an object.";
            ObjectIdField.AddLabel("ID", ESleekSide.LEFT);
            Container.AddChild(ObjectIdField);

            //ROYGBIV
            HighlightColor = Color.red;
            ObjectHightlightColorButton = new SleekButtonState(new GUIContent("Red"), new GUIContent("Orange"), new GUIContent("Yellow"), new GUIContent("Green"), new GUIContent("Indigo"), new GUIContent("Violet"));
            ObjectHightlightColorButton.PositionOffset_X = -100;
            ObjectHightlightColorButton.PositionOffset_Y = 165;
            ObjectHightlightColorButton.PositionScale_X = 0.5f;
            ObjectHightlightColorButton.PositionScale_Y = 0.5f;
            ObjectHightlightColorButton.SizeOffset_X = 200;
            ObjectHightlightColorButton.SizeOffset_Y = 30;
            ObjectHightlightColorButton.tooltip = "Changes the color of the object highlight.";
            ObjectHightlightColorButton.onSwappedState += OnClickedObjectHighlightColorButton;
            Container.AddChild(ObjectHightlightColorButton);

            ObjectHighlightButton = Glazier.Get().CreateButton();
            ObjectHighlightButton.PositionOffset_X = -100;
            ObjectHighlightButton.PositionOffset_Y = 205;
            ObjectHighlightButton.PositionScale_X = 0.5f;
            ObjectHighlightButton.PositionScale_Y = 0.5f;
            ObjectHighlightButton.SizeOffset_X = 200;
            ObjectHighlightButton.SizeOffset_Y = 30;
            ObjectHighlightButton.Text = "Highlight Objects By ID";
            ObjectHighlightButton.TooltipText = "Highlights the specified object.";
            ObjectHighlightButton.OnClicked += OnClickedObjectHighlightButton;
            Container.AddChild(ObjectHighlightButton);

            ObjectHighlightSelectionButton = Glazier.Get().CreateButton();
            ObjectHighlightSelectionButton.PositionOffset_X = -100;
            ObjectHighlightSelectionButton.PositionOffset_Y = 245;
            ObjectHighlightSelectionButton.PositionScale_X = 0.5f;
            ObjectHighlightSelectionButton.PositionScale_Y = 0.5f;
            ObjectHighlightSelectionButton.SizeOffset_X = 200;
            ObjectHighlightSelectionButton.SizeOffset_Y = 30;
            ObjectHighlightSelectionButton.Text = "Highlight Objects By Selection";
            ObjectHighlightSelectionButton.TooltipText = "Highlights the selected objects.";
            ObjectHighlightSelectionButton.OnClicked += OnClickedObjectHighlightSelectionButton;
            Container.AddChild(ObjectHighlightSelectionButton);

            clearObjectHighlightButton = Glazier.Get().CreateButton();
            clearObjectHighlightButton.PositionOffset_X = -100;
            clearObjectHighlightButton.PositionOffset_Y = 285;
            clearObjectHighlightButton.PositionScale_X = 0.5f;
            clearObjectHighlightButton.PositionScale_Y = 0.5f;
            clearObjectHighlightButton.SizeOffset_X = 200;
            clearObjectHighlightButton.SizeOffset_Y = 30;
            clearObjectHighlightButton.Text = "Clear Object Highlights";
            clearObjectHighlightButton.TooltipText = "Clears all object highlights.";
            clearObjectHighlightButton.OnClicked += OnClickedClearObjectHighlightButton;
            Container.AddChild(clearObjectHighlightButton);

            EditorCustomUIAutoSpacer spacer = new(this, 0, 40);

            OnUpdateManager.Instance.OnUpdate += HandleHotkey;
            Level.onLevelExited += HandleLevelExit;
        }

        public static void HandleLevelExit()
        {
            OnUpdateManager.Instance.OnUpdate -= HandleHotkey;
            Level.onLevelExited -= HandleLevelExit;
        }

        private static void HandleHotkey()
        {
            if (Level.isLoaded && Level.isEditor)
            {
                LevelObject obj = Instance.FocusedLevelObject;
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.C) && obj != null)
                {
                    ObjectAsset asset = obj.asset;
                    GUIUtility.systemCopyBuffer = $"{asset.GUID}\n{asset.id}\n{asset.objectName}";
                }
                
            }
        }
    }

    [HarmonyPatch(typeof(EditorLevelObjectsUI))]
    [HarmonyPatch("OnUpdate")]
    class OnUpdateObjectUIPatch
    {
        [HarmonyPostfix]
        internal static void OnUpdatePostfix()
        {
            EditorCustomObjectUI.Instance?.UpdateFields();
        }

    }

    [HarmonyPatch(typeof(EditorLevelObjectsUI))]
    [HarmonyPatch("open")]
    class EditorObjectsUIOpenPatch
    {
        [HarmonyPostfix]
        internal static void openPostfix()
        {
            EditorCustomObjectUI.Instance.Open();
        }
    }

    [HarmonyPatch(typeof(EditorLevelObjectsUI))]
    [HarmonyPatch("close")]
    class EditorObjectsUIClosePatch
    {
        [HarmonyPostfix]
        internal static void closePostfix()
        {
            EditorCustomObjectUI.Instance.Close();
        }
    }

    [HarmonyPatch(typeof(EditorObjects))]
    [HarmonyPatch("Update")]
    class UpdateEditorObjectsPatch
    {
        [HarmonyPostfix]
        internal static void UpdatePostfix()
        {
            EditorCustomObjectUI.Instance?.UpdateHighlights();
        }
    }
}
