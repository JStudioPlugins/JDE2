using HarmonyLib;
using JDE2.Managers;
using JDE2.SituationDependent;
using JDE2.Tools;
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

        public override int ContainerOffsetX => 600;
        public override int ContainerOffsetY => 10;
        public override float ContainerPositionScaleX => 1f;
        public override int ContainerSizeOffsetX => -20;
        public override int ContainerSizeOffsetY => -20;
        public override float ContainerSizeScaleX => 1f;
        public override float ContainerSizeScaleY => 1f;

        private static ISleekFloat32Field XField;

        private static ISleekFloat32Field YField;

        private static ISleekFloat32Field ZField;

        private static ISleekButton ApplyButton;

        private static SleekButtonState TransformStateButton;

        private static ISleekUInt16Field ObjectIdField;

        private static SleekButtonState ObjectHightlightColorButton;

        private static ISleekButton ObjectHighlightButton;

        private static ISleekButton ObjectHighlightSelectionButton;

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
                            XField.state = FocusedLevelObject.transform.position.x;
                            YField.state = FocusedLevelObject.transform.position.y;
                            ZField.state = FocusedLevelObject.transform.position.z;
                        }
                    }
                    else
                    {
                        LastPosition = FocusedLevelObject.transform.position;
                        XField.state = FocusedLevelObject.transform.position.x;
                        YField.state = FocusedLevelObject.transform.position.y;
                        ZField.state = FocusedLevelObject.transform.position.z;
                    }
                }
                else if (TransformStateButton.state == 1)
                {
                    if (LastRotation != null || updateAnyways)
                    {
                        if (FocusedLevelObject.transform.eulerAngles != LastRotation)
                        {
                            LastRotation = FocusedLevelObject.transform.eulerAngles;
                            XField.state = FocusedLevelObject.transform.eulerAngles.x;
                            YField.state = FocusedLevelObject.transform.eulerAngles.y;
                            ZField.state = FocusedLevelObject.transform.eulerAngles.z;
                        }
                    }
                    else
                    {
                        LastRotation = FocusedLevelObject.transform.eulerAngles;
                        XField.state = FocusedLevelObject.transform.eulerAngles.x;
                        YField.state = FocusedLevelObject.transform.eulerAngles.y;
                        ZField.state = FocusedLevelObject.transform.eulerAngles.z;
                    }
                }
                else
                {
                    if (LastScale != null || updateAnyways)
                    {
                        if (FocusedLevelObject.transform.localScale != LastScale)
                        {
                            LastRotation = FocusedLevelObject.transform.localScale;
                            XField.state = FocusedLevelObject.transform.localScale.x;
                            YField.state = FocusedLevelObject.transform.localScale.y;
                            ZField.state = FocusedLevelObject.transform.localScale.z;
                        }
                    }
                    else
                    {
                        LastRotation = FocusedLevelObject.transform.localScale;
                        XField.state = FocusedLevelObject.transform.localScale.x;
                        YField.state = FocusedLevelObject.transform.localScale.y;
                        ZField.state = FocusedLevelObject.transform.localScale.z;
                    }
                }
            }
            else
            {
                XField.state = 0f;
                YField.state = 0f;
                ZField.state = 0f;
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
                        Vector3 difference = new Vector3(XField.state - Instance.FocusedLevelObject.transform.position.x, YField.state - Instance.FocusedLevelObject.transform.position.y, ZField.state - Instance.FocusedLevelObject.transform.position.z);
                        Vector3 change = new Vector3(levelObj.transform.position.x + difference.x, levelObj.transform.position.y + difference.y, levelObj.transform.position.z + difference.z);
                        LevelObjects.registerTransformObject(levelObj.transform, change, levelObj.transform.rotation, levelObj.transform.localScale, levelObj.transform.position, levelObj.transform.rotation, levelObj.transform.localScale);
                        levelObj.transform.position = change;
                    }
                    else if (TransformStateButton.state == 1)
                    {
                        LevelObject levelObj = FindLevelObject(obj.transform.gameObject);
                        Vector3 original = Instance.FocusedLevelObject.transform.eulerAngles;
                        Vector3 difference = new Vector3(XField.state - original.x, YField.state - original.y, ZField.state - original.z);
                        Vector3 levelObjOriginal = levelObj.transform.eulerAngles;
                        Vector3 change = new Vector3(levelObjOriginal.x + difference.x, levelObjOriginal.y + difference.y, levelObjOriginal.z + difference.z);
                        LevelObjects.registerTransformObject(levelObj.transform, levelObj.transform.position, Quaternion.Euler(change), levelObj.transform.localScale, levelObj.transform.position, levelObj.transform.rotation, levelObj.transform.localScale);
                    }
                    else
                    {
                        LevelObject levelObj = FindLevelObject(obj.transform.gameObject);
                        Vector3 difference = new Vector3(XField.state - Instance.FocusedLevelObject.transform.localScale.x, YField.state - Instance.FocusedLevelObject.transform.localScale.y, ZField.state - Instance.FocusedLevelObject.transform.localScale.z);
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
                    
                    if (obj.asset.id == ObjectIdField.state)
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
            XField.positionOffset_X = -100;
            XField.positionOffset_Y = -115;
            XField.positionScale_X = 0.5f;
            XField.positionScale_Y = 0.5f;
            XField.sizeOffset_X = 200;
            XField.sizeOffset_Y = 30;
            XField.state = 0f;
            XField.addLabel("X", ESleekSide.LEFT);
            Container.AddChild(XField);

            YField = Glazier.Get().CreateFloat32Field();
            YField.positionOffset_X = -100;
            YField.positionOffset_Y = -75;
            YField.positionScale_X = 0.5f;
            YField.positionScale_Y = 0.5f;
            YField.sizeOffset_X = 200;
            YField.sizeOffset_Y = 30;
            YField.state = 0f;
            YField.addLabel("Y", ESleekSide.LEFT);
            Container.AddChild(YField);

            ZField = Glazier.Get().CreateFloat32Field();
            ZField.positionOffset_X = -100;
            ZField.positionOffset_Y = -35;
            ZField.positionScale_X = 0.5f;
            ZField.positionScale_Y = 0.5f;
            ZField.sizeOffset_X = 200;
            ZField.sizeOffset_Y = 30;
            ZField.state = 0f;
            ZField.addLabel("Z", ESleekSide.LEFT);
            Container.AddChild(ZField);

            TransformStateButton = new SleekButtonState(new GUIContent("Translate"), new GUIContent("Rotate"), new GUIContent("Scale"));
            TransformStateButton.positionOffset_X = -100;
            TransformStateButton.positionOffset_Y = 5;
            TransformStateButton.positionScale_X = 0.5f;
            TransformStateButton.positionScale_Y = 0.5f;
            TransformStateButton.sizeOffset_X = 200;
            TransformStateButton.sizeOffset_Y = 30;
            TransformStateButton.addLabel("Mode", ESleekSide.LEFT);
            TransformStateButton.tooltip = "Changes the precise mode.";
            TransformStateButton.onSwappedState += OnClickedTransformStateButton;
            Container.AddChild(TransformStateButton);

            ApplyButton = Glazier.Get().CreateButton();
            ApplyButton.positionOffset_X = -100;
            ApplyButton.positionOffset_Y = 45;
            ApplyButton.positionScale_X = 0.5f;
            ApplyButton.positionScale_Y = 0.5f;
            ApplyButton.sizeOffset_X = 200;
            ApplyButton.sizeOffset_Y = 30;
            ApplyButton.text = "Apply";
            ApplyButton.tooltipText = "Applies all translations.";
            ApplyButton.onClickedButton += OnClickedApplyButton;
            Container.AddChild(ApplyButton);

            Highlighted = new Dictionary<Transform, Color>();

            ObjectIdField = Glazier.Get().CreateUInt16Field();
            ObjectIdField.positionOffset_X = -100;
            ObjectIdField.positionOffset_Y = 125;
            ObjectIdField.positionScale_X = 0.5f;
            ObjectIdField.positionScale_Y = 0.5f;
            ObjectIdField.sizeOffset_X = 200;
            ObjectIdField.sizeOffset_Y = 30;
            ObjectIdField.tooltipText = "The unsigned 16-bit integer ID of an object.";
            ObjectIdField.addLabel("ID", ESleekSide.LEFT);
            Container.AddChild(ObjectIdField);

            //ROYGBIV
            HighlightColor = Color.red;
            ObjectHightlightColorButton = new SleekButtonState(new GUIContent("Red"), new GUIContent("Orange"), new GUIContent("Yellow"), new GUIContent("Green"), new GUIContent("Indigo"), new GUIContent("Violet"));
            ObjectHightlightColorButton.positionOffset_X = -100;
            ObjectHightlightColorButton.positionOffset_Y = 165;
            ObjectHightlightColorButton.positionScale_X = 0.5f;
            ObjectHightlightColorButton.positionScale_Y = 0.5f;
            ObjectHightlightColorButton.sizeOffset_X = 200;
            ObjectHightlightColorButton.sizeOffset_Y = 30;
            ObjectHightlightColorButton.tooltip = "Changes the color of the object highlight.";
            ObjectHightlightColorButton.onSwappedState += OnClickedObjectHighlightColorButton;
            Container.AddChild(ObjectHightlightColorButton);

            ObjectHighlightButton = Glazier.Get().CreateButton();
            ObjectHighlightButton.positionOffset_X = -100;
            ObjectHighlightButton.positionOffset_Y = 205;
            ObjectHighlightButton.positionScale_X = 0.5f;
            ObjectHighlightButton.positionScale_Y = 0.5f;
            ObjectHighlightButton.sizeOffset_X = 200;
            ObjectHighlightButton.sizeOffset_Y = 30;
            ObjectHighlightButton.text = "Highlight Objects By ID";
            ObjectHighlightButton.tooltipText = "Highlights the specified object.";
            ObjectHighlightButton.onClickedButton += OnClickedObjectHighlightButton;
            Container.AddChild(ObjectHighlightButton);

            ObjectHighlightSelectionButton = Glazier.Get().CreateButton();
            ObjectHighlightSelectionButton.positionOffset_X = -100;
            ObjectHighlightSelectionButton.positionOffset_Y = 245;
            ObjectHighlightSelectionButton.positionScale_X = 0.5f;
            ObjectHighlightSelectionButton.positionScale_Y = 0.5f;
            ObjectHighlightSelectionButton.sizeOffset_X = 200;
            ObjectHighlightSelectionButton.sizeOffset_Y = 30;
            ObjectHighlightSelectionButton.text = "Highlight Objects By Selection";
            ObjectHighlightSelectionButton.tooltipText = "Highlights the selected objects.";
            ObjectHighlightSelectionButton.onClickedButton += OnClickedObjectHighlightSelectionButton;
            Container.AddChild(ObjectHighlightSelectionButton);

            clearObjectHighlightButton = Glazier.Get().CreateButton();
            clearObjectHighlightButton.positionOffset_X = -100;
            clearObjectHighlightButton.positionOffset_Y = 285;
            clearObjectHighlightButton.positionScale_X = 0.5f;
            clearObjectHighlightButton.positionScale_Y = 0.5f;
            clearObjectHighlightButton.sizeOffset_X = 200;
            clearObjectHighlightButton.sizeOffset_Y = 30;
            clearObjectHighlightButton.text = "Clear Object Highlights";
            clearObjectHighlightButton.tooltipText = "Clears all object highlights.";
            clearObjectHighlightButton.onClickedButton += OnClickedClearObjectHighlightButton;
            Container.AddChild(clearObjectHighlightButton);

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
