using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioScript : MonoBehaviour
{
    Slider _audioSlider;
    void Start()
    {
        _audioSlider = GetComponent<Slider>();
        AudioListener.volume =_audioSlider.value;
    }

    public void ChangeVolume()
    {
        AudioListener.volume = _audioSlider.value;
    }
}
