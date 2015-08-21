using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITest : MonoBehaviour {

	public Text sliderText;

	public void ButtonClickCallback()
	{
		
	}

	public void SliderValueChangedCallback(Slider slider)
	{
		sliderText.text = slider.value.ToString();
	}
}
