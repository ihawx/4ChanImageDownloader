/*                                            Unoffical 4Chan Image Downloader
 *                              ****THIS IS NOT AN OFFICIAL 4CHAN APPLICATION****
 * This simple program can search through all the boards on 4chan, look for a given key words through catalog
 * and in case it finds them, download all the images in a thread which contained the keyword.
 * Because the program only searches through the catalogs, it will only identify thread if the key word is in 
 * OP, subject of the thread or three last replies.
 * You can set the interval in between searches to whatever you want, but there's no need to search every second
 * as most boards are too slow anyway for you to miss out on a picture you want in case that someone replies with
 * a searched keyword.
 * You can also specify which words you want to ignore. In case that you choose to do so, every thread which
 * contains both the searched keyword and a word set as ignored will be ignored and won't be searched.
 * 
 * To learn more about the 4Chan API visit this page: https://github.com/4chan/4chan-API
 * 
 * This program uses the Newtonsoft Json.NET library which can be found here: http://www.newtonsoft.com/json
 * It uses the MIT License which is contained below:
 * The MIT License (MIT)
 * Copyright (c) 2007 James Newton-King
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
 * WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * This program was created by iHawX (github: https://github.com/ihawx) in 2015. Use it, modify it, share it at your own will.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;


namespace webProject
{
    class Program
    {

        static void Main(string[] args)
        {
            System.Net.WebClient wc = new System.Net.WebClient();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Welcome to 4chan image downloader!\nThis isn't an offical 4Chan application\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Which boards do you wish to search?");
            // gets the list of boards (eg.: enter diy for /diy/ board)
            List<string> boards = GetSearchList(false);
            // gets the waiting period between searches in seconds
            Console.WriteLine("How many seconds do you want to wait between searches?");
            var waittime = Convert.ToInt32(Console.ReadLine());
            // gets the list of key words
            Console.WriteLine("Which words do you wish to search for?");
            List<string> words = GetSearchList(false);
            // gets the list of a words that you wish to ignore
            Console.WriteLine("Which words do you wish to ignore?");
            List<string> badwords = GetSearchList(true);


            while ( true )
            {
                foreach ( string board in boards )
                {
                    // gets the path where files will be downloaded in format /path/board/date/
                    string dlPath = GetDownloadPath(board);
                    // downloads the catalog of a given board and deserializes it
                    string targetPath = @"http://a.4cdn.org/" + board + "/catalog.json";
                    string dlText = wc.DownloadString(targetPath);
                    string deserialized = JsonConvert.DeserializeObject(dlText).ToString();
                    var page = deserialized.Split('\n');
                    // finds out which threads contain the key words
                    List<string> toDownload = GetDownloadList(page, words, badwords, board);
                    // if no threads are found
                    if ( toDownload.Count == 0 )
                    {
                        Console.WriteLine("Couldn't find any threads");
                    }

                    Console.WriteLine("\n");
                    // goes through each found thread
                    foreach ( string n in toDownload )
                    {
                        // downloads the page of a given thread and deserializes it
                        Console.WriteLine("Entering thread no.: " + n);
                        string newPath = @"http://a.4cdn.org/" + board + @"/thread/" + n + ".json";
                        dlText = wc.DownloadString(newPath);
                        deserialized = JsonConvert.DeserializeObject(dlText).ToString();
                        var lines = deserialized.Split('\n');
                        // finds all pictures and gifs/webms in a given thread and downloads them
                        SearchAndDownload(lines, dlPath, board, wc, n);
                    }
                }

                System.Threading.Thread.Sleep(waittime * 1000);
                Console.WriteLine("\nBeginning new search !");
            }
        }

        static List<string> GetDownloadList(string[] page, List<string> words, List<string> badwords, string board)
        {
            List<string> toDownload = new List<string>();
            string no = ""; // post number
            string resto = ""; // post number of an original post to which this post replies
            bool toBeAdded = false;
            string foundWord = ""; // identifies the found word just for our information
            foreach ( string a in page )
            {

                if ( a.Contains("\"no\":") )
                {
                    // gets the post number, puts it in a correct format if needed
                    no = a.Substring(14, a.LastIndexOf(',') - 14);
                    if ( no.Contains("o\":") )
                    {
                        no = no.Substring(4);
                    }
                }
                if ( a.Contains("\"com\":") || a.Contains("\"sub\":") )
                {
                    // checks whether the post or it's subject contain some of the key words
                    foreach ( string w in words )
                    {
                        foreach ( string bad in badwords )
                        {
                            // cannot check if the list already contains the OP in case that the key word was found in a reply
                            // because of the format of a downloaded page
                            if ( a.ToLower().Contains(w.ToLower()) && !toDownload.Contains(no) && ( !a.ToLower().Contains(bad.ToLower()) ) )
                            {
                                foundWord = w;
                                toBeAdded = true;
                            }
                        }
                    }
                }
                if ( a.Contains("\"resto\":") )
                {
                    if ( toBeAdded )
                    {
                        toBeAdded = false;
                        // puts the line in a correct format
                        resto = a.Replace("\"resto\":", "");
                        resto = resto.Replace(" ", "");
                        resto = resto.Replace(",", "");
                        resto = resto.Replace("\n", "");
                        // if resto beggins with 0, than it isn't a reply
                        if ( resto.Substring(0, 1).Contains("0") )
                        {
                            if ( !toDownload.Contains(no) )
                            {
                                toDownload.Add(no);
                                Console.WriteLine("Post no. " + no + " on board /" + board + "/ contains word: " + foundWord);
                            }

                        }
                        else
                        {
                            // i have no idea where that carriage return comes from, but it messes everything up
                            resto = resto.Replace("\r", "");
                            if ( !toDownload.Contains(resto) )
                            {
                                toDownload.Add(resto);
                                Console.WriteLine("Post no. " + no + " on board /" + board + "/ contains word: " + foundWord + "  OP no.: " + resto);
                            }
                        }

                    }
                }
            }

            return toDownload;
        }

        static List<string> GetSearchList(bool emptyList)
        {
            // adds input from a console into a list
            // press ENTER when you don't want to add any more parameters
            string input = Console.ReadLine();
            List<string> output = new List<string>();
            while ( input.Length == 0 && !emptyList)
            {
                Console.WriteLine("You have to enter at least one parameter");
                input = Console.ReadLine();
            }
            output.Add(input);
            while ( input.Length > 0 )
            {
                input = Console.ReadLine();
                if ( input.Length > 0 )
                {
                    output.Add(input);
                }
            }

            return output;
        }

        static string GetDownloadPath(string board)
        {
            // the path for downloades is in a format /path specified bellow/board/current date/
            string dlPath = @"C:\Users\Lubos\Pictures\4Chan\"; //directory for downloads

            dlPath += board + @"\";
            // puts the files in a folder for current day
            string date = DateTime.Now.ToString();
            date = date.Substring(0, date.IndexOf(":") - 2);
            date = date.Replace(" ", "");
            date = date.Replace(".", "-");
            dlPath += date + @"\";
            // in case that the directory doesn't exist yet, create it
            if ( !System.IO.Directory.Exists(dlPath) )
            {
                System.IO.Directory.CreateDirectory(dlPath);
            }

            return dlPath;
        }

        static void SearchAndDownload(string[] lines, string dlPath, string board, System.Net.WebClient wc, string n)
        {
            int total = 0, duplicate = 0; // total number of files in a thread and number of files already existing in a download directory
            string fileName = ""; // name of the file
            string ext = ""; // extension of the file
            string tim = ""; // UNIX timestamp + milliseconds
            string toWrite = ""; // string that holds the text content of a thread
            string now = ""; // time of a post
            foreach ( string s in lines )
            {
                if ( s.Contains("\"now\":") )
                {
                    // gets the time of a post
                    now = s.Substring(12, s.LastIndexOf(',') - 12);
                }
                if ( s.Contains("\"filename\":") )
                {
                    // gets the name of file, replaces : with - to avoid errors
                    fileName = s.Substring(19, s.LastIndexOf(',') - 20);
                    fileName = fileName.Replace(":", "-");
                }
                if ( s.Contains("\"ext\":") )
                {
                    // gets the extension of a file
                    ext = s.Substring(14, s.LastIndexOf(',') - 15);
                }
                if ( s.Contains("\"tim\":") )
                {
                    // gets the UNIX timestamp + milliseconds
                    tim = s.Substring(13, s.LastIndexOf(',') - 13);
                    total++;
                    /*
                     * NOTE: In case that you want to be sure, that you get all the pictures, but you are afraid, that there might be
                     *       some pictures with same name, but different in content, save the file in format UNIX_timestamp.extension
                     *       instead of file_name.extension. To change this, go to method "DownloadFile"
                     */
                    //checks for duplicates
                    if ( !System.IO.File.Exists(dlPath + fileName + ext) )
                    {
                        DownloadFile(dlPath, fileName, ext, board, tim, wc);
                    }
                    else
                    {
                        duplicate++;
                    }
                }

                if ( s.Contains("\"com\":") )
                {
                    // gets the text of a post and adds it to a string with a timestamp
                    toWrite += now + ":\n" + s.Substring(13, s.LastIndexOf(',') - 13) + "\n";
                }

            }

            // prints a summary of downloads in a given thread
            Console.Write("Files found: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(total);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("       Files downloaded: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(total - duplicate);
            Console.ForegroundColor = ConsoleColor.White;

            // writes the text contents of a thread in a file
            WriteThreadInTextFile(toWrite, dlPath, n);

        }

        static void DownloadFile(string dlPath, string fileName, string ext, string board, string tim, System.Net.WebClient wc)
        {
            //Start the download
            string imgPath = @"http://i.4cdn.org/" + board + @"/" + tim + ext; // adress where all the images on 4chan can be found
            Console.Write("Downloading image: " + fileName);
            wc.DownloadFile(imgPath, dlPath + fileName + ext); // to get all the unique images, change "fileName" to "tim"
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("  --- Done");
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void WriteThreadInTextFile(string toWrite, string dlPath, string n)
        {
            // make the downloaded text a lit bit more readable
            // still needs a lot more work
            toWrite = toWrite.Replace("a href", "");
            toWrite = toWrite.Replace("class=\\\"quotelink\\\">", "");
            toWrite = toWrite.Replace("</a><br><span class=\\\"quote\\\">&gt;", " ");
            toWrite = toWrite.Replace("\"<=\\\"#", ">>");
            toWrite = toWrite.Replace("\\\" &gt;&gt;", " ");
            toWrite = toWrite.Replace("</a><br>", " ");
            toWrite = toWrite.Replace("<br><br>", " ");
            // file is saved to download_path/thread_number.txt
            System.IO.File.WriteAllText(dlPath + n + ".txt", toWrite, Encoding.UTF8);
        }
    }
}
