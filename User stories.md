# User Stories

## Игрок (пользователь)

- Я, как игрок, хочу перемещаться между сценами, чтобы последовательно изучать сюжет.
- Я, как игрок, хочу нажимать E при наведении на объекты, чтобы получать информацию об объектах и персонажах.
- Я, как игрок, хочу нажимать E при приближении к NPC, чтобы запускать диалог.
- Я, как игрок, хочу видеть текстовые подсказки, чтобы понимать, что происходит в игре.
- Я, как игрок, хочу запускать сюжетные сцены, чтобы наблюдать реконструкцию событий.
- Я, как игрок, хочу получать доступ к новым сценам после выполнения условий, чтобы продвигаться дальше по сюжету.


## Разработчик

- Я, как разработчик, хочу добавлять новые сцены, чтобы расширять проект.
- Я, как разработчик, хочу настраивать интерактивные объекты, чтобы управлять логикой взаимодействия.
- Я, как разработчик, хочу редактировать текстовые элементы, чтобы корректировать сюжет и диалоги.
- Я, как разработчик, хочу управлять переходами между сценами, чтобы контролировать логику игры.

## Тестировщик
- Я, как тестировщик, хочу проверять работу интерактивных объектов, чтобы при клике отображалась правильная информация.
- Я, как тестировщик, хочу проверять запуск сюжетных сцен, чтобы они активировались при выполнении условий.
- Я, как тестировщик, хочу проверять отображение текстов, чтобы избежать ошибок и некорректного форматирования.
- Я, как тестировщик, хочу проверять поведение игры при разных действиях пользователя, чтобы выявить возможные баги.

