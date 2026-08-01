
// using Windows.Media.Capture.Core;
using System.IO;
// using Windows.Devices.Bluetooth.GenericAttributeProfile;

class DataManager
{

    List<string> LENOVO = new List<string>();
    List<string> KAYTUS = new List<string>();
    List<string> DELL = new List<string>();
    List<string> HPE = new List<string>();
    List<string> NVIDIA = new List<string>();
    List<string> XFUSION = new List<string>();
    List<string> SUPERMICRO = new List<string>();

    List<string> exclusionReq = new List<string>();


    public void VendorClassify(string addText, string vendorText)
    {
        switch (vendorText)
        {
            case "LENOVO":
                LENOVO.Add(addText);
                break;
            case "KAYTUS":
                KAYTUS.Add(addText);
                break;
            case "DELL EMC":
                DELL.Add(addText);
                break;
            case "HPE":
                HPE.Add(addText);
                break;
            case "NVIDIA":
                NVIDIA.Add(addText);
                break;
            case "XFUSION":
                XFUSION.Add(addText);
                break;
            case "SUPERMICRO":
                SUPERMICRO.Add(addText);
                break;
        }
    }


    public void ResultPrint()
    {
        ConditionOutput(LENOVO, "LENOVO");
        ConditionOutput(KAYTUS, "KAYTUS");
        ConditionOutput(DELL, "DELL EMC");
        ConditionOutput(HPE, "HPE");
        ConditionOutput(XFUSION, "XFUSION");
        ConditionOutput(NVIDIA, "NVIDIA");
        ConditionOutput(SUPERMICRO, "SUPERMICRO");

    }

    private void ConditionOutput(List<string> temp, string vendorText)
    {

        if (temp.Count > 0)
        {

            System.Console.WriteLine("[" + vendorText + "]");
            System.Console.WriteLine();

            for (int i = 0; i < temp.Count; i++)
            {
                System.Console.WriteLine($"{i + 1}) ");
                System.Console.WriteLine(temp[i]);
                System.Console.WriteLine();
            }

            System.Console.WriteLine();
        }
    }

    public void ListCleaner()
    {
        LENOVO.Clear();
        KAYTUS.Clear();
        DELL.Clear();
        HPE.Clear();
        XFUSION.Clear();
        NVIDIA.Clear();
        SUPERMICRO.Clear();
    }

}