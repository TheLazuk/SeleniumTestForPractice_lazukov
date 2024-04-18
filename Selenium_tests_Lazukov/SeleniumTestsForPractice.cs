using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Selenium_tests_Lazukov;

public class SeleniumTestsForPractice
{
    public ChromeDriver driver;
    
    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        Authorize();
    }

    [Test]
    public void AuthorizationTest()
    {
        // Проверить загрузку заголовка "Новости" на главной странице сайта
        var titlePageElement = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        var titleInBrowser = driver.Title;
        Assert.Multiple(() =>
        {
            Assert.That(titlePageElement.Text, Is.EqualTo("Новости"));
            Assert.That(titleInBrowser, Is.EqualTo("Новости"));
        });
    }

    [Test]
    public void NavigationTest()
    {
        // 1. Кликнуть на кнопку "бокового меню"
        driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']")).Click();
        // 2. Кликнуть на "Сообщества"
        driver.FindElements(By.CssSelector("[data-tid='Community']")).First(element => element.Displayed).Click();
        // 3. Проверить корректность названия заголовка на странице "Сообщества"
        driver.FindElement(By.CssSelector("[data-tid='Title']")).Text.Should().Be("Сообщества");
    }

    [Test]
    public void ProfileSearchingTest()
    {
        // 1. Ввести корректное ФИО сотрудника в поисковую строку
        driver.FindElement(By.CssSelector("[data-tid='SearchBar']")).Click();
        driver.FindElement(By.CssSelector("[placeholder='Поиск сотрудника, подразделения, сообщества, мероприятия']"))
            .SendKeys("Дмитрий Лазуков");
        // 2. Кликнуть по второй всплывающей подсказке
        driver.FindElements(By.CssSelector("[data-tid='ComboBoxMenu__item']"))[1].Click();
        // 3. Проверить на соответствие URL и ФИО найденного сотрудника 
        var profileUrl = driver.Url;
        var profileName = driver.FindElement(By.CssSelector("[data-tid='EmployeeName']"));
        Assert.Multiple(() =>
        {	
            Assert.That(profileUrl == 
                        "https://staff-testing.testkontur.ru/profile/d545b3e9-027d-43d4-9887-caffa112a9f0",
                "открылся другой профиль");
            Assert.That(profileName.Text, Is.EqualTo("Дмитрий Лазуков"));
        });
    }
   
    [Test]
    public void CommunityFilterTabTest()
    {
        // 1. Перейти на страницу "Списка сообществ"
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");
        // 2. Дождаться появления "Ленты сообществ"
        new WebDriverWait(driver, TimeSpan.FromSeconds(3))
            .Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Feed']")));
        // 3. Кликнуть по в кладке "Я участник"
        driver.FindElements(By.CssSelector("[data-tid='Item']"))[1].Click();
        // 4. Проверить URL на соответсвие переходу (href=/communities?activeTab=isMember)
        Assert.That(driver.Url == "https://staff-testing.testkontur.ru/communities?activeTab=isMember",
            "url вкладки: " + driver.Url + ", а должен быть href='/communities?activeTab=isMember'");
    }

    [Test]
    public void ThanksInfoTest()
    {
        // 1. Перейти на страницу чужого профиля
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/profile/4394e44b-ec78-401d-ae08-ee0e65cf821c");
        // 2. Кликнуть по кнопке "Поблагодарить"
        driver.FindElement(By.CssSelector("[data-tid='ThankButton']")).Click();
        // 3. Заполнить поле ввода
        driver.FindElement
                (By.CssSelector("[placeholder='Укажите, за что вы благодарите сотрудника']")).SendKeys("тест");
        // 4. Кликнуть по кнопке "Поблагодарить"
        driver.FindElement(By.CssSelector("[data-tid='SaveButton']")).Click();
        // 5. Кликнуть по кнопке просмотра "Списка благодарностей"
        driver.FindElement(By.CssSelector("[class='sc-juXuNZ sc-bFSbwS kLoedI bAKETB']")).Click();
        // 6. Проверить корректность отображения "ФИО поблагодарившего" и "текста сообщения" 
        var author = driver.FindElement(By.CssSelector("[data-tid='Author']"));
        var thanksText = driver.FindElement(By.CssSelector("[data-tid='Comment']"));
        Assert.Multiple(() =>
        {	
            Assert.That(author.Text, Is.EqualTo("Дмитрий Лазуков"));
            Assert.That(thanksText.Text, Is.EqualTo("тест"));
        });
    }
    
        public void Authorize()
    {
        // 1. Перейти на страницу "Авторизации"
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        // 2. Заполнить поле "Логин" корректными данными
        driver.FindElement(By.Id("Username")).SendKeys("lazukov87@yandex.ru");
        // 3. Заполнить поле "Пароль" корректными данными
        driver.FindElement(By.Name("Password")).SendKeys("w5pYTob9nyxm4RQV*Kh#");
        // 4. Кликнуть по кнопке "Войти"
        driver.FindElement(By.Name("button")).Click();
        // 5. Дождаться перехода на URL главной страницы сайта
        new WebDriverWait(driver, TimeSpan.FromSeconds(3))
            .Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news"));
    }     
      
    [TearDown]
    public void TearDown()
    {
        driver.Close();
        driver.Quit();
    }
}