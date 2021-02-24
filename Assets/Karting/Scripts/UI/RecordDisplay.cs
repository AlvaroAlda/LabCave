using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KartGame.Track;
using TMPro;

public class RecordDisplay : MonoBehaviour
{
    //Record actual
    private float currentRecord;

    //Texto del cartel de records
    private TextMeshProUGUI shownText;

    //El record transformado a string
    private string recordString;

    [SerializeField]
    private string noRecordText;

    [SerializeField]
    private string recordText;

    void Start()
    {
        shownText = GetComponent<TextMeshProUGUI>();

        //Carga el record actual
        currentRecord = PlayerPrefs.GetFloat("BestTime", -1);

        EvaluateRecord();
    }

    /// <summary>
    /// Evalua el record existente
    /// </summary>
    private void EvaluateRecord()
    {
        if(currentRecord > 0)
        {
            // Show texto con el record
            recordString = TimeDisplay.getTimeString(currentRecord);
            shownText.text = recordText + recordString;
        }

        else
        {
            // No existe ningun record aun
            shownText.text = noRecordText;
        }
    }
   
}
