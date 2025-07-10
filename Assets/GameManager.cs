using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    
    public DataRetrieveState dataState;
    public You[] yous;
    public GameObject youPrefab;



    [Header("For SN Data Retrieval")]
    [SerializeField] private List<string> studentNumbers;
    public UnityWebRequest studentNumbersRequest;


    [Header("For YOU Data Retrieval")]
    [SerializeField] private UnityWebRequest[] youDataRequests;
    [SerializeField] private bool[] finishedYouRetrievalProcess;

    public enum DataRetrieveState{
        IDLE,
        GETTING_STUDENT_NUMBERS,
        GETTING_YOU_DATA,
        FINISHED,
        ERROR
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(dataState != DataRetrieveState.IDLE){return;}
        dataState = DataRetrieveState.GETTING_STUDENT_NUMBERS;
        
        // Start student number requests
        studentNumbersRequest = UnityWebRequest.Get("");
    }

    void StudentNumberRetrieveUpdate(){

        if(studentNumbersRequest.isDone){
            string studentNumbersString = studentNumbersRequest.result.ToString();
            
            studentNumbers = new();
            string currentStudentNumber = "";
            foreach (char character in studentNumbersString){
                if(character == '|') { // '|' acts as a separator for each student number in the list
                    studentNumbers.Add(currentStudentNumber);
                    currentStudentNumber = "";
                }
                else{
                    currentStudentNumber += character;
                }
            }


            
            InitiateYouDataRetrieval();

        };
        
    }

    void InitiateYouDataRetrieval(){
        dataState = DataRetrieveState.GETTING_YOU_DATA;

        youDataRequests = new UnityWebRequest[studentNumbers.Count]; // Might need to be a jagged array if we need multiple requests per YOU.
        finishedYouRetrievalProcess = new bool[youDataRequests.Length];
        for (int i = 0; i < youDataRequests.Length; i++)
        {
            youDataRequests[i] = UnityWebRequest.Get(""+studentNumbers[i]);
        }

        
    }

    void YouRetrieveUpdate(){
        for (int i = 0; i < youDataRequests.Length; i++)
        {
            
            if(youDataRequests[i].isDone){

                string returnedString = studentNumbersRequest.result.ToString();
                You you = Instantiate(youPrefab, transform).GetComponent<You>();
                
                // Set you variables with returned request here
                
                
                // foreach (char character in studentNumbersString){
                
                // }

                finishedYouRetrievalProcess[i] = true;
            };
                

            // Go to next state here.
        }
    }
    void FinishRetrieval(){
        // Initiate intros and any startupp sequences for the program
        dataState = DataRetrieveState.FINISHED;


    }

    void Update(){
        switch (dataState)
        {
            case DataRetrieveState.GETTING_STUDENT_NUMBERS:
                StudentNumberRetrieveUpdate();
                break;
                
            case DataRetrieveState.GETTING_YOU_DATA:
                StudentNumberRetrieveUpdate();
                break;
        }
    }


}
