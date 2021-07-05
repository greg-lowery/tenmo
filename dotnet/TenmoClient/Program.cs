using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly ApiService apiService = new ApiService();
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
                Console.WriteLine("1: View your current balance");
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
                        Account account = apiService.GetAccountInfo();
                        Console.WriteLine($"Your account balance is {account.Balance:C2}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else if (menuSelection == 2)
                {

                }
                else if (menuSelection == 3)
                {

                }
                else if (menuSelection == 4)
                {
                    decimal amtToTransfer;
                    string userInput;
                    List<ReturnUser> userList = new List<ReturnUser>();

                    try
                    {
                        Console.WriteLine("\nList of Avalaible TEnmo Users:\n");
                        userList = apiService.GetAllUsers();
                        for (int i = 0; i < userList.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) Username: {userList[i].Username}     User Id: {userList[i].UserId}\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.WriteLine("Please choose a user to transfer TE Bucks to:");

                    int selectedUser;
                    userInput = Console.ReadLine();
                    while (!int.TryParse(userInput, out selectedUser) || selectedUser <= 0 || selectedUser > userList.Count)
                    {
                        Console.WriteLine("Invalid user selection. Please try again.");
                        userInput = Console.ReadLine();
                    }

                    int destinationUserId = userList[selectedUser - 1].UserId;


                    //Account accountToVerify = new Account();
                    //accountToVerify.AccountId = accountId;
                    //bool transferAccountExists = false;
                    //bool sufficientFundsToTransfer = false;
                    //try
                    //{
                    //    transferAccountExists = apiService.VerifyTransferAccountExists(accountToVerify);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                    //}

                    Console.WriteLine("Please enter the amount you would like to transfer in the format 0.00:");
                    userInput = Console.ReadLine();

                    while (!decimal.TryParse(userInput, out amtToTransfer) || userInput.Substring(userInput.IndexOf('.') + 1).Length != 2)
                    {
                        Console.WriteLine("Invalid transfer amount submitted. Please try again.");
                        userInput = Console.ReadLine();
                    }
                
                    bool sufficientFundsToTransfer = false;

                    try
                    {
                        sufficientFundsToTransfer = apiService.VerifySufficientFunds(amtToTransfer);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    //if (!accountExists)
                    //{
                    //    Console.WriteLine("Account entered does not exist. Please make a new request.");
                    //}
                    if (!sufficientFundsToTransfer)
                    {
                        Console.WriteLine("You do not have sufficient funds to make this transfer. Please make a new request.");
                    }

                    if (sufficientFundsToTransfer)
                    {
                        
                        Transfer newtransfer = new Transfer();
                        ReturnUser transferDestinationUser = new ReturnUser();
                        transferDestinationUser.UserId = destinationUserId;
                        newtransfer.TransferAmount = amtToTransfer;


                        try
                        {
                            Transfer successfulTransfer = apiService.SubmitTransferForProcessing(newtransfer, transferDestinationUser);
                            Console.WriteLine($"Transfer {successfulTransfer.TransferId} of {successfulTransfer.TransferAmount} to {userList[selectedUser - 1].Username} was successful. Thank You!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        
                    }
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
