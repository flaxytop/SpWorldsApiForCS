# SpWorldsApiForCS
Это библиотека C# для управлением API SpWorlds. Документация к API [тут](https://github.com/sp-worlds/api-docs).
# Как начать?
Подключение библиотеки происходит через [nuget](https://www.nuget.org/packages/spw)
#### nuget
    dotnet add package spw --version 1.0.2
# Команды 
### Примечание
Используется асиннхроность, 2 варианта использование команд:
1. await sp.Command();
2. sp.Command().Result;
### Деректива подключения
    using spw;
### Создание класса
    SpWorlds sp = new Spworlds("id", "token");
### Правильный token и id
    await sp.IsSpWallet();
*Возвращает bool*
### Получить баланс
    await sp.GetBalance();
*Возвращает int*
### Получить никнейм по DiscordId
    await sp.GetUser("DiscordId");
*Возвращает string*
### Отправить АРы
    await sp.SendPayment(amount, "receiver", "message");
*Возвращает bool*
### Создать ссылку на оплату
    await sp.CreatePayment(amount, "redirectUrl", "webhookUrl", "data");
*Возвращает string*
### Проверка оплаты
    await sp.Validator("webhook", "Xbody_hash");
*Возвращает bool*
#### Exception
# BabRequest
Неправильная форма запроса
# EncorredTokenOrId
Неверный token или id
# UnknowError
Неизвестная ошибка
