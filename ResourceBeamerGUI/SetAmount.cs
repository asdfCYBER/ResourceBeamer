using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ResourceBeamer.GUI
{
    public class SetAmount : MonoBehaviour
    {
        //necessary, should be assigned in the inspector
        public InputField AmountInputGO;
        public GameObject IncrementButtonGO;
        public int IncrementOnClick;

        public static int SelectedAmount;

        void Start()
        {
            IncrementButtonGO.GetComponent<Button>().onClick.AddListener(AmountAdjust);
        }

        void AmountAdjust()
        {
            if (AmountInputGO.text == "")
            {
                AmountInputGO.text = "0";
            }
            SelectedAmount = int.Parse(AmountInputGO.text); //add exception handling for null
            SelectedAmount += IncrementOnClick;
            AmountInputGO.text = SelectedAmount.ToString();
        }
    }
}