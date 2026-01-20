using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Buttonに登録されているTargetGraphic以外の画像のカラーも一緒に変えたいときに使う
/// 根底クラスのDoStateTransitionに従って同じタイミングで動作する
/// </summary>
[RequireComponent(typeof(TransitionFollowingColorsButtonTargets))]
public class Button_MultiTarget : Button
{
	private TransitionFollowingColorsButtonTargets data;

	protected override void Awake()
	{
		base.Awake();
		data = GetComponent<TransitionFollowingColorsButtonTargets>();
	}

	// 根底クラスにある実装をそのままカラー用に抜き出して、複数画像に同じタイミングで適用出来るようにした
	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		base.DoStateTransition(state, instant);
		Color tintColor;
		switch (state)
		{
			case SelectionState.Normal:
				tintColor = colors.normalColor;
				break;
			case SelectionState.Highlighted:
				tintColor = colors.highlightedColor;
				break;
			case SelectionState.Pressed:
				tintColor = colors.pressedColor;
				break;
			case SelectionState.Selected:
				tintColor = colors.selectedColor;
				break;
			case SelectionState.Disabled:
				tintColor = colors.disabledColor;
				break;
			default:
				tintColor = Color.black;
				break;
		}

		foreach (var target in data.Targets)
		{
			target.CrossFadeColor(tintColor * colors.colorMultiplier, instant ? 0f : colors.fadeDuration, true, true);
		}
	}
}

