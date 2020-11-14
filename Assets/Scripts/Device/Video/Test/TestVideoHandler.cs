using Core.OrderStart;
using Device.UI;
using Device.Utils;
using UnityEngine;

namespace Device.Video.Test
{
    public class TestVideoHandler: VideoHandler, IStarter
    {
        [Header("External")] 
        [SerializeField] private CameraTypes setupType;
        public void OnStart()
        {
            objectHandler = gameObject.AddComponent<ObjectHandler>();
            Initialize(setupType);

            CameraUsageRequest.FindWebCams();
            OnCameraAuthorized(null);
        }
    }
}