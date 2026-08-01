using ABI.System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {

        SeleniumScripts instance = new SeleniumScripts(); // SeleniumScripts Class 동적할당

        instance.options.AddArgument("--log-level=3");                 // 에러 로그 최소화 (3: FATAL만 표시)
        instance.options.AddArgument("--disable-logging");             // 로깅 기능 비활성화
        instance.options.AddArgument("--ignore-certificate-errors");   // SSL handshake 에러 무시
        instance.options.AddArgument("--allow-running-insecure-content");
        instance.options.AddArgument("--disable-features=GCM");

        instance.service.HideCommandPromptWindow = false;


        instance.Login(); // Login() 메서드 실행
        bool isRun = true; // 반복문 조건 변수
        
        

        while(isRun) // 조건에 의한 반복
        {
            System.Console.WriteLine("티켓 번호를 입력하세요 : ");
            string ticketNumber = Console.ReadLine(); // 티켓 번호를 입력받음

            instance.TicketList(ticketNumber);  // 입력받은 ticketNumber를 TicketList()에 매개변수로 넣어주고 TicketList를 실행
            instance.OpenURL(); // TicketList로 정리된 TicketNumber를 바탕으로 FLT Ticket Open 이후 내부 로직에 따라 해당 티켓의 CI정보 출력


            // 정보를 출력한 이후 옵션 선택
            System.Console.WriteLine();
            System.Console.WriteLine("정보를 가져왔습니다."); 
            System.Console.WriteLine("다음 행동을 선택하세요");
            System.Console.WriteLine("1. 티켓 조회");
            System.Console.WriteLine("2. 종료");

            System.Console.Write(" > ");
            string input = System.Console.ReadLine();  // 옵션을 입력받음

            switch (input)  // 입력받은 옵션에 따라 Loop를 빠져나갈지 다른 티켓을 조회할지를 선택
            {
                case "1":   // Ticket 조회
                    instance.isVendorTextFirst = true;
                    instance.dataManager.ListCleaner();
                    System.Console.Clear();
                    break;
                case "2": // Script 종료
                    isRun = false;
                    instance.driver.Quit();
                    instance.driver.Dispose();
                    System.Environment.Exit(0);
                    break;
                case "3":
                    
                    break;

            }
        }

    }
        


}

