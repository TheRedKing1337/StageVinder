using System;
using System.Net;
using System.Diagnostics;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Linq;
using OpenQA.Selenium.Interactions;
using System.IO;

namespace StageVinder
{
    class Program
    {
        private static List<string> gameKeywords = new List<string>() { "unity", "game", "unreal", "gamedeveloper", " vr ", " ar " };
        private static List<string> dotnetKeywords = new List<string>() { "c#", ".net" };
        static void Main(string[] args)
        {
            NewFinder();
        }
        private static void NewFinder()
        {
            //URL and keywords that are searched
            string url = "https://stagemarkt.nl/stages-en-leerbanen/?Termen=gamedeveloper&PlaatsPostcode=&Straal=0&Land=e883076c-11d5-11d4-90d3-009027dcddb5&ZoekenIn=A&Page=71&Longitude=&Latitude=&Regio=&Plaats=&Niveau=&SBI=&Kwalificatie=&Sector=&Kenmerk=&RandomSeed=130&Leerweg=&Internationaal=&Beschikbaarheid=&AlleWerkprocessenUitvoerbaar=&LeerplaatsGewijzigd=&Sortering=1&Bron=STA";

            //Open webpage
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);

            int pageIndex = 0;
            //As long as there are new pages available
            do
            {
                //Go to next page if not on the first,
                //get the first element if on page 1, else get the last(second), because on page 1 onwards the button is the same for both forward and back
                if(pageIndex == 1) driver.Navigate().GoToUrl(driver.FindElement(By.XPath("/html/body/section/div[2]/main/div[1]/div[2]/a")).GetAttribute("href"));
                else driver.Navigate().GoToUrl(driver.FindElements(By.XPath("/html/body/section/div[2]/main/div[1]/div[2]/a")).Last().GetAttribute("href"));

                //Foreach element in the page check for keywords, if found save in relevant file
                var results = driver.FindElements(By.ClassName("c-link-blocks-single"));
                foreach (IWebElement result in results)
                {
                    CheckElement(result);                      
                }
                //Show which page is current and increment page
                Console.WriteLine("Finished searching page: " + pageIndex);
                pageIndex++;

            } 
            //Checks if a next page is available
            while (driver.FindElements(By.XPath("/html/body/section/div[2]/main/div[1]/div[2]/a")).Count > 0);

            driver.Close();
        }
        private static void CheckElement(IWebElement element)
        {
            //Gets the description text box in current element
            var description = element.FindElement(By.XPath(".//div[1]")).FindElement(By.XPath(".//p"));
            //If found game keyword write the info to a file
            foreach (string keyword in gameKeywords)
            {
                if (description.Text.ToLower().Contains(keyword))
                {
                    if (WriteToFile(element, "GameResults.txt"))
                    {
                        Console.WriteLine("Possible game stage found at: " + element.FindElement(By.XPath(".//div[1]")).FindElement(By.XPath(".//h3")).Text);
                    }
                    return;
                }
            }
            //If found dotnet keyword write the info to a file
            foreach (string keyword in dotnetKeywords)
            {
                if (description.Text.ToLower().Contains(keyword))
                {
                    if (WriteToFile(element, "DotnetResults.txt"))
                    {
                        Console.WriteLine("Possible dotnet stage found at: " + element.FindElement(By.XPath(".//div[1]")).FindElement(By.XPath(".//h3")).Text);
                    }
                    return;
                }
            }
        }
        private static bool WriteToFile(IWebElement element, string filename)
        {
            try
            {
                //Get the needed info from the element
                string name = element.FindElement(By.XPath(".//div[1]")).FindElement(By.XPath(".//h3")).Text;
                string location = element.FindElement(By.XPath(".//div[1]")).FindElement(By.XPath(".//ul")).FindElements(By.XPath(".//*")).First().FindElement(By.XPath(".//span")).Text;
                string description = element.FindElement(By.XPath(".//div[1]")).FindElement(By.XPath(".//p")).Text;
                string url = element.GetAttribute("href");

                //Write info to given filepath
                StreamWriter file = File.AppendText(filename);
                file.WriteLine(name);
                file.WriteLine(location);
                file.WriteLine(description);
                file.WriteLine(url);
                file.WriteLine("%-----------------------------------------------------------------------------------------------------------");
                file.Close();

                return true;
            }
            catch
            {
                Console.WriteLine("Error occured trying to write to file");
            }
            return false;
        }
        private static void LegacyFinder()
        {
            // WebClient w = new WebClient();
            //// string s = w.DownloadString("https://stagemarkt.nl/Zoeken/Home/Resultaten?t=gamedeveloper&s=&z=&l=Nederland&b=True&c=&lw=BOL&n=&rg=&i=&pg=3&srt=alfabet&e=False&ToonOpKaart=False&ViewType=Lijst&SeedValue=67&LeerbedrijfId=0&p=");

            // //Console.WriteLine(s);

            // // 2.
            // foreach (LinkItem i in LinkFinder.Find(s))
            // {
            //     Console.WriteLine(i.Text);
            //     Console.WriteLine(i.Href);
            // }
            string url = "https://stagemarkt.nl/Zoeken/Home/Resultaten?t=gamedeveloper&s=&z=&l=Nederland&b=True&c=&lw=&n=&rg=&i=&pg=82&srt=alfabet&e=False&ToonOpKaart=False&ViewType=Lijst&SeedValue=73&LeerbedrijfId=0&p=";
            //string url = "https://stagemarkt.nl/Zoeken/Home/Resultaten?t=gamedeveloper&s=&z=&l=Nederland&b=True&c=&lw=BOL&n=&rg=&i=&pg=0&srt=alfabet&e=False&ToonOpKaart=False&ViewType=Lijst&SeedValue=65&LeerbedrijfId=0&p=";
            List<string> linksWithKeyWords = new List<string>();
            List<string> namesWithKeyWords = new List<string>();
            List<string> keywords = new List<string>() { "unity", "game", "unreal", "gamedeveloper", " vr ", " ar ", "c#", ".net" };


            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);

