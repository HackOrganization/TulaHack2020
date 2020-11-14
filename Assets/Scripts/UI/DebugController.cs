using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DebugController: MonoBehaviour
    {
        [Header("Debug")] 
        [SerializeField] private Text fpsText;
        [SerializeField] private Text neuronFpsText;
        [SerializeField] private Text tickText;
        private float _lastTimePoint;
        private float _counter;

        public void Log()
        {
            var newTimePoint = Time.time;
            
            fpsText.text = $"FPS:\n{1f / Time.deltaTime : ###.##}";
            neuronFpsText.text = $"Neuron FPS:\n{1f / (newTimePoint - _lastTimePoint) :###.##}";
            tickText.text = $"TICK:\n{++_counter}";
            
            _lastTimePoint = newTimePoint;
        }
    }
}