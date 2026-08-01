using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Input;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V149.Input;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using SeleniumExtras.WaitHelpers;
using System.ComponentModel.Design;
using Windows.Graphics.Printing.OptionDetails;


class SeleniumScripts
{
    public ChromeDriver driver; // Chrome 을 사용하기 위한 ChromeDriver 할당
    public DataManager dataManager = new DataManager(); // data를 관리하기 위한 dataManager 할당

    public ChromeOptions options = new ChromeOptions();
    public ChromeDriverService service = ChromeDriverService.CreateDefaultService();

    private string ID = "";  // ID
    private string PW = "";  // PW

    List<string> ticketList = new List<string>();  // ticket 번호가 들어갈 List 선언
    string resultText = "";  // ticket 결과를 출력해 줄 List 선언

    string serviceFlt = "";
    string serviceHostName = "";
    string serviceException = "";

    private string vendorText = "";       // 내가 조회한 Ticket의 제조사를 저장할 string 선언
    public bool isVendorTextFirst = true; // vendor Text를 출력했는지 안했는지를 알게해줄 bool 변수 선언

    private bool isFirst = true;        // SYSOP 웹사이트 초기 접근 시 무시하고 보내기 Check 여부를 묻기위한 bool 변수 선언

    

    public void OpenURL() // 티켓 URL로 접속하기 위한 메서드
    {
        string url = ""; // 티켓 URL을 담을 url 변수 선언

        for (int i = 0; i < ticketList.Count; i++) // ticketList.Count = 내가 조회해야할 티켓 개수만큼 반복
        {
            url = "URL 주소가 들어갑니다" + (ticketList[i]) + ".html"; // 티켓의 url  (ticketList[i] 는 list에 저장된 FLT Ticket index를 의미)
            driver.Navigate().GoToUrl(url); // 셀레니움으로 chrome URL을 변경 

            JoinCI(); // CI 조회
            BringInfo(ticketList[i]); // 조회된 CI에서의 위치정보, 자산번호, S/N등을 가져옴
            Thread.Sleep(1500);
            ServiceCheck(ticketList[i]); // 해당 FLT티켓에 등록된 서버가 서비스가 구동중인지 여부를 확인함

            dataManager.VendorClassify(resultText, vendorText); // 벤더별 정리

        }

        dataManager.ResultPrint(); // 정리된 결과 출력


    }

    public void Login() // Login 관련
    {
        // ID와 PW를 입력받음
        System.Console.Write("사번을 입력하세요 : ");
        ID = System.Console.ReadLine(); 
        System.Console.Write("비밀번호를 입력하세요 : ");
        PW = System.Console.ReadLine();
        
        driver = new ChromeDriver(service, options); 

        //티켓 URL 접속
        driver.Navigate().GoToUrl("URL 주소가 들어갑니다");

        Thread.Sleep(3000);

        // Login창에서의 ID 입력 TextBox와 PW 입력 TextBox의 XPath값을 입력
        var loginID = driver.FindElement(By.XPath("//*[@id=\"user_id\"]"));
        var loginPW = driver.FindElement(By.XPath("//*[@id=\"user_pw\"]"));

        // 입력한 경로의 TextBox에 ID와 PW를 입력한 후 Enter 버튼을 눌러 로그인
        loginID.SendKeys(ID);
        loginPW.SendKeys(PW);

        loginPW.SendKeys(Keys.Enter);

        Thread.Sleep(1000);

        System.Console.Clear();

    }

    public void TicketList(string inputText) // Ticket 번호를 담을 List에 값을 넣어주기 전 입력받은 Ticket Number 중 공백, 개행, 쉼표 등을 제거 후 무결한 상태로 가공하기 위한 메서드드
    {
        ticketList.Clear();

        string cleanedInput = Regex.Replace(inputText, @"[\s,]+", ""); // Regex.Replace = 주어진 정규 표현식에 매칭되는 부분을 새로운 문자열로 대체 , (@[\s,] = 공백또는 쉼표)

        string tempList = ""; // 임시 문자열을 담을 변수 선언
        int charCount = 0; // 문자 개수를 카운팅 할 변수 선언

        for(int i = 0; i < cleanedInput.Length; i++) // Regex.Replace를 사용하여 가공된 문자열을 해당 문자열의 길이 만큼 반복 (string == char[] 과 같으므로 Length로 길이를 알 수 있음)
        {
            tempList += cleanedInput[i]; //임시 문자열에 문자들을 담음 
            charCount++; // 문자를 담을때마다 Count 1씩 증가

            if(charCount == 17) // 임시 문자열에 17개의 문자가 담겼다면 (FLT티켓은 총 17글자)
            {
                charCount = 0; // Count를 다시 0으로 되돌려서 다음 문자를 카운트 할 준비
                
                ticketList.Add(tempList); // 임시 문자열을 List에 추가
                tempList = ""; // 임시 문자열을 다시 공백으로 되돌려 새로운 문자를 받을 준비를 함
            }

        }
    }

