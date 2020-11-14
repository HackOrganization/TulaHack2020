using System.Collections.Generic;
using Core;
using Device.Hardware.HighLevel;
using Device.Hardware.LowLevel;
using Device.Hardware.LowLevel.Utils.Communication;
using Device.Utils;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.EventType;

namespace Device.Hardware.Test
{
    public class HardwareTestController: HardwareController
    {
        [Header("Sliders")] 
        [SerializeField] private Slider wideFieldSlider;
        [SerializeField] private Slider tightFieldXSlider;
        [SerializeField] private Slider tightFieldYSlider;

        [Header("Text components")] 
        [SerializeField] private Text wideFieldSliderValueText;
        [SerializeField] private Text tightFieldSliderXValueText;
        [SerializeField] private Text tightFieldSliderYValueText;

        private IEnumerable<Slider> Sliders => new[]
        {
            wideFieldSlider, tightFieldXSlider, tightFieldYSlider
        };
        
        private void Start()
        {
            wideFieldController = gameObject.AddComponent<WideFieldCameraController>();
            tightFieldController = gameObject.AddComponent<TightFieldCameraController>();
            
            Initialize();
        }


        private new void Initialize()
        {
            base.Initialize();

            SetUpSliders();
        }

        private void SetUpSliders()
        {
            foreach (var slider in Sliders)
            {
                slider.minValue = 0;
                slider.maxValue = CommunicationParams.FULL_LOOP_STEPS;
                slider.wholeNumbers = true;
            }
            
            wideFieldSlider.onValueChanged.AddListener(
                value => SetNewPosition(
                    wideFieldSliderValueText,
                    CameraTypes.WideField, 
                    value, 
                    RectTransform.Axis.Horizontal));
            
            tightFieldXSlider.onValueChanged.AddListener(
                value => SetNewPosition(
                    tightFieldSliderXValueText,
                    CameraTypes.TightField, 
                    value, 
                    RectTransform.Axis.Horizontal));
            
            tightFieldYSlider.onValueChanged.AddListener(
                value => SetNewPosition(
                    tightFieldSliderYValueText,
                    CameraTypes.TightField,
                    value, 
                    RectTransform.Axis.Vertical));
        }

        private void SetNewPosition(Text textComponent, CameraTypes cameraType, float value, RectTransform.Axis axis)
        {
            textComponent.text = $"{value}";
            
            var vectorValue =
                axis == RectTransform.Axis.Horizontal
                    ? new Vector2Int((int)value, 0)
                    : new Vector2Int(0, (int)value);
                    
            EventManager.RaiseEvent(EventType.DeviceGoPosition, cameraType, ushort.MinValue, vectorValue);
        }
    }
}