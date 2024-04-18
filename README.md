![Image alt](https://github.com/flaxytop/SpWorldsApiForCS/blob/2.0.0-beta/src/logo/logo.jpg)

# SpWorldsApiForCS 2.0.0
Это библиотека C# для управлением API SpWorlds. Документация к API [тут](https://github.com/sp-worlds/api-docs).
# Как начать?
Подключение библиотеки происходит через [nuget](https://www.nuget.org/packages/spw)
#### nuget
    dotnet add package spw --version 2.0.0
# Команды 
### Примечание
Можно использывать асиннхронные и синхронные методы
### Деректива подключения
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
await sp.GetCardInfoAsync();
//or
sp.GetCardInfo();
```
*Возвращает SPCardUser ([Типы](#Types))*

### Получить никнейм по DiscordId
```cs
await sp.GetUserAsync("DiscordId");
//or
sp.GetUser("DiscordId");
```
*Возвращает SPUser ([Типы](#Types))*

### Отправить АРы
```cs
await sp.SendPaymentAsync(amount, "receiver", "message");
//or
sp.SendPayment(amount, "receiver", "message");
```
*Возвращает bool*

### Создать ссылку на оплату
```cs
await sp.CreatePaymentAsync(payment); //class SPPayment
//or
sp.CreatePayment(payment); //class SPPayment
```
*Возвращает string(url)*

### Проверка оплаты
```cs
await sp.ValidatorAsync("webhook", "Xbody_hash");
//or
sp.Validator("webhook", "Xbody_hash");
```
*Возвращает bool*

### Получение аккаунта владельца токена
```cs
await sp.GetAccountAsync();
//or
sp.GetAccount();
```
*Возвращает SPAccount ([Типы](#Types))*

### Получение карт игрока
```cs
await sp.GetCardsAsync(username);
//or
sp.GetCards(username);
```
*Возвращает SPCard[] ([Типы](#Types))*

### Установка вебхука для карты
```cs
await sp.SetWebhookAsync(webhook);
//or
sp.SetWebhookAsync(webhook);
```
*Возвращает bool*

# Types
#### SPAccount
Используется (в return): *GetAccount()*
Содержимое:
```cs
int id 
string username 
string status 
string[] roles 
SPCity city 
SPCard[] cards
string createdAt
```

#### SPCard
Используется (в return): *GetCards(username)*
Содержимое:
```cs
string name 
string number 
```

#### SPCardUser
Используется: **
Содержимое:
```cs
int balance 
string webhook 
```

#### SPCity
Используется (в return): *GetAccount()*
Содержимое:
```cs
string id 
string name 
int x 
int y 
bool isMayor
```
#### SPItem
Используется (в return): *CreatePayment()*
Содержимое:
```cs
string name 
int count 
int amount 
string comment // can be null
```
#### SPPayment
Используется: *CreatePayment()*
Содержимое:
```cs
SPItem item 
string redirectUrl 
string webhookUrl
string data 
```
#### SPTransaction
Используется: _После устоновления webhook {*SetWebhookAsync(webhook)*}, приходят транзакции (для парса)_
Содержимое:
```cs
string id 
string name 
string type 
string sender_username 
string sender_number 
string receiver_username 
string receiver_number 
string comment 
string createdAt 
```
#### SPUser
Используется: *GetCardInfo()*
Содержимое:
```cs
string username 
string uuid //Minecraft uuid
```

# Exceptions
#### BabRequestException
Неправильная форма запроса
#### UnathorizedException
Неверный token или id
#### BadGatewayException
spworlds api отключен
