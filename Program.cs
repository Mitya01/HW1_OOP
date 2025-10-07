using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Представляет товар в вендинговом автомате
/// </summary>
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

/// <summary>
/// Управляет операциями вендингового автомата
/// </summary>
public class VendingMachine
{
    private List<Product> products;
    private Dictionary<int, int> coins;
    private int currentBalance;
    private int initialCoinBalance; // Фиксированная сумма для выдачи сдачи

    public VendingMachine()
    {
        products = new List<Product>();
        coins = new Dictionary<int, int>();
        InitializeCoins();
        currentBalance = 0;

        // Сохраняем начальную сумму, которая всегда должна оставаться для сдачи
        initialCoinBalance = CalculateTotalCoins();

        // Добавляем начальные товары для примера, можно убрать
        AddProduct("Вода", 50, 10);
        AddProduct("Шоколадка", 80, 9);
        AddProduct("Чипсы", 150, 8);
        AddProduct("Лимонад", 100, 1);
    }

    /// <summary>
    /// Инициализирует начальные монеты
    /// </summary>
    private void InitializeCoins()
    {
        int[] denominations = { 1, 2, 5, 10, 20, 50, 100 };
        foreach (int denom in denominations)
        {
            coins[denom] = 10; // Начальное количество монет каждого номинала, для выдачи сдачи
        }
    }

    /// <summary>
    /// Вычисляет общую сумму монет
    /// </summary>
    private int CalculateTotalCoins()
    {
        return coins.Sum(coin => coin.Key * coin.Value);
    }

    /// <summary>
    /// Показывает доступные товары
    /// </summary>
    public void DisplayProducts()
    {
        Console.WriteLine("\n=== Доступные товары ===");
        Console.WriteLine("-----------------------");

        if (products.Count == 0)
        {
            Console.WriteLine("Товаров нет");
            return;
        }

        for (int i = 0; i < products.Count; i++)
        {
            Product product = products[i];
            string status = product.Quantity > 0 ? $"{product.Quantity} шт." : "НЕТ В НАЛИЧИИ";
            Console.WriteLine($"{i + 1}. {product.Name} - {product.Price} руб. ({status})");
        }
    }

    /// <summary>
    /// Вносит монеты в автомат
    /// </summary>
    public void InsertCoin(int denomination)
    {
        if (coins.ContainsKey(denomination))
        {
            currentBalance += denomination;
            coins[denomination]++;
            Console.WriteLine($"[OK] Внесено {denomination} руб. Баланс: {currentBalance} руб.");
        }
        else
        {
            Console.WriteLine("[ОШИБКА] Неподдерживаемый номинал монеты");
        }
    }

    /// <summary>
    /// Возвращает текущий баланс
    /// </summary>
    public int GetCurrentBalance()
    {
        return currentBalance;
    }

    /// <summary>
    /// Покупает выбранный товар
    /// </summary>
    public void PurchaseProduct(int productIndex)
    {
        if (productIndex < 0 || productIndex >= products.Count)
        {
            Console.WriteLine("[ОШИБКА] Неверный выбор товара");
            return;
        }

        Product product = products[productIndex];

        if (product.Quantity <= 0)
        {
            Console.WriteLine("[ОШИБКА] Товар закончился");
            return;
        }

        if (currentBalance < product.Price)
        {
            Console.WriteLine($"[ОШИБКА] Не хватает денег. Нужно: {product.Price} руб., у вас: {currentBalance} руб.");
            return;
        }

        // Выдаем товар
        product.Quantity--;
        int change = currentBalance - product.Price;

        Console.WriteLine($"[УСПЕХ] Вы купили {product.Name}!");

        if (change > 0)
        {
            Console.WriteLine($"[СДАЧА] Сдача: {change} руб.");
            GiveChange(change);
        }

        currentBalance = 0;
    }

