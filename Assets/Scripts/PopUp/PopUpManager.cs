
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;
using Pelumi.ObjectPool;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance { get; private set; }

    [SerializeField] private TextPopUp popUpPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            string text = NumberConverter.ConvertNumberToString(Random.Range(100, 1000000));
            PopUpAtTextPosition(mousePosition, Vector3.one / 8, text, Random.ColorHSV());
        }
    }

    public void PopUpTextAtTransfrom(Transform spawnPosition, Vector3 randomIntensity, string text, Color color, bool parent = false)
    {
        TextPopUp textPopUp = ObjectPoolManager.SpawnObject(popUpPrefab);
        if (parent) textPopUp.transform.SetParent(spawnPosition);
        SetPopUpInfo(textPopUp, spawnPosition.position, randomIntensity, text, color);
    }

    public void PopUpAtTextPosition(Vector3 spawnPosition, Vector3 randomIntensity, string text, Color color)
    {
        TextPopUp textPopUp = ObjectPoolManager.SpawnObject(popUpPrefab);
        SetPopUpInfo(textPopUp, spawnPosition, randomIntensity, text, color);
    }

    public void SetPopUpInfo(TextPopUp textPopUp, Vector3 spawnPosition, Vector3 randomIntensity, string text, Color color)
    {
        Vector2 finalPos = spawnPosition += new Vector3
            (
            Random.Range(-randomIntensity.x, randomIntensity.x),
                Random.Range(-randomIntensity.y, randomIntensity.y),
                Random.Range(-randomIntensity.z, randomIntensity.z)
            );
        textPopUp.Init(text, color, finalPos);
    }
}