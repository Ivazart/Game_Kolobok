using System;
using _Project.UI;
using Global;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject restartGamePopup;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button selectStageButton;
    [SerializeField] private Button restartLevelButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button restartCheckpointButton;
    [SerializeField] private Button restartGameButton;
    [SerializeField] private Button restartGameCancelButton;
    [SerializeField] private Button restartGameOKButton;
    [SerializeField] private LoadStage loadStage;
    [SerializeField] private SoundMenu soundMenu;
    private SceneController sceneController => SceneController.Instance;
    private SaveController saveController => SaveController.Instance;
    private GameController gameController => GameController.Instance;
    private void Awake()
    {
        closeButton.onClick.AddListener(CloseMenu);
        restartLevelButton.onClick.AddListener(RestartLevel);
        selectStageButton.onClick.AddListener(OpenSelectStage);
        exitButton.onClick.AddListener(ExitGame);
        restartCheckpointButton.onClick.AddListener(RestartCheckpoint);
        restartGameButton.onClick.AddListener(OpenPopupRestartGame);
        restartGameCancelButton.onClick.AddListener(ClosePopupRestartGame);
        restartGameOKButton.onClick.AddListener(RestartGame);
        menuButton.onClick.AddListener(OpenMenu);
       // loadStage.OnLoadStageFinished += LoadStageFinished;
        loadStage.OnLoadSelectLevel += LoadSelectLevel;
    }

    private void LoadStageFinished()
    {
        loadStage.gameObject.SetActive(false);
        CloseMenu();
    }
    
    private void OpenMenu()
    {
        loadStage.gameObject.SetActive(false);
        menuPanel.SetActive(true);
        soundMenu.ShowSoundButton(true);
        Time.timeScale = 0f;
    }
    
    private void CloseMenu()
    {
        menuPanel.SetActive(false);
        soundMenu.ShowSoundButton(false);
        Time.timeScale = 1f;
    }
    
    private void OpenSelectStage()
    {
        menuPanel.SetActive(false);
        loadStage.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
    
    private void RestartLevel()
    {
        gameController.RestartLevel();
    }

    private void RestartCheckpoint()
    {
        gameController.RestartCheckpoint();
    }
    
    private void OpenPopupRestartGame()
    {
        restartGamePopup.SetActive(true);
    }
    
    private void ClosePopupRestartGame()
    {
        restartGamePopup.SetActive(false);
    }
    
    //Удалить все сохранения, начать игру сначала
    private void RestartGame()
    {
        gameController.RestartGame();
    }

    private void ExitGame()
    {
         Application.Quit();
    }

    private void LoadSelectLevel()
    {
        gameController.LoadSelectLevel();
    }
}