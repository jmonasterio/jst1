using UnityEngine;
using System.Collections;

public class Blinker : MonoBehaviour
{

    public float Duration;
    public float Interval;
    public Color TextOffColor;
    public float AlphaTransparency; // 0.0f (blink off completely), 0.1 (dim), 0.2 (brigher), etc.

    public void Blink()
    {
        if (GetComponent<TextMesh>())
        {
            BlinkText( Duration, Interval, TextOffColor);
        }
        else if (GetComponent<SpriteRenderer>())
        {
            BlinkSpriteAlpha( Duration, Interval, AlphaTransparency);
        }
    }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void BlinkText(float duration, float interval, Color offColor)
    {
        if (GetComponent<TextMesh>())
        {
            StartCoroutine(BlinkTextCoroutine(duration, interval, offColor));
        }
    }

    //function to blink the text 
    private IEnumerator BlinkTextCoroutine(float duration, float interval, Color offColor)
    {
        var textMesh = GetComponent<TextMesh>();
        var originalColor = textMesh.color;
        var endTime = Time.time + duration;

        // Blink until duration is over.
        while (Time.time < endTime)
        {
            textMesh.color = offColor;
            yield return new WaitForSeconds(interval);
            textMesh.color = originalColor;
            yield return new WaitForSeconds(interval);

        }
    }

    public void BlinkSpriteAlpha(float duration, float interval, float alphaTransparency)
    {
        if (GetComponent<SpriteRenderer>() != null)
        {
            StartCoroutine(BlinkSpriteAlphaCoroutine(duration, interval, alphaTransparency));
        }
    }


    public void BlinkSprite(float duration, float interval)
    {
        if (GetComponent<SpriteRenderer>() != null)
        {
            StartCoroutine(BlinkSpriteCoroutine(duration, interval));
        }
    }


    //function to blink the text 
    private IEnumerator BlinkSpriteCoroutine(float duration, float interval)
    {
        var sprite = GetComponent<SpriteRenderer>();
        var childSprites = GetComponentsInChildren<SpriteRenderer>();
        var originalEnabled = sprite.enabled;
        var endTime = Time.time + duration;

        // Blink until duration is over.
        while (Time.time < endTime)
        {
            sprite.enabled = !originalEnabled;
            foreach (var child in childSprites)
            {
                child.enabled = !originalEnabled;
            }
            yield return new WaitForSeconds(interval);
            sprite.enabled = originalEnabled;
            foreach (var child in childSprites)
            {
                child.enabled = originalEnabled;
            }
            yield return new WaitForSeconds(interval);

        }
    }

    //function to blink the text 
    private IEnumerator BlinkSpriteAlphaCoroutine(float duration, float interval, float alphaTransparency)
    {
        var sprite = GetComponent<SpriteRenderer>();
        var childSprites = GetComponentsInChildren<SpriteRenderer>();
        var endTime = Time.time + duration;

        var originalColor = sprite.color;
        var transpColor = new Color(originalColor.r, originalColor.g, originalColor.b, alphaTransparency);

        // Blink until duration is over.
        while (Time.time < endTime)
        {
            sprite.color = transpColor;
            foreach (var child in childSprites)
            {
                child.color = transpColor;
            }
            yield return new WaitForSeconds(interval);
            sprite.color = originalColor;
            foreach (var child in childSprites)
            {
                child.color = originalColor;
            }
            yield return new WaitForSeconds(interval);

        }
    }

}
