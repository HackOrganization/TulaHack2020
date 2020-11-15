using Device.Utils;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Logging
{
    public class LogPredictionController: Singleton<LogPredictionController>
    {
        [Header("Colors")] 
        [SerializeField] private Color detectedColor;
        [SerializeField] private Color lossColor;

        [Header("Text")] 
        [SerializeField] private Text predictionText;

        private bool _detectionResult;

        public static void UpdateInfo(byte value)
        {
            if(Instance == null)
                return;
            
            Instance.UpdateText(value);
        }

        private void UpdateText(byte value)
        {
            predictionText.text = $"Точность: {value: #00}";
            
            var isDetected = value >= Params.WIDEFIELD_DETECTION_PROBABILITY;
            if (_detectionResult != isDetected)
            {
                predictionText.color = isDetected
                    ? detectedColor
                    : lossColor;
                _detectionResult = isDetected;
            }
        }
    }
}