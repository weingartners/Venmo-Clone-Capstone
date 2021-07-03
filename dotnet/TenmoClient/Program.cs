using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly ApiService api = new ApiService();
        
        static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        ApiUser user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your account balances");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    try
                    {
                        List<decimal> balances = api.GetBalances(UserService.GetUserId());
                        foreach (decimal balance in balances)
                        {
                            Console.Write("\nYour current balance is: ");
                            Console.WriteLine(balance.ToString("C2"));
                        }
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message); 
                    }
                }
                else if (menuSelection == 2)
                {
                    int selection = -1;
                    List<Transfer> transfers = api.GetTransfersById(UserService.GetUserId());
                    List<int> transferIds = new List<int>();
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine("Transfers");
                    Console.WriteLine("ID         From/To          Amount");
                    Console.WriteLine("----------------------------------");
                    foreach (Transfer transfer in transfers)
                    {
                        if (transfer.ReceivingUserId == UserService.GetUserId() )
                        {
                            Console.WriteLine($"{transfer.TransferId}      From: {transfer.FromUserName}  Amount: {transfer.TransferAmount.ToString("C2")}");
                        }
                        else
                        {
                            Console.WriteLine($"{transfer.TransferId}      To: {transfer.ToUserName}   Amount: {transfer.TransferAmount.ToString("C2")}");
                        }
                        transferIds.Add(transfer.TransferId);
                    }
                    Console.WriteLine("----------------------------------");
                    Console.Write("Please enter transfer ID to view details (0 to cancel): ");
                    bool parseSelection = consoleService.TryParseInt32(Console.ReadLine(), ref selection);
                    while ((!transferIds.Contains(selection) || parseSelection == false) && selection != 0)
                    {
                        Console.Write("Please enter a valid Transfer ID: ");
                        parseSelection = consoleService.TryParseInt32(Console.ReadLine(), ref selection);
                    }
                    
                    if (selection == 0)
                    {
                        MenuSelection();
                    }
                    
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine("Transfer Details");
                    Console.WriteLine("----------------------------------");
                    
                    foreach (Transfer transfer in transfers)
                    {
                        if (transfer.TransferId == selection)
                        {
                            Console.WriteLine($"Id: {selection}");
                            Console.WriteLine($"From: {transfer.FromUserName}");
                            Console.WriteLine($"To: {transfer.ToUserName}");
                            if (transfer.TypeId == 1)
                            {
                                Console.WriteLine("Type: Request");
                            }
                            else
                            {
                                Console.WriteLine("Type: Send");
                            }
                            if (transfer.StatusId == 1)
                            {
                                Console.WriteLine("Status: Pending");
                            }
                            else if (transfer.StatusId == 2)
                            {
                                Console.WriteLine("Status: Approved");
                            }
                            else
                            {
                                Console.WriteLine("Status: Rejected");
                            }
                            Console.WriteLine($"Amount: {transfer.TransferAmount.ToString("C2")}");
                        }   
                    }
                    
                }
                else if (menuSelection == 3)
                {
                    int idSelection = -1;
                    Transfer transferRequest = new Transfer();

                    List<Transfer> requests = api.GetRequests(UserService.GetUserId());
                    List<int> transferIds = new List<int>();
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine("Requests");
                    Console.WriteLine("ID         To          Amount");
                    Console.WriteLine("----------------------------------");
                    foreach (Transfer request in requests)
                    {
                        Console.WriteLine($"{request.TransferId}      To: {request.ToUserName}   Amount: {request.TransferAmount.ToString("C2")}");
                        transferIds.Add(request.TransferId);
                    }
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine("Please enter transfer ID to approve/reject (0 to cancel): ");
                    bool parseIdSelection  = consoleService.TryParseInt32(Console.ReadLine(), ref idSelection);

                    while ((!transferIds.Contains(idSelection) || parseIdSelection == false) && idSelection != 0)
                    {
                        Console.Write("Error: Please select a valid Transfer ID: ");
                        parseIdSelection = consoleService.TryParseInt32(Console.ReadLine(), ref idSelection);
                    }
                    if (idSelection == 0)
                    {
                        MenuSelection();
                    }
                    foreach (Transfer request in requests)
                    {
                        if (idSelection == request.TransferId)
                        {
                            transferRequest = request;
                        }
                    }
                    Console.WriteLine("1: Approve\n2: Reject\n0: Don't approve or reject\n---------\nPlease choose an option:");
                    bool parseResponseSelection = int.TryParse(Console.ReadLine(), out int responseSelection);

                    while (parseResponseSelection == false)
                    {
                        Console.WriteLine("Please enter a valid option: ");
                        parseResponseSelection = int.TryParse(Console.ReadLine(), out responseSelection);
                    }
                    if (responseSelection == 1)
                    {
                        api.CompleteRequest(transferRequest.SendingUserId, transferRequest.TransferAmount, transferRequest.ReceivingUserId, transferRequest.TypeId, transferRequest.StatusId);
                        api.UpdateStatus(transferRequest.TransferId, transferRequest.SendingUserId, transferRequest.TransferAmount, transferRequest.ReceivingUserId, transferRequest.TypeId, 2);
                    }
                    if (responseSelection == 2)
                    {
                        api.UpdateStatus(transferRequest.TransferId, transferRequest.SendingUserId, transferRequest.TransferAmount, transferRequest.ReceivingUserId, transferRequest.TypeId, 3);
                    }
                    if (responseSelection == 0)
                    {
                        MenuSelection();
                    }


                }
                else if (menuSelection == 4)
                {
                    int userSelection = -1;
                    Console.WriteLine("Who would you like to send TE bucks to?");
                    List<int> userIds = new List<int>();
                    List<ApiUser> users = api.GetUsers();
                    Console.WriteLine("\n-------------------------------------------");
                    Console.WriteLine("Users\nID        Name");
                    Console.WriteLine("-------------------------------------------");
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].UserId != UserService.GetUserId())
                        {
                            Console.WriteLine($"{users[i].UserId}. {users[i].Username}");
                        }
                        userIds.Add(users[i].UserId);
                    }
                    Console.WriteLine("---------\n");
                    Console.Write("Enter ID of user you are sending to (0 to cancel): ");
                    bool canParse = consoleService.TryParseInt32(Console.ReadLine(), ref userSelection);

                    while ((!userIds.Contains(userSelection) || canParse == false) && userSelection != 0)
                    {
                        Console.Write("Error: Please select a valid User ID: ");
                        canParse = consoleService.TryParseInt32(Console.ReadLine(), ref userSelection);
                    }
                    if (userSelection == 0)
                    {
                        MenuSelection();
                    }

                    Console.Write("Please enter a valid TE bucks amount: ");
                    bool parseAmountToSend = decimal.TryParse(Console.ReadLine(), out decimal amountToSend);

                    while (amountToSend <= 0 || amountToSend > api.GetBalances(UserService.GetUserId())[0] || parseAmountToSend == false)
                    {
                        if (amountToSend <= 0)
                        {
                            Console.Write("Error: Please enter a valid amount: ");
                            parseAmountToSend = decimal.TryParse(Console.ReadLine(), out amountToSend);
                        }
                        else 
                        {
                            Console.Write($"Error: Insufficient funds ({api.GetBalances(UserService.GetUserId())[0].ToString("C2")}), please enter a valid amount: ");
                            parseAmountToSend = decimal.TryParse(Console.ReadLine(), out amountToSend);
                        }
                        
                    }

                    api.TransferMoney(UserService.GetUserId(), amountToSend, userSelection, 2, 2);
                    Console.WriteLine($"Your balance is: {api.GetBalances(UserService.GetUserId())[0].ToString("C2")}");

                }
                else if (menuSelection == 5)
                {
                    int userSelection = -1;
                    Console.WriteLine("Who would you like to request TE bucks from?");
                    List<int> userIds = new List<int>();
                    List<ApiUser> users = api.GetUsers();
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].UserId != UserService.GetUserId())
                        {
                            Console.WriteLine($"{users[i].UserId}. {users[i].Username}");
                        }
                        userIds.Add(users[i].UserId);
                    }
                    Console.Write("Enter ID of user you are requesting from (0 to cancel): ");
                    bool canParse = consoleService.TryParseInt32(Console.ReadLine(), ref userSelection);

                    while ((!userIds.Contains(userSelection) || canParse == false) && userSelection != 0)
                    {
                        Console.Write("Error: Please select a valid User ID: ");
                        canParse = consoleService.TryParseInt32(Console.ReadLine(), ref userSelection);
                    }
                    if(userSelection == 0)
                    {
                        MenuSelection();
                    }
                    Console.Write("Please enter a valid TE bucks amount: ");
                    bool parseAmountToSend = decimal.TryParse(Console.ReadLine(), out decimal amountToSend);

                    while (amountToSend <= 0 || parseAmountToSend == false)
                    {
                        Console.Write("Error: Please enter a valid TE bucks amount: ");
                        parseAmountToSend = decimal.TryParse(Console.ReadLine(), out amountToSend);
                    }
                    api.SaveTransfer(userSelection, amountToSend, UserService.GetUserId(), 1, 1);

                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new ApiUser()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
