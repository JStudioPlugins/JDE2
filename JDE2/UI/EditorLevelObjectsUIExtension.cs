using DanielWillett.UITools.API.Extensions;
using DanielWillett.UITools.API.Extensions.Members;
using DanielWillett.UITools.Util;
using JDE2.Utils;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MeasurementTool = JDE2.Utils.MeasurementTool;

namespace JDE2.UI;

[UIExtension(typeof(EditorLevelObjectsUI))]
public class EditorLevelObjectsUIExtension : UIExtensionWrapper
{
    [ExistingMember("container")]
    private readonly SleekFullscreenBox _container;

    private readonly SleekFullscreenBox _extraContainer;

    private bool active;

    [ExistingMember("materialPaletteOverrideField")]
    private readonly ISleekField _materialPaletteOverrideField;

    [ExistingMember("materialIndexOverrideField")]
    private readonly ISleekInt32Field _materialIndexOverrideField;

    [ExistingMember("snapTransformField")]
    private readonly ISleekFloat32Field _snapTransformField;

    [ExistingMember("snapRotationField")]
    private readonly ISleekFloat32Field _snapRotationField;

    [ExistingMember("transformButton")]
    private readonly SleekButtonIcon _transformButton;

    [ExistingMember("rotateButton")]
    private readonly SleekButtonIcon _rotateButton;

    [ExistingMember("scaleButton")]
    private readonly SleekButtonIcon _scaleButton;

    [ExistingMember("coordinateButton")]
    private readonly SleekButtonState _coordinateButton;

    public void OpenHighlighting()
    {
        _materialIndexOverrideField.IsVisible = false;
        _materialPaletteOverrideField.IsVisible = false;
        _snapTransformField.IsVisible = false;
        _snapRotationField.IsVisible = false;
        _transformButton.IsVisible = false;
        _rotateButton.IsVisible = false;
        _scaleButton.IsVisible = false;
        _coordinateButton.IsVisible = false;

        _measurementButton.IsVisible = false;

        _highlightingButton.IsVisible = false;
        
        _objectIdField.IsVisible = true;

        _objectHightlightColorButton.IsVisible = true;

        _objectHighlightButton.IsVisible = true;

        _objectHighlightSelectionButton.IsVisible = true;

        _clearObjectHighlightButton.IsVisible = true;

        _exitButton.IsVisible = true;
        

        HighlightingTool.Get().SelectedColor = HighlightingTool.ColorFromIndex(_objectHightlightColorButton.state);
    }

    public void CloseHighlighting()
    {
        _materialIndexOverrideField.IsVisible = true;
        _materialPaletteOverrideField.IsVisible = true;
        _snapTransformField.IsVisible = true;
        _snapRotationField.IsVisible = true;
        _transformButton.IsVisible = true;
        _rotateButton.IsVisible = true;
        _scaleButton.IsVisible = true;
        _coordinateButton.IsVisible = true;
        _highlightingButton.IsVisible = true;

        _measurementButton.IsVisible = true;

        _objectIdField.IsVisible = false;

        _objectHightlightColorButton.IsVisible = false;

        _objectHighlightButton.IsVisible = false;

        _objectHighlightSelectionButton.IsVisible = false;

        _clearObjectHighlightButton.IsVisible = false;

        _exitButton.IsVisible = false;


        HighlightingTool.Get().ClearHighlights();
    }

    public void OpenMeasurements()
    {
        MeasurementTool.Activate();

        _materialIndexOverrideField.IsVisible = false;
        _materialPaletteOverrideField.IsVisible = false;
        _snapTransformField.IsVisible = false;
        _snapRotationField.IsVisible = false;
        _transformButton.IsVisible = false;
        _rotateButton.IsVisible = false;
        _scaleButton.IsVisible = false;
        _coordinateButton.IsVisible = false;
        _measurementButton.IsVisible = false;
        _highlightingButton.IsVisible = false;

        MeasurementTool.Activate();

        EditorUIExtension.Get().ModifyMessageBox("[Ctrl] + [Left Click] to place measurement nodes.");

        _exitButton.IsVisible = true;
    }

