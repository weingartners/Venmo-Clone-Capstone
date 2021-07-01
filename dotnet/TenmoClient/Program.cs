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
                            Console.WriteLine(balance);
                        }
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message); 
                    }
                }
                else if (menuSelection == 2)
                {
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
                    int selection = int.Parse(Console.ReadLine());
                    if (selection == 0)
                    {
                        menuSelection = -1;
                    }
                    while (!transferIds.Contains(selection))
                    {
                        Console.Write("Error: Please select a valid Transfer ID: ");
                        selection = int.Parse(Console.ReadLine());
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

                }
                else if (menuSelection == 4)
                {
                    Console.WriteLine("Who would you like to send TE bucks to?");
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
                    Console.Write("Please select a number: ");
                    int userSelection = int.Parse(Console.ReadLine());
                    
                    while (!userIds.Contains(userSelection))
                    {
                        Console.Write("Error: Please select a valid User ID: ");
                        userSelection = int.Parse(Console.ReadLine());
                    }
                    Console.Write("Please enter a valid TE bucks amount: ");
                    decimal amountToSend = decimal.Parse(Console.ReadLine());
                    while (amountToSend <= 0 || amountToSend > api.GetBalances(UserService.GetUserId())[0])
                    {
                        if (amountToSend <= 0)
                        {
                            Console.Write("Error: Please enter a non-negative amount: ");
                            amountToSend = decimal.Parse(Console.ReadLine());
                        }
                        else
                        {
                            Console.Write($"Error: Insufficient funds ({api.GetBalances(UserService.GetUserId())[0].ToString("C2")}), please enter a valid amount: ");
                            amountToSend = decimal.Parse(Console.ReadLine());
                        }
                    }

                    api.TransferMoney(UserService.GetUserId(), amountToSend, userSelection, 2, 2);
                    Console.WriteLine($"Your balance is: {api.GetBalances(UserService.GetUserId())[0].ToString("C2")}");

                }
                else if (menuSelection == 5)
                {
                    
                    
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