    /// <summary>
    /// Выдает сдачу
    /// </summary>
    private void GiveChange(int amount)
    {
        if (amount <= 0) return;

        Console.Write("Выдана сдача: ");
        int remaining = amount;

        // Даем сдачу самыми крупными монетами
        foreach (int coin in coins.Keys.OrderByDescending(x => x))
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
            Console.WriteLine($"[ВНИМАНИЕ] Не смогли выдать {remaining} руб. сдачи");
        }
    }

    /// <summary>
    /// Возвращает внесенные деньги
    /// </summary>
    public void ReturnMoney()
    {
        if (currentBalance > 0)
        {
            Console.WriteLine($"[ВОЗВРАТ] Возвращаем {currentBalance} руб.");
            GiveChange(currentBalance);
            currentBalance = 0;
        }
        else
        {
            Console.WriteLine("[ИНФО] Нет денег для возврата");
        }
    }

    /// <summary>
    /// Добавляет новый товар (для администратора)
    /// </summary>
    public bool AddProduct(string name, int price, int quantity)
    {
        // Проверяем название
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("[ОШИБКА] Название товара не может быть пустым");
            return false;
        }

        // Проверяем цену
        if (price <= 0)
        {
            Console.WriteLine("[ОШИБКА] Цена должна быть больше 0");
            return false;
        }

        // Проверяем количество
        if (quantity <= 0)
        {
            Console.WriteLine("[ОШИБКА] Количество не может быть отрицательным или равно 0");
            return false;
        }

        // Ищем товар с таким же названием (без учета регистра)
        Product existingProduct = products.Find(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (existingProduct != null)
        {
            // Товар уже существует - проверяем, не изменилась ли цена
            if (existingProduct.Price != price)
            {
                Console.WriteLine($"[ВНИМАНИЕ] Цена товара '{name}' изменена с {existingProduct.Price} на {price} руб.");
                existingProduct.Price = price;
            }

            // Обновляем количество
            existingProduct.Quantity += quantity;
            Console.WriteLine($"[УСПЕХ] Количество товара '{name}' увеличено на {quantity}. Теперь: {existingProduct.Quantity} шт.");
        }
        else
        {
            // Товара нет - добавляем новый
            products.Add(new Product(name, price, quantity));
            Console.WriteLine($"[УСПЕХ] Товар '{name}' добавлен. Количество: {quantity} шт.");
        }

        return true;
    }

    /// <summary>
    /// Показывает общую сумму денег в автомате (для администратора)
    /// </summary>
    public void DisplayTotalMoney()
    {
        int total = CalculateTotalCoins();
        Console.WriteLine($"=== Финансовая информация ===");
        Console.WriteLine($"Общая сумма в автомате: {total} руб.");
        Console.WriteLine($"Начальный запас для сдачи: {initialCoinBalance} руб.");
        Console.WriteLine($"Прибыль: {total - initialCoinBalance} руб.");
    }

    /// <summary>
    /// Собирает деньги из автомата (для администратора)
    /// </summary>
    public void CollectMoney()
    {
        int currentTotal = CalculateTotalCoins();

        // Вычисляем только прибыль (текущая сумма минус начальный запас для сдачи)
        int profit = currentTotal - initialCoinBalance;

        if (profit > 0)
        {
            Console.WriteLine($"[СБОР] Собрано прибыли: {profit} руб.");

            // Восстанавливаем начальный запас монет для сдачи
            coins.Clear();
            InitializeCoins();

            Console.WriteLine("[УСПЕХ] Деньги собраны. Начальный запас для сдачи восстановлен.");
        }
        else
        {
            Console.WriteLine($"[ИНФО] Прибыль отсутствует. Текущая сумма: {currentTotal} руб., минимальный запас: {initialCoinBalance} руб.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        VendingMachine vm = new VendingMachine();
        bool isRunning = true;

        Console.WriteLine("=== Добро пожаловать в вендинговый автомат! ===");

        while (isRunning)
        {
            Console.WriteLine("\n=================================");
            Console.WriteLine($"Ваш баланс: {vm.GetCurrentBalance()} руб.");
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
                    Console.WriteLine("[ОШИБКА] Неверная команда");
                    break;
            }
        }
    }

    /// <summary>
    /// Режим внесения нескольких монет подряд
    /// </summary>
    static void InsertCoinsMode(VendingMachine vm)
    {
        bool inCoinMode = true;

        while (inCoinMode)
        {
            Console.WriteLine($"\nТекущий баланс: {vm.GetCurrentBalance()} руб.");
            Console.WriteLine("=== Внесение монет ===");
            Console.WriteLine("1. Внести монету");
            Console.WriteLine("2. Вернуться назад");
            Console.Write("Выберите: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введите номинал монеты (1, 2, 5, 10, 20, 50, 100): ");
                    if (int.TryParse(Console.ReadLine(), out int denomination))
                    {
                        vm.InsertCoin(denomination);
                    }
                    else
                    {
                        Console.WriteLine("[ОШИБКА] Неверный номинал");
                    }
                    break;

                case "2":
                    inCoinMode = false;
                    break;

                default:
                    Console.WriteLine("[ОШИБКА] Неверная команда");
                    break;
            }
        }
    }

    /// <summary>
    /// Меню покупки товара
    /// </summary>
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
            Console.WriteLine("[ОШИБКА] Неверный номер товара");
        }
    }

    /// <summary>
    /// Режим администратора
    /// </summary>
    static void AdminMode(VendingMachine vm)
    {
        Console.Write("Введите пароль: ");
        string password = Console.ReadLine();

        if (password != "admin")
        {
            Console.WriteLine("[ОШИБКА] Неверный пароль");
            return;
        }

        bool inAdminMode = true;

        while (inAdminMode)
        {
            Console.WriteLine("\n=== Режим администратора ===");
            Console.WriteLine("1. Добавить товар");
            Console.WriteLine("2. Показать общую сумму");
            Console.WriteLine("3. Собрать деньги");
            Console.WriteLine("4. Назад");
            Console.Write("Выберите: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddProductMenu(vm);
                    break;

                case "2":
                    vm.DisplayTotalMoney();
                    break;

                case "3":
                    vm.CollectMoney();
                    break;

                case "4":
                    inAdminMode = false;
                    break;

                default:
                    Console.WriteLine("[ОШИБКА] Неверная команда");
                    break;
            }
        }
    }

    /// <summary>
    /// Меню добавления товара
    /// </summary>
    static void AddProductMenu(VendingMachine vm)
    {
        Console.Write("Введите название товара: ");
        string name = Console.ReadLine();

        Console.Write("Введите цену: ");
        string priceInput = Console.ReadLine();

        Console.Write("Введите количество: ");
        string quantityInput = Console.ReadLine();

        // Проверяем, что ввели числа
        if (!int.TryParse(priceInput, out int price))
        {
            Console.WriteLine("[ОШИБКА] Цена должна быть числом!");
            return;
        }

        if (!int.TryParse(quantityInput, out int quantity))
        {
            Console.WriteLine("[ОШИБКА] Количество должно быть числом!");
            return;
        }

        // Пробуем добавить товар
        bool success = vm.AddProduct(name, price, quantity);
        if (!success)
        {
            Console.WriteLine("[ОШИБКА] Не удалось добавить товар");
        }
    }
}
