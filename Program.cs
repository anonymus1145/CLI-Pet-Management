using System;
using System.IO;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using static System.Text.Encoding;
using System.Text;
using System.Xml;

// the ourAnimals array will store the following: 
string? animalSpecies = "";
string? animalID = "";
string? animalAge = "";
string? animalPhysicalDescription = "";
string? animalPersonalityDescription = "";
string? animalNickname = "";

// variables that support data entry
int maxPets = 8;
string? readResult;
string menuSelection = "";
int petCount = 0;
string anotherPet = "y";
bool validEntry = false;
int petAge = 0;

//Calling the encoding so taht you can read from the DB
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// array used to store runtime data, there is no persisted data
string[,] ourAnimals = new string[maxPets, 6];

// create some initial ourAnimals array entries
for (int i = 0; i < maxPets; i++)
{
    switch (i)
    {
        case 0:
            animalSpecies = "dog";
            animalID = "d1";
            animalAge = "2";
            animalPhysicalDescription = "medium sized cream colored female golden retriever weighing about 65 pounds. housebroken.";
            animalPersonalityDescription = "loves to have her belly rubbed and likes to chase her tail. gives lots of kisses.";
            animalNickname = "lola";
            break;

        case 1:
            animalSpecies = "dog";
            animalID = "d2";
            animalAge = "9";
            animalPhysicalDescription = "large reddish-brown male golden retriever weighing about 85 pounds. housebroken.";
            animalPersonalityDescription = "loves to have his ears rubbed when he greets you at the door, or at any time! loves to lean-in and give doggy hugs.";
            animalNickname = "loki";
            break;

        case 2:
            animalSpecies = "cat";
            animalID = "c3";
            animalAge = "1";
            animalPhysicalDescription = "small white female weighing about 8 pounds. litter box trained.";
            animalPersonalityDescription = "friendly";
            animalNickname = "Puss";
            break;

        case 3:
            animalSpecies = "cat";
            animalID = "c4";
            animalAge = "?";
            animalPhysicalDescription = "";
            animalPersonalityDescription = "";
            animalNickname = "";

            break;

        default:
            animalSpecies = "";
            animalID = "";
            animalAge = "";
            animalPhysicalDescription = "";
            animalPersonalityDescription = "";
            animalNickname = "";
            break;

    }

    ourAnimals[i, 0] = "ID #: " + animalID;
    ourAnimals[i, 1] = "Species: " + animalSpecies;
    ourAnimals[i, 2] = "Age: " + animalAge;
    ourAnimals[i, 3] = "Nickname: " + animalNickname;
    ourAnimals[i, 4] = "Physical description: " + animalPhysicalDescription;
    ourAnimals[i, 5] = "Personality: " + animalPersonalityDescription;
}

