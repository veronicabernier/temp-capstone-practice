using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using Michsky.MUIP;

public class EditRecipe : MonoBehaviour
{
    [Header("Input Fields")]
    public InputField brandInputField;
    public InputField roastInputField;
    public InputField BeantypeInputField;
    public InputField brewMethodInputField;
    public InputField coffeeeWeightInputField;
    public InputField grindSettingInputField;

    [Header("Other")]
    public ButtonManager submitButton;
    public GameObject spinner;
    public NotificationManager popup;
    public NotificationManager popupSuccess;
    public string afterSceneName;

    private RecipeData selectedRecipe = RecipeItem.SelectedRecipe;

    // Start is called before the first frame update
    void Start()
    {
        /*RecipeData selectedRecipe = RecipeItem.SelectedRecipe;*/
        if (selectedRecipe == null)
        {
            Debug.LogError("No recipe selected!");
            return;
        }
        else if (selectedRecipe != null)
        {
            brandInputField.text = selectedRecipe.brand;
            roastInputField.text = selectedRecipe.roast;
            BeantypeInputField.text = selectedRecipe.bean_type;
            brewMethodInputField.text = selectedRecipe.brew_method;
            coffeeeWeightInputField.text = selectedRecipe.coffee_weight.ToString();
            grindSettingInputField.text = selectedRecipe.grind_setting.ToString();
        }


    }
    public void onSubmit()
    {
        string validFieldsError = validFields();
        if (validFieldsError == "")
        {
            WWWForm form = new WWWForm();
            form.AddField("brand", brandInputField.text);
            form.AddField("roast", roastInputField.text);
            form.AddField("bean_type", BeantypeInputField.text);
            form.AddField("brew_method", brewMethodInputField.text);
            form.AddField("coffee_weight", coffeeeWeightInputField.text);
            form.AddField("grind_setting", grindSettingInputField.text);

            StartCoroutine(PostRequest(form));

        }
        else
        {
            popup.description = validFieldsError;
            popup.UpdateUI();
            popup.Open();
        }

        IEnumerator PostRequest(WWWForm form)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Post(PostInformation.address + PostInformation.userid + "/" + selectedRecipe.recipeid + " /edit", form))
            {
                webRequest.method = "PUT";

                submitButton.isInteractable = false;
                spinner.SetActive(true);
                yield return webRequest.SendWebRequest();
                submitButton.isInteractable = true;
                spinner.SetActive(false);

                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    popup.description = webRequest.error;
                    popup.UpdateUI();
                    popup.Open();
                }
                else
                {
                    popupSuccess.description = "";
                    popupSuccess.UpdateUI();
                    popupSuccess.Open();
                    yield return new WaitForSecondsRealtime(popupSuccess.timer);
                    new SceneChanger().changeScene(afterSceneName);
                }
            }
        }



    }


    private string validFields()
    {
        if (brandInputField.text == "" || roastInputField.text == "" || BeantypeInputField.text == "" || brewMethodInputField.text == "" || coffeeeWeightInputField.text == "" || grindSettingInputField.text == "")
        {
            return "Complete all fields.";
        }

        return "";
    }

}

