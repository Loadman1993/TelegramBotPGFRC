using TelegramBotPGFRC;
using static System.Net.Mime.MediaTypeNames;

TelegramBotClient botClient = new TelegramBotClient("***************************************");

var metBot = new BotEngine(botClient);


await metBot.ListenForMessagesAsync();


public class BotEngine
{


    //Messages and user info


    private readonly TelegramBotClient _botClient;

    public BotEngine(TelegramBotClient botClient)
    {
        _botClient = botClient;
    }
    public async Task ListenForMessagesAsync()
    {
        using var cts = new CancellationTokenSource();


        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
            );

        var me = await _botClient.GetMeAsync();

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();
    }

    private Dictionary<long, PatientsAntropometrics> PatientsAntropometrics = new Dictionary<long, PatientsAntropometrics>();

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {




        switch (update.Type)
        {
            case UpdateType.Message:

                if (update.Message?.Text == "/start")
                {
                    await SendStartMenu(update.Message.Chat.Id, botClient, cancellationToken);
                }
                else
                {
                   
                    //await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Пожалуйста, подождите, пока я обработаю введенные данные...", cancellationToken: cancellationToken);
                     string userText = update.Message.Text;
                    Console.WriteLine($"Пользователь ввел: {userText}");
                }
                break;

            case UpdateType.CallbackQuery:

                if (update.CallbackQuery?.Data == "Рост")
                {
                    await botClient.SendTextMessageAsync(
                         chatId: update.CallbackQuery.Message.Chat.Id,
                         replyMarkup: new ForceReplyMarkup(),
                         text: "Введите Рост",
                         cancellationToken: cancellationToken
                    );

                    bool statement = false;
                    int maxAttempts = 3;
                    int attempts = 0;

                    while (!statement && attempts < maxAttempts)
                    {
                        var updates = await botClient.GetUpdatesAsync(offset: update.Id + 1, allowedUpdates: new List<UpdateType> { UpdateType.Message }, cancellationToken: cancellationToken);

                        foreach (var newUpdate in updates)
                        {
                            if (newUpdate.Message?.Chat.Id == update.CallbackQuery.Message.Chat.Id && newUpdate.Message?.Text != null)
                            {
                                Console.WriteLine($"Пользователь ввел рост: {newUpdate.Message.Text}");

                                if (decimal.TryParse(newUpdate.Message?.Text, out decimal height))
                                {
                                    PatientsAntropometrics[update.CallbackQuery.Message.Chat.Id] = new PatientsAntropometrics { Height = height };


                                    await Console.Out.WriteLineAsync("ввел " + PatientsAntropometrics[update.CallbackQuery.Message.Chat.Id].Height);
                                    statement = true;
                                    break;
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(chatId: newUpdate.Message?.Chat.Id, text: "Вы ввели некорректные данные", cancellationToken: cancellationToken);
                                    attempts++;
                                    await Task.Delay(10000);
                                }
                            }

                            await Task.Delay(3000); // Пауза, чтобы не перегружать процессор
                            Console.WriteLine(" Жду ввода...");
                        }
                    }

                    if (statement)
                    {
                        var keyboard = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Креатинин") } });

                        await botClient.SendTextMessageAsync(
                            chatId: update.CallbackQuery.Message.Chat.Id,
                            text: "Выберите действие:",
                            replyMarkup: keyboard,
                            cancellationToken: cancellationToken
                        );
                    }
                }
                else if (update.CallbackQuery?.Data == "Креатинин")
                {
                    await botClient.SendTextMessageAsync(
                         chatId: update.CallbackQuery.Message.Chat.Id,
                         replyMarkup: new ForceReplyMarkup(),
                         text: "Введите концентрацию креатинина",
                         cancellationToken: cancellationToken
                    );

                    bool statement = false;
                    int maxAttempts = 3;
                    int attempts = 0;

                    while (!statement && attempts < maxAttempts)
                    {
                        var updates = await botClient.GetUpdatesAsync(offset: update.Id + 1, allowedUpdates: new List<UpdateType> { UpdateType.Message }, cancellationToken: cancellationToken);

                        foreach (var newUpdate in updates)
                        {
                            if (newUpdate.Message?.Chat.Id == update.CallbackQuery.Message.Chat.Id && newUpdate.Message?.Text != null)
                            {
                                Console.WriteLine($"Пользователь ввел концентрацию креатинина: {newUpdate.Message.Text}");

                                if (decimal.TryParse(newUpdate.Message?.Text, out decimal screatinine))
                                {
                                    PatientsAntropometrics[update.CallbackQuery.Message.Chat.Id].SCreatinine = screatinine;

                                    await Console.Out.WriteLineAsync("ввел креатинина " + PatientsAntropometrics[update.CallbackQuery.Message.Chat.Id].SCreatinine);
                                   
                                    statement = true;
                                    break;
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(chatId: newUpdate.Message?.Chat.Id, text: "Вы ввели некорректные данные", cancellationToken: cancellationToken);
                                    attempts++;
                                    await Task.Delay(10000);
                                }
                            }

                            await Task.Delay(3000); // Пауза, чтобы не перегружать процессор
                            Console.WriteLine(" Жду ввода...");
                        }
                    }

                    if (statement)
                    {
                        var keyboard = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Цистатин С"), InlineKeyboardButton.WithCallbackData("Пропустить") } });

                        await botClient.SendTextMessageAsync(
                            chatId: update.CallbackQuery.Message.Chat.Id,
                            text: "Выберите действие:",
                            replyMarkup: keyboard,
                            cancellationToken: cancellationToken
                        );
                    }
                }
                else if (update.CallbackQuery?.Data == "Цистатин С")
                {
                    await botClient.SendTextMessageAsync(
                         chatId: update.CallbackQuery.Message.Chat.Id,
                         replyMarkup: new ForceReplyMarkup(),
                         text: "Введите концентрацию Цистатина С",
                         cancellationToken: cancellationToken
                    );

                    bool statement = false;
                    int maxAttempts = 3;
                    int attempts = 0;

                    while (!statement && attempts < maxAttempts)
                    {
                        var updates = await botClient.GetUpdatesAsync(offset: update.Id + 1, allowedUpdates: new List<UpdateType> { UpdateType.Message }, cancellationToken: cancellationToken);

                        foreach (var newUpdate in updates)
                        {
                            if (newUpdate.Message?.Chat.Id == update.CallbackQuery.Message.Chat.Id && newUpdate.Message?.Text != null)
                            {
                                Console.WriteLine($"Пользователь ввел концентрацию Цистатина С: {newUpdate.Message.Text}");

                                if (decimal.TryParse(newUpdate.Message?.Text, out decimal sCystatinC))
                                {
                                    PatientsAntropometrics[update.CallbackQuery.Message.Chat.Id].SCystatinC = sCystatinC;

                                    await Console.Out.WriteLineAsync("ввел Цистатина С " + PatientsAntropometrics[update.CallbackQuery.Message.Chat.Id].SCystatinC);
                                    
                                    statement = true;
                                    break;
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(chatId: newUpdate.Message?.Chat.Id, text: "Вы ввели некорректные данные", cancellationToken: cancellationToken);
                                    attempts++;
                                    await Task.Delay(10000);
                                }
                            }

                            await Task.Delay(3000); // Пауза, чтобы не перегружать процессор
                            Console.WriteLine(" Жду ввода...");
                        }
                    }

                    if (statement)
                    {
                        var keyboard1 = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Рассёт") } });

                        await botClient.SendTextMessageAsync(
                            chatId: update.CallbackQuery.Message.Chat.Id,
                            text: "Выберите действие:",
                            replyMarkup: keyboard1, // Важно: добавляем клавиатуру в параметры метода
                            cancellationToken: cancellationToken
                        );
                    }

                }
               


                    else if (update.CallbackQuery?.Data == "Рассёт")
                    {
                        // Получение данных из словаря PatientsAntropometrics
                        PatientsAntropometrics userAntropometrics = PatientsAntropometrics[update.CallbackQuery.Message.Chat.Id];

                       
                        var creatinineEstimator = new CreatinineBedsideSchwartz();

                        // Вычисление оценки креатинина по формуле
                        decimal creatinineEstimation = creatinineEstimator.Calculate(userAntropometrics);

                        // Отправка результата пользователю
                        await botClient.SendTextMessageAsync(
                            chatId: update.CallbackQuery.Message.Chat.Id,
                            text: $"Рост: {userAntropometrics.Height}\nКреатинин: {userAntropometrics.SCreatinine}\nЦистатин С: {userAntropometrics.SCystatinC}\nОценка креатинина: {creatinineEstimation}",
                            cancellationToken: cancellationToken
                        );
                    }


                
                break;

            case UpdateType.Unknown:
                Console.WriteLine("сработал Unknown");
                break;
        } 
    

        if (update.Message is not { } message)
        {
            Console.WriteLine("Null сработал");
            return;
        }
        if (message.Text is not { } messageText)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы или тут", cancellationToken: cancellationToken);
            return;
        }
        
        
        Console.WriteLine($"Received a '{messageText}' message in chat {message.Chat.Id} with Id№ '{message.MessageId}' ");


    }

   

    private async Task SendStartMenu(long chatId, ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
     new[]
     {
         InlineKeyboardButton.WithCallbackData("Рост"),

     }
 });

        await botClient.SendTextMessageAsync(
           chatId: chatId, replyMarkup: keyboard,
           text: "Выберите действие:",
            cancellationToken: cancellationToken
        );
    }
    


    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}