// display the top-level menu options
do
{
    Console.Clear();

    Console.WriteLine("Welcome to the Contoso PetFriends app. Your main menu options are:");
    Console.WriteLine(" 1. List all of our current pet information");
    Console.WriteLine(" 2. Add a new animal friend to the list");
    Console.WriteLine(" 3. Ensure animal ages and physical descriptions are complete");
    Console.WriteLine(" 4. Ensure animal nicknames and personality descriptions are complete");
    Console.WriteLine(" 5. Edit an animal’s age");
    Console.WriteLine(" 6. Edit an animal’s personality description");
    Console.WriteLine(" 7. Display all cats with a specified characteristic");
    Console.WriteLine(" 8. Display all dogs with a specified characteristic");
    Console.WriteLine(" 9. See all the pets we have in the database");
    Console.WriteLine("10. Save the animal list in the database");
    Console.WriteLine("11. Clear the database");
    Console.WriteLine();
    Console.WriteLine("Enter your selection number (or type Exit to exit the program)");

    readResult = Console.ReadLine();
    if (readResult != null)
    {
        menuSelection = readResult.ToLower();
        // NOTE: We could put a do statement around the menuSelection entry to ensure a valid entry, but we
        //  use a conditional statement below that only processes the valid entry values, so the do statement 
        //  is not required here. 
    }

    // use switch-case to process the selected menu option
    switch (menuSelection)
    {
        case "1":
            // List all of our current pet information
            for (int i = 0; i < maxPets; i++)
            {
                if (ourAnimals[i, 0] != "ID #: ")
                {
                    Console.WriteLine();
                    for (int j = 0; j < 6; j++)
                    {
                        Console.WriteLine(ourAnimals[i, j].ToString());
                    }
                }
            }
            Console.WriteLine("\n\rPress the Enter key to continue");
            readResult = Console.ReadLine();

            break;

        case "2":
            // Add a new animal friend to the ourAnimals array
            //
            // The ourAnimals array contains
            //    1. the species (cat or dog). a required field
            //    2. the ID number - for example C17
            //    3. the pet's age. can be blank at initial entry.
            //    4. the pet's nickname. can be blank.
            //    5. a description of the pet's physical appearance. can be blank.
            //    6. a description of the pet's personality. can be blank.

            anotherPet = "y";
            petCount = 0;
            for (int i = 0; i < maxPets; i++)
            {
                if (ourAnimals[i, 0] != "ID #: ")
                {
                    petCount += 1;
                }
            }

            if (petCount < maxPets)
            {
                Console.WriteLine($"We currently have {petCount} pets that need homes. We can manage {(maxPets - petCount)} more.");
            }

            while (anotherPet == "y" && petCount < maxPets)
            {
                // get species (cat or dog) - string animalSpecies is a required field 
                do
                {
                    Console.WriteLine("\n\rEnter 'dog' or 'cat' to begin a new entry");
                    readResult = Console.ReadLine();
                    if (readResult != null)
                    {
                        animalSpecies = readResult.ToLower();
                        if (animalSpecies != "dog" && animalSpecies != "cat")
                        {
                            //Console.WriteLine($"You entered: {animalSpecies}.");
                            validEntry = false;
                        }
                        else
                        {
                            validEntry = true;
                        }
                    }
                } while (validEntry == false);

                // build the animal the ID number - for example C1, C2, D3 (for Cat 1, Cat 2, Dog 3)
                animalID = animalSpecies.Substring(0, 1) + (petCount + 1).ToString();

                // get the pet's age. can be ? at initial entry.
                do
                {
                    Console.WriteLine("Enter the pet's age or enter ? if unknown");
                    readResult = Console.ReadLine();
                    if (readResult != null)
                    {
                        animalAge = readResult;
                        if (animalAge != "?")
                        {
                            validEntry = int.TryParse(animalAge, out petAge);
                        }
                    }
                } while (validEntry == false);


                // get a description of the pet's physical appearance - animalPhysicalDescription can be blank.
                do
                {
                    Console.WriteLine("Enter a physical description of the pet (size, color, gender, weight, housebroken)");
                    readResult = Console.ReadLine();
                    if (readResult != null)
                    {
                        animalPhysicalDescription = readResult.ToLower();
                        if (animalPhysicalDescription == "")
                        {
                            animalPhysicalDescription = "tbd";
                        }
                    }
                } while (validEntry == false);


                // get a description of the pet's personality - animalPersonalityDescription can be blank.
                do
                {
                    Console.WriteLine("Enter a description of the pet's personality (likes or dislikes, tricks, energy level)");
                    readResult = Console.ReadLine();
                    if (readResult != null)
                    {
                        animalPersonalityDescription = readResult.ToLower();
                        if (animalPersonalityDescription == "")
                        {
                            animalPersonalityDescription = "tbd";
                        }
                    }
                } while (validEntry == false);


                // get the pet's nickname. animalNickname can be blank.
                do
                {
                    Console.WriteLine("Enter a nickname for the pet");
                    readResult = Console.ReadLine();
                    if (readResult != null)
                    {
                        animalNickname = readResult.ToLower();
                        if (animalNickname == "")
                        {
                            animalNickname = "tbd";
                        }
                    }
                } while (validEntry == false);

                // store the pet information in the ourAnimals array (zero based)
                ourAnimals[petCount, 0] = "ID #: " + animalID;
                ourAnimals[petCount, 1] = "Species: " + animalSpecies;
                ourAnimals[petCount, 2] = "Age: " + animalAge;
                ourAnimals[petCount, 3] = "Nickname: " + animalNickname;
                ourAnimals[petCount, 4] = "Physical description: " + animalPhysicalDescription;
                ourAnimals[petCount, 5] = "Personality: " + animalPersonalityDescription;

                // increment petCount (the array is zero-based, so we increment the counter after adding to the array)
                petCount = petCount + 1;

                // check maxPet limit
                if (petCount < maxPets)
                {
                    // another pet?
                    Console.WriteLine("Do you want to enter info for another pet (y/n)");
                    do
                    {
                        readResult = Console.ReadLine();
                        if (readResult != null)
                        {
                            anotherPet = readResult.ToLower();
                        }

                    } while (anotherPet != "y" && anotherPet != "n");
                }
                //NOTE: The value of anotherPet (either "y" or "n") is evaluated in the while statement expression - at the top of the while loop
            }

            if (petCount >= maxPets)
            {
                Console.WriteLine("We have reached our limit on the number of pets that we can manage.");
                Console.WriteLine("Press the Enter key to continue.");
                readResult = Console.ReadLine();
            }

            break;

        case "3":
            // Ensure animal ages and physical descriptions are complete

            // Check for ages
            for (int i = 0; i < maxPets; i++)
            {
                if (((ourAnimals[i, 2] == "Age: ?") || (ourAnimals[i, 2] == "Age: ")) && (ourAnimals[i, 0]) != "ID #: ")
                {
                    Console.WriteLine($"The {ourAnimals[i, 0]} has no age!");

                    do
                    {
                        Console.Write($"Enter a age for {ourAnimals[i, 0]}: ");
                        animalAge = Console.ReadLine();
                    } while (animalAge == "");

                    if (animalAge != null)
                    {
                        ourAnimals[i, 2] = "Age: " + animalAge;

                        Console.WriteLine($"You add it {ourAnimals[i, 2]} to {ourAnimals[i, 0]}");

                        // Check for physical description
                        if (ourAnimals[i, 4] == "Physical description: ")
                        {
                            Console.WriteLine($"The {ourAnimals[i, 0]} has no physical description!");

                            do
                            {
                                Console.Write($"Enter a physical description for {ourAnimals[i, 0]}: ");
                                animalPhysicalDescription = Console.ReadLine();
                            } while (animalPhysicalDescription == "");

                            if (animalPhysicalDescription != null)
                            {
                                ourAnimals[i, 4] = "Physical description: " + animalPhysicalDescription.ToLower();

                                Console.WriteLine($"You add it the next {ourAnimals[i, 4]} to {ourAnimals[i, 0]}");
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Age and physical description fields are complete for all of our friends.");
            Console.WriteLine("Press the Enter key to continue.");
            readResult = Console.ReadLine();
            break;

        case "4":
            // Ensure animal nicknames and personality descriptions are complete

            // Check for nicknames
            for (int i = 0; i < maxPets; i++)
            {
                if ((ourAnimals[i, 3] == "Nickname: ") && (ourAnimals[i, 0]) != "ID #: ")
                {
                    Console.WriteLine($"The {ourAnimals[i, 0]} has no nickname!");

                    do
                    {
                        Console.Write($"Enter a nickname for {ourAnimals[i, 0]}: ");
                        animalNickname = Console.ReadLine();
                    } while (animalNickname == "");

                    if (animalNickname != null)
                    {
                        ourAnimals[i, 3] = "Nickname: " + animalNickname;
                        Console.WriteLine($"You add it {ourAnimals[i, 3]} to {ourAnimals[i, 0]}");

                        // Check for personality description
                        if (ourAnimals[i, 5] == "Personality: ")
                        {
                            Console.WriteLine($"The {ourAnimals[i, 0]} has no personality description!");

                            do
                            {
                                Console.Write($"Enter a personality description for {ourAnimals[i, 0]}: ");
                                animalPersonalityDescription = Console.ReadLine();
                            } while (animalPersonalityDescription == "");

                            if (animalPersonalityDescription != null)
                            {
                                ourAnimals[i, 4] = "Physical description: " + animalPersonalityDescription.ToLower();
                                Console.WriteLine($"You add it the next {ourAnimals[i, 5]} to {ourAnimals[i, 0]}");
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Nickname and personality description fields are complete for all of our friends.");
            Console.WriteLine("Press the Enter key to continue.");
            readResult = Console.ReadLine();
            break;

        case "5":
            // Edit an animal’s age");
            Console.Write("Insert the nickname of the pet you want to change the age: ");
            string pet = Console.ReadLine() ?? "".ToLower();
            string age = "";

            for (int i = 0; i < maxPets; i++)
            {
                if (ourAnimals[i, 3] == $"Nickname: {pet}")
                {
                    Console.Write("Insert the new age: ");
                    age = Console.ReadLine() ?? "";
                    ourAnimals[i, 2] = "Age: " + age;
                }
            }

            Console.WriteLine("Press the Enter key to continue.");
            readResult = Console.ReadLine();
            break;

        case "6":
            // Edit an animal’s personality description");
            Console.Write("Insert the nickname of the pet you want to change the personality description: ");
            pet = Console.ReadLine() ?? "".ToLower();
            string newDescription = "";

            for (int i = 0; i < maxPets; i++)
            {
                if (ourAnimals[i, 3] == $"Nickname: {pet}")
                {
                    Console.Write("Insert the new description: ");
                    newDescription = Console.ReadLine() ?? "";
                    ourAnimals[i, 4] = "Physical description: " + newDescription;
                }
            }

            Console.WriteLine("Press the Enter key to continue.");
            readResult = Console.ReadLine();
            break;

        case "7":
            // Display all cats with a specified characteristic
            Console.Write("Insert the specific characteristic for the cat: ");
            string characteristic = Console.ReadLine() ?? "";

            Console.WriteLine($"The next cats have the {characteristic} characteristic: ");

            for (int i = 0; i < maxPets; i++)
            {
                if (ourAnimals[i, 1] == "Species: cat")
                {
                    string[] split = ourAnimals[i, 4].Split(' ', '.', ',');

                    for (int j = 0; j < split.Length; j++)
                    {
                        if (characteristic == split[j])
                        {
                            Console.WriteLine(ourAnimals[i, 3]);
                            break;
                        }
                    }
                }
            }

            Console.WriteLine("Press the Enter key to continue.");
            readResult = Console.ReadLine();
            break;

        case "8":
            // Display all dogs with a specified characteristic
            Console.Write("Insert the specific characteristic for the dog: ");
            characteristic = Console.ReadLine() ?? "";

            Console.WriteLine($"The next dogs have the {characteristic} characteristic: ");

            for (int i = 0; i < maxPets; i++)
            {
                if (ourAnimals[i, 1] == "Species: dog")
                {
                    string[] split = ourAnimals[i, 4].Split(' ', '.', ',');

                    for (int j = 0; j < split.Length; j++)
                    {
                        if (characteristic == split[j])
                        {
                            Console.WriteLine(ourAnimals[i, 3]);
                            break;
                        }
                    }
                }
            }
            Console.WriteLine("Press the Enter key to continue.");
            readResult = Console.ReadLine();
            break;

        case "9":
            //Read from the database
            ReadFromDb();
            break;

        case "10":
            //Save in the database
            //Connect to the DB
            string connectionString = "server=sql7.freesqldatabase.com; port=3306; uid=sql7633471; pwd=1QQvSxdfNq; database=sql7633471; charset=utf8; sslMode=none;";
            MySqlConnection connection = new MySqlConnection(connectionString);

            Console.WriteLine("Connect to MySql DB..... \n");

            using (connection)
            {
                try
                {
                    //Open connection to DB
                    connection.Open();
                    Console.WriteLine("Connection is " + connection.State.ToString() + Environment.NewLine);

                    //Create an SQL command
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.Text;

                    for (int i = 0; i < maxPets; i++)
                    {
                        animalID = ourAnimals[i, 0].Remove(0, 6);
                        animalSpecies = ourAnimals[i, 1].Remove(0, 9);
                        animalAge = ourAnimals[i, 2].Remove(0, 5);
                        animalPhysicalDescription = ourAnimals[i, 4].Remove(0, 22);
                        animalPersonalityDescription = ourAnimals[i, 5].Remove(0, 13);
                        animalNickname = ourAnimals[i, 3].Remove(0, 10);

                        if (animalID == " ")
                        {
                            command.CommandText = "INSERT INTO Pet_Management (animalID, animalSpecies, animalAge, animalPhysicalDescription, animalPersonalityDescription, animalNickname) VALUES (' " + animalID + " ', ' " + animalSpecies + " ', ' " + animalAge + " ', ' " + animalPhysicalDescription + " ', ' " + animalPersonalityDescription + " ', ' " + animalNickname + " ')";
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            command.CommandText = "UPDATE Pet_Management SET animalAge = ' " + animalAge + " ', animalSpecies = ' " + animalSpecies + " ', animalPhysicalDescription = ' " + animalPhysicalDescription + " ', animalPersonalityDescription = ' " + animalPersonalityDescription + " ', animalNickname = ' " + animalNickname + " '  WHERE animalID = ' " + animalID + " '";
                            command.ExecuteNonQuery();
                        }

                    }
                    Console.WriteLine("Writing data in the database was succesful!");

                    //Close the connection to DB
                    connection.Close();
                    Console.WriteLine("Connection is " + connection.State.ToString() + Environment.NewLine);
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    Console.WriteLine("Error: " + ex.Message.ToString());
                }
                finally
                {
                    Console.WriteLine("Press enter to exit....");
                    Console.Read();
                }
            }
            break;

        case "11":
            ClearDb();
            break;


        default:
            break;
    }

} while (menuSelection != "exit");


//Method to read from DB
static void ReadFromDb()
{
    //Connect to the DB
    string connectionString = "server=sql7.freesqldatabase.com; port=3306; uid=sql7633471; pwd=1QQvSxdfNq; database=sql7633471; charset=utf8; sslMode=none;";
    MySqlConnection connection = new MySqlConnection(connectionString);

    Console.WriteLine("Connect to MySql DB..... \n");

    using (connection)
    {
        try
        {
            //Open connection to DB
            connection.Open();
            Console.WriteLine("Connection is " + connection.State.ToString() + Environment.NewLine);

            //Create an SQL command
            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "select * from Pet_Management";

            //Read the result
            MySqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string animalID = "ID #: " + reader[0].ToString();
                    string animalSpecies = "Species: " + reader[1].ToString();
                    string animalAge = "Age: " + reader[2].ToString();
                    string animalNickname = "Nickname: " + reader[5].ToString();
                    string animalPhysicalDescription = "Physical description: " + reader[4].ToString();
                    string animalPersonalityDescription = "Personality: " + reader[3].ToString();

                    Console.WriteLine(animalID);
                    Console.WriteLine(animalSpecies);
                    Console.WriteLine(animalAge);
                    Console.WriteLine(animalNickname);
                    Console.WriteLine(animalPhysicalDescription);
                    Console.WriteLine(animalPersonalityDescription);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("-- Data empty --");
            }
            //Close the connection to DB
            connection.Close();
            Console.WriteLine("Connection is " + connection.State.ToString() + Environment.NewLine);
        }
        catch (MySql.Data.MySqlClient.MySqlException ex)
        {
            Console.WriteLine("Error: " + ex.Message.ToString());
        }
        finally
        {
            Console.WriteLine("Press enter to exit....");
            Console.Read();
        }
    }
}

//Method to clear the DB
static void ClearDb()
{
    //Connect to the DB
    string connectionString = "server=sql7.freesqldatabase.com; port=3306; uid=sql7633471; pwd=1QQvSxdfNq; database=sql7633471; charset=utf8; sslMode=none;";
    MySqlConnection connection = new MySqlConnection(connectionString);

    Console.WriteLine("Connect to MySql DB..... \n");

    using (connection)
    {
        try
        {
            //Open connection to DB
            connection.Open();
            Console.WriteLine("Connection is " + connection.State.ToString() + Environment.NewLine);

            //Create an SQL command
            MySqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "delete from Pet_Management";

            //Execute the command
            command.ExecuteNonQuery();

            Console.WriteLine("Clearing data in the database was succesful!");

            //Close the connection to DB
            connection.Close();
            Console.WriteLine("Connection is " + connection.State.ToString() + Environment.NewLine);
        }
        catch (MySql.Data.MySqlClient.MySqlException ex)
        {
            Console.WriteLine("Error: " + ex.Message.ToString());
        }
        finally
        {
            Console.WriteLine("Press enter to exit....");
            Console.Read();
        }
    }
}