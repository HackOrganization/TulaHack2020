using System;
using System.Collections.Generic;
using Core;
using Device.Hardware.HighLevel;
using Device.Hardware.LowLevel;
using Device.Hardware.LowLevel.Utils.Communication;
using Device.Utils;
using UnityEngine;
using EventType = Core.EventType;

namespace Device.Hardware.Test
{
    public class HardwareKeyboardTestController: HardwareController
    {
        private static readonly Dictionary<int, Vector2Int> KeyMap = new Dictionary<int, Vector2Int>
        {
            {-1 , Vector2Int.left},
            {1 , Vector2Int.right},
            
            {-2 , Vector2Int.down},
            {2 , Vector2Int.up},
        };
        
        private static readonly Vector2Int NullValue = Vector2Int.zero;
        private static readonly Vector2Int NUllDeltaValue = Vector2Int.zero;
        
        private static readonly Vector2Int MinValue = Vector2Int.zero;
        private static readonly Vector2Int MaxValue = new Vector2Int(CommunicationParams.FULL_LOOP_STEPS, CommunicationParams.FULL_LOOP_STEPS);

        [Header("Settings")] 
        [SerializeField] private bool autoExecute;
        [SerializeField, Range(1, 10)] private int speed = 1;
        
        
        private Vector2Int[] _wideFiledValues = new Vector2Int[2];
        private Vector2Int[] _tightFiledValues = new Vector2Int[2];

        private void Start()
        {
            if(!autoExecute)
                return;
            
            wideFieldController = gameObject.AddComponent<WideFieldCameraController>();
            tightFieldController = gameObject.AddComponent<TightFieldCameraController>();
            
            base.Initialize();
        }

        private void Update()
        {
            
            if (Input.GetKey(KeyCode.A))
                _tightFiledValues[0] += KeyMap[-1] * speed;
            if (Input.GetKey(KeyCode.D))
                _tightFiledValues[0] += KeyMap[1] * speed;
            if (Input.GetKey(KeyCode.S))
                _tightFiledValues[0] += KeyMap[-2] * speed;
            if (Input.GetKey(KeyCode.W))
                _tightFiledValues[0] += KeyMap[2] * speed;
            
            if (Input.GetKey(KeyCode.Q))
                _wideFiledValues[0] += KeyMap[-1] * speed;
            if (Input.GetKey(KeyCode.E))
                _wideFiledValues[0] += KeyMap[1] * speed;

            _wideFiledValues[0].Clamp(MinValue, MaxValue);
            _tightFiledValues[0].Clamp(MinValue, MaxValue);


            SendPosition(CameraTypes.WideField, ref _wideFiledValues);
            SendPosition(CameraTypes.TightField, ref _tightFiledValues);
        }

        private void SendPosition(CameraTypes cameraType, ref Vector2Int[] array)
        {
            if (!array.IsChanged())
                return;
            
            array[1] = array[0];
            EventManager.RaiseEvent(EventType.DeviceGoPosition, cameraType, ushort.MinValue, array[1]);            
        }
    }

    public static class Extensions
    {
        public static bool IsChanged(this Vector2Int[] array)
            => array[0] != array[1];
    }
}