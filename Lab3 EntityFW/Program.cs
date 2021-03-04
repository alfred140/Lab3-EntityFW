using Microsoft.Data.SqlClient;
using System;

namespace Lab3_EntityFW
{
    class Program
    {
        private static string sqlShowAll = "select Butiksnamn, Titel, Concat(Antal, ' st') As Antal " +
                                "from LagerSaldo " +
                                "JOIN Böcker ON ISBN13 = LagerSaldo.ISBN " +
                                "JOIN Butiker ON ButikID = Butiker.[Identity-ID] ";

        private static string sqlShowStores = "select Butiksnamn from Butiker";
        private static string sqlShowBooks = "select Titel from Böcker";

        static void Main(string[] args)
        {
            bool runtime = true;

            while (runtime)
            {
                Console.WriteLine("\nSkriv siffran för en av följande kommandon\n");

                Console.WriteLine("1 <Visa alla lagersaldon>\n2 <Redigera>\n");
                string input = Console.ReadLine();

                if (input == "1")
                {
                    dataReader(sqlShowAll);
                    continue;
                }
                else if (input == "2")
                {
                    Console.WriteLine("Startar redigering");
                }
                else
                {
                    Console.WriteLine("Inget kunde visas, kontrollera inmatning");
                    continue;
                }

                bool edit = true;

                while (edit)
                {
                    dataReader(sqlShowStores);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Skriv butiksnamnet för att visa böckerna i den");
                    Console.ResetColor();
                    string butiksNamn = Console.ReadLine();

                    if (dataReader(sqlShowAll + $"where Butiksnamn = '{butiksNamn}'") == true)
                    {
                        //Data hittades
                    }
                    else
                    {
                        break;
                    }
                tryAgain:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Vill du ta bort eller lägga till en bok? \n");
                    Console.ResetColor();
                    Console.WriteLine("1 <Lägg till> \n2 <Ta bort>");
                    string addOrRemove = Console.ReadLine();

                    if(addOrRemove == "1")
                    {
                        dataReader(sqlShowBooks);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Skriv Bokens titel du vill lägga till i " + butiksNamn);
                        Console.ResetColor();
                        string bokNamn = Console.ReadLine();
                        dataAdder(butiksNamn, bokNamn);
                    }
                    else if(addOrRemove == "2")
                    {
                        dataReader(sqlShowAll + $"where Butiksnamn = '{butiksNamn}'");
                        Console.WriteLine("Skriv Bokens titel du vill bort från " + butiksNamn);
                        string bokNamn = Console.ReadLine();

                        dataDelete(butiksNamn, bokNamn);
                    }
                    else
                    {
                        Console.WriteLine("Fel inmatning, försök igen");
                        goto tryAgain;
                    }
                    

                    break;
                }
            }
        }


        private static bool dataReader(string viewCommand)
        {
            string connectionString = "Server=localhost;Database=lab 2;Trusted_Connection=True";
            using SqlConnection connection = new SqlConnection(connectionString);
            {
                connection.Open();

                SqlCommand command = new SqlCommand
                    (viewCommand, connection);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write($"{reader.GetName(i)}: ".PadRight(45));
                    }
                    Console.ResetColor();
                    Console.WriteLine("\n");


                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (i == reader.FieldCount - 1)
                            {
                                Console.Write($"{reader.GetValue(i)}");
                                Console.WriteLine("\n");
                            }
                            else
                            {
                                Console.Write($"{reader.GetValue(i)}".PadRight(45, '.'));
                            }
                        }                                               
                    }
                    connection.Close();
                    return true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Inget att visa, Försök igen");
                    Console.ResetColor();
                    connection.Close();
                    return false;
                }
            }
        }

        private static void dataDelete(string butik, string titel)
        {
            string connectionString = "Server=localhost;Database=lab 2;Trusted_Connection=True";
            using SqlConnection connection = new SqlConnection(connectionString);
            {
                connection.Open();                

                SqlCommand command = new SqlCommand

                    ("Delete LagerSaldo from LagerSaldo"+
                    " inner JOIN Böcker ON ISBN13 = LagerSaldo.ISBN"+
                    " inner JOIN Butiker ON ButikID = Butiker.[Identity-ID]"+

                    $" where Butiksnamn = '{butik}' and Titel = '{titel}'", connection);
                int numOfRecords = command.ExecuteNonQuery();

                Console.WriteLine("Antel rader borttagna: " + numOfRecords);

                connection.Close();
            }
        }
        private static void dataAdder(string butik, string titel)
        {
            string connectionString = "Server=localhost;Database=lab 2;Trusted_Connection=True";
            using SqlConnection connection = new SqlConnection(connectionString);
            {
                connection.Open();

                SqlCommand command = new SqlCommand

                    ("INSERT INTO LagerSaldo(ButikID, ISBN)" +

                    " select Butiker.[Identity-ID], Böcker.ISBN13" +
                    " from Butiker, Böcker" +

                    $" where Butiksnamn = '{butik}' and Titel = '{titel}' " +                    

                    " update LagerSaldo" +

                    " set Antal = 1" +
                    " from LagerSaldo" +
                    " inner join Böcker ON LagerSaldo.ISBN = ISBN13" +
                    " inner JOIN Butiker ON [Identity-ID] = LagerSaldo.ButikID" +

                    $" where Butiksnamn = '{butik}' and Titel = '{titel}'", connection);

                try
                {
                    int numOfRecords = command.ExecuteNonQuery();
                    Console.WriteLine("Antel rader ändrade: " + numOfRecords);
                }
                catch
                {
                    Console.WriteLine("Boken finns redan i denna butik");
                }
                
                connection.Close();
            }
        }
    }
}