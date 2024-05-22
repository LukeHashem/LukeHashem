using System.Text;
using JollyWrapper;
using System.Collections;
using System.IO.Compression;

namespace MorseCodeTranslator
{
    public class User
    {
        public string sName { get; set; }

        public User(string name)
        {
            sName = name;
        }
    }

    public interface ISaveToFile
    {
        public void FileSave(string sText, string sText2, string sUsername, string iEncrypted, string sKey);
    }

    public class EncryptedText : ISaveToFile
    {
        public void FileSave(string sText, string sText2, string sUsername, string iEncrypted, string sKey)
        {
            AESEncryption aes = new AESEncryption(sKey);

            sText = aes.Encrypt(sText);
            sText2 = aes.Encrypt(sText2);

            QueryParms parms = new QueryParms()
            {
            {"@Text", sText },
            {"@Text2", sText2},
            {"@username", sUsername},
            {"@encrypted", iEncrypted }
            };

            Database.ExecuteNonQuery("INSERT INTO `t_SavedText` (`textID`, `EnglishText`, `MorseText`, `username`, `IsEncrypted`) VALUES (NULL, @Text, @Text2, @username, @encrypted);", parms);
            
            Console.WriteLine("Data has been saved");
            Console.WriteLine("");
        }
    }

    public class PlainText : ISaveToFile
    {
        public void FileSave(string sText, string sText2, string sUsername, string iEncrypted,string sKey)
        {
            QueryParms parms = new QueryParms()
        {
            {"@Text", sText },
            {"@Text2", sText2},
            {"@username", sUsername},
            {"@encrypted", iEncrypted }
        };
            Database.ExecuteNonQuery("INSERT INTO `t_SavedText` (`textID`, `EnglishText`, `MorseText`, `username`, `IsEncrypted`) VALUES (NULL, @Text, @Text2, @username, @encrypted);", parms);

            Console.WriteLine("Data has been saved");
        }
    }

    public class MorseStandard
    {
        public List<string> StandardMenu()
        {
            //User Selects which Standard to use
            int iStanChoice = 0;
            int iCount = 1;

            //Loads the files
            var files = Directory.GetFiles(System.Environment.CurrentDirectory + @"\Standards");
            string[] Standards = files.ToArray();          
            
            //Prevents Crashing if User Enters the Wrong Data Type
            try
            {
                StandardMenuText(Standards); //Calls Menu Text

                iStanChoice = Convert.ToInt16(Console.ReadLine());
            }
            catch
            {
                //Prevents Crashing
            }

            //Validates input
            while (iStanChoice < 1 || iStanChoice > Standards.Length)
            {
                Console.Clear();           

                Console.WriteLine("Error, please input a valid integer.");
                Console.WriteLine("");

                //Prevents Crashing if User Enters the Wrong Data Type
                try
                {
                    StandardMenuText(Standards); //Calls Menu Text

                    iStanChoice = Convert.ToInt16(Console.ReadLine());
                }
                catch
                {
                    //Prevents Crashing
                }
            }

            List<string> MorseCode = new List<string>();

            foreach(var file in Standards)
            {
                if(iCount == iStanChoice)
                {
                    MorseCode = LoadStandard(file);
                }
                iCount++;
            }

            return MorseCode;
        }

