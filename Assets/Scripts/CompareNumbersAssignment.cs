using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareNumbersAssignment : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int firstNumber = Random.Range(1, 100);
        Debug.Log($"First number is: {firstNumber}.");
        int secondNumber = Random.Range(1, 100);
        Debug.Log($"Second number is: {secondNumber}.");

        compareNumbers(firstNumber, secondNumber);
    }

    private void compareNumbers(int firstNumber, int secondNumber){
        if(firstNumber < secondNumber){
            Debug.Log($"{firstNumber} is smaller than {secondNumber}.");
        }
        else if(secondNumber < firstNumber){
            Debug.Log($"{secondNumber} is smaller than {firstNumber}.");
        }
        else{
            Debug.Log("The numbers are equal.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