    public void CloseMeasurements()
    {
        _materialIndexOverrideField.IsVisible = true;
        _materialPaletteOverrideField.IsVisible = true;
        _snapTransformField.IsVisible = true;
        _snapRotationField.IsVisible = true;
        _transformButton.IsVisible = true;
        _rotateButton.IsVisible = true;
        _scaleButton.IsVisible = true;
        _coordinateButton.IsVisible = true;
        _highlightingButton.IsVisible = true;

        _measurementButton.IsVisible = true;

        MeasurementTool.Deactivate();

        EditorUIExtension.Get().ModifyMessageBox(isVisible: false);

        EditorUI.message(EEditorMessage.OBJECTS);
    }

    private readonly SleekButtonIcon _highlightingButton;

    private readonly SleekButtonIcon _measurementButton;

    private readonly ISleekUInt16Field _objectIdField;

    private readonly SleekButtonState _objectHightlightColorButton;

    private readonly ISleekButton _objectHighlightButton;

    private readonly ISleekButton _objectHighlightSelectionButton;

    private readonly ISleekButton _clearObjectHighlightButton;

    private readonly ISleekButton _exitButton;

    public EditorLevelObjectsUIExtension()
    {
        Bundle icons = new Bundle(Path.Combine(Main.Directory, "Assets", "Icons.unity3d"), false);

        _extraContainer = new SleekFullscreenBox();
        _extraContainer.PositionOffset_X = 10f;
        _extraContainer.PositionOffset_Y = 90f;
        _extraContainer.PositionScale_X = 1f;
        _extraContainer.SizeOffset_X = -20f;
        _extraContainer.SizeOffset_Y = -100f;
        _extraContainer.SizeScale_X = 1f;
        _extraContainer.SizeScale_Y = 1f;
        active = false;
        EditorUI.window.AddChild(_extraContainer);

        _measurementButton = new SleekButtonIcon(icons.load<Texture2D>("Measurements"));
        _measurementButton.PositionOffset_Y = -390;
        _measurementButton.PositionScale_Y = 1f;
        _measurementButton.SizeOffset_X = 200f;
        _measurementButton.SizeOffset_Y = 30f;
        _measurementButton.text = "Measurements";
        _measurementButton.tooltip = "Enables measurement (JDE2).";
        _measurementButton.onClickedButton += OnClickedMeasurementButton;
        _extraContainer.AddChild(_measurementButton);

        _highlightingButton = new SleekButtonIcon(icons.load<Texture2D>("Highlighting"));
        _highlightingButton.PositionOffset_Y = -350f;
        _highlightingButton.PositionScale_Y = 1f;
        _highlightingButton.SizeOffset_X = 200f;
        _highlightingButton.SizeOffset_Y = 30f;
        _highlightingButton.text = "Highlighting";
        _highlightingButton.tooltip = "Shows highlighting options (JDE2).";
        _highlightingButton.onClickedButton += OnClickedHighlightingButton;
        _extraContainer.AddChild(_highlightingButton);

        _objectIdField = Glazier.Get().CreateUInt16Field();
        _objectIdField.PositionOffset_Y = -230;
        _objectIdField.PositionScale_Y = 1f;
        _objectIdField.SizeOffset_X = 200;
        _objectIdField.SizeOffset_Y = 30;
        _objectIdField.TooltipText = "The unsigned 16-bit integer ID of an object.";
        _objectIdField.AddLabel("ID", ESleekSide.RIGHT);
        _objectIdField.IsVisible = false;
        _extraContainer.AddChild(_objectIdField);

        _objectHightlightColorButton = new SleekButtonState(new GUIContent("Red"), new GUIContent("Orange"), new GUIContent("Yellow"), new GUIContent("Green"), new GUIContent("Indigo"), new GUIContent("Violet"));
        _objectHightlightColorButton.PositionOffset_Y = -190;
        _objectHightlightColorButton.PositionScale_Y = 1f;
        _objectHightlightColorButton.SizeOffset_X = 200;
        _objectHightlightColorButton.SizeOffset_Y = 30;
        _objectHightlightColorButton.tooltip = "Changes the color of the object highlight.";
        _objectHightlightColorButton.onSwappedState += OnClickedObjectHighlightColorButton;
        _objectHightlightColorButton.IsVisible = false;
        _extraContainer.AddChild(_objectHightlightColorButton);

        _objectHighlightButton = Glazier.Get().CreateButton();
        _objectHighlightButton.PositionOffset_Y = -150;
        _objectHighlightButton.PositionScale_Y = 1f;
        _objectHighlightButton.SizeOffset_X = 200;
        _objectHighlightButton.SizeOffset_Y = 30;
        _objectHighlightButton.Text = "Highlight Objects By ID";
        _objectHighlightButton.TooltipText = "Highlights the specified object.";
        _objectHighlightButton.OnClicked += OnClickedObjectHighlightButton;
        _objectHighlightButton.IsVisible = false;
        _extraContainer.AddChild(_objectHighlightButton);

        _objectHighlightSelectionButton = Glazier.Get().CreateButton();
        _objectHighlightSelectionButton.PositionOffset_Y = -110;
        _objectHighlightSelectionButton.PositionScale_Y = 1f;
        _objectHighlightSelectionButton.SizeOffset_X = 200;
        _objectHighlightSelectionButton.SizeOffset_Y = 30;
        _objectHighlightSelectionButton.Text = "Highlight Objects By Selection";
        _objectHighlightSelectionButton.TooltipText = "Highlights the selected objects.";
        _objectHighlightSelectionButton.OnClicked += OnClickedObjectHighlightSelectionButton;
        _objectHighlightSelectionButton.IsVisible = false;
        _extraContainer.AddChild(_objectHighlightSelectionButton);

        _clearObjectHighlightButton = Glazier.Get().CreateButton();
        _clearObjectHighlightButton.PositionOffset_Y = -70;
        _clearObjectHighlightButton.PositionScale_Y = 1f;
        _clearObjectHighlightButton.SizeOffset_X = 200;
        _clearObjectHighlightButton.SizeOffset_Y = 30;
        _clearObjectHighlightButton.Text = "Clear Object Highlights";
        _clearObjectHighlightButton.TooltipText = "Clears all object highlights.";
        _clearObjectHighlightButton.OnClicked += OnClickedClearObjectHighlightButton;
        _clearObjectHighlightButton.IsVisible = false;
        _extraContainer.AddChild(_clearObjectHighlightButton);

        _exitButton = Glazier.Get().CreateButton();
        _exitButton.PositionOffset_Y = -30;
        _exitButton.PositionScale_Y = 1f;
        _exitButton.SizeOffset_X = 200;
        _exitButton.SizeOffset_Y = 30;
        _exitButton.Text = "Back";
        _exitButton.TooltipText = "Closes.";
        _exitButton.OnClicked += OnClickedExitButton;
        _exitButton.IsVisible = false;
        _extraContainer.AddChild(_exitButton);
        icons.unload();
        this.OnOpened += Open;
        this.OnClosed += Close;
        //AnimatePositionScale(x, y, ESleekLerp.EXPONENTIAL, 20f);

    }

