using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras;
using System.Threading;

namespace SeleniumAssignment
{
    internal class Program
    {

        static void Main(string[] args)
        {

            //TaupoWeatherSearch(); //task 1
            //TradeMeITJobSearch(); //task 2
            //ValidateInternalLinks(); //task 3
            //CheckCartTotalDeleteOne(); //task 4
            //CheckCartTotalDeleteExpensive(); //task 5
            //OutputTradeMeLinks();   //trask 6
        }

        public static void TaupoWeatherSearch()
        {
            IWebDriver driver = new ChromeDriver(); //creating chrome driver
            driver.Navigate().GoToUrl("https://www.google.co.nz"); //navigates to the website
            driver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/input")).SendKeys("Taupo Weather"); 
            driver.FindElement(By.XPath("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/input")).SendKeys(Keys.Enter);
            driver.Quit();
        }

        public static void TradeMeITJobSearch()
        {
            IWebDriver driver = new ChromeDriver(); //creating chrome driver
            driver.Navigate().GoToUrl("https://www.trademe.co.nz/a/"); //navigates to the website
            driver.FindElement(By.Id("search")).SendKeys("IT Jobs");
            driver.FindElement(By.Id("search")).SendKeys(Keys.Enter);
            driver.Quit();
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
                    //url += "random"; //test if it's not catching WebException
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

        public static void CheckCartTotalDeleteOne()
        {
            IWebDriver driver = new ChromeDriver(); //creating chrome driver

            generateItemsToCart(driver, "homefeatured", 3);
            verifyCheckoutTotal(driver);
            driver.FindElement(By.ClassName("icon-trash")).Click(); //deletes first item
        }

        public static void CheckCartTotalDeleteExpensive()
        {
            IWebDriver driver = new ChromeDriver(); //creating chrome driver

            generateItemsToCart(driver, "homefeatured", 4);
            verifyCheckoutTotal(driver);
            DeleteMostExpensiveItemFromCart(driver);    
        }

        public static void OutputTradeMeLinks()
        {
            IWebDriver driver = new ChromeDriver(); //creating chrome driver
            driver.Navigate().GoToUrl("https://www.trademe.co.nz/a/"); //navigates to the website
            IReadOnlyCollection<IWebElement> links = driver.FindElements(By.TagName("a"));

            foreach (IWebElement link in links)
            {
                string url = link.GetAttribute("href"); //gets link text
                if (url != null)
                {
                    if (url.StartsWith("https://www.trademe.co.nz/") && //internal link
                        (url.Contains("property") || url.Contains("services"))) //contains property or services
                    { Console.WriteLine(url); }
                }
            }
            Console.WriteLine("Link Output Done");
        }

        public static void DeleteMostExpensiveItemFromCart(IWebDriver driver)
        {
            IReadOnlyCollection<IWebElement> checkOutItems = driver.FindElements(By.ClassName("cart_item"));
            decimal mostExpensive = 0;

            foreach (IWebElement item in checkOutItems) //determines highest price
            { 
                decimal price = Decimal.Parse(item.FindElement(By.ClassName("cart_total")).Text.Remove(0, 1));
                if (price > mostExpensive) { mostExpensive = price; }
            }

            foreach (IWebElement item in checkOutItems) // searches and deletes highest price
            { 
                decimal price = Decimal.Parse(item.FindElement(By.ClassName("cart_total")).Text.Remove(0, 1));
                if (price == mostExpensive) { item.FindElement(By.ClassName("icon-trash")).Click(); }
            }
            Console.WriteLine("Deleted: " + mostExpensive.ToString());

        }

        //reusable functions here
        /// <summary>
        /// generates products onto the cart
        /// </summary>
        /// <param name="driver">IWebDriver</param>
        /// <param name="listName">the tab or collection name</param>
        /// <param name="count">number of products you want to add to cart</param>
        public static void generateItemsToCart(IWebDriver driver, string listName, int count)
        {
            
            driver.Navigate().GoToUrl("http://automationpractice.com/index.php"); //navigates to the website
            var rand = new Random();

            //gets homefeatured product count
            IWebElement popularList = driver.FindElement(By.Id("homefeatured"));
            IReadOnlyCollection<IWebElement> products = popularList.FindElements(By.ClassName("available-now")); //fetches products from homefeatured list
            int productCount = products.Count();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);


            for (int i = 0; i < count; i++)
            {
                int product = rand.Next(productCount)+1;
                Console.WriteLine(product.ToString());
                var addToCart = driver.FindElement(By.XPath($"//*[@id=\"homefeatured\"]/li[{product}]/div/div[2]/div[2]/a[1]"));
                addToCart.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement continueBtn = wait.Until(e => e.FindElement(By.ClassName("continue")));
                driver.FindElement(By.ClassName("continue")).Click(); //continues shopping
            }
        }

        /// <summary>
        /// checks the if the actual total of items is equal to the website total
        /// </summary>
        /// <param name="driver">IWebDriver</param>
        public static void verifyCheckoutTotal(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("http://automationpractice.com/index.php?controller=order");

            IReadOnlyCollection<IWebElement> checkOutItems = driver.FindElements(By.ClassName("cart_item"));
            decimal totalPrice = 0;

            foreach (IWebElement item in checkOutItems) //adds all product in cart's total price
            {
                decimal price = Decimal.Parse(item.FindElement(By.ClassName("cart_total")).Text.Remove(0, 1));
                totalPrice += price;
            }

            decimal websiteTotalProductPrice = Decimal.Parse(driver.FindElement(By.Id("total_product")).Text.Remove(0, 1));

            Console.WriteLine("Total price: $" + totalPrice.ToString());
            Console.WriteLine("Website total price: $" + websiteTotalProductPrice.ToString());

            if (totalPrice == websiteTotalProductPrice) //comparing total
            {
                Console.WriteLine("Website is displaying correct Total");
            }
            else
            {
                Console.WriteLine("There's something wrong with the Total");
            }
        }
    }

}
