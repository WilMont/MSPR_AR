using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotsAndShare : MonoBehaviour
{
    //The UnityEngine.UI.Button object used to open the "share screenshot" menu.
    public Button shareButton;

    //The UnityEngine.Ui.Image object representing the "share" menu.
    public Image shareFormImage;

    //The UnityEngine.UI.InputField object used to retrieve the user's email.
    public InputField clientEmailInputField;

    //The UnityEngine.UI.InputField object used to retrieve the user's name.
    public InputField clientFirstNameInputField;

    //The UnityEngine.UI.Button object used to send the user's inputs (email and name).
    public Button sendClientInfosButton;

    //The UnityEngine.UI.Button object used to cancel the sharing of the screenshot.
    public Button cancelShareButton;

    //Popup appearing if the user entered invalid informations.
    public Image invalidInfosPopup;

    //Image with an alert text if the user entered invalid informations.
    //public Image invalidInfosAlert;

    //Button to close the invalid infos alert.
    public Button closeInvalidInfosPopupButton;

    //Path where the screenshot file has been saved.
    private string screenshotPath = "";

    //Text to display error messages on the Android interface.
    public Text errorMessageText;

    //The snake image.
    public Image snakeImage;

    //The monkey image.
    public Image monkeyImage;

    /// <summary>
    /// Actions to do before first frame, at the start of the application.
    /// </summary>
    private void Start()
    {
        IsShareFormVisible(false);
        IsInvalidInfosAlertVisible(false);
        errorMessageText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Set the share screenshot menu and its childs visible or not.
    /// </summary>
    private void IsShareFormVisible(bool visible)
    {
        shareFormImage.gameObject.SetActive(visible);
        for (int i = 0; i < shareFormImage.transform.childCount; i++)
        {
            shareFormImage.transform.GetChild(i).gameObject.SetActive(visible);
        }
    }

    /// <summary>
    /// Set the invalid infos alert and its childs visible or not.
    /// </summary>
    private void IsInvalidInfosAlertVisible(bool visible)
    {
        invalidInfosPopup.gameObject.SetActive(visible);
        for (int i = 0; i < invalidInfosPopup.transform.childCount; i++)
        {
            invalidInfosPopup.transform.GetChild(i).gameObject.SetActive(visible);
        }
    }

    /// <summary>
    /// Function to start the processus of taking the screenshot, saving it then sharing it.
    /// </summary>
    public void TakeScreenShotAndShare()
    {
        //We hide the "Share" button to prevent the user from screenshotting the input menu.
        shareButton.gameObject.SetActive(false);
        //We begin the screenshot process.
        StartCoroutine(TakeScreenshotAndSaveV2());
    }

    /// <summary>
    /// Take a screenshot and saves it (Using NativeShare plugin).
    /// </summary>
    private IEnumerator TakeScreenshotAndSaveV2()
    {
        yield return new WaitForEndOfFrame();
        try
        {
            Debug.Log("Screenshot capture beginning");

            shareButton.gameObject.SetActive(false);
            snakeImage.gameObject.SetActive(false);
            monkeyImage.gameObject.SetActive(false);

            Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            ss.Apply();

            screenshotPath = Path.Combine(Application.temporaryCachePath, "shared_img.png");
            File.WriteAllBytes(screenshotPath, ss.EncodeToPNG());

            Debug.Log("Screenshot taken.");

            // To avoid memory leaks
            Destroy(ss);

            shareButton.gameObject.SetActive(true);
            snakeImage.gameObject.SetActive(true);
            monkeyImage.gameObject.SetActive(true);

            //We display the user input menu.
            IsShareFormVisible(true);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }

    /// <summary>
    /// Verify inputs provided by the user and continue with the sharing processus if they are valid.
    /// </summary>
    public void VerifiyInputsAndShare()
    {
        if (InputFieldsAreValid(clientEmailInputField.text, clientFirstNameInputField.text))
        {
            //Send user's email and first name through API.
            //TransferUserInfosAPI(clientEmailInputField.text, clientFirstNameInputField.text);

            IsShareFormVisible(false);
            //StartCoroutine(shareScreenshot(screenshotPath));
            StartCoroutine(ShareScreenshotV2());
        }
        else
        {
            IsInvalidInfosAlertVisible(true);
        }
        //Sinon afficher popup informations incorrectes.

    }

    /// <summary>
    /// Check if the values in the email and name input fields are valid.
    /// </summary>
    private bool InputFieldsAreValid(string email, string name)
    {
        //Using Microsoft's EmailAddressAttribute.IsValid to determine if email is valid.
        EmailAddressAttribute emailAddressAttribute = new EmailAddressAttribute();
        if (emailAddressAttribute.IsValid(email) && !string.IsNullOrEmpty(name) && name.Length > 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    ///// <summary>
    ///// Send the email and the first name provided by the user to the CRM with an API.
    ///// </summary>
    ///// <param name="email">Email of the user from the corresponding input</param>
    ///// <param name="firstName">First name of the user from the corresponding input</param>
    //private async void TransferUserInfosAPI(string email, string firstName)
    //{
    //    var client = new RestClient("https://api.hubapi.com/crm/v3/objects/contacts");
    //    var request = new RestRequest("", Method.Post);
    //    request.AddHeader("Content-Type", "application/json");
    //    request.AddQueryParameter("hapikey", "eu1-a7a1-2304-40d3-9658-9d57b6dce06d");

    //    var body = JsonConvert.SerializeObject(new ContactHubspot(email, name));
    //    //request.AddParameter("application/json", body, ParameterType.RequestBody);
    //    request.AddBody(body, "application/json");
    //    RestResponse response = await client.ExecuteAsync(request);
    //    //Console.WriteLine(response.Content);
    //    //Console.WriteLine();
    //}

    /// <summary>
    /// Share the screenshot from the selected path to the selected media.
    /// </summary>
    private IEnumerator ShareScreenshotV2()
    {
        yield return new WaitForEndOfFrame();
        try
        {
            Debug.Log("Sharing processed beginning");

            new NativeShare().AddFile(screenshotPath)
                .SetSubject("Taken from the Cerealis AR app !").SetText("#cerealis #coloring #AR")
                .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
                .Share();

            // Share on WhatsApp only, if installed (Android only)
            //if( NativeShare.TargetExists( "com.whatsapp" ) )
            //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();

            //We close the user inputs menu.
            IsShareFormVisible(false);
            //We set the "Share" button visible again.
            shareButton.gameObject.SetActive(true);
        }
        catch (Exception e)
        {
            PrintErrorOnUserScreen(e.Message);
            Debug.Log(e.Message);
        }

    }

    //Close the "share" form.
    public void CancelScreenshotShare()
    {
        shareButton.gameObject.SetActive(true);
        IsShareFormVisible(false);
        IsInvalidInfosAlertVisible(false);
    }

    //Close the invalid informations alert message.
    public void CloseInvalidInfosAlert()
    {
        IsInvalidInfosAlertVisible(false);
    }

    /// <summary>
    /// Display the catched error message furnished.
    /// </summary>
    /// <param name="errorMessage">The catched error message to display.</param>
    private void PrintErrorOnUserScreen(string errorMessage)
    {
        errorMessageText.gameObject.SetActive(true);
        errorMessageText.text = errorMessage;
    }

}
