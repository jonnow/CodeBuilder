using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBuilder
{
    class Program
    {
        const string versionNumber = "1.0";
        const int MAX_ROWS = 100;

        static void Main(string[] args)
        {
            string fileName = "";
            string fileLocation = @"C:\Users\jonno\";
            int count = 0;
            Row[] Rows = new Row[MAX_ROWS];

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("__  __________  __   ______          __        ____        _ __    __         ");
            Console.WriteLine("\\ \\/ /_  __/ / / /  / ____/___  ____/ /__     / __ )__  __(_) /___/ /__  _____");
            Console.WriteLine(" \\  / / / / / / /  / /   / __ \\/ __  / _ \\   / __  / / / / / / __  / _ \\/ ___/");
            Console.WriteLine(" / / / / / /_/ /  / /___/ /_/ / /_/ /  __/  / /_/ / /_/ / / / /_/ /  __/ /    ");
            Console.WriteLine("/_/ /_/  \\____/   \\____/\\____/\\__,_/\\___/  /_____/\\__,_/_/_/\\__,_/\\___/_/     ");
            Console.WriteLine("                                                                              ");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Welcome to the YTU Code Builder v" + versionNumber + "!\n");
            Console.WriteLine("Please ensure your txt file is in the following format: ");
            Console.WriteLine("\t- Name, type, required, customValidator");
            Console.WriteLine("\t  (Name can be anything you want, type must be a textbox or dropdownlist)");
            Console.WriteLine("\t- One question entry per line");
            Console.WriteLine("\t- Line items seperated with a comma");
            Console.WriteLine("\nPlease enter the .txt file name that contains the list of questions for adding a participant:");

            fileName = Console.ReadLine();
            // Add the extension if it hasn't got one
            if (fileName.IndexOf(".") <= 0)
            {
                fileName = fileName + ".txt";
            }

            try
            {
                StreamReader sr = new StreamReader(fileLocation + fileName);
                StreamWriter writer = new StreamWriter(fileLocation + "output.aspx");

                while (sr.EndOfStream == false)
                {
                    Rows[count] = new Row();

                    List<string> rowItems = sr.ReadLine().Split(',').ToList<string>();
                    int? isRequired = null,
                                    customValidator = null;
                    string markup = "";

                    // Clean out any user entered spaces
                    for (int i = 0; i < rowItems.Count; i++)
                    {
                        rowItems[i] = rowItems[i].Trim();
                    }

                    //  Is it a required field?
                    if (rowItems.FindIndex(x => x.Contains("required")) >= 0)
                    {
                        isRequired = rowItems.FindIndex(x => x.Contains("required"));
                        Rows[count].Validation = true;
                    }

                    //  Does it need a custom validator?
                    if (rowItems.FindIndex(x => x.Contains("customValidator")) >= 0)
                    {
                        customValidator = rowItems.FindIndex(x => x.Contains("customValidator"));
                        Rows[count].Validation = true;
                    }


                    // Type of input
                    switch (rowItems[1])
                    {
                        case "textbox":
                            Rows[count].Type = "Textbox";
                            Rows[count].ID = "txt";
                            break;
                        case "ddl":
                        case "dropdownlist":
                        case "select":
                            Rows[count].Type = "DropDownList";
                            Rows[count].ID = "ddl";
                            break;
                        default:
                            break;
                    }

                    string firstWord = rowItems.First();
                    // First entry - the name & ID
                    if (rowItems.First().Split(' ').Length > 1)
                    {
                        List<string> nameList = rowItems.First().Split(' ').ToList<string>();
                        foreach (string word in nameList)
                        {
                            if (word.ToUpper() == "ID")
                            {
                                // Check if the word is ID
                                Rows[count].ID += word.ToUpper();
                            }
                            else
                            {
                                Rows[count].ID += word.Substring(0, 1).ToUpper() + String.Join("", word.Skip(1));
                            }
                        }
                    }
                    else
                    {

                        Rows[count].ID += firstWord.Substring(0, 1).ToUpper() + String.Join("", firstWord.Skip(1));
                    }

                    Rows[count].Name = firstWord.Substring(0, 1).ToUpper() + String.Join("", firstWord.Skip(1));


                    markup = "<div class=\"row\">\n";

                    for (int i = 0; i < rowItems.Count; i++)
                    {
                        if (i == 0)
                        {
                            markup += "\t<div class=\"cell";
                            if (isRequired.HasValue)
                            {
                                markup += " hasValidator";
                            }
                            else
                            {
                                markup += " label";
                            }
                            markup += "\">" + Rows[count].Name + ":</div>";



                        }

                        if (Rows[count].Validation == true && i == isRequired)
                        {
                            markup += "\n\t<div class=\"cell isValidator\">";
                            markup += "\n\t\t<asp:RequiredFieldValidator ID=\"" + Rows[count].ID + "_RequiredFieldValidator\" runat=\"server\" ControlToValidate=\"" + Rows[count].ID + "\" Display=\"Dynamic\" Text=\"*\" CssClass=\"error\" />";
                            markup += "\n\t</div>";
                        }

                        if (Rows[count].Validation == true && i == customValidator)
                        {
                            markup += "\n\t<div class=\"cell isValidator\">";
                            markup += "\n\t\t<asp:CustomFieldValidator ID=\"" + Rows[count].ID + "_CustomFieldValidator\" runat=\"server\" ControlToValidate=\"" + Rows[count].ID + "\" Display=\"Dynamic\" Text=\"*\" CssClass=\"error\" OnServerValidate=\"" + Rows[count].ID + "_CustomFieldValidator\" />";
                            markup += "\n\t</div>";
                        }
                    }

                    markup += "\n\t<div class=\"cell value\">";
                    markup += "\n\t\t<asp:" + Rows[count].Type + " runat=\"server\" id=\"" + Rows[count].ID;
                    // Add a single item to the dropdown list
                    if (Rows[count].Type == "DropDownList")
                    {
                        markup += "\" />";
                        markup += "\n\t\t\t<asp:ListItem Text=\"XXXX\" Value=\"XXXX\" />";
                        markup += "\n\t\t</asp:DropDownList>";
                    }
                    else
                    {
                        markup += "\" />";
                    }
                    markup += "\n\t</div>";

                    markup += "\n</div>";

                    writer.WriteLine(markup);
                    count++;
                }

                writer.WriteLine();
                // ALWAYS CLOSE THE STREAM!
                writer.Close();
            }
            catch
            {
                Console.WriteLine("File could not be found!");
            }

            Console.WriteLine("\n\nBuilding complete!\n\nPress any key to continue.");

            Console.ReadKey();
        }

        class Row
        {
            public string Name;
            public string Type;
            public bool Validation;
            public string Required;
            public string CustomValidator;
            public string ID;
        }
    }
}
