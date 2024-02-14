using HarmonyLib;
using JDE2.Managers;
using JDE2.SituationDependent;
using JDE2.Utils;
using MathEngine;
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
        private static ISleekField XField;

        private static float _x;

        [Element(1)]
        private static ISleekField YField;

        private static float _y;

        [Element(2)]
        private static ISleekField ZField;

        private static float _z;

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
                            _x = FocusedLevelObject.transform.position.x;
                            _y = FocusedLevelObject.transform.position.y;
                            _z = FocusedLevelObject.transform.position.z;
                        }
                    }
                    else
                    {
                        LastPosition = FocusedLevelObject.transform.position;
                        _x = FocusedLevelObject.transform.position.x;
                        _y = FocusedLevelObject.transform.position.y;
                        _z = FocusedLevelObject.transform.position.z;
                    }
                }
                else if (TransformStateButton.state == 1)
                {
                    if (LastRotation != null || updateAnyways)
                    {
                        if (FocusedLevelObject.transform.eulerAngles != LastRotation)
                        {
                            LastRotation = FocusedLevelObject.transform.eulerAngles;
                            _x = FocusedLevelObject.transform.eulerAngles.x;
                            _y = FocusedLevelObject.transform.eulerAngles.y;
                            _z = FocusedLevelObject.transform.eulerAngles.z;
                        }
                    }
                    else
                    {
                        LastRotation = FocusedLevelObject.transform.eulerAngles;
                        _x = FocusedLevelObject.transform.eulerAngles.x;
                        _y = FocusedLevelObject.transform.eulerAngles.y;
                        _z = FocusedLevelObject.transform.eulerAngles.z;
                    }
                }
                else
                {
                    if (LastScale != null || updateAnyways)
                    {
                        if (FocusedLevelObject.transform.localScale != LastScale)
                        {
                            LastScale = FocusedLevelObject.transform.localScale;
                            _x = FocusedLevelObject.transform.localScale.x;
                            _y = FocusedLevelObject.transform.localScale.y;
                            _z = FocusedLevelObject.transform.localScale.z;
                        }
                    }
                    else
                    {
                        LastScale = FocusedLevelObject.transform.localScale;
                        _x = FocusedLevelObject.transform.localScale.x;
                        _y = FocusedLevelObject.transform.localScale.y;
                        _z = FocusedLevelObject.transform.localScale.z;
                    }
                }
            }
            else
            {
                _x = 0f;
                _y = 0f;
                _z = 0f;
                LastPosition = new Vector3();
                LastRotation = new Vector3();
                LastScale = new Vector3();
            }
            ApplyFields();
        }

        private static void ApplyFields()
        {
            XField.Text = _x.ToString();
            YField.Text = _y.ToString();
            ZField.Text = _z.ToString();
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
                        Vector3 difference = new Vector3(_x - Instance.FocusedLevelObject.transform.position.x, _y - Instance.FocusedLevelObject.transform.position.y, _z - Instance.FocusedLevelObject.transform.position.z);
                        Vector3 change = new Vector3(levelObj.transform.position.x + difference.x, levelObj.transform.position.y + difference.y, levelObj.transform.position.z + difference.z);
                        LevelObjects.registerTransformObject(levelObj.transform, change, levelObj.transform.rotation, levelObj.transform.localScale, levelObj.transform.position, levelObj.transform.rotation, levelObj.transform.localScale);
                        levelObj.transform.position = change;
                    }
                    else if (TransformStateButton.state == 1)
                    {
                        LevelObject levelObj = FindLevelObject(obj.transform.gameObject);
                        Vector3 original = Instance.FocusedLevelObject.transform.eulerAngles;
                        Vector3 difference = new Vector3(_x - original.x, _y - original.y, _z - original.z);
                        Vector3 levelObjOriginal = levelObj.transform.eulerAngles;
                        Vector3 change = new Vector3(levelObjOriginal.x + difference.x, levelObjOriginal.y + difference.y, levelObjOriginal.z + difference.z);
                        LevelObjects.registerTransformObject(levelObj.transform, levelObj.transform.position, Quaternion.Euler(change), levelObj.transform.localScale, levelObj.transform.position, levelObj.transform.rotation, levelObj.transform.localScale);
                    }
                    else
                    {
                        LevelObject levelObj = FindLevelObject(obj.transform.gameObject);
                        Vector3 difference = new Vector3(_x - Instance.FocusedLevelObject.transform.localScale.x, _y - Instance.FocusedLevelObject.transform.localScale.y, _z - Instance.FocusedLevelObject.transform.localScale.z);
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

            XField = Glazier.Get().CreateStringField();
            XField.PositionOffset_X = 500;
            XField.PositionOffset_Y = -115;
            XField.PositionScale_X = 0.5f;
            XField.PositionScale_Y = 0.5f;
            XField.SizeOffset_X = 200;
            XField.SizeOffset_Y = 30;
            XField.Text = 0f.ToString();
            XField.AddLabel("X", ESleekSide.LEFT);
            XField.OnTextSubmitted += XField_OnTextSubmitted;
            _x = 0f;
            Container.AddChild(XField);

            YField = Glazier.Get().CreateStringField();
            YField.PositionOffset_X = -100;
            YField.PositionOffset_Y = -75;
            YField.PositionScale_X = 0.5f;
            YField.PositionScale_Y = 0.5f;
            YField.SizeOffset_X = 200;
            YField.SizeOffset_Y = 30;
            YField.Text = 0f.ToString();
            YField.AddLabel("Y", ESleekSide.LEFT);
            YField.OnTextSubmitted += YField_OnTextSubmitted;
            _y = 0f;
            Container.AddChild(YField);

            ZField = Glazier.Get().CreateStringField();
            ZField.PositionOffset_X = -100;
            ZField.PositionOffset_Y = -35;
            ZField.PositionScale_X = 0.5f;
            ZField.PositionScale_Y = 0.5f;
            ZField.SizeOffset_X = 200;
            ZField.SizeOffset_Y = 30;
            ZField.Text = 0f.ToString();
            ZField.AddLabel("Z", ESleekSide.LEFT);
            ZField.OnTextSubmitted += ZField_OnTextSubmitted;
            _z = 0f;
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

        private void ZField_OnTextSubmitted(ISleekField field)
        {
            var math = new MathEngine.MathEngine();
            _z = (float)math.Calculate(ZField.Text);
        }

        private void YField_OnTextSubmitted(ISleekField field)
        {
            var math = new MathEngine.MathEngine();
            _y = (float)math.Calculate(YField.Text);
        }

        private void XField_OnTextSubmitted(ISleekField field)
        {
            var math = new MathEngine.MathEngine();
            _x = (float)math.Calculate(XField.Text);
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
