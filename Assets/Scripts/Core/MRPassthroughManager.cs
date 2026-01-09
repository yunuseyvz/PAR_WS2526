using UnityEngine;

namespace LanguageTutor.Core
{
    /// <summary>
    /// Manages Mixed Reality passthrough functionality for Meta Quest devices.
    /// Enables passthrough mode to see the real world through the headset.
    /// </summary>
    public class MRPassthroughManager : MonoBehaviour
    {
        [Header("Passthrough Settings")]
        [SerializeField] private bool enablePassthroughOnStart = true;
        [SerializeField] private float passthroughOpacity = 1f;
        
        private OVRPassthroughLayer passthroughLayer;
        private OVRManager ovrManager;

        private void Start()
        {
            InitializePassthrough();
        }

        private void InitializePassthrough()
        {
            // Get or add OVRManager
            ovrManager = FindObjectOfType<OVRManager>();
            if (ovrManager == null)
            {
                GameObject ovrManagerObj = new GameObject("OVRManager");
                ovrManager = ovrManagerObj.AddComponent<OVRManager>();
            }

            // Enable passthrough in OVRManager
            ovrManager.isInsightPassthroughEnabled = true;

            // Get or add passthrough layer to the camera rig
            OVRCameraRig cameraRig = FindObjectOfType<OVRCameraRig>();
            if (cameraRig != null)
            {
                passthroughLayer = cameraRig.GetComponent<OVRPassthroughLayer>();
                if (passthroughLayer == null)
                {
                    passthroughLayer = cameraRig.gameObject.AddComponent<OVRPassthroughLayer>();
                }

                // Configure passthrough layer
                passthroughLayer.textureOpacity = passthroughOpacity;
                passthroughLayer.overlayType = OVROverlay.OverlayType.Underlay;
                
                if (enablePassthroughOnStart)
                {
                    EnablePassthrough();
                }
            }
            else
            {
                Debug.LogError("MRPassthroughManager: No OVRCameraRig found in scene!");
            }
        }

        public void EnablePassthrough()
        {
            if (passthroughLayer != null)
            {
                passthroughLayer.enabled = true;
                passthroughLayer.hidden = false;
                Debug.Log("Passthrough enabled");
            }
        }

        public void DisablePassthrough()
        {
            if (passthroughLayer != null)
            {
                passthroughLayer.enabled = false;
                passthroughLayer.hidden = true;
                Debug.Log("Passthrough disabled");
            }
        }

        public void SetPassthroughOpacity(float opacity)
        {
            passthroughOpacity = Mathf.Clamp01(opacity);
            if (passthroughLayer != null)
            {
                passthroughLayer.textureOpacity = passthroughOpacity;
            }
        }

        public void TogglePassthrough()
        {
            if (passthroughLayer != null && passthroughLayer.enabled)
            {
                DisablePassthrough();
            }
            else
            {
                EnablePassthrough();
            }
        }
    }
}
