using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HCButton : Button
{
    private bool isProcessing = false;

    public AudioManager.E_AUDIO SFX { get; set; } = AudioManager.E_AUDIO.Click1;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable || isProcessing) return;

        base.OnPointerClick(eventData); // 기본 클릭 동작 호출
        StartCoroutine(HandleClickOnce());

        AudioManager.Instance.PlaySFX(SFX);
    }

    private IEnumerator HandleClickOnce()
    {
        isProcessing = true;

        yield return DoSomethingAsync();

        isProcessing = false;
    }

    private IEnumerator DoSomethingAsync()
    {
        yield return new WaitForSeconds(0.7f);
    }
}
