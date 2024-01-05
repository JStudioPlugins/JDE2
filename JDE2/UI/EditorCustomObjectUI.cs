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
    public class EditorCustomObjectUI : EditorCustomUI, IEditorDependent, IDisposable
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

        private static void OnClickedTransformStateButton(SleekButtonState button, int index)
        {
            Instance.UpdateFields(true);
        }



        public EditorCustomObjectUI() : base()
        {
            Instance = this;
            Bundle bundle = Bundles.getBundle("/Bundles/Textures/Edit/Icons/EditorLevelObjects/EditorLevelObjects.unity3d");

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

            TransformStateButton = new SleekButtonState(new GUIContent("Translate", bundle.load<Texture2D>("Transform")), new GUIContent("Rotate", bundle.load<Texture2D>("Rotate")), new GUIContent("Scale", bundle.load<Texture2D>("Scale")));
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

            EditorCustomUIAutoSpacer spacer = new(this, 0, 40);

            OnUpdateManager.Instance.OnUpdateEvent += HandleHotkey;
            bundle.unload();
        }

        ~EditorCustomObjectUI()
        {
            Dispose();
        }

        public void Dispose()
        {
            OnUpdateManager.Instance.OnUpdateEvent -= HandleHotkey;
            GC.SuppressFinalize(this);
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
}
