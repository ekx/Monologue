using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Text.RegularExpressions;

namespace Monologue
{
    [DisallowMultipleComponent]
	[RequireComponent(typeof(Text), typeof(AudioSource))]
	public class Monologue : BaseMeshEffect
	{
        public float SecondsPerChar = 0.06f;
        public BeepTrigger BeepTrigger = BeepTrigger.Character;

        public float Volume = 1f;
        public float Pitch = 1f;

        public BeepType BeepType = BeepType.Generated;

        public AudioClip Sample;

        public WaveType WaveType = WaveType.Square;
        public float BeepLengthSeconds = 0.01f;     
		public int BaseFrequency = 400;
        public float BaseVolume = 0.15f;    

        /// <summary>
        /// Event thats called when text is finished animating and is shown in its entirety on screen
        /// </summary>
        public TextOutputFinishedEvent OnTextOutputFinished;
		
        /// <summary>
		/// Returns whether text is currently animating
		/// </summary>
		public bool TextOutputFinished
		{
            get { return currentChar >= text.cachedTextGenerator.characterCount - 2; }
        }

		protected override void Awake()
		{
            base.Awake();
            text = GetComponent<Text>();
            audioSource = GetComponent<AudioSource>();
        }
		
		protected void Update()
		{
            secondsSinceLastChar += Time.deltaTime;
            secondsSinceLastBeep += Time.deltaTime;

            if (!TextOutputFinished && secondsSinceLastChar >= SecondsPerChar)
			{
                bool regexMatch;

                do
                {
                    regexMatch = false;
                    currentChar++;

                    foreach (Match match in tagRegex.Matches(text.text))
                    {
                        if (match.Index <= currentChar && match.Index + match.Length > currentChar)
                        {
                            regexMatch = true;
                        }
                    }
                } while (regexMatch);

                graphic.SetVerticesDirty();

                if (TextOutputFinished) OnTextOutputFinished.Invoke();

                secondsSinceLastChar = 0f;
                switch (BeepTrigger)
                {
                    case BeepTrigger.Character:
                        if (!char.IsWhiteSpace(text.text[currentChar])) TriggerBeep();
                        break;
                    case BeepTrigger.Word:
                        if (currentChar > 0 && char.IsWhiteSpace(text.text[currentChar - 1])) TriggerBeep();
                        break;
                }
            }
		}

        private void TriggerBeep()
        {
            audioSource.volume = Volume;
            audioSource.pitch = Pitch;

            if (BeepType == BeepType.AudioSample)
            {
                audioSource.clip = Sample;
                audioSource.Play();
            }
            else
            {
                secondsSinceLastBeep = 0f;
            }
        }

        /// <summary>
		/// Starts animating given text.
		/// </summary>
        /// <param name="input">Text to animate</param>
        public void AnimateText(string input)
		{
            text.text = input;
            currentChar = -1;
        }
		
        /// <summary>
        /// Skips animating text and immediatly shows it on screen
        /// </summary>
		public void AdvanceText()
		{
            if (TextOutputFinished)
            {
                return;
            }

            currentChar = text.cachedTextGenerator.characterCount - 2;
            graphic.SetVerticesDirty();
            OnTextOutputFinished.Invoke();
        }

        public override void ModifyMesh(VertexHelper helper)
        {
            for (int i = 0; i < helper.currentVertCount; i++)
            {
                UIVertex vert = new UIVertex();
                helper.PopulateUIVertex(ref vert, i);

                if (currentChar < 0)
                {
                    vert.color = Color.clear;
                }
                else
                {
                    if (i / 4 > currentChar)
                    {
                        vert.color = Color.clear;
                    }
                }

                helper.SetUIVertex(vert, i);
            }
        }

        #region Audio

        public void OnAudioFilterRead(float[] data, int channels)
        {
            // Update step size in case frequency has changed
            step = BaseFrequency * cycleLength / samplingFrequency;
            
            for (var i = 0; i < data.Length; i = i + channels)
            {
                phase = phase + step;

                // Silence if the beep is already finished or audio sample is used
                if (BeepType == BeepType.AudioSample || secondsSinceLastBeep > BeepLengthSeconds)
                {
                    data[i] = data[i];
                }
                // Else calculate selected wave
                else
                {
                    switch (WaveType)
                    {
                        case WaveType.Sine:
                            data[i] = Sine();
                            break;
                        case WaveType.Triangle:
                            data[i] = Triangle();
                            break;
                        case WaveType.Square:
                            data[i] = Square();
                            break;
                        case WaveType.Sawtooth:
                            data[i] = Sawtooth();
                            break;
                        case WaveType.Noise:
                            data[i] = Noise();
                            break;
                    }
                }

                // If we have stereo, we copy the mono data to each channel
                if (channels == 2) data[i + 1] = data[i];
                // Loop phase
                if (phase > cycleLength) phase = 0;
            }
            
        }

        private float Sine()
        {
            return BaseVolume * Mathf.Sin(phase);
        }
        
        private float Triangle()
        {
            float pos = phase / cycleLength;
        
            if (pos < 0.5f)
            {
                return Mathf.Lerp(-BaseVolume, BaseVolume, pos * 2f);
            }
            else
            {
                return Mathf.Lerp(BaseVolume, -BaseVolume, (pos - 0.5f) * 2f);
            }
        }
        
        private float Square()
        {
            return phase < Mathf.PI ? BaseVolume : 0f;
        }
        
        private float Sawtooth()
        {
            return Mathf.Lerp(-BaseVolume, BaseVolume, phase / cycleLength);
        }
        
        private float Noise()
        {
            return (float) (-BaseVolume + (random.NextDouble() * (BaseVolume - (-BaseVolume))));
        }

        #endregion	

        private Text text;
        private AudioSource audioSource;
        private Regex tagRegex = new Regex("<.*?>");
        private System.Random random = new System.Random();

        private int currentChar = -1;
        private float secondsSinceLastChar = 0f;
        private float secondsSinceLastBeep = 0f;

        private float step;
        private float phase;
        private float samplingFrequency = 44100;

        private const float cycleLength = 2 * Mathf.PI;
    }
	
	[Serializable]
	public class TextOutputFinishedEvent : UnityEvent { }

    public enum BeepTrigger
    {
        Character,
        Word
    }

    public enum BeepType
    {
        AudioSample,
        Generated
    }

    public enum WaveType
    {
        Sine,
        Triangle,
        Square,
        Sawtooth,
        Noise
    }
}