using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyUI.Dialogs
{
    public class Dialog
    {
        public string Title = "Star rider is pissin";
        public string Message = "Star rider Lore.";
    }


    public class DialogUI : MonoBehaviour
    {
        [SerializeField] Text titleUIText;
        [SerializeField] Text messageUIText;
        [SerializeField] Button closeUIButton;
        [SerializeField] GameObject canvas;

        Dialog dialog = new Dialog();
        //Singleton Pattern
        public static DialogUI Instance;

        void Awake()
        {
            Instance = this;

            //Add close event listener
            closeUIButton.onClick.RemoveAllListeners();
            closeUIButton.onClick.AddListener(Hide);
        }

        //Set Dialog Title
        public DialogUI SetTitle(string title)
        {
            dialog.Title = title;
            return Instance;
        }

        //Set Dialog Message
        public DialogUI SetMessage(string message)
        {
            dialog.Message = message;
            return Instance;
        }
        

        //Show the message
        public void Show()
        {
            titleUIText.text = dialog.Title;
            titleUIText.text = dialog.Message;


            canvas.SetActive(true);
        }

        //Hide the message
        public void Hide()
        {
            canvas.SetActive(false);

            //reset dialog
            dialog = new Dialog();
        }
    }
}
