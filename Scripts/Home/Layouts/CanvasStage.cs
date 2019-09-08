using UnityEngine;
using System.Collections;

public class CanvasStage : MonoBehaviour {

	private LoadimationAnimation _loadingAnimation;

	void Start () {
	
		#if !UNITY_EDITOR && UNITY_WEBGL
			WebGLInput.captureAllKeyboardInput = true;
		#endif

		// Enabling pixelPerfect performs poorly on mobile devices atm
		// We enable when running on desktop

		/*if (!Application.isEditor) {

			// We trun off PixelPerfect for mobile devices.  This checks andsets accordingly
			#if UNITY_WEBGL_API
			GetComponent<Canvas>().pixelPerfect = (GameControl.IsPhoneDetected() || GameControl.GetBrowserVersion() == "Trident 7") ? false : true;
			#endif
		} 
		else GetComponent<Canvas>().pixelPerfect = true;*/

        GetComponent<Canvas>().pixelPerfect = false;

        // Load up the LoadingAnimation spinner
        _loadingAnimation = GetComponentInChildren<LoadimationAnimation>();

		// ShowLoader ();
	}

	public void ShowLoader()
	{
		_loadingAnimation.StartAnimation();

		LeanTween.scale(_loadingAnimation.gameObject, new Vector3 (66, 66, 66), 0.75f)
			.setEase(LeanTweenType.easeInOutCubic)
			.setDelay(0.1f)
			.setOnComplete(() => {



			});
	}

	public void HideLoader()
	{
		LeanTween.scale(_loadingAnimation.gameObject, new Vector3 (0, 0, 0), 0.75f)
			.setEase(LeanTweenType.easeInOutCubic)
			.setDelay(0.1f)
			.setOnComplete(() => {

				_loadingAnimation.StopAnimation();

			});
	}


}
