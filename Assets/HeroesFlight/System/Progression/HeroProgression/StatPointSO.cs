using HeroesFlight.Common.Progression;
using UnityEngine;

[CreateAssetMenu(fileName = "HPAttributeSO", menuName = "HeroProgression/HPAttributeSO", order = 1)]
public class StatPointSO : ScriptableObject
{
    [SerializeField] private StatAttributeType statPointType;
    [TextArea(3, 10)]
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private StatPointInfo[] keyValues;

    public Sprite Icon => icon;
    public StatAttributeType StatPointType => statPointType;
    public string Description => description;
    public StatPointInfo[] KeyValues => keyValues;
}
