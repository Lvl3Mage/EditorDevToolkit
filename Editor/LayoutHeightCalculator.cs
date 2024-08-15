using UnityEditor;

namespace Lvl3Mage.EditorDevToolkit.Editor
{
	public class LayoutHeightCalculator
	{
		int segments;
		float totalHeight;
		public void AddSegment(float height){
			totalHeight += height;
			segments++;
		}
		public float GetTotalHeight(){
			return totalHeight + (segments-1) * EditorGUIUtility.standardVerticalSpacing;
		}
	}
}