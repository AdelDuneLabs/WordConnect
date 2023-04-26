using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace WordConnect
{
	public class WordBoardCell : UIMonoBehaviour
	{
		#region Enums

		public enum State
		{
			Blank,
			Hint,
			Found
		}

		#endregion

		#region Inspector Variables

		public Text			letterText;
		public Image		letterBackground;
		public GameObject	coinObject;
		public Color		normalTextColor	= Color.white;
		public Color		foundTextColor	= Color.white;
		private Vector3 orginalPos;

		#endregion

		#region Properties

		public int		Row				{ get; set; }
		public int		Col				{ get; set; }
		public bool 	HasCoin 		{ get; set; }
		public State 	CurrentState	{ get; set; }

		public System.Action<int, int> OnCellClicked { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invoked when the button for this cell is clicked
        /// </summary>
        /// 
        private void Awake()
        {
			orginalPos = GetComponent<RectTransform>().position; 
        }
        public void OnClick()
		{
			if (OnCellClicked != null)
			{
				OnCellClicked(Row, Col);
			}



		}

		/// <summary>
		/// Updates the state of this cell
		/// </summary>
		public void SetState(State state)
		{
			CurrentState = state;

			// Only set the letter background it the letter is found
			letterBackground.gameObject.SetActive(state == State.Found);

			// Set the color of the letter text
			Color letterColor	= (state == State.Found) ? foundTextColor : normalTextColor;
			letterColor.a		= (state != State.Blank) ? 1f : 0f;

			if(state == State.Found)
			{
				letterBackground.DOFade(0, 0);
                letterText.rectTransform.position = GameController.Instance.GetSelectedPos.position;


				letterText.transform.DOLocalMove(orginalPos, 0.5f).OnComplete(() =>
				{
                    letterBackground.DOFade(1, 0.1f);
                });


            }

			letterText.color = letterColor;

			// Hide the coin if the state is now found of hint
			if (HasCoin && (state == State.Found || state == State.Hint))
			{
				coinObject.SetActive(false);
			}
		}

		private void SetupAndPlay(UIAnimation anim, AnimationCurve animationCurve, float startDelay)
		{
			anim.style				= UIAnimation.Style.Custom;
			anim.animationCurve		= animationCurve;
			anim.startDelay			= startDelay;
			anim.startOnFirstFrame	= true;
			anim.Play();
		}

		#endregion
	}
}