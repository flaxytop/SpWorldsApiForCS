![Image alt](https://github.com/flaxytop/SpWorldsApiForCS/blob/2.0.0-beta/src/logo/logo.jpg)
# SpWorldsApiForCS
Это библиотека C# для управлением API SpWorlds наш [github](https://github.com/flaxytop/SpWorldsApiForCS). Документация к API [тут](https://github.com/sp-worlds/api-docs).
# Как начать?
Подключение библиотеки происходит через [nuget](https://www.nuget.org/packages/spw)
#### nuget
    dotnet add package spw --version 1.1.2
# Команды 
### Примечание
Можно использовать асинхронные и синхронные функции
### Директива подключения
```cs
using spw;
```
### Создание класса
```cs
SpWorlds sp = new SpWorlds("id", "token");
```
### Правильный token и id
```cs
await sp.IsSpWalletAsync();
//or
sp.IsSpwallet();
```
*Возвращает bool*
### Получить баланс
```cs
await sp.GetBalanceAsync();
//or
sp.GetBalance();
```
*Возвращает int*
### Получить никнейм по DiscordId
```cs
await sp.GetUserAsync("DiscordId");
//or
sp.GetUser("DiscordId");
```
*Возвращает string*
### Отправить АРы
```cs
await sp.SendPaymentAsync(amount, "receiver", "message");
//or
sp.SendPayment(amount, "receiver", "message");
```
*Возвращает bool*
### Создать ссылку на оплату
```cs
await sp.CreatePaymentAsync(amount, "redirectUrl", "webhookUrl", "data");
//or
sp.CreatePayment(amount, "redirectUrl", "webhookUrl", "data");
```
*Возвращает string*
### Проверка оплаты
```cs
await sp.ValidatorAsync("webhook", "Xbody_hash");
//or
sp.Validator("webhook", "Xbody_hash");
```
*Возвращает bool*
### Получение uuid mojang
```cs
await sp.GetMojangUuid("name")
//or
sp.GetMojangUuid("name")
```
*Возвращает string*
# Exceptions
#### BabRequestException
Неправильная форма запроса
#### UnathorizedException
Неверный token или id
#### BadGatewayException
Spworlds api отключен
