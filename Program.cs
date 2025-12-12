using System;
using System.Collections.Generic;

public class Product
{
    public string Name { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }

    public Product(string name, int price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}

public class VendingMachine
{
    private List<Product> products;
    private Dictionary<int, int> coins;
    private int currentBalance;
    private int initialCoinBalance;

    public VendingMachine()
    {
        products = new List<Product>();
        coins = new Dictionary<int, int>();
        InitializeCoins();
        currentBalance = 0;
        initialCoinBalance = CalculateTotalCoins();
    }

    private void InitializeCoins()
    {
        int[] denominations = { 1, 2, 5, 10, 20, 50, 100 };
        foreach (int denom in denominations)
        {
            coins[denom] = 10;
        }
    }

    private int CalculateTotalCoins()
    {
        int total = 0;
        foreach (var coin in coins)
        {
            total += coin.Key * coin.Value;
        }
        return total;
    }

    public void DisplayProducts()
    {
        Console.WriteLine("\n=== Доступные товары ===");

        if (products.Count == 0)
        {
            Console.WriteLine("Товаров нет");
            return;
        }

        for (int i = 0; i < products.Count; i++)
        {
            Product product = products[i];
            string status = "НЕТ В НАЛИЧИИ";
            if (product.Quantity > 0)
            {
                status = product.Quantity + " шт.";
            }
            Console.WriteLine($"{i + 1}. {product.Name} - {product.Price} руб. ({status})");
        }
    }

    public void InsertCoin(int denomination)
    {
        if (coins.ContainsKey(denomination))
        {
            currentBalance += denomination;
            coins[denomination]++;
            Console.WriteLine($"Внесено {denomination} руб. Баланс: {currentBalance} руб.");
        }
        else
        {
            Console.WriteLine("Неподдерживаемый номинал монеты");
        }
    }

    public int GetCurrentBalance()
    {
        return currentBalance;
    }

    public void PurchaseProduct(int productIndex)
    {
        if (productIndex < 0 || productIndex >= products.Count)
        {
            Console.WriteLine("Неверный выбор товара");
            return;
        }

        Product product = products[productIndex];

        if (product.Quantity <= 0)
        {
            Console.WriteLine("Товар закончился");
            return;
        }

        if (currentBalance < product.Price)
        {
            Console.WriteLine($"Не хватает денег. Нужно: {product.Price} руб., у вас: {currentBalance} руб.");
            return;
        }

        product.Quantity--;
        int change = currentBalance - product.Price;

        Console.WriteLine($"Вы купили {product.Name}!");

        if (change > 0)
        {
            Console.WriteLine($"Сдача: {change} руб.");
            GiveChange(change);
        }

        currentBalance = 0;
    }

    private void GiveChange(int amount)
    {
        if (amount <= 0) return;

        Console.Write("Выдана сдача: ");
        int remaining = amount;

        foreach (int coin in GetSortedCoins())
        {
            while (remaining >= coin && coins[coin] > 0)
            {
                remaining -= coin;
                coins[coin]--;
                Console.Write($"{coin} ");
            }
        }

        Console.WriteLine();

        if (remaining > 0)
        {
            Console.WriteLine($"Не смогли выдать {remaining} руб. сдачи");
        }
    }

    private List<int> GetSortedCoins()
    {
        List<int> coinList = new List<int>();
        foreach (int coin in coins.Keys)
        {
            coinList.Add(coin);
        }
        coinList.Sort();
        coinList.Reverse();
        return coinList;
    }

    public void ReturnMoney()
    {
        if (currentBalance > 0)
        {
            Console.WriteLine($"Возвращаем {currentBalance} руб.");
            GiveChange(currentBalance);
            currentBalance = 0;
        }
        else
        {
            Console.WriteLine("Нет денег для возврата");
        }
    }

    public bool AddProduct(string name, int price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Название товара не может быть пустым");
            return false;
        }

        if (price <= 0)
        {
            Console.WriteLine("Цена должна быть больше 0");
            return false;
        }

        if (quantity <= 0)
        {
            Console.WriteLine("Количество должно быть больше 0");
            return false;
        }

        products.Add(new Product(name, price, quantity));
        Console.WriteLine($"Товар '{name}' добавлен");
        return true;
    }

    public void DisplayTotalMoney()
    {
        int total = CalculateTotalCoins();
        Console.WriteLine($"Общая сумма в автомате: {total} руб.");
        Console.WriteLine($"Прибыль: {total - initialCoinBalance} руб.");
    }

