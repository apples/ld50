using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI
{
	[ExecuteInEditMode()]
	public class ProgressBar : MonoBehaviour
	{
		#if UNITY_EDITOR
		[MenuItem("GameObject/UI/Radial Progress Bar")]
		public static void AddRadialProgressBar()
		{
			GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/RadialProgressBar"));
			obj.transform.SetParent(Selection.activeGameObject.transform, false);
		}
		#endif
		
		public int minimum;
		public int maximum;
		public int current;
		public Image mask;
		public Image fill;
		public Color color;
		public TextMeshProUGUI tmpText;
		public bool displayText;

		void Start()
		{
			tmpText.gameObject.SetActive(displayText);
		}
		void Update()
		{
			GetCurrentFill();
		}
    
		void GetCurrentFill(){
			float currentOffset = current - minimum;
			float maximumOffset = maximum - minimum;
			float fillAmount = currentOffset / maximumOffset;
			fill.fillAmount = fillAmount;

			if (displayText)
			{
				tmpText.text = current.ToString();
			}

			fill.color = color;
			tmpText.color = color;
		}
	}
}
