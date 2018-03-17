﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public Text questionText;
    public Text scoreText;
    public Text timeText;
    public Transform answerButtonParent;
    public int playerScore;
    public GameObject QuestionDisplay;
    public GameObject endGameDisplay;
    public BasicObjectPool answerButtonPool;
    private DataController dataController;
    private RoundData roundData;
    private QuestionData[] questionPool;
    private bool isRoundActive;
    private int questionIndex;
    private float timeRemaining;

    private List<GameObject> answerButtonObjects = new List<GameObject>();

	// Use this for initialization
	void Start () {
        dataController = FindObjectOfType<DataController>();
        roundData = dataController.GetCurrentRoundData();
        questionPool = roundData.questions;
        playerScore = 0;
        questionIndex = 0;
        timeRemaining = roundData.timeLimitInSeconds;
        UpdateTime();
        ShowQuestions();
        isRoundActive = true;
	}

    private void ShowQuestions()
    {
        RemoveAnswerButton();
        QuestionData questionData = questionPool[questionIndex];
        questionText.text = questionData.questionText;

        for (int i = 0; i < questionData.answers.Length; i++)
        {
            GameObject answerButtonObject = answerButtonPool.GetObject();
            answerButtonObject.transform.SetParent(answerButtonParent);
            answerButtonObjects.Add(answerButtonObject);

            AnswerButton answerButton = answerButtonObject.GetComponent<AnswerButton>();
            answerButton.Setup(questionData.answers[i]);
        }
    }
	
    private void RemoveAnswerButton()
    {
        while (answerButtonObjects.Count > 0)
        {
            answerButtonPool.ReturnObject(answerButtonObjects[0]);
            answerButtonObjects.RemoveAt(0);
        }
    }

    public void AnswerClicked(bool isCorrect)
    {
        if (isCorrect)
        {
            // adds player score
            playerScore += roundData.pointsAddedForCorrectAnswer;
            scoreText.text = "Score; " + playerScore.ToString();
        }
        if (questionPool.Length > questionIndex + 1)
        {
            questionIndex++;
            ShowQuestions();
        }
        else
        {
            // end game
            EndRound();
        }
    }

    public void EndRound()
    {
        isRoundActive = false;
        QuestionDisplay.SetActive(false);
        endGameDisplay.SetActive(true);
    }

    public void StartOver()
    {
        SceneManager.LoadScene("MenuScreen");
    }

    private void UpdateTime()
    {
        timeText.text = "Time: " + Mathf.Round(timeRemaining).ToString();
    }

	// Update is called once per frame
	void Update () {
        timeRemaining -= Time.deltaTime;
        UpdateTime();

        if (timeRemaining <= 0)
        {
            EndRound();
        }
	}
}
