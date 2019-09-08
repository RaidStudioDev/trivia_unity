using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager 
{
	public bool IsMute = false;
	
	private float _volume = 0.08f;
	
	public float Volume {
		
		get {
			return _volume;
		}
		
		set {
			_volume = value;
		}
		
	}

	// COUNTDOWN 
	// http://leanaudioplay.dentedpixel.com/?d=a:fvb:8,,.001987448,,,8~8,,1,,-,.5,.2122908,-,,8~~~0~~1 

	public void PlayCountdown(float volumeOverride = 0.0f)
	{
		if (GameControl.instance.IsRunningIE) return;

		volumeOverride = VolumeOverrideCheck(volumeOverride);

		AnimationCurve volumeCurve = new AnimationCurve( new Keyframe(0f, 1f, 0f, -1f), new Keyframe(0.5f, 0.2122908f, -1f, 0f));
		AnimationCurve frequencyCurve = new AnimationCurve( new Keyframe(0f, 0.001987448f, 0f, 0f));

		AudioClip audioClip = LeanAudio.createAudio(volumeCurve, frequencyCurve, LeanAudio.options().setWaveSquare());

		LeanAudio.play( audioClip, _volume ); //a:fvb:8,,.001987448,,,8~8,,1,,-,.5,.2122908,-,,8~~~0~~1

	}

	public void PlayCountdownGo(float volumeOverride = 0.0f)
	{
		if (GameControl.instance.IsRunningIE) return;

		volumeOverride = VolumeOverrideCheck(volumeOverride);

		// COUNTDOWN GO!!
		// http://leanaudioplay.dentedpixel.com/?d=a:fvb:8,.02695933,.001464968,,,8~8,,1,,-,.5,.1566879,-,,8~~~0~~1

		AnimationCurve volumeCurve = new AnimationCurve( new Keyframe(0f, 1f, 0f, -1f), new Keyframe(0.5f, 0.1566879f, -1f, 0f));
		AnimationCurve frequencyCurve = new AnimationCurve( new Keyframe(0.02695933f, 0.001464968f, 0f, 0f));

		AudioClip audioClip = LeanAudio.createAudio(volumeCurve, frequencyCurve, LeanAudio.options().setWaveSquare());

		LeanAudio.play( audioClip, volumeOverride ); //a:fvb:8,.02695933,.001464968,,,8~8,,1,,-,.5,.1566879,-,,8~~~0~~1
	}

	// TOO SLOW
	// http://leanaudioplay.dentedpixel.com/?d=a:fvb:8,.03811491,.003057325,,,.5,.006242038,,,8~8,.001859264,.9363058,,-,.5,.1804671,-,,8~~~0~~1

	public void PlayTooSlow(float volumeOverride = 0.0f)
	{
		if (GameControl.instance.IsRunningIE) return;

		volumeOverride = VolumeOverrideCheck(volumeOverride);

		AnimationCurve volumeCurve = new AnimationCurve( new Keyframe(0.001859264f, 0.9363058f, 0f, -1f), new Keyframe(0.5f, 0.1804671f, -1f, 0f));
		AnimationCurve frequencyCurve = new AnimationCurve( new Keyframe(0.03811491f, 0.003057325f, 0f, 0f), new Keyframe(0.5f, 0.006242038f, 0f, 0f));

		AudioClip audioClip = LeanAudio.createAudio(volumeCurve, frequencyCurve, LeanAudio.options().setWaveSquare());

		LeanAudio.play( audioClip, volumeOverride ); //a:fvb:8,.03811491,.003057325,,,.5,.006242038,,,8~8,.001859264,.9363058,,-,.5,.1804671,-,,8~~~0~~1
	}

	// pulse beep semi - low - http://leanaudioplay.dentedpixel.com/?d=a:fvb:8,,.001987448,,,8~8,,1,,-,.5,.7377388,-,,8~~~0~~2
	// pulse beep low - http://leanaudioplay.dentedpixel.com/?d=a:fvb:8,.0009296319,.003821656,,,8~8,,1,,-,.5,.7377388,-,,8~~~0~~2

	// High Pitch with Vibrato 0.5
	// http://leanaudioplay.dentedpixel.com/?d=a:fvb:8,.2984478,.0005949656,,,8~8,,1,,-,.5,.7377388,-,,8~.1,,,~~0~~2

	public void PlayCountdownHighPitchVibrato(float volumeOverride = 0.0f)
	{
		if (GameControl.instance.IsRunningIE) return;

		volumeOverride = VolumeOverrideCheck(volumeOverride);

		AnimationCurve volumeCurve = new AnimationCurve( new Keyframe(0f, 1f, 0f, -1f), new Keyframe(0.5f, 0.7377388f, -1f, 0f));
		AnimationCurve frequencyCurve = new AnimationCurve( new Keyframe(0.2984478f, 0.0005949656f, 0f, 0f));

		AudioClip audioClip = LeanAudio.createAudio(volumeCurve, frequencyCurve, LeanAudio.options().setVibrato( new Vector3[]{ new Vector3(0.1f,0f,0f)} ).setWaveSawtooth());

		LeanAudio.play( audioClip, volumeOverride ); //a:fvb:8,.2984478,.0005949656,,,8~8,,1,,-,.5,.7377388,-,,8~.1,,,~~0~~2
	}

	// Extra High Pitch END with Vibrato 0.75
	// http://leanaudioplay.dentedpixel.com/?d=a:fvb:8,.2984478,.0005949656,,,8~8,,1,,-,.5,.7377388,-,,8~.1,,,~~0~~2

	public void PlayCountdownHighPitchEndVibrato(float volumeOverride = 0.0f)
	{
		if (GameControl.instance.IsRunningIE) return;

		volumeOverride = VolumeOverrideCheck(volumeOverride);

		AnimationCurve volumeCurve = new AnimationCurve( new Keyframe(0f, 1f, 0f, -1f), new Keyframe(0.75f, 0.7377388f, -1f, 0f));
		AnimationCurve frequencyCurve = new AnimationCurve( new Keyframe(0.2959186f, 0.0005263158f, 0f, 0f));

		AudioClip audioClip = LeanAudio.createAudio(volumeCurve, frequencyCurve, LeanAudio.options().setVibrato( new Vector3[]{ new Vector3(0.1f,0f,0f)} ).setWaveSawtooth());

		LeanAudio.play( audioClip ); //a:fvb:8,.2959186,.0005263158,,,8~8,,1,,-,.75,.7377388,-,,8~.1,,,~~0~~2
	}

	// Click Low Tone 0.25
	// http://leanaudioplay.dentedpixel.com/?d=a:fvb:8,.01105769,.001647597,,,8~8,,1,,-,.2376075,.7377388,-,,.25,,,,8~~~0~~

	public void PlayLowToneButton(float volumeOverride = 0.45f)
	{
		if (GameControl.instance.IsRunningIE) return;

		volumeOverride = VolumeOverrideCheck(volumeOverride);

		AnimationCurve volumeCurve = new AnimationCurve( new Keyframe(0f, 1f, 0f, -1f), new Keyframe(0.1795745f, 0.7224256f, -1f, 0f), new Keyframe(0.25f, 0f, 0f, 0f));
		AnimationCurve frequencyCurve = new AnimationCurve( new Keyframe(0.01105769f, 0.001647597f, 0f, 0f));

		AudioClip audioClip = LeanAudio.createAudio(volumeCurve, frequencyCurve, LeanAudio.options());

		LeanAudio.play( audioClip, volumeOverride ); //a:fvb:8,.01105769,.001647597,,,8~8,,1,,-,.1795745,.7224256,-,,.25,,,,8~~~0~~
	}
	
	
	private float VolumeOverrideCheck(float volumeOverride = 0.0f)
	{
		if (volumeOverride == 0.0f) volumeOverride = _volume; 

		return volumeOverride;
	}
}