    public void CollectMoney()
    {
        int currentTotal = CalculateTotalCoins();
        int profit = currentTotal - initialCoinBalance;

        if (profit > 0)
        {
            Console.WriteLine($"Собрано прибыли: {profit} руб.");
            coins.Clear();
            InitializeCoins();
            Console.WriteLine("Деньги собраны");
        }
        else
        {
            Console.WriteLine($"Прибыль отсутствует");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        VendingMachine vm = new VendingMachine();
        bool isRunning = true;

        Console.WriteLine("Добро пожаловать в вендинговый автомат!");

        while (isRunning)
        {
            Console.WriteLine($"\nВаш баланс: {vm.GetCurrentBalance()} руб.");
            Console.WriteLine("1. Посмотреть товары");
            Console.WriteLine("2. Внести монеты");
            Console.WriteLine("3. Купить товар");
            Console.WriteLine("4. Вернуть деньги");
            Console.WriteLine("5. Режим администратора");
            Console.WriteLine("6. Выход");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    vm.DisplayProducts();
                    break;
                case "2":
                    InsertCoinsMode(vm);
                    break;
                case "3":
                    BuyProductMenu(vm);
                    break;
                case "4":
                    vm.ReturnMoney();
                    break;
                case "5":
                    AdminMode(vm);
                    break;
                case "6":
                    isRunning = false;
                    Console.WriteLine("До свидания!");
                    break;
                default:
                    Console.WriteLine("Неверная команда");
                    break;
            }
        }
    }

    static void InsertCoinsMode(VendingMachine vm)
    {
        bool inCoinMode = true;

        while (inCoinMode)
        {
            Console.WriteLine($"\nТекущий баланс: {vm.GetCurrentBalance()} руб.");
            Console.WriteLine("1. Внести монету");
            Console.WriteLine("2. Вернуться назад");
            Console.Write("Выберите: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Введите номинал монеты (1, 2, 5, 10, 20, 50, 100): ");
                if (int.TryParse(Console.ReadLine(), out int denomination))
                {
                    vm.InsertCoin(denomination);
                }
                else
                {
                    Console.WriteLine("Неверный номинал");
                }
            }
            else if (choice == "2")
            {
                inCoinMode = false;
            }
            else
            {
                Console.WriteLine("Неверная команда");
            }
        }
    }

    static void BuyProductMenu(VendingMachine vm)
    {
        vm.DisplayProducts();
        Console.Write("Введите номер товара: ");

        if (int.TryParse(Console.ReadLine(), out int productNumber))
        {
            vm.PurchaseProduct(productNumber - 1);
        }
        else
        {
            Console.WriteLine("Неверный номер товара");
        }
    }

    static void AdminMode(VendingMachine vm)
    {
        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();

        if (password != "admin")
        {
            Console.WriteLine("Неверный пароль");
            return;
        }

        bool inAdminMode = true;

        while (inAdminMode)
        {
            Console.WriteLine("\nРежим администратора");
            Console.WriteLine("1. Добавить товар");
            Console.WriteLine("2. Показать общую сумму");
            Console.WriteLine("3. Собрать деньги");
            Console.WriteLine("4. Назад");
            Console.Write("Выберите: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                AddProductMenu(vm);
            }
            else if (choice == "2")
            {
                vm.DisplayTotalMoney();
            }
            else if (choice == "3")
            {
                vm.CollectMoney();
            }
            else if (choice == "4")
            {
                inAdminMode = false;
            }
            else
            {
                Console.WriteLine("Неверная команда");
            }
        }
    }

    static void AddProductMenu(VendingMachine vm)
    {
        Console.Write("Введите название товара: ");
        string name = Console.ReadLine();

        Console.Write("Введите цену: ");
        string priceInput = Console.ReadLine();

        Console.Write("Введите количество: ");
        string quantityInput = Console.ReadLine();

        if (!int.TryParse(priceInput, out int price))
        {
            Console.WriteLine("Цена должна быть числом!");
            return;
        }

        if (!int.TryParse(quantityInput, out int quantity))
        {
            Console.WriteLine("Количество должно быть числом!");
            return;
        }

        bool success = vm.AddProduct(name, price, quantity);
        if (!success)
        {
            Console.WriteLine("Не удалось добавить товар");
        }
    }
}