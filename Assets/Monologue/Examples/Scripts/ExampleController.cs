using UnityEngine;
using UnityEngine.UI;

namespace Monologue.Examples
{
    public class ExampleController : MonoBehaviour
	{
		public Text ExampleText;
        public GameObject ContinueIcon;
        public Monologue ExampleMonologue;

        public void Update ()
		{
			if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
			{
				if (ExampleMonologue.TextOutputFinished)
				{
					ExampleMonologue.AnimateText(exampleString);
                    ContinueIcon.SetActive(false);
                }
				else
				{
					ExampleMonologue.AdvanceText();
				}
			}
		}

		public void OnTextOutputFinished()
		{
            ContinueIcon.SetActive(true);
        }

        private const string exampleString = @"This is an example <color=#FE6200>text</color> to test the monologue system.";
    }
}
