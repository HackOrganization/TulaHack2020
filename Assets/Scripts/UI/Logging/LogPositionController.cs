using UnityEngine;
using UnityEngine.UI;
using Utils;

using MathWideField = Core.MathConversion.Utils.WideFieldParams;
using MathTightField = Core.MathConversion.Utils.TightFieldParams;

namespace UI.Logging
{
    public class LogPositionController: Singleton<LogPositionController>
    {
        [Header("WideField")] 
        [SerializeField] private Text azimuthWideFieldPositionText;
        
        [Header("TightField")] 
        [SerializeField] private Text azimuthTightFieldPositionText;
        [SerializeField] private Text elevationTightFieldPositionText;

        public static void UpdateInfo(in Vector2Int[] newPositions)
        {
            if(Instance == null)
                return;
            
            Instance.UpdateTexts(in newPositions);
        }

        private void UpdateTexts(in Vector2Int[] newPositions)
        {
            azimuthWideFieldPositionText.text = $"{-newPositions[0].x / MathWideField.AngleToSteps :###.00}";
            azimuthTightFieldPositionText.text = $"{-newPositions[1].x / MathTightField.AzimuthAngleToSteps :###.00}";
            elevationTightFieldPositionText.text = $"{-newPositions[1].y / MathTightField.ElevationAngleToSteps :###:.00}";
            
            // azimuthWideFieldPositionText.text = $"{-newPositions[0].x :###.00}";
            // azimuthTightFieldPositionText.text = $"{-newPositions[1].x :###.00}";
            // elevationTightFieldPositionText.text = $"{-newPositions[1].y :###.00}";
        }
    }
}