        static void StandardMenuText(string[] Standards)
        {

            int iCount = 1;

            Console.WriteLine("Select which Standard");

            foreach(string file in Standards)
            {
                Console.WriteLine(iCount + ") " + file.Substring((System.Environment.CurrentDirectory + @"\Standards\").Length));
                iCount++;
            }

            Console.WriteLine("Option: ");
        }

        public List<string> LoadStandard(string sStandard)
        {
            List<string> MorseCode = new List<string>();

            string[] MorseCodeTranscript = File.ReadAllLines(sStandard);
            foreach (string line in MorseCodeTranscript)
            {
                MorseCode.Add(line);
            }

            return MorseCode;
        }
    }

    public class Applications
    {
        public string MainMenu()
        {
            string sChoice = "";

            MenuText(); //Calls Menu Text

            sChoice = Console.ReadLine() ?? "";

            //Validates Menu Choice
            while (sChoice != "1" && sChoice != "2" && sChoice != "3" && sChoice != "4" && sChoice != "5" && sChoice != "6")
            {
                Console.Clear();

                Console.WriteLine("Error not a valid menu option.");
                Console.WriteLine("");

                MenuText(); //Calls Menu Text

                sChoice = Console.ReadLine() ?? "";
            }

            return sChoice;
        }

        static void MenuText()
        {
            //Main Menu where User Selects Function
            Console.WriteLine("1) Create Morse Code");
            Console.WriteLine("2) Create Text from Morse Code");
            Console.WriteLine("3) Morse Code Training");
            Console.WriteLine("4) Load Saved Translations");
            Console.WriteLine("5) User Management");
            Console.WriteLine("6) Quit");

            Console.Write("Option: ");

        }

        public string StartUp()
        {
            string sUsername = "";

            Console.WriteLine("Welcome to the program");
            Console.WriteLine("");

            sUsername = LoginProcedure();

            Console.Clear();

            return sUsername;
        }

        static string LoginProcedure()
        {
            bool blogin = false;
            UserLogin user = new UserLogin();

            Console.WriteLine("Remember you are typing inputs are hidden for security purposes.");

            //Gets the login variables
            string sUsername = Utilities.GetString("Username: ");

            string sPassword = PasswordInput();

            blogin = user.CheckLogin(sUsername, sPassword);
            //Validation
            while (blogin != true)
            {
                Console.Clear();

                Console.WriteLine("Error, login not recognised");
                Console.WriteLine("");
                Console.WriteLine("Remember you are typing inputs are hidden for security purposes.");

                sUsername = Utilities.GetString("Username: ");

                sPassword = PasswordInput();

                blogin = user.CheckLogin(sUsername, sPassword);
            }
            
            return sUsername;
        }

        static string PasswordInput()
        {
            Console.Write("Password: ");

            StringBuilder passwordBuilder = new StringBuilder();

            bool bLoginInput = true;
            char cNewLine = '\r';
            //Loops Until users Inputs Enter
            while (bLoginInput)
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);//Reads Inputted Characters
                char cpasswordChara = consoleKeyInfo.KeyChar;//Stores Inputted Characters Temporarily

                if (cpasswordChara == cNewLine)
                {
                    bLoginInput = false;//User Inputs Enter
                }
                else
                {
                    passwordBuilder.Append(cpasswordChara.ToString());//Stores Inputted Characters Permenantly
                }
            }

            Console.WriteLine("");

            return passwordBuilder.ToString();
        }
    }

    internal class Program
    {
        static void EnToMorse(List<string> MorseCode, string sUsername)
        {
            MorseStandard Standard = new MorseStandard();
            MorseLang Morse = new MorseLang();

            MorseCode = Standard.StandardMenu();

            //Grabs Sentence User wants to Translate
            Console.WriteLine("Type your sentence you want to translate to morse code:");
            string sRawText = Console.ReadLine() ?? "";
            //Sends Data to Class then Outputs Morse Code
            string sTranslated = Morse.TranslateMorse(sRawText, MorseCode);

            Console.WriteLine(sTranslated);
            Console.WriteLine("");

            SavingTranslate(sRawText, sTranslated, sUsername);
        }

        static void MorseToEn(List<string> MorseCode, string sUsername)
        {
            MorseStandard Standard = new MorseStandard();
            MorseLang morse = new MorseLang();

            MorseCode = Standard.StandardMenu();

            //Grabs Sentence User wants to Turn to Morse
            Console.WriteLine("Since its not on a keyboard copy this character · for the dots.");
            Console.WriteLine("Type your morse code you want to translate to english code:"); 
            string sRawText = Console.ReadLine() ?? "";
            //Checks the User used Morse Characters
            while (!(sRawText.Contains("·") || sRawText.Contains("-")))
            {
                Console.Clear();

                Console.WriteLine("Error please input valid morse characters.");
                Console.WriteLine("");

                Console.WriteLine("Since its not on a keyboard copy this character · for the dots.");
                Console.WriteLine("Type your sentence you want to translate to morse code:");
                sRawText = Console.ReadLine() ?? "";
            }

            //Sends Data to Class then Outputs English
            string sTranslated = morse.TranslateEnglish(sRawText, MorseCode);
            //Checks if Final Variable is blank
            if (sTranslated == "") 
            {
                Console.WriteLine("Error inputted morse doesnt correspond with any characters.");
                Console.WriteLine("Restarting process...");
                Console.WriteLine("");

                MorseToEn(MorseCode, sUsername);
            }
            Console.WriteLine(sTranslated);
            Console.WriteLine("");

            SavingTranslate(sTranslated, sRawText, sUsername);
        }

