using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomTimer;

namespace Level {
public delegate void WaveBegin();
public delegate void WaveBeat(int freq); // Happens in crucial Waves
public delegate void WaveEnd();
/// Given a WaveList that contains timefreq, beat at the crucial time and also
/// telling the freq
///
/// Terms:
/// time -> 1% elapse in the duration
/// freq -> the number of
/// spawns should happen
public class WaveManager : MonoBehaviour {
  public event WaveBegin onWaveBegin;
  public event WaveEnd onWaveEnded;
  public event WaveBeat onWaveBeat;

  [Tooltip("Set to false if this gameobject is used as a component")]
  [SerializeField]
  private bool isStandAlone = false;

  [field:SerializeField]
  public List<WaveDetail> WaveList { get; private set; }

  [SerializeField]
  private Image uiTimelineTemplate;
  [SerializeField]
  private GameObject uiTimelineHolder;
  [SerializeField]
  private TimerVisual timerVisual;
  public Timer timer;
  private float totalTime;

  private IEnumerator Start() {
    if (isStandAlone) {
      yield return _Begin();
    }
    yield return null;
  }
  public void Begin() { StartCoroutine(_Begin()); }
  private IEnumerator _Begin() {
    UpdateWaveUI();
    onWaveBegin?.Invoke();
    timerVisual.timer = timer;
    timerVisual.enabled = true;
    timerVisual.StartTimer(totalTime);
    yield return ProcessBeginWaveBeats();
    timerVisual.enabled = false;
    onWaveEnded?.Invoke();
  }
  public void UpdateWaveUI() {
    float baseHeight = uiTimelineTemplate.rectTransform.sizeDelta.y;
    float baseWidth = uiTimelineTemplate.rectTransform.sizeDelta.x;
    float totalTime = 0.0f;
    foreach (WaveDetail waveDetail in WaveList) {
      totalTime += waveDetail.DurationInSeconds + waveDetail.WaitTime;
    }
    this.totalTime = totalTime;
    Debug.Log($"Total time: {totalTime}");
    foreach (WaveDetail waveDetail in WaveList) {
      GameObject instance = Instantiate(uiTimelineTemplate.gameObject);
      Image instanceImage = instance.GetComponent<Image>();
      float ratio =
          (waveDetail.DurationInSeconds + waveDetail.WaitTime) / totalTime;

      instanceImage.rectTransform.sizeDelta =
          new Vector2(baseWidth * ratio, baseHeight);
      Debug.Log($"Ratio: {ratio}");
      instanceImage.color = waveDetail.UIColor;
      instance.transform.SetParent(uiTimelineHolder.transform);
    }
  }
  private IEnumerator ProcessBeginWaveBeats() {
    TimeDiffFreq timeDiffFreq;
    for (int i = 0; i < WaveList.Count; i++) {
      WaveDetail currentWaveDetail = WaveList[i];
      timeDiffFreq = GetDiscreteSampleBaseOnAnimCurve(
          currentWaveDetail.NumberOfGuestSpawns,
          currentWaveDetail.SpawnDistribution);

      float FRAC = 1 / 100.0f;
      for (int time = 0; time < timeDiffFreq.freqList.Count; time++) {
        Debug.Log(timeDiffFreq.timeDiffList[time]);
        yield return new WaitForSeconds(timeDiffFreq.timeDiffList[time] * FRAC *
                                        currentWaveDetail.DurationInSeconds);
        onWaveBeat?.Invoke(timeDiffFreq.freqList[time]);
      }
      yield return new WaitForSeconds(currentWaveDetail.WaitTime);
    }
    yield return null;
  }
  // Creates a list containing the number of spawn after a certain time frame
  public TimeDiffFreq GetDiscreteSampleBaseOnAnimCurve(int numOfSamples,
                                                       AnimationCurve curve) {
    List<int> freqList = new List<int>();
    List<int> timeDiffList = new List<int>();
    TimeDiffFreq res = new TimeDiffFreq();
    res.freqList = freqList;
    res.timeDiffList = timeDiffList;

    int spawnCount = 0;
    int prevCount = 0;

    int timeFrame = 0;
    int prevFrame = 0;
    for (int i = 0; i <= 100; i++) {
      prevCount = spawnCount;
      spawnCount = (int)(curve.Evaluate(0.01f * i) * numOfSamples);
      int difference = spawnCount - prevCount;
      timeFrame++;
      if (difference == 0)
        continue;
      freqList.Add(difference);

      timeDiffList.Add(timeFrame - prevFrame);
      prevFrame = timeFrame;
    }
    return res;
  }
  public int GetTotalFreq() {
    int total = 0;
    WaveList.ForEach((waveList) => { total += waveList.NumberOfGuestSpawns; });
    return total;
  }
}
}