#User Flow
```mermaid
graph TD
classDef startend fill:#d4edda,stroke:#28a745,stroke-width:2px,color:#000000;
classDef decision fill:#fff3cd,stroke:#ffc107,stroke-width:2px,color:#000000;
classDef process fill:#e2e3e5,stroke:#383d41,stroke-width:2px,color:#000000;
classDef ui fill:#cce5ff,stroke:#004085,stroke-width:2px,color:#000000;
classDef quest fill:#f8d7da,stroke:#721c24,stroke-width:2px,color:#000000;

    Start(["Запуск приложения"]) --> MainMenu["Главное меню"]
    
    MainMenu -->|"Начать новую игру"| NewGame["Создание нового сохранения"] --> PrologueSpawn["Спавн: Квартира офицеров"]
    MainMenu -->|"Продолжить игру"| LoadGame["Загрузка последнего сохранения"] --> CheckScene{"Есть сохранение?"}
    CheckScene -->|"Да"| LoadScene["Загрузка сцены + восстановление состояния"]
    CheckScene -->|"Нет"| PrologueSpawn
    MainMenu -->|"Настройки"| SettingsUI["Настройки: звук, управление, графика"] --> MainMenu
    MainMenu -->|"Выход"| Exit(["Закрытие приложения"])

    LoadScene --> GameLoop

    %% ПРОЛОГ: КВАРТИРА ОФИЦЕРОВ
    PrologueSpawn --> Quest1["Задание 1: Осмотрите помещение"]
    Quest1 --> ExploreLoop["Исследование локации<br/>Point-and-click навигация"]
    
    ExploreLoop --> ClickObject{"Клик по объекту?"}
    ClickObject -->|"Нет"| ExploreLoop
    ClickObject -->|"Да, неинтерактивный"| ShowDesc["Краткое описание объекта"] --> ExploreLoop
    
    ClickObject -->|"Да, интерактивный"| CheckObject{"Какой объект?"}
    
    CheckObject -->|"Стол с картами"| InspectTable["Осмотр стола<br/>Описание игры в штосс"] --> DiaryEntry1["Запись в дневник:<br/>«Штосс. Игра, в которой всё решают секунды...»"] --> SetFlag1["Флаг: inspected_table = true"]
    CheckObject -->|"Окно"| InspectWindow["Осмотр окна<br/>Ночной Петербург"] --> SetFlag2["Флаг: inspected_window = true"]
    CheckObject -->|"Камин + портрет"| InspectFireplace["Осмотр камина<br/>Портрет генерала"] --> SetFlag3["Флаг: inspected_fireplace = true"]
    CheckObject -->|"Книжный шкаф"| InspectBookshelf["Осмотр шкафа<br/>Карамзин, уставы"] --> SetFlag4["Флаг: inspected_bookshelf = true"]
    CheckObject -->|"Томский"| TalkTomsky["Разговор с Томским<br/>«Давно не виделись!»"] --> SetFlag5["Флаг: talked_tomsky = true"]
    
    SetFlag1 --> CheckFlags
    SetFlag2 --> CheckFlags
    SetFlag3 --> CheckFlags
    SetFlag4 --> CheckFlags
    SetFlag5 --> CheckFlags

    CheckFlags{"Все 5 объектов осмотрены?"}
    CheckFlags -->|"Нет"| ExploreLoop
    CheckFlags -->|"Да"| Quest2["Задание 2: Поговорите со старыми знакомыми"]
    
    Quest2 --> TalkOfficers["Диалог с офицерами"]
    TalkOfficers --> DialogueStart["Офицер 1: «Опять не везёт! А ты, Имя, не подсядешь? Помнишь Германна?...»"]
    DialogueStart --> DialogueContinue["Диалог: обсуждение безумия Германна, тайны трёх карт, смерти графини"]
    DialogueContinue --> DiaryEntry2["Запись в дневник:<br/>«Они говорят разное. Графиня умерла раньше, но Германн помешался после...»"]
    DiaryEntry2 --> UnlockDoor["Разблокировка двери<br/>Подсветка + смена курсора"]
    
    UnlockDoor --> ClickDoor{"Клик по двери?"}
    ClickDoor -->|"Нет"| ExploreLoop
    ClickDoor -->|"Да"| Cutscene["Кат-сцена: затемнение + текст/арт"] --> LoadScene1["Загрузка Сцены 1: Двор графини"]

    %% ПАРАЛЛЕЛЬНЫЕ СИСТЕМЫ
    ExploreLoop --> OpenJournal{"Нажат TAB / Клик «Журнал»?"}
    OpenJournal -->|"Да"| JournalUI["Открытие Журнала:<br/>вкладки Дневник/Досье"] --> JournalNav["Навигация: перелистывание, выбор персонажа"] --> CloseJournal["Закрытие по Esc/×<br/>Автосохранение"] --> ExploreLoop
    OpenJournal -->|"Нет"| ExploreLoop

    ExploreLoop --> OpenPause{"Нажат Esc?"}
    OpenPause -->|"Да"| PauseMenu["Меню паузы:<br/>Продолжить / Настройки / В главное меню / Выход"]
    PauseMenu -->|"Продолжить"| ExploreLoop
    PauseMenu -->|"Настройки"| SettingsUI
    PauseMenu -->|"В главное меню"| MainMenu
    PauseMenu -->|"Выход"| Exit
    OpenPause -->|"Нет"| ExploreLoop

    %% СОХРАНЕНИЯ
    DiaryEntry1 -.-> AutoSave["Автосохранение:<br/>флаги + состояние журнала + позиция"]
    DiaryEntry2 -.-> AutoSave
    CloseJournal -.-> AutoSave
    LoadScene1 -.-> AutoSave

    class Start,Exit startend;
    class ClickObject,CheckObject,CheckFlags,ClickDoor,OpenJournal,OpenPause,CheckScene decision;
    class ExploreLoop,PrologueSpawn,InspectTable,InspectWindow,InspectFireplace,InspectBookshelf,TalkTomsky,TalkOfficers,DialogueStart,DialogueContinue,Cutscene,LoadScene1,LoadGame,LoadScene,NewGame process;
    class SettingsUI,JournalUI,JournalNav,CloseJournal,PauseMenu,ShowDesc ui;
    class Quest1,Quest2,DiaryEntry1,DiaryEntry2,SetFlag1,SetFlag2,SetFlag3,SetFlag4,SetFlag5,CheckFlags,UnlockDoor,AutoSave quest;
```
