using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class HUD : SingletonBehaviour<HUD>
{
    [SerializeField] private WeaponData _weaponData;

    [SerializeField] private TextMeshProUGUI _bestTimeText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _killsText;
    [SerializeField] private GameObject _resetText;
    [SerializeField] private Image _killsIcon;

    protected override void Initialize()
    {

    }

    protected override void Shutdown()
    {

    }

    public void SetBestTime(float time)
    {
        _bestTimeText.text = $"{time:0.000}";
    }

    public void SetTimeAndKills(float time, int kills)
    {
        _timeText.text = $"{time:0.000}";
        _killsText.text = $"{kills}";
        ApplyKillIconColor();
    }

    private void ApplyKillIconColor()
    {
        _killsIcon.color = _weaponData.Stage.KillColorMotif;
    }

    public void ToggleReset(bool enable)
    {
        _resetText.gameObject.SetActive(enable);
    }
}