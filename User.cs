using JollyWrapper;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BCrypt.Net;

namespace MorseCodeTranslator
{

    public class UserLogin
    {
        public bool CheckLogin(string sUsername, string sPassword)
        {
            QueryParms parms = new QueryParms()
                {
                    {"@username", sUsername }
                };

            //Checks Login Credentials
            QueryData users = Database.ExecuteQuery("SELECT * FROM `t_users` WHERE `username` = @username",parms).Result;

            bool bLogin = false;
            //Loops over Results
            foreach (var user in users)
            {
                if (BCrypt.Net.BCrypt.Verify(sPassword, user["password"]))
                {
                    bLogin = true;
                }
            }

            return bLogin;
        }

        public string UserAccountMenu()
        {
            string sChoice = "";
            //Menu Options To Manage User Accounts
            sChoice = AccountMenuText();

            //Validates User Menu Choice
            while (sChoice != "1" && sChoice != "2" && sChoice != "3" && sChoice != "4")
            {
                Console.WriteLine("Error not a valid menu option.");
                Console.WriteLine("");

                sChoice = AccountMenuText();
            }

            return sChoice;
        }

        static string AccountMenuText()
        {
            string sChoice = "";

            Console.WriteLine("1) Add new user");
            Console.WriteLine("2) Edit existing user");
            Console.WriteLine("3) Delete existing user");
            Console.WriteLine("4) Exit");

            Console.Write("Option: ");
            sChoice = Console.ReadLine() ?? "";

            return sChoice;
        }

        public void AddNewUser()
        {
            Console.WriteLine("You have chosen to add a new user.");
            string addUserFirst = Utilities.GetString("First name: ");
            string addUserLast = Utilities.GetString("Last name: ");
            string addUserName = Utilities.GetString("Username: ");
            //Hashes the Password
            string addUserPass = BCrypt.Net.BCrypt.HashPassword(Utilities.GetString("Password: "));
            //Creates A New User
            QueryParms parms = new QueryParms()
                {
                    {"@name", addUserFirst },
                    {"@surname", addUserLast },
                    {"@username", addUserName },
                    {"@password", addUserPass }

                };

            Database.ExecuteNonQuery("INSERT INTO `t_users` (`userID`, `firstName`, `lastName`, `username`, `password`, `TIMESTAMP`) VALUES (NULL, @name, @surname, @username, @password, current_timestamp());", parms);

            Console.Clear();

            Console.WriteLine("User Successfully Created.");
            Console.WriteLine("");
        }

        public void EditUser()
        {
            Console.WriteLine("You have chosen to edit a user.");
            string sUserEdit = Utilities.GetString("Please enter the user ID you want to edit: ");

            QueryData users = Database.ExecuteQuery("SELECT * FROM `t_users`").Result;

            bool bExist = false;
            //Finds Corresponding User ID
            foreach (var user in users)
            {
                if (sUserEdit == user["userID"])
                {
                    bExist = true;
                }
            }

            Console.Clear();

            if (bExist == true)
            {
                QueryData user = Database.ExecuteQuery("SELECT * FROM `t_users` WHERE `t_users`.`userID` = @val", sUserEdit).Result;

                //Allows to Edit User Information
                foreach (var User in user)
                {
                    Console.WriteLine("You are editing: ");
                    Console.WriteLine("First name: " + User["firstName"]);
                    Console.WriteLine("Last name: " + User["lastName"]);
                    Console.WriteLine("Username: " + User["username"]);
                    Console.WriteLine("");
                }
                string editUserFirst = Utilities.GetString("Enter First name: ");
                string editUserLast = Utilities.GetString("Enter Last name: ");
                string editUserName = Utilities.GetString("Enter Username: ");

                QueryParms parms = new QueryParms()
                {
                    {"@name", editUserFirst },
                    {"@surname", editUserLast },
                    {"@username", editUserName },
                    {"@id", sUserEdit }
                };

                Database.ExecuteNonQuery("UPDATE `t_users` SET `firstName` = @name, `lastName` = @surname, `username` = @username WHERE `t_users`.`userID` = @id", parms);

                Console.Clear();

                Console.WriteLine("User has been updated.");
            }
            else
            {
                Console.WriteLine("Error, no user with inputted ID returning to main menu.");
                Console.WriteLine("");
            }
        }

        public void DeleteUser()
        {
            Console.WriteLine("You have chosen to delete a user.");
            string sUserDelete = Utilities.GetString("Please enter the user ID you want to delete: ");

            QueryData users = Database.ExecuteQuery("SELECT * FROM `t_users`").Result;

            bool bExist = false;
            //Finds the Corresponding User
            foreach (var user in users)
            {
                if (sUserDelete == user["userID"])
                {
                    bExist = true;
                }
            }

            Console.Clear();

            if (bExist == true)
            {
                QueryData user = Database.ExecuteQuery("SELECT * FROM `t_users` WHERE `t_users`.`userID` = @val", sUserDelete).Result;

                //Deletes the User
                foreach (var User in user)
                {
                    Console.WriteLine("You are deleting: ");
                    Console.WriteLine("First name: " + User["firstName"]);
                    Console.WriteLine("Last name: " + User["lastName"]);
                    Console.WriteLine("Username: " + User["username"]);
                    Console.WriteLine("Password: " + User["password"]);
                    Console.WriteLine("");
                }
                Console.WriteLine("Are you sure you want to delete this user? (yes, no)");
                string sDeleteConfirm = (Console.ReadLine() ?? "").ToLower();

                Console.Clear();

                //Validates Delete Input
                while (sDeleteConfirm != "yes" && sDeleteConfirm != "no")
                {
                    Console.WriteLine("Error, please confirm with either yes or no.");
                    Console.WriteLine("Are you sure you want to delete this user? (yes, no)");
                    sDeleteConfirm = (Console.ReadLine() ?? "").ToLower();

                    Console.Clear();
                }

                if (sDeleteConfirm == "yes")
                {
                    Console.WriteLine("Deleting...");
                    Database.ExecuteQuery("DELETE FROM t_users WHERE `t_users`.`userID` = @val", sUserDelete);

                    Console.Clear();

                    Console.WriteLine("Delete complete");
                }
                else
                {
                    Console.Clear();

                    Console.WriteLine("Deletion aborted returning to menu.");
                    Console.WriteLine("");
                }
            }
            else
            {
                Console.WriteLine("Error, no user with inputted ID returning to main menu.");
                Console.WriteLine("");
            }
        }
    }
}