    private void OnClickedMeasurementButton(ISleekElement button)
    {
        OpenMeasurements();
    }

    private void Close()
    {
        if (active)
        {
            active = false;
            _extraContainer.AnimateOutOfView(1f, 0f);
        }
    }

    private void Open()
    {
        if (!active)
        {
            active = true;
            _extraContainer.AnimateIntoView();
        }
    }

    private void OnClickedObjectHighlightButton(ISleekElement button)
    {
        HighlightingTool.Get().AddObjectIdHighlights();
    }

    private void OnClickedObjectHighlightSelectionButton(ISleekElement button)
    {
        HighlightingTool.Get().AddObjectSelectionHighlights();
    }

    private void OnClickedClearObjectHighlightButton(ISleekElement button)
    {
        HighlightingTool.Get().ClearHighlights();
    }

    private void OnClickedExitButton(ISleekElement button)
    {
        CloseHighlighting();
        CloseMeasurements();
    }

    private void OnClickedObjectHighlightColorButton(SleekButtonState button, int index)
    {
        HighlightingTool.Get().SelectedColor = HighlightingTool.ColorFromIndex(_objectHightlightColorButton.state);
    }

    private void OnClickedHighlightingButton(ISleekElement button)
    {
        OpenHighlighting();
    }

    public override void Dispose()
    {
        _extraContainer.TryRemoveChild(_highlightingButton);
        _extraContainer.TryRemoveChild(_objectIdField);
        _extraContainer.TryRemoveChild(_objectHightlightColorButton);
        _extraContainer.TryRemoveChild(_objectHighlightButton);
        _extraContainer.TryRemoveChild(_objectHighlightSelectionButton);
        _extraContainer.TryRemoveChild(_clearObjectHighlightButton);
        _extraContainer.TryRemoveChild(_exitButton);
        EditorUI.window.TryRemoveChild(_extraContainer);
    }

}


