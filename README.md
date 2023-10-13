# SpWorldsApiForCS
Это библиотека C# для управлением API SpWorlds. Документация к API [тут](https://github.com/sp-worlds/api-docs).
# Как начать?
Подключение библиотеки происходит через [nuget](https://www.nuget.org/packages/spw)
#### nuget
    dotnet add package spw --version 1.1.0
# Команды 
### Примечание
Можно использывать асиннхронные и синхронные функции
### Деректива подключения
    using spw;
### Создание класса
    SpWorlds sp = new Spworlds("id", "token");
### Правильный token и id
    await sp.IsSpWalletAsync();
    *or*
    sp.IsSpwallet
*Возвращает bool*
### Получить баланс
    await sp.GetBalanceAsync();
    *or*
    sp.GetBalance();
*Возвращает int*
### Получить никнейм по DiscordId
    await sp.GetUserAsync("DiscordId");
    *or*
    sp.GetUser("DiscordId");
*Возвращает string*
### Отправить АРы
    await sp.SendPaymentAsync(amount, "receiver", "message");
    *or*
    sp.SendPayment(amount, "receiver", "message");
*Возвращает bool*
### Создать ссылку на оплату
    await sp.CreatePaymentAsync(amount, "redirectUrl", "webhookUrl", "data");
    *or*
    sp.CreatePayment(amount, "redirectUrl", "webhookUrl", "data");
*Возвращает string*
### Проверка оплаты
    await sp.ValidatorAsync("webhook", "Xbody_hash");
    *or*
    sp.Validator("webhook", "Xbody_hash");
*Возвращает bool*
# Exception
#### BabRequest
Неправильная форма запроса
#### EncorredTokenOrId
Неверный token или id
#### UnknowError
Неизвестная ошибка