    public void JoinCI() // Ntree의 TicketURL로 접근 후 해당 티켓에 있는 서버의 CI PopUp을 열기위한 메서드
    {
        try // try Catch문은 예외처리 문임 Ticket이 처리대기와 완료된 티켓의 CI PopUp Xpath값이 서로 달라 try { } 구문 안에 있는게 먼저 실행되고 해당 구문에서 오류가 나면 Catch문을 실행시킴
        {
            var assetNumber = driver.FindElement(By.XPath("//*[@id=\"popup_stage\"]/div[1]/div[5]/div[1]/dl/dd/div[3]/div[1]/table/tbody/tr/td[6]/a"));
            assetNumber.Click();
        }
        catch
        {
            var assetNumber = driver.FindElement(By.XPath("//*[@id=\"popup_stage\"]/div[1]/div[5]/div[1]/dl/dd/div/div[1]/table/tbody/tr/td[5]/a"));
            assetNumber.Click();
        }

        Thread.Sleep(1500);


    }

    private void BringInfo(string ticketNum) // CI PopUp 클릭 이후 해당 PopUp에서 정보를 가져옴
    {
        IList<string> windowHandles = new List<string>(driver.WindowHandles); // 새로운 탭이 열렸기 때문에 WindowHandle 열려있는 모든 Chrome 탭의 Handle을 가져옴
        driver.SwitchTo().Window(windowHandles[1]); // 새탭을 컨트롤 하기 위해 전환 (기존의 windowHandle 은 windowHandles[0] 새로운 windowHandle 은 WindowHandles[1]

        //위치정보를 알기위한 XPath값 (상면-열, 랙위치, 홀번호 순으로 쪼개어져 있음)
        var whiteSpaceXPATH = driver.FindElement(By.XPath("//*[@id=\"popup_stage\"]/div/div[1]/div/table/tbody/tr[6]/td/div/div/span[3]"));
        var rackXPATH = driver.FindElement(By.XPath("//*[@id=\"popup_stage\"]/div/div[1]/div/table/tbody/tr[6]/td/div/div/span[4]"));
        var holeXPATH = driver.FindElement(By.XPath("//*[@id=\"popup_stage\"]/div/div[1]/div/table/tbody/tr[6]/td/div/div/span[5]"));

        // 어떤 Vendor사의 서버인지 알기위한 XPath 값을 가져옴
        var vendor = driver.FindElement(By.XPath("//*[@id=\"popup_stage\"]/div/div[1]/div/table/tbody/tr[2]/td[1]/span"));
        // 자산번호 
        var assetNUmber = driver.FindElement(By.XPath("//*[@id=\"popup_stage\"]/div/div[1]/div/table/tbody/tr[1]/td[2]/span"));
        // S/N 번호
        var serialNumber = driver.FindElement(By.XPath("//*[@id=\"popup_stage\"]/div/div[1]/div/table/tbody/tr[2]/td[3]/span"));
        // 해당 서버의 HostName
        var hostName = driver.FindElement(By.XPath("//*[@id=\"popup_stage\"]/div/div[2]/div[2]/div[1]/div/table/tbody/tr[1]/td[1]/div/span"));

        // 쪼개어서 가져온 위치정보를 하나의 문자열로 만듬
        string locationInfo = (whiteSpaceXPATH.Text + rackXPATH.Text + holeXPATH.Text);

        // 전역변수인 VendorText에 어떤 Vendor의 서버인지 대입
        vendorText = vendor.Text;

        // 가져온 정보를 저장할 문자 형식에 맞춰 LIST에 저장
        resultText =  ticketNum + " / " + hostName.Text + "\n" + locationInfo + "\n" + " > " + assetNUmber.Text + " / " + serialNumber.Text+ "\n";

        serviceFlt = ticketNum;
        serviceHostName = hostName.Text;
        serviceException = $"URL 주소가 들어갑니다";

        driver.Close(); // 열려있는 PopUp을 닫음

    }

