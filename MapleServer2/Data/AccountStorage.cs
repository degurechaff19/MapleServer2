using System.Collections.Generic;
using MapleServer2.Types;
using MySql.Data.MySqlClient;
using System.Data;
using System;

namespace MapleServer2.Data {
    // Class for retrieving and storing account data
    public static class AccountStorage {
        // Temp account and character ids
        public const long DEFAULT_ACCOUNT_ID = 1;

        public static int TickCount = 0;

        // Dictionary of character ids for the account
        public static Dictionary<long, List<long>> accountCharacters = new Dictionary<long, List<long>>();

        // Dictionary of characters for the account
        public static Dictionary<long, Player> characters = new Dictionary<long, Player>();

        static AccountStorage() {
            // Connection String y'all can make a class for another purposes 
            string myConnection = "datasource=localhost;port=3306;username=hiiro;password=hiiro";
            MySqlConnection myConn = new MySqlConnection(myConnection);
            myConn.Open();
            string query = "Select * FROM ms_ph.char_hud WHERE AccID = 1";
            MySqlDataAdapter returnVal = new MySqlDataAdapter(query, myConn);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);

            try
            {
                // Add temp characters

                // This Loop below is for "accountCharacter" LISTING
                List<long> charid = new List<long>();
                long UID = 0;
                foreach (DataRow CharRow in dt.Rows)
                {
                    /*CharID*/
                    long Chars = (Int32)CharRow["CharID"];
                    long[] input = { Chars };
                    foreach (long chr in input)
                    {
                        charid.Add(chr);
                    }
                    /*UserID*/
                    UID = (Int32)CharRow["AccID"];
                }
                accountCharacters.Add(UID, charid);

                // This Loop below is for Character Creation & Details like levels,jobs etc..
                foreach (DataRow row in dt.Rows)
                {
                    /*CharID*/
                    long CHARs = (Int32)row["CharID"];
                    //Int to byte (for gender)
                    int intValue = (Int32)row["Gender"];
                    byte[] intBytes = BitConverter.GetBytes(intValue);
                    byte[] result = intBytes;
                    //Details
                    UID = (Int32)row["AccID"];
                    string name = (string)row["Name"];
                    int job = (Int32)row["Job"];
                    short level = Convert.ToInt16((Int32)row["Level"]);
                    byte gender = result[0];
                    string motto = (string)row["Motto"];
                    string home = (string)row["HomeName"];
                    long Mesos = (Int32)row["Mesos"];
                    long Merets = (Int32)row["Merets"];
                    long ValorToken = (Int32)row["ValorToken"];
                    long Treva = (Int32)row["Treva"];
                    long Rue = (Int32)row["Rue"];
                    long HaviFruit = (Int32)row["HaviFruit"];
                    long MesoToken = (Int32)row["MesoToken"];
                    short Storage = Convert.ToInt16((Int32)row["Storage"]);

                    //Character Create
                    characters.Add(CHARs, Player.Char1(UID, CHARs, name, job, level, gender, motto, home, Mesos, Merets, ValorToken, Treva, Rue, HaviFruit, MesoToken, Storage));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //if you want to close the mysqlcon, just uncomment this
            // myConn.Close();
        }

        // Retrieves a list of character ids for an account
        public static List<long> ListCharacters(long accountId) {
            return accountCharacters.GetValueOrDefault(accountId, new List<long>());
        }

        // Adds new character
        public static void AddCharacter(Player data) {
            characters.Add(data.CharacterId,data);
            // Connection String y'all can make a class for another purposes 
            string myConnection = "datasource=localhost;port=3306;username=hiiro;password=hiiro";
            MySqlConnection myConn = new MySqlConnection(myConnection);
            myConn.Open();
            MySqlCommand comm = myConn.CreateCommand();
            comm.CommandText = "INSERT INTO ms_ph.char_hud(AccID,CharID,Name,Job,Level,Gender) VALUES(?aid, ?cid, ?name, ?job, ?level, ?gender)";
            comm.Parameters.Add("?aid", MySqlDbType.Int64).Value = data.AccountId;
            comm.Parameters.Add("?cid", MySqlDbType.Int32).Value = data.CharacterId;
            comm.Parameters.Add("?name", MySqlDbType.String).Value = data.Name;
            comm.Parameters.Add("?job", MySqlDbType.Int32).Value = data.JobId;
            comm.Parameters.Add("?level", MySqlDbType.Int16).Value = data.Level;
            comm.Parameters.Add("?gender", MySqlDbType.Int32).Value = data.Gender;
            comm.ExecuteNonQuery();
            myConn.Close();
        }

        // Retrieves a specific character for an account
        public static Player GetCharacter(long characterId) {
            return characters.GetValueOrDefault(characterId);
        }

        // Updates a character
        public static void UpdateCharacter(Player data) {
            characters.Remove(data.CharacterId);
            characters.Add(data.CharacterId, data);
        }
    }
}