using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatformCoordinator : MonoBehaviour
{
    [SerializeField] private List<GameObject> platformSets;
    [SerializeField] private int numberOfActiveSets;
    [SerializeField] private float interval;

    private int _activeIdx;
    private float _timer;

    private void Start()
    {
        if (platformSets.Count == 0) return;

        numberOfActiveSets = Clamp(numberOfActiveSets);
        for (int i = 0; i < numberOfActiveSets; i++)
        {
            platformSets[i].SetActive(true);
        }
        for (int i = numberOfActiveSets; i < platformSets.Count; i++)
        {
            platformSets[i].SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (platformSets.Count == 0) return;

        _timer += Time.fixedDeltaTime;
        if (_timer >= interval)
        {
            _timer = 0;
            platformSets[_activeIdx].SetActive(false);
            platformSets[Clamp(_activeIdx + numberOfActiveSets)].SetActive(true);
            _activeIdx = Clamp(++_activeIdx);
        }
    }

    private int Clamp(int idx) => idx % platformSets.Count;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (platformSets.Count == 0)
        {
            Debug.LogWarning("No platform set has been assigned to the disappearing platform coordinator.");
        }
    }
#endif
}
