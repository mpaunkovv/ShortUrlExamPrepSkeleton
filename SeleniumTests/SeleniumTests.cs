using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace SeleniumTests
{
    public class SeleniumTests
    {
        private WebDriver driver;
        private const string baseUrl = "https://shorturl.mpaunkov.repl.co/";
        [SetUp]
        public void OpenWebApp()
        {

            this.driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Url = baseUrl;


        }
        [TearDown]
        public void CloseWebApp()
        {
            driver.Quit();
        }
        [Test]
        public void Test_Table_TopLeftCell()
        {
            var linkShortUrl = driver.FindElement(By.LinkText("Short URLs"));
            linkShortUrl.Click();
            var tableHeaderLeft = driver.FindElement(By.CssSelector("th:nth-child(1)"));
            Assert.That(tableHeaderLeft.Text, Is.EqualTo("Original URL"));

        }
        [Test]
        public void Test_AddValidUrl()
        {
            var urlToAdd = "https://url" + DateTime.Now.Ticks + ".com";
            var linkAddUrl = driver.FindElement(By.LinkText("Add URL"));
            linkAddUrl.Click();

            var inputAddUrl = driver.FindElement(By.Id("url"));
            inputAddUrl.SendKeys(urlToAdd);

            var createButton = driver.FindElement(By.XPath("//button[@type='submit']"));
            createButton.Click();

            Assert.That(driver.PageSource.Contains(urlToAdd), Is.True);

            var tableLastRow = driver.FindElements(By.CssSelector("table > tbody > tr")).Last();
            var tableLastRowFirstCell = tableLastRow.FindElements(By.CssSelector("td")).First();
            Assert.That(tableLastRowFirstCell.Text, Is.EqualTo(urlToAdd));
        }
        [Test]
        public void Test_AddInvalidUrl()
        {

            var linkAddUrl = driver.FindElement(By.LinkText("Add URL"));
            linkAddUrl.Click();

            var inputAddUrl = driver.FindElement(By.Id("url"));
            inputAddUrl.SendKeys("asdas");

            var createButton = driver.FindElement(By.XPath("//button[@type='submit']"));
            createButton.Click();
            var errorMsg = driver.FindElement(By.XPath("//div[@class='err']"));

            Assert.That(errorMsg.Text, Is.EqualTo("Invalid URL!"));

            // Assert.That(driver.PageSource.Contains("Invalid URL!"), Is.True);
        }
        [Test]
        public void Test_GoToNonExistingUrl()
        {
            driver.Navigate().GoToUrl("https://shorturl.nakov.repl.co/go/invalid536524");
            var errorMsg = driver.FindElement(By.XPath("//div[@class='err']"));

            Assert.That(errorMsg.Text, Is.EqualTo("Cannot navigate to given short URL"));
            Assert.That(errorMsg.Displayed, "Error message is not displayed");

        }
        [Test]
        public void Test_VerifyCounterIncrease()
        {
            var linkShortUrl = driver.FindElement(By.LinkText("Short URLs"));
            linkShortUrl.Click();
            var tableFirstRow = driver.FindElements(By.CssSelector("table > tbody > tr")).First();
            var oldCounter = int.Parse(tableFirstRow.FindElements(By.CssSelector("td")).Last().Text);

            var linkToClickCell = tableFirstRow.FindElements(By.CssSelector("td"))[1];
            var linkToClick = linkToClickCell.FindElement(By.TagName("a"));
            linkToClick.Click();

            driver.SwitchTo().Window(driver.WindowHandles[0]);

            driver.Navigate().Refresh();

            tableFirstRow = driver.FindElements(By.CssSelector("table > tbody > tr")).First();

            var newCounter = int.Parse(tableFirstRow.FindElements(By.CssSelector("td")).Last().Text);

            Assert.That(newCounter, Is.EqualTo(oldCounter + 1));
        }
    }
}