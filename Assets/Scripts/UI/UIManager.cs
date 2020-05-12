using FishTank.SaveState;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Camera screenshotCamera;
    public FoodFeeder foodFeeder;
    public Budget budget;

    public MedicineApplicator medApplicator;
    public TankDirtiness tankDirtiness;
    public SavedObjectTypeManager savedTypeManager;

    public GameObject fishMetricsPrefab;
    public GameObject metricsParent;
    public GameObject fishParent;
    public GameObject poopParent;
    public GameObject foodParent;

    public float medicinePrice = 10f;

    public float foodPrice = 5f;

    public Button foodButton;
    public Button medicineButton;
    public Button cleanButton;
    public Button buyButton;

    public BuyDialog buyDialog;

    public AudioMixerGroup uiMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup musicMixer;

    public AudioSource buttonClickAudio;
    public AudioSource airPumpAudio;

    private float cleanerTimeout = 0f;
    private float cleanerCountdown = 90f;
    internal int foodInventory = 200;
    internal int medicineInventory = 10;

    private AudioSource[] audioSources;

    public GameObject pauseMenu;
    public GameObject quitConfirmDlg;
    public GameObject newGameConfirmDlg;

    public float saveInterval = 30f;
    private float saveCountdown = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Start the allowance countdown
        budget.allowanceCountdown = budget.allowanceTime;

        if (foodFeeder == null)
            foodFeeder = FindObjectOfType<FoodFeeder>();

        if (tankDirtiness == null)
            tankDirtiness = FindObjectOfType<TankDirtiness>();

        if (medApplicator == null)
            medApplicator = FindObjectOfType<MedicineApplicator>();

        if (savedTypeManager == null)
            savedTypeManager = FindObjectOfType<SavedObjectTypeManager>();

        SetMetrics();

        audioSources = FindObjectsOfType<AudioSource>();

        if (buttonClickAudio != null)
        {
            foreach(var button in Resources.FindObjectsOfTypeAll<Button>())
            {
                //If the button doesn't already have a clipplayer attached, set it to the default button click sound.
                if (button.GetComponent<AudioClipPlayer>() == null)
                    button.onClick.AddListener(PlayButtonClick);
            }
        }

        LoadGame(Preferences.LastTank);
        saveCountdown = saveInterval;

        //This makes sure that if we restart while paused, the timescale resets to 1.0f
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //If the game is paused, we want to skip the update loop
        if (Time.timeScale <= 0f)
            return;

        if (cleanerTimeout > 0f)
            cleanerTimeout -= Time.deltaTime;

        cleanButton.interactable = CanClean();

        cleanButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Clean Tank ({Mathf.RoundToInt(cleanerTimeout)} s)";
        foodButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Add Food ({foodInventory})";
        medicineButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Add Medicine ({medicineInventory})";

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ShowPauseMenu(!pauseMenu.activeSelf);
        }

        CheckAllowance();

        saveCountdown -= Time.deltaTime;
        if (saveCountdown <= 0f)
        {
            Debug.Log($"Saving Tank:{Preferences.LastTank}");
            saveCountdown = saveInterval;
            SaveGame(Preferences.LastTank);
            Debug.Log($"Saved Tank:{Preferences.LastTank}");
        }
    }
    private void PlayButtonClick()
    {
        buttonClickAudio.Play();
    }

    private bool CanClean()
    {
        return cleanerTimeout <= 0f;
    }
    private bool CanFeed()
    {
        return foodInventory > 0;
    }
    private bool CanMedicate()
    {
        return medicineInventory > 0;
    }

    public void CleanTank()
    {
        cleanerTimeout = cleanerCountdown;
        TankDirtiness.Clean();
    }

    public void FeedFish()
    {
        var count = foodInventory < 10 ? foodInventory : 10;
        foodInventory -= count;
        foodFeeder.AddFood(count);
    }

    public void MedicateFish()
    {
        if (CanMedicate())
        {
            var count = medicineInventory < 1 ? medicineInventory : 1;
            medicineInventory -= count;
            for(int x = 0; x < count; x++)
                medApplicator.AddMedicine();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        //Time.timeScale = 1.0f;
        var scene = SceneManager.GetActiveScene();
        //ShowPauseMenu(false);
        SceneManager.UnloadSceneAsync(scene.buildIndex);
        SceneManager.LoadSceneAsync(scene.buildIndex, LoadSceneMode.Single);
    }

    //private void NewGameSceneUnload_Completed(AsyncOperation obj)
    //{
    //    SceneManager.LoadSceneAsync(activeSceneIndex).completed += NewGameSceneReloaded_Completed;
    //}

    public bool AirPumpSoundEnabled
    {
        get
        {
            return airPumpAudio != null ? airPumpAudio.isPlaying : false;
        }
        set
        {
            if (airPumpAudio != null)
            {
                airPumpAudio.mute = !value;
                //if (!value)
                //{
                //    airPumpAudio.loop = false;
                //    airPumpAudio.Stop();
                //}
                //else
                //{
                //    airPumpAudio.loop = true;
                //    if (!airPumpAudio.isPlaying)
                //        airPumpAudio.Play();
                //}
            }
        }
    }

    public void ShowQuitConfirm()
    {
        quitConfirmDlg.SetActive(true);
        newGameConfirmDlg.SetActive(false);
    }
    public void ShowNewGameConfirm()
    {
        quitConfirmDlg.SetActive(false);
        newGameConfirmDlg.SetActive(true);
    }
    public void ShowBuyDialog()
    {
        buyDialog.gameObject.SetActive(true);
        buyDialog.RefreshView();
    }

    public void ShowPauseMenu(bool visible)
    {
        //Starts or stops the timescale.
        Time.timeScale = visible ? 0f : 1f;

        pauseMenu.SetActive(visible);
        quitConfirmDlg.SetActive(false);
        newGameConfirmDlg.SetActive(false);

        foreach (var src in audioSources)
        {
            if (buttonClickAudio == null || src != buttonClickAudio)
                src.enabled = !visible;
        }
    }

    private void CheckAllowance()
    {
        budget.allowanceCountdown -= Time.deltaTime;
        if (budget.allowanceCountdown <= 0f)
        {
            budget.budget += budget.allowance;
            budget.allowanceCountdown = budget.allowanceTime;

            if (buyDialog != null)
                buyDialog.RefreshView();
        }
    }

    #region "Save and Load"

    private string TankNumberToSaveName(int tankNumber)
    {
        return $"Tank-{tankNumber}";
    }

    public void SaveGame(int tankNumber)
    {
        SaveGame(TankNumberToSaveName(tankNumber));
        Preferences.LastTank = tankNumber;
    }
    public void SaveGame(string saveName)
    {
        //Create the save state that will be saved
        SaveState saveState = new SaveState()
        {
            SaveName = saveName,
            Budget = budget.budget,
            FoodCount = foodInventory,
            MedicineCount = medicineInventory,
            TankDirtiness = tankDirtiness.totalDirtiness,
            Fish = GetFish(),
            FishPoop = GetPoop(),
            Food = GetFood()
        };

        //If the screenshotCamera is available and set up correctly, convert the RenderTexture to a PNG and add it to the SaveState
        if (screenshotCamera != null && screenshotCamera.targetTexture != null)
        {
            var renderTex = screenshotCamera.targetTexture;
            Texture2D rgbTex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGBA32, false);
            rgbTex.Apply(false);
            Graphics.CopyTexture(renderTex, rgbTex);
            saveState.ScreenshotPng = System.Convert.ToBase64String(rgbTex.EncodeToPNG());
        }

        //Set the stream to the appropriate PlayerPrefs kvp
        FileHelper.SaveFile<SaveState>(saveState, Preferences.SavePath(saveName));
        //Preferences.SetSave(saveName, saveStream.ToArray());
    }

    public void LoadGame(int tankNumber)
    {
        LoadGame(TankNumberToSaveName(tankNumber));
        Preferences.LastTank = tankNumber;
    }
    public void LoadGame(string saveName)
    {
        //If there's no save, just return.
        if (!File.Exists(Preferences.SavePath(saveName)))
            return;

        //Load the MemoryStream with the data from the appropriate PlayerPref.
        SaveState state = FileHelper.LoadFile<SaveState>(Preferences.SavePath(saveName));

        //Set the food, medicine, budget and dirtiness
        budget.budget = state.Budget;
        tankDirtiness.totalDirtiness = state.TankDirtiness;
        foodInventory = state.FoodCount;
        medicineInventory = state.MedicineCount;

        //Apply the state to the current tank
        SetFish(state.Fish);
        SetFood(state.Food);
        SetPoop(state.FishPoop);


        SetMetrics();
    }

    private List<FoodSave> GetFood()
    {
        var food = foodParent.GetComponentsInChildren<Food>().ToList();
        food.ForEach(f => f.OnBeforeSave());
        return food.Select(f => SaveState.ConvertCommon<FoodSave, Food>(f)).ToList();
    }

    private void SetFood(IEnumerable<IFood> food)
    {
        ClearObjects<Food>();
        AddObjects<IFood, Food>(ref foodParent, food, savedTypeManager.FoodPrefabLookup);
    }

    private List<PoopSave> GetPoop()
    {
        var poop = poopParent.GetComponentsInChildren<FishPoop>().ToList();
        poop.ForEach(f => f.OnBeforeSave());
        return poop.Select(p => SaveState.ConvertCommon<PoopSave, FishPoop>(p)).ToList();
    }

    private void SetPoop(IEnumerable<IPoop> poop)
    {
        ClearObjects<FishPoop>();
        //AddObjects<FishPoop>(ref poopParent, poop.Select(p => SaveState.ConvertCommon<FishPoop, IPoop>(p)), savedTypeManager.PoopPrefabLookup);
        AddObjects<IPoop, FishPoop>(ref poopParent, poop, savedTypeManager.PoopPrefabLookup);
    }

    private List<FishSave> GetFish()
    {
        var fish = fishParent.GetComponentsInChildren<FishController>().ToList();
        fish.ForEach(f => f.OnBeforeSave());
        return fish.Select(f => SaveState.ConvertCommon<FishSave, FishController>(f)).ToList();
    }
    private void SetFish(IEnumerable<IFish> fish)
    {
        ClearObjects<FishController>();
        //AddObjects<FishController>(ref fishParent, fish.Select(f => SaveState.ConvertCommon<FishController, IFish>(f)), savedTypeManager.FishSpeciesLookup);
        AddObjects<IFish, FishController>(ref fishParent, fish, savedTypeManager.FishSpeciesLookup);
    }

    private void ClearObjects<T>() where T : MonoBehaviour
    {
        foreach (var obj in FindObjectsOfType<T>())
        {
            obj.transform.parent = null;
            Destroy(obj.gameObject);
        }
    }
    private void AddObjects<S, D>(ref GameObject parent, IEnumerable<S> items, Dictionary<string, GameObject> typeLookup) where S : IKeyedObject where D : MonoBehaviour, IKeyedObject
    {
        foreach (var item in items)
        {
            GameObject prefab = typeLookup.ContainsKey(item.ObjectKey) ? typeLookup[item.ObjectKey] : typeLookup.First().Value;

            GameObject newObject = Instantiate<GameObject>(prefab);
            var existingComponent = newObject.GetComponent<D>();
            SaveState.ConvertCommon<D, S>(item, ref existingComponent);
            newObject.transform.SetParent(parent.transform);

            //Destroy(prefab);
            //try
            //{
            //    Destroy(item, 1f);
            //}
            //catch(Exception ex)
            //{
            //    Debug.LogError(ex);
            //}
        }
    }

    private void SetMetrics()
    {
        for (int x = metricsParent.transform.childCount - 1; x >= 0; x--)
        {
            var child = metricsParent.transform.GetChild(x);
            child.SetParent(null);
            Destroy(child.gameObject);
        }


        foreach (var fish in fishParent.GetComponentsInChildren<FishController>())
        {
            GameObject newMetrics = Instantiate(fishMetricsPrefab);
            var fishMetrics = newMetrics.GetComponentInChildren<FishMetrics>();
            fishMetrics.fish = fish;
            fishMetrics.transform.SetParent(metricsParent.transform);
        }
    }
    #endregion "Save and Load"
}
