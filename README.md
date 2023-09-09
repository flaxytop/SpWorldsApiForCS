# SpWorldsApiForCS
Это библиотека C# для управлением API SpWorlds. Документация к API [тут](https://github.com/sp-worlds/api-docs).
# Как начать?
Подключение библиотеки происходит через nuget
#### nuget
    dotnet add package spw --version 1.0.2
# Команды 
### Деректива подключения
    using spw;
### Создание класса
    SpWorlds sp = new Spworlds("ваш айди", "ваш токен");
### Правильный token и id
    await sp.IsSpWallet();
Возвращает bool
### Получить баланс
    await sp.GetBalance();
    //or sp.GetBalance().Result;