        Start:

            int totalOnPage = driver.FindElements(By.ClassName("list__item__title")).Count;
            Console.WriteLine(totalOnPage + " links found!");
            string currentURL = driver.Url;

            //For each link in the main page
            for (int i = 0; i < totalOnPage; i++)
            {
                while (driver.Url != currentURL)
                {
                    driver.Navigate().GoToUrl(currentURL);
                    Thread.Sleep(10);
                }
                //find all the links on the result page
                var links = driver.FindElements(By.ClassName("list__item__title"));
                if (i > links.Count - 1)
                {
                    Console.WriteLine("Somehow exceeded the amount of links on the page??");
                    break;
                }
                //scroll to element
                Actions actions = new Actions(driver);
                actions.MoveToElement(links[i]);
                actions.Perform();
                //scroll down 100 pixels to avoid annoying popup
                var js = String.Format("window.scrollTo({0}, {1})", 0, links[i].Location.Y - 100);
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript(js);

                //actually click link
                links[i].Click();
                //var descriptionBoxes = driver.FindElements(By.CssSelector(".row.table-row.pot"));
                var descriptionBoxes = driver.FindElements(By.TagName("a"));
                foreach (var descriptionBox in descriptionBoxes)
                {
                    if (descriptionBox.Text.Contains("Software developer"))
                    {
                        //scroll to element
                        Actions actions2 = new Actions(driver);
                        actions2.MoveToElement(descriptionBox);
                        actions2.Perform();
                        //scroll down 100 pixels to avoid annoying popup
                        var js2 = String.Format("window.scrollTo({0}, {1})", 0, descriptionBox.Location.Y - 100);
                        IJavaScriptExecutor jse2 = (IJavaScriptExecutor)driver;
                        jse2.ExecuteScript(js2);

                        //Open description
                        descriptionBox.Click();
                        var jobs = driver.FindElements(By.ClassName("jobs"));
                        //Search all the open jobs for the keywords
                        foreach (var job in jobs)
                        {
                            foreach (string keyword in keywords)
                            {
                                if (job.Text.ToLower().Contains(keyword))
                                {
                                    Console.WriteLine("FOUND KEYWORD HYPE");
                                    //open file
                                    StreamWriter file = (keyword == "c#" || keyword == ".net") ? File.AppendText("DotnetResults.txt") : File.AppendText("GameResults.txt");
                                    //Naam
                                    namesWithKeyWords.Add(driver.FindElement(By.Id("leerbedrijfNaam")).Text);
                                    file.WriteLine(namesWithKeyWords[namesWithKeyWords.Count - 1]);
                                    //Adres
                                    file.WriteLine(driver.FindElement(By.ClassName("adres")).Text);
                                    //Beschrijving
                                    var children = job.FindElement(By.XPath(".//*")).FindElements(By.XPath(".//*"));
                                    int index = 0;
                                    foreach (var child in children)
                                    {
                                        if (child.Text.Contains("Wat ga je doen?"))
                                        {
                                            file.WriteLine(children[index + 2].Text);
                                            break;
                                            //file.WriteLine(child.FindElements(By.XPath(".//*")).Last().Text);
                                            //file.WriteLine(child.FindElement(By.CssSelector("col-xs-12.col-sm-8.small")).Text);
                                        }
                                        index++;
                                    }
                                    //URL
                                    linksWithKeyWords.Add(driver.Url);
                                    file.WriteLine(linksWithKeyWords[linksWithKeyWords.Count - 1]);
                                    //Padding
                                    file.WriteLine("%-----------------------------------------------------------------------------------------------------------");
                                    //Close file
                                    file.Close();
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
                Console.WriteLine("Checked: " + driver.FindElement(By.Id("leerbedrijfNaam"))?.Text);
                //navigate back to results page
                driver.Navigate().Back();
                Thread.Sleep(10);
            }
            var nextLinks = driver.FindElements(By.ClassName("page-link"));
            foreach (var link in nextLinks)
            {
                if (link.Text.Contains("volgende"))
                {
                    //scroll to element
                    Actions actions2 = new Actions(driver);
                    actions2.MoveToElement(link);
                    actions2.Perform();
                    //scroll down 100 pixels to avoid annoying popup
                    var js = String.Format("window.scrollTo({0}, {1})", 0, link.Location.Y - 100);
                    IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                    jse.ExecuteScript(js);

                    link.Click();
                    goto Start;
                }
            }

            foreach (string result in linksWithKeyWords)
            {
                Console.WriteLine(result);
            }

            driver.Quit();
        }
    }
}