    private void ServiceCheck(string fltNum) // SYSOP에서 서비스 체크
    {
        IList<string> windownHandles = new List<string>(driver.WindowHandles); //PopUp UI가 닫혀서 기존 탭으로 driver를 잡아줌
        driver.SwitchTo().Window(windownHandles[0]); // 하나의 탭만 열려있으니까 windownHandles[0]

        driver.Navigate().GoToUrl("URL 주소가 들어갑니다"); // 열려있는 탭에서 서비스 조회로 URL 이동
        
        Thread.Sleep(1000);

        if(isFirst == true) // 해당 Browser에서 서비스 조회 처음 방문한 것이라면 '무시하고 보내기'를 클릭해줘야함 두번째 방문시에는 IF문 안의 실행문이 실행되지 않도록 예외처리를 위한 조건
        {
            WebDriverWait sysopDelay = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement sysopDelayElement = sysopDelay.Until(d => d.FindElement(By.Id("proceed-button")));
            var pass = driver.FindElement(By.Id("proceed-button"));
            Thread.Sleep(1000);
            pass.Click();
            isFirst = false;
        }

        Thread.Sleep(3000);

        WebDriverWait delay = new WebDriverWait(driver, TimeSpan.FromMinutes(10));
        //IWebElement delayElement = delay.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//*[@id=\"crq\"]")));
        IWebElement delayElement = delay.Until(d => d.FindElement(By.XPath("//*[@id=\"crq\"]")));
        
        var fltSearchBox = driver.FindElement(By.XPath("//*[@id=\"crq\"]")); // Verify - Halt에서의 FLT Num 을 입력하기 위한 TextBox의 XPath값을 입력
        fltSearchBox.SendKeys(fltNum); // 해당 TextBox에 FLT Number 입력
        fltSearchBox.SendKeys(Keys.Enter); // Search 시작

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30)); // 특정 조건을 만족할때까지 대기 시간을 설정 (최대 100초까지 기다림)

        IWebElement waitElement = wait.Until(driver =>  // 해당 조건을 설정하기 위한 람다식으로 delegate 함수를 받음 (wait.Util()은 대리자가 반환하는 값이 null이 아닐 때까지 대기)
        {
            var element = driver.FindElement(By.XPath("//*[@id=\"resultLog\"]")); // element 요소에 SYSOP 서비스 체크 결과가 나오는 TextBox의 XPath값을 넣어줌

            return !string.IsNullOrWhiteSpace(element.Text) ? element : null; // 해당 TextBox가 빈칸이 아닐때 element 요소를 반환
        });

        if(waitElement.Text.Contains("서비스 제외가 필요합니다")) // 반환된 element 요소의 Text에서 "서비스 제외가 필요합니다" 라는 문자열이 들어있는지를 찾음
        {
            resultText += " > 서비스 제외 필요";
        }
        else if (waitElement.Text.Contains("already down")) // 반환된 element 요소의 Text에서 "already down" 라는 문자열이 들어있는지를 찾음
        {
           resultText += " > Already Down";
        }
        else if (waitElement.Text.Contains("PRCS[OK]|BOND0[OK]|SSD[OK]")) // 반환된 element의 요소의 Text에서 "PRCS[OK]|BOND0[OK]|SSD[OK]" 라는 문자열이 들어있는지를 찾음
        {
            resultText += " > PRCS[OK]|BOND0[OK]|SSD[OK] 셧다운 가능"; 
        }
        else if (waitElement.Text.Contains("PRCS[OK]|BOND0[OK]|SSD[]")) // SSD가 안보이는 경우
        {
            resultText += " > PRCS[OK]|BOND0[OK]|SSD[]";
        }
        else if (waitElement.Text == null)
        {
            resultText += " > Time Out Error 수동 서비스 체크 필요";
        }
        else
        {
            resultText += " > 담당자 확인 필요";
        }

    }
}