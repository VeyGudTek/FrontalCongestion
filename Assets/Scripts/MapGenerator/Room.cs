using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [SerializeField]
    Transform Display;

    [SerializeField]
    private float Level;
    [SerializeField]
    private float Height;

    [SerializeField]
    private float LeftBound;
    [SerializeField]
    private float RightBound;
    [SerializeField]
    private float ForwardBound;
    [SerializeField]
    private float BackBound;

    [SerializeField]
    private List<(float start, float end)> AvailableLeft;
    [SerializeField]
    private List<(float start, float end)> AvailableRight;
    [SerializeField]
    private List<(float start, float end)> AvailableForward;
    [SerializeField]
    private List<(float start, float end)> AvailableBack;

    private float CenterX => (LeftBound + RightBound) / 2f;
    private float CenterZ => (ForwardBound + BackBound) / 2f;
    private float CenterY => (Level + Height / 2f);

    private float ScaleX => RightBound - LeftBound;
    private float ScaleZ => ForwardBound - BackBound;

    public void SetBounds(float left, float right, float forward, float back, float level, float height)
    {
        Level = level;
        Height = height;

        LeftBound = left;
        RightBound = right;
        ForwardBound = forward;
        BackBound = back;

        AvailableLeft =    new List<(float start, float end)> { (back, forward) };
        AvailableRight =   new List<(float start, float end)> { (back, forward) };
        AvailableForward = new List<(float start, float end)> { (left, right) };
        AvailableBack =    new List<(float start, float end)> { (left, right) };

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Display.transform.position = new Vector3(CenterX, CenterY, CenterZ);
        Display.localScale = new Vector3(ScaleX, Height, ScaleZ);
    }
}
