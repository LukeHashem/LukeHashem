using MorseCodeTranslator;
using System;

public class MorseLang
{

	public MorseLang()
	{
    
    }

	public string TranslateMorse(string sRawText, List<string> MorseCode)
	{
		string sFinishedMorse = "";
		string stemp = "";
		//Loops for all Characters being Translated
		for(int iCount = 0; iCount < sRawText.Length; iCount++)
		{
			//Sets Current Character to a String so it can be Translated
			string stempex = sRawText[iCount].ToString().ToUpper();
			//Loops over all Values until One Matches with Character
			foreach(string letter in MorseCode)
			{
				if(letter.StartsWith(@"#"))
				{
					stemp = "";
				}
                else if (letter == "")
                {
					//Stops stemp Being set to Default on Empty Values
                }
                else if(stempex == " ")
				{
					stemp = "| ";                 
                }
				else if(stempex == letter.Substring(0, 1))
				{
					stemp = letter.Substring(2) + " ";
                }
			}

            sFinishedMorse += stemp;
        }
		return sFinishedMorse;
	}

	public string TranslateEnglish(string sRawMorse, List<string> MorseCode)
	{
        string sFinishedEn = "";
		string stemp = "";
		int iCount = 0;

		while (iCount < sRawMorse.Length)
		{

            if (!(sRawMorse[iCount].ToString() == " "))
            {
                stemp += sRawMorse[iCount].ToString();
            }

            if (sRawMorse[iCount].ToString() == " " || iCount == sRawMorse.Length - 1)
			{
				foreach (string letter in MorseCode)
				{
					if (letter.Contains(@"#") || letter == "")
					{
						//·- | ·
					}
					else if (stemp == letter.Substring(2))
					{
						sFinishedEn += letter.Substring(0, 1);
						stemp = "";
					}
				}
			}
			else if (sRawMorse[iCount].ToString() == "|")
			{
				sFinishedEn += " ";
                stemp = "";
            }

            
			iCount++;
		}

		return sFinishedEn;
    }

    public string RandomMorse(List<string> MorseCode)
    {
        string sRandomMorse = "";
        int iCount = 0;
        bool bHaveLetter = false;
        Random NumRand = new Random();

        //Loops Until Matching Value is found
        while (bHaveLetter != true)
        {

            //Generates Random Number
            int Random = NumRand.Next(6, 43);

            foreach (string letter in MorseCode)
            {
                if (iCount == Random && letter != " ")
                {
                    sRandomMorse = letter;
                    bHaveLetter = true;
                }

                iCount++;
            }
        }
        return sRandomMorse;
    }
}
