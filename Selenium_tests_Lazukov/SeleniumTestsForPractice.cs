using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Selenium_tests_Lazukov;

public class SeleniumTestsForPractice
{
    [Test]
    public void Authorization()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        
        // - зайти в Chrome (с помощью вебдрайвера)
        var driver = new ChromeDriver(options);
        
        // - перейти по url https://staff-testing.testkontur.ru
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        Thread.Sleep(5000);
        
        // - ввести логин и пароль
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("lazukov87@yandex.ru");

        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("w5pYTob9nyxm4RQV*Kh#");
        
        Thread.Sleep(3000);
        
        // - нажать на кнопку "Войти"
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();
        
        Thread.Sleep(3000);

        // - проверяем, что мы находимся на нужной странице
        var currentUrl = driver.Url;
        Assert.That(currentUrl == "https://staff-testing.testkontur.ru/news");
        
        // закрываем браузер и убиваем процесс драйвера
        driver.Quit();
    }
}