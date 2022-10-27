using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;

namespace SeleniumAssignment
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //TaupoWeatherSearch(); //task 1
            //TradeMeITJobSearch(); //task 2
            ValidateInternalLinks();
        }

        public static void TaupoWeatherSearch()
        {
            IWebDriver driver = new ChromeDriver(); //creating chrome driver
            driver.Navigate().GoToUrl("https://www.google.co.nz"); //navigates to the website
            driver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/input")).SendKeys("Taupo Weather"); 
            driver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/input")).SendKeys(Keys.Enter);
        }

        public static void TradeMeITJobSearch()
        {
            IWebDriver driver = new ChromeDriver(); //creating chrome driver
            driver.Navigate().GoToUrl("https://www.trademe.co.nz/a/"); //navigates to the website
            driver.FindElement(By.Id("search")).SendKeys("IT Jobs");
            driver.FindElement(By.Id("search")).SendKeys(Keys.Enter);
        }

        public static void ValidateInternalLinks()
        {
            IWebDriver driver = new ChromeDriver(); //creating chrome driver
            driver.Navigate().GoToUrl("http://automationpractice.com/index.php"); //navigates to the website
            IReadOnlyCollection<IWebElement> links = driver.FindElements(By.TagName("a"));

            String homePage = "http://automationpractice.com";

            foreach (IWebElement link in links)
            {
                string url = link.GetAttribute("href"); //gets link
                
                if (url == null)
                {
                    Console.WriteLine("URL is empty");
                    continue;
                }
                else if (url.StartsWith(homePage))
                {
                    url += "random";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); 
                    try //checks if we get a response
                    {
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Console.WriteLine(response.StatusCode + $" - URL: {url}");
                        response.Close();
                    }
                    catch (WebException e) //catch HTML response errors
                    {
                        var errorResponse = (HttpWebResponse)e.Response;
                        Console.WriteLine(errorResponse.StatusCode + $" - URL: {url}");
                        Console.WriteLine(link.Text + " - " + link.Location);
                        errorResponse.Close();
                    }
                }
            } //end of loop
            Console.WriteLine("Link Validation Done");
        }

    }
}