        static void SavingTranslate(string sRawText, string sTranslated, string sUsername)
        {
            bool bEncrypt = false;
            bool bSaveText = false;
            string iEncrypted = "0";
            string sKey = "";

            PlainText PlainSave = new PlainText();
            EncryptedText EncryptSave = new EncryptedText();

            //Grabs User Choice
            bSaveText = Utilities.BoolChoice("Would you like to save your translation? Y/N");

            Console.Clear();

            if (bSaveText == true)
            {
                //Grabs User Choice
                bEncrypt = Utilities.BoolChoice("Would you like to encrypt your saved text? Y/N");

                Console.Clear();           

                if (bEncrypt == true)
                {
                    iEncrypted = "1";

                    sKey = Utilities.GetString("What key will you use to encrypt: ");

                    Console.Clear();

                    EncryptSave.FileSave(sRawText, sTranslated, sUsername, iEncrypted, sKey);
                }
                else
                {
                    PlainSave.FileSave(sRawText, sTranslated, sUsername, iEncrypted, sKey);
                }
            }

            bool bDownloadText = Utilities.BoolChoice("Would you like to download your translation? Y/N");

            Console.Clear();

            if (bDownloadText == true)
            {
                SaveCompressDownload(sRawText);
            }
        }

        static void SaveCompressDownload(string sTranslated)
        {
            string sFileName = Utilities.GetString("What will you name the file: ");

            Console.Clear();

            HuffmanTree huffmanTree = new HuffmanTree();

            //Builds the Huffman Tree
            huffmanTree.Build(sTranslated);

            //Encodes Data
            BitArray encoded = huffmanTree.Encode(sTranslated);
            //Writes the Translated Binary to a Text File
            using (StreamWriter writer = new StreamWriter(sFileName + ".txt"))
            {
                foreach (bool bit in encoded)
                {
                    writer.Write((bit ? 1 : 0) + "");
                }

                writer.Close();
            }

            using (ZipArchive Zip = ZipFile.Open("Downloads/" + sFileName + ".zip", ZipArchiveMode.Create))
            {
                Zip.CreateEntryFromFile(sFileName + ".txt", sFileName + ".txt");
            }
        }

        static void MorseTrain(List<string> MorseCode)
        {
            string sTrainGuess = "";
            string sAgain = "";

            MorseStandard stand = new MorseStandard();
            MorseLang morse = new MorseLang();

            MorseCode = stand.StandardMenu();
            //Runs Code that Picks a Morse Character Randomly
            string sRandom = morse.RandomMorse(MorseCode);

            Console.WriteLine("Your random morse is: " + sRandom.Substring(2));
            Console.WriteLine("Now input what this character is in english: ");
            sTrainGuess = Console.ReadLine() ?? "";
            //Checks if the User Got it Right
            if (sTrainGuess.ToUpper() == sRandom.Substring(0, 1))
            {
                Console.WriteLine("Congratulations you are correct.");
            }
            else
            {
                Console.WriteLine("Sorry that is incorrect, the answer was " + sRandom.Substring(0, 1) + ".");
            }

            sAgain = RandomAgainText();
            //Validation
            while (sAgain != "N" && sAgain != "Y")
            {
                Console.WriteLine("Error please either input y or n for comfirmation.");
                sAgain = RandomAgainText();
            }
            //Checks if the User Wants to do it Againver
            if (sAgain == "Y")
            {
                MorseTrain(MorseCode);
            }
            else
            {
                Console.Clear();

                Console.WriteLine("Returning to main menu.");
                Console.WriteLine("");
            }
        }

