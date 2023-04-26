using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WordConnect
{
	public class WordConnectMenu : MonoBehaviour
	{
		[MenuItem("Tools/Bizzy Bee Games/Word Connect Level Builder")]
		public static void OpenLevelBuilderWindow()
		{
			EditorWindow.GetWindow<LevelBuilderWindow>("Level Builder");
		}
	}
}