        static string RandomAgainText()
        {
            Console.WriteLine("");
            Console.WriteLine("Would you like to go again? Y/N");
            string sAgain = (Console.ReadLine() ?? "").ToUpper();

            return sAgain;
        }

        static void LoadTranslation(string sUsername)
        {
            string sTranID = "";
            bool bDecrypt = false;
            string sTranslatedEn = "";
            string sTranslatedMorse = "";

            Console.WriteLine("Translations done by user " + sUsername + ".");

            QueryData CurrentUser = Database.ExecuteQuery("SELECT * FROM `t_SavedText` WHERE `username` = @val", sUsername).Result;
            
            foreach(var SavedText in CurrentUser)
            {
                Console.WriteLine("ID: " + SavedText["textID"]);
                Console.WriteLine("    English Text: " + SavedText["EnglishText"]);
                Console.WriteLine("    Morse Text: " + SavedText["MorseText"]);
                Console.WriteLine("");
            }

            bDecrypt = Utilities.BoolChoice("Is there an translation you would like to decrypt? Y/N");

            if (bDecrypt == true)
            {
                Console.WriteLine("Select which translation you woud like to decrypt.");
                Console.Write("Id: ");

                sTranID = Console.ReadLine() ?? "";

                bool NoID = true;

                foreach (var SavedText in CurrentUser)
                {
                    if(sTranID == SavedText["textID"])
                    {
                        NoID = false;

                        if (SavedText["IsEncrypted"] == "True")
                        {
                            string sKey = Utilities.GetString("What is the key: ");

                            AESEncryption aes = new AESEncryption(sKey);

                            sTranslatedEn = aes.Decrypt(SavedText["EnglishText"]);
                            sTranslatedMorse = aes.Decrypt(SavedText["MorseText"]);

                            Console.Clear();

                            if (sTranslatedMorse != "Error" && sTranslatedEn != "Error")
                            {
                                Console.WriteLine("    English Text: " + sTranslatedEn);
                                Console.WriteLine("    Morse Text: " + sTranslatedMorse);
                                Console.WriteLine("");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error this translation is not encrypted returning to main menu.");
                            Console.WriteLine("");
                        }
                    }
                }

                if(NoID == true)
                {
                    Console.Clear();

                    Console.WriteLine("Error there is no translation with a matching ID.");
                    Console.WriteLine("");
                }
            }
            else
            {
                Console.Clear();

                Console.WriteLine("Returning to main menu.");
                Console.WriteLine("");
            }
        }

        static void Main(string[] args)
        {          
            Database.Init("plesk.remote.ac", "WS313624_OOP_MorseCode", "Strain@97", "WS313624_OOP_MorseCode", "SSLMode=None");

            string sUserChoice = "";
            string sChoice = "";
            string sUsername = "";

            //Allows to Call from Classes
            UserLogin user = new UserLogin();
            Applications App = new Applications();

            //Creates List to Collect Values from Text files
            List<string> MorseCode = new List<string>();
            //Runs Start Up Code
            sUsername = App.StartUp();
            //Stores Current User
            User CurrentUser = new User(sUsername);
            //Loops Until User Chooses to Leave
            do
            {
                //Main Menu
                sChoice = App.MainMenu();

                Console.Clear();

                switch (sChoice)
                {
                    case "1":

                        EnToMorse(MorseCode, sUsername);

                        break;
                    case "2":

                        MorseToEn(MorseCode, sUsername);

                        break;
                    case "3":

                        MorseTrain(MorseCode);

                        break;
                    case "4":

                        LoadTranslation(sUsername);

                        break;
                    case "5":
                        do
                        {
                            //User Menu Selection
                            sUserChoice = user.UserAccountMenu();

                            Console.Clear();

                            switch (sUserChoice)
                            {
                                case "1":
                                    user.AddNewUser();
                                    break;
                                case "2":
                                    user.EditUser();
                                    break;
                                case "3":
                                    user.DeleteUser();
                                    break;
                            }

                        } while (sUserChoice != "4");
                        break;
                }

            } while (sChoice != "6");
        }
    }
}