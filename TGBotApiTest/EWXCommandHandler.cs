using me.ewerestr.ewxtelegrambot.Utils;
using System;

namespace me.ewerestr.ewxtelegrambot.Components
{
    class EWXCommandHandler
    {
        private bool _allowListen = false;
        private bool _debug = false;
        // private bool _postFlag = false;

        // entry block
        public EWXCommandHandler()
        {
            EWXTelegramBot.PrintLine("Обработчик команд запущен");
        }

        // methods block
        public void Start()
        {
            _allowListen = true;
            Listen();
        }

        private void Listen()
        {
            EWXTelegramBot.PrintLine("Слушатель командной строки запущен. Введите нужную вам команду");
            try
            {
                while (_allowListen)
                {
                    string line = Console.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        EWXCommand cmd = new EWXCommand(line);
                        HandleCommand(cmd);
                    }
                }
            }
            catch (Exception e)
            {
                EWXTelegramBot.StackTrace(this.GetType().Name, "Listen", e.GetType().ToString(), e.Message, e.StackTrace);
            }
        }

        public bool HandleCommand(EWXCommand cmd, string from = null)
        {
            try
            {
                bool stop = false;
                string cmessage = null;
                switch (cmd.GetCommand().ToLower())
                {
                    case "test":
                        {
                            if (from != null) cmessage = "OK!";
                            else cmessage = "Ты в своем уме? Ты пишешь эту команду прямо, мать его, в командную строку!";
                            break;
                        }
                    case "start":
                        {
                            EWXTelegramBot.GetController().Start();
                            cmessage = "Принято! Подготовка к старту основного вычислительного модуля...";
                            break;
                        }
                    case "stop":
                        {
                            stop = true;
                            cmessage = "Принято! Подготовка к остановке программы...";
                            break;
                        }
                    case "saveall":
                        {
                            EWXTelegramBot.SaveAll();
                            cmessage = "Текущая конфигурация успешно сохранена!";
                            break;
                        }
                    case "setlongpolltimeout":
                        {
                            if (cmd.HasArguments())
                            {
                                int lInt;
                                if (int.TryParse(cmd.GetArgumentsArray()[0], out lInt))
                                {
                                    EWXTelegramBot.GetController().SetLongpollTimeout(lInt);
                                    cmessage = "Успех! Новое значение интервала опроса Longpoll-сервера Telegram установлено!";
                                }
                                else cmessage = "Некорректные аргументы команды. Пример: \"setLongpollTimeout 25\"";
                            }
                            else cmessage = "Команда должна иметь аргументы. Пример: \"setLongpollTimeout 25\"";
                            break;
                        }
                    case "settelegramtoken": // Deprecated 
                        {
                            if (cmd.HasArguments())
                            {
                                string lString = EWXTelegramBot.GetController().TestTelegramToken(cmd.GetArgumentsArray()[0]);
                                if (string.IsNullOrEmpty(lString)) cmessage = "Token is invalid. Try other token or make it right";
                                else
                                {
                                    EWXTelegramBot.GetController().SetTelegramToken(cmd.GetArgumentsArray()[0]);
                                    EWXTelegramBot.GetController().SetBotUsername(lString);
                                    cmessage = "Успех! Токен прошел валидацию. Установка токена как основгого и установка имени бота.";
                                }
                            }
                            else cmessage = "Команда должна иметь аргументы. Пример: \"setTelegramToken 1234567890:AAAbbb-CCCddd-EEEfffGGG-hhh-XXXzzzL\"";
                            break;
                        }
                    case "setsecretlength":
                        {
                            if (cmd.HasArguments())
                            {
                                int lInt;
                                if (int.TryParse(cmd.GetArgumentsArray()[0], out lInt))
                                {
                                    if (lInt >= 10 && lInt <= 128)
                                    {
                                        EWXTelegramBot.GetController().SetSecretLength(lInt);
                                        cmessage = "Успех! Новое значение длины секретного кода установлено!"/* + lInt*/;
                                    }
                                    else cmessage = "Недопустимое значение. Длина может вариьроваться от 10 до 128 символов.";
                                }
                                else cmessage = "Некорректные аргументы команды. Аргумент должен быть числом. Пример: \"setSecretLength 64\"";
                            }
                            else cmessage = "Команда должна иметь аргументы. Пример: \"setSecretLength 64\"";
                            break;
                        }
                    case "setpostinterval":
                        {
                            if (cmd.HasArguments())
                            {
                                DateTime startPoint;
                                int[] dt = new int[] {DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0};
                                if (cmd.GetArgumentsCount() > 1)
                                {
                                    string starttime = cmd.GetArgumentsArray()[1];
                                    if (starttime.Contains(":"))
                                    {
                                        string[] q = starttime.Split(':');
                                        if (int.TryParse(q[0], out dt[3]) && int.TryParse(q[1], out dt[4]))
                                        {
                                            // otsosi
                                        }
                                    }
                                    else
                                    {
                                        cmessage = "Некорректные аргументы команды. Стартовое время должно быть в формате ЧЧ:ММ. Пример: \"setPostInterval 1d2h3m4s 10:00\"";
                                        break;
                                    }
                                }
                                int[] ir = EWXTelegramBot.ParseCooldown(cmd.GetArgumentsArray()[0]);
                                if (ir[0] < 0)
                                {
                                    cmessage = "Некорректные аргументы команды. Неверная трактовка интервала публикаций. Пример: \"setPostInterval 1d2h3m4s 10:00\"";
                                }
                                else
                                {
                                    startPoint = new DateTime(dt[0], dt[1], dt[2], dt[3], dt[4], dt[5]);
                                    DateTime endPoint = startPoint.AddDays(ir[0]).AddHours(ir[1]).AddMinutes(ir[2]).AddSeconds(ir[3]);
                                    TimeSpan w = endPoint.Subtract(startPoint);
                                    int[] nd = new int[] {w.Days, w.Hours, w.Minutes, w.Seconds};
                                    EWXTelegramBot.GetController().SetNextPostDate(endPoint);
                                    EWXTelegramBot.GetController().SetPostInterval(nd);
                                    cmessage = "Успех! Интервал публикаций изменен. ";
                                    cmessage += "Новый интервал - " + w.Days + " дней " + w.Hours + " часов " + w.Minutes + " минут " + w.Seconds + " секунд. ";
                                    cmessage += "Расчет времени выполнен от " + (cmd.GetArgumentsCount() > 1 ? startPoint.ToString("G").Split(' ')[1] + " сегодняшнего дня. " : "момента выполнения команды. ");
                                    cmessage += "Следующая публикация запланирована на " + endPoint.ToString("G");
                                }
                            }
                            break;
                        }
                    case "generatesecret":
                        {
                            string s = EWXTelegramBot.GetController().GenerateSecretCode();
                            cmessage = "Новый секретный код администратора сгенерирован: " + s;
                            break;
                        }
                    case "generatechannelsecret":
                        {
                            string s = EWXTelegramBot.GetController().GenerateChannelSecretCode();
                            cmessage = "Новый секретный код канала сгенерирован: " + s;
                            break;
                        }
                    case "invite":
                        {
                            EWXTelegramBot.GetController().Invite();
                            cmessage = "Приглашение активировано!";
                            break;
                        }
                    case "forcepost": // deprecated
                        {
                            if (EWXTelegramBot.GetController().IsWorking())
                            {
                                if (EWXTelegramBot.GetController().HasPeers())
                                {
                                    if (EWXTelegramBot.GetController().Post()) cmessage = "Новая публикация успешно выполнена!";
                                    else cmessage = EWXTelegramBot.GetController().GetLastError();
                                }
                                else cmessage = "Не удалось опубликовать запись, потому что список получателей пуст.";
                            }
                            else cmessage = "Не удалось опубликовать запись, потому что главный вычислительный модуль не запущен. Попробуйте выполнить команду \"start\"";
                            break;
                        }
                    case "authyandex":
                        {
                            if (string.IsNullOrEmpty(from))
                            {
                                EWXTelegramBot.GetController().AuthorizeYandex();
                                cmessage = "Успех! Программа авторизована в сервисах Yandex!";
                            }
                            else cmessage = "Вы не можете авторизоваться в сервисах Yandex при помощи удаленной команды, это возможно только из окна программы бота.";
                            break;
                        }
                    case "forcesync":
                        {
                            EWXTelegramBot.GetController().SyncData();
                            cmessage = "Успех! Программа успешно синхронизировалась с сервисами Yandex!";
                            break;
                        }
                    case "forcedump":
                        {
                            EWXTelegramBot.GetLogger().ForceDump();
                            cmessage = "Успех! Дамп лога выгружен";
                            break;
                        }
                    case "getdataholder":
                        {
                            EWXLocalData ld = EWXTelegramBot.GetLocalData();
                            if (ld != null)
                            {
                                if (!ld.IsEmpty())
                                {
                                    cmessage = "Информация о текущем локальном хранилище:" + Environment.NewLine;
                                    cmessage += "Всего материалов доступно: " + ld.GetAllMaterialsCount() + Environment.NewLine;
                                    cmessage += "Всего изображений: " + ld.GetImagesCount() + Environment.NewLine;
                                    cmessage += "Всего аудиозаписей: " + ld.GetAudiosCount() + Environment.NewLine;
                                    cmessage += "Опубликовано изображений: " + ld.GetPostedImageList().Count + Environment.NewLine;
                                    cmessage += "Опубликовано аудиозаписей: " + ld.GetPostedAudioList().Count + Environment.NewLine;
                                    cmessage += "Неопубликованных изображений: " + ld.GetUnpostedImageList().Count + Environment.NewLine;
                                    cmessage += "Неопубликованных аудиозаписей: " + ld.GetUnpostedAudioList().Count;
                                    if (from == null) cmessage = "#  #  #  #  #  #  #" + Environment.NewLine + cmessage + Environment.NewLine + "#  #  #  #  #  #  #  #";
                                }
                                else
                                {
                                    cmessage = "Локальное хранилище пусто";
                                }
                            }
                            else
                            {
                                cmessage = "Локальное хранилище или пусто, или случилась непредвиденная ошибка. Проверьте локальную папку или свяжитесь с разработчиком";
                            }
                            break;
                        }
                    case "getstatus":
                        {
                            cmessage = EWXTelegramBot.GetController().GetLiteStatus();
                            break;
                        }
                    case "getdetstatus":
                        {
                            cmessage = EWXTelegramBot.GetController().GetDetailedStatus();
                            break;
                        }
                    case "getupdates":
                        {
                            cmessage = EWXTelegramBot.GetController().CheckUpdates() ? "Новая версия EWX TelegramBot уже доступна! Используйте команду \"installUpdate\", чтобы начать обновление." : "У вас актуальная версия программы.";
                            break;
                        }
                    case "installupdates":
                        {
                            cmessage = "Упс... Эта команда временно недоступна, потому что программа-обновлятор еще не готова.";
                            break;
                        }
                    case "allowpostagain":
                        {
                            EWXTelegramBot.GetController().SetPostAgain(true);
                            cmessage = "Успех! Программа будет публиковать материалы повторно, если неопубликованных не останется.";
                            break;
                        }
                    case "denypostagain":
                        {
                            EWXTelegramBot.GetController().SetPostAgain(false);
                            cmessage = "Успех! Программа не будет публиковать материалы повторно, если неопубликованных не останется. Программа будет ждать вашего вмешательства.";
                            break;
                        }
                        /*
                    case "allowsaveafterpost":
                        {
                            EWXTelegramBot.GetController().SetSaveAfterPost(true);
                            cmessage = "Success! The bot will save all it's data after each post";
                            break;
                        }
                    case "denysaveafterpost":
                        {
                            EWXTelegramBot.GetController().SetSaveAfterPost(false);
                            cmessage = "Success! The bot won't save all it's data after each post. It'll be waiting for your command";
                            break;
                        }
                        */
                    case "eraseconfig":
                        {
                            EWXTelegramBot.Erase(0);
                            cmessage = "Успех! Конфигурация очищена!";
                            break;
                        }
                    case "eraselogs":
                        {
                            EWXTelegramBot.Erase(1);
                            cmessage = "Успех! Логи очищены!";
                            break;
                        }
                    case "eraseaudios":
                        {
                            EWXTelegramBot.Erase(2);
                            cmessage = "Успех! Локальное хранилище аудиозаписей очищено!";
                            break;
                        }
                    case "eraseimages":
                        {
                            EWXTelegramBot.Erase(3);
                            cmessage = "Успех! Локальное хранилище изображений очищено!";
                            break;
                        }
                    case "eraselocalfolder":
                        {
                            EWXTelegramBot.Erase(4);
                            cmessage = "Успех! Локальное хранилище полностью очищено!";
                            break;
                        }
                    case "erasedatafolder":
                        {
                            EWXTelegramBot.Erase(5);
                            cmessage = "Успех! Корневая папка программы очищена!";
                            break;
                        }
                    case "refill":
                        {
                            cmessage = "Упс... Эта команда временно недоступна, потому что ее метод еще не готов окончательно.";
                            break;
                        }
                    case "restartlogger":
                        {
                            cmessage = EWXTelegramBot.RestartLogger() ? "Успех! Логгер перезапущен (Или запущен, если был отключен ранее)" : "Не удалось перезапустить логгер.";
                            break;
                        }

                    case "help":
                        {
                            if (cmd.HasArguments())
                            {
                                switch (cmd.GetArgumentsArray()[0].ToLower())
                                {
                                    case "test":
                                        {
                                            cmessage = "Отладочная команда. Является тайной и служит лишь для проверки доступности программы. Если с доступом никаких проблем не выявлено - в ответ придет сообщение с текстом \"ОК!\"";
                                            break;
                                        }
                                    case "start":
                                        {
                                            cmessage = "start - Команда для ручного запуска главного вычислительного модуля. Обычно используется только при первом запуске, так как после первого запуска программа не знает токенов Телеграм-бота и Яндекс, поэтому ей некуда обращаться и вычислительный модуль при любом действии будет возвращать ошибку. Также команда может быть полезна в случае ошибки автозапуска модуля, но вероятность такого без внешнего вмешательства практически нулевая.";
                                            break;
                                        }
                                    case "stop":
                                        {
                                            cmessage = "stop - Команда для принудительной остановки выполенния программы. Используется в основном перед перезагрузкой сервера. При выполнении команды происходит форсированные дамп логов и сохранение текущего состояния всех модулей программы.";
                                            break;
                                        }
                                    case "saveall":
                                        {
                                            cmessage = "saveAll - Команда для принудительного сохранения текущего состояния всех модулей программы. Будет полезна тем, кто чувствует надвигающиеся неполадки.";
                                            break;
                                        }
                                    case "setlongpolltimeout":
                                        {
                                            cmessage = "setLongpollTimeout - Команда, устанавливающая в секундах частоту опроса серверов Телеграм на предмет наличия новых сообщений от администраторов. Не трогайте эту команду если вы не знаете, что делаете.";
                                            break;
                                        }
                                    case "settelegramtoken":
                                        {
                                            cmessage = "setTelegramToken - Команда для подключения к ней профиля Телеграм-бота. Принимает один параметр - токен. Токен можно получить у бота, который создает ботов (@BotFather), приношу извинения за тавтологию. Пример использования команды: \"setTelegramToken 1234567890:AAAbbb-CCCddd-EEEfffGGG-hhh-XXXzzzL\"";
                                            break;
                                        }
                                    case "setsecretlength":
                                        {
                                            cmessage = "setSecretLength - Команда, задающая длину секретного кода авторизации. Команда принимает один целочисленный параметр - число от 10 до 128. Пример использования команды: \"setSecretLength 64\"";
                                            break;
                                        }
                                    case "generatesecret":
                                        {
                                            cmessage = "generateSecret - Команда, генерирующая секретный код авторизации для пользователя. Используется для получения доступа к функционалу программы через личные сообщения боту. Сгенерированный код необходимо отправить боту личным сообщением, если код действителен - программа сообщит об этом.";
                                            break;
                                        }
                                    case "invite":
                                        {
                                            cmessage = "invite - Команда, запускающая слушатель добавления в канал\\группу. Выполнив эту команду программа начинает отслеживать все добавления в каналы и группы. После обнаружения добавления программа авторизует ресурс и в дальнейшем будет отправлять в него свои публикации. Команда нужна для защиты от неверных публикаций, ведь добавить бота в канал или группу может абсолютно любой пользователь, но программа не должна делиться своим контентом со всеми.";
                                            break;
                                        }
                                    case "forcepost":
                                        {
                                            cmessage = "forcePost - Команда, выполняющая моментальную внеплановую публикацию, не влияя тем самым на изначальный график публикаций. Была добавлена для тестирования функционала, но оставлена, так как может быть полезна в некоторых ситуациях.";
                                            break;
                                        }
                                    case "authyandex":
                                        {
                                            cmessage = "authYandex - Команда, вызывающая окно авторизации в сервисах Яндекс. Данную команду можно выполнять только из командной строки, из личных сообщений боту выполнять ее не имеет никакого смысла. После выполнения откроется страница в браузере, в которой следует войти в аккаунт Яндекс и предоставить программе запрошенные разрешения, после чего начнется исследование облачного хранилища Яндекс на наличие необходимых материалов. Под объектив программы попадает только папка \"EWXTelegramBot\" в корневой папке облачного хранилища.";
                                            break;
                                        }
                                    case "forcesync":
                                        {
                                            cmessage = "forceSync - Команда, запускающая немедленную синхронизацию с облачным хранилищем Яндекс. Используется для подгрузки новых материалов вне графика. Команду следует выполнять только после авторизации программы в сервисах яндекса. В ином случае программа оповестит, что действие невозможно.";
                                            break;
                                        }
                                    case "getdataholder":
                                        {
                                            cmessage = "getDataHolder - Команда, генерирующая краткий отчет о синхронизированных на момент выполнения материалах в локальном хранилище. Информацию о количестве опубликованных материалов, неопубликованных и общие суммы.";
                                            break;
                                        }
                                    case "getupdates":
                                        {
                                            cmessage = "";
                                            break;
                                        }
                                    case "installupdates":
                                        {
                                            cmessage = "";
                                            break;
                                        }
                                    case "eraseconfig":
                                        {
                                            cmessage = "";
                                            break;
                                        }
                                    case "eraselogs":
                                        {
                                            cmessage = "";
                                            break;
                                        }
                                    case "eraseaudios":
                                        {
                                            cmessage = "";
                                            break;
                                        }
                                    case "eraseimages":
                                        {
                                            cmessage = "";
                                            break;
                                        }
                                    case "eraselocalfolder":
                                        {
                                            cmessage = "";
                                            break;
                                        }
                                    case "erasedatafolder":
                                        {
                                            cmessage = "";
                                            break;
                                        }
                                    case "restartlogger":
                                        {
                                            cmessage = "";
                                            break;
                                        }
                                    case "help":
                                        {
                                            cmessage = "";
                                            break;
                                        }
                                    default:
                                        {
                                            cmessage = "Ой, кажется в базе данных Помощника нет информации о такой команде, как \"" + cmd.GetCommand() + "\". Убедитесь, верно ли вы ее ввели";
                                            break;
                                        }
                                }
                                break;
                            }
                            else
                            {
                                cmessage = "Список доступных команд:" + Environment.NewLine;
                                cmessage += "start - Запустить главный вычислительный модуль бота, если он не запустился автоматически" + Environment.NewLine;
                                cmessage += "stop - Остановить выполнение программы сервера, данные сохранятся перед завершением работы" + Environment.NewLine;
                                cmessage += "saveAll - Сохранить текущую конфигурацию" + Environment.NewLine;
                                cmessage += "setPostInterval - Установить временной интервал между публикациями" + Environment.NewLine;
                                cmessage += "setLongpollTimeout - Установить интервал опроса LongPoll сервера Telegram" + Environment.NewLine;
                                cmessage += "setSecretLength - Установить длину секретного кода" + Environment.NewLine;
                                cmessage += "generateSecret - Сгенерировать секретный код добавления администратора" + Environment.NewLine;
                                cmessage += "generateChannelSecret - Сгенерировать секретный код добавления канала" + Environment.NewLine;
                                cmessage += "invite - Активировать слушатель приглашений" + Environment.NewLine;
                                cmessage += "forcePost - Немедленная публикация" + Environment.NewLine;
                                cmessage += "forceSync - Немедленная синхронизация данных с Yandex" + Environment.NewLine;
                                cmessage += "getStatus - Отобразить текущий статус" + Environment.NewLine;
                                cmessage += "getDetStatus - Отобразить текущий статус в деталях" + Environment.NewLine;
                                cmessage += "getUpdates - Проверить наличие обновлений" + Environment.NewLine;
                                cmessage += "allowPostAgain - Разрешить использовать материалы повторно" + Environment.NewLine;
                                cmessage += "denyPostAgain - Запретить использовать материалы повторно" + Environment.NewLine;
                                cmessage += "eraseConfig - Очистить файл конфигурации (Deprecated)" + Environment.NewLine;
                                cmessage += "eraseAudios - Очистить локальную папку аудиозаписей (Deprecated)" + Environment.NewLine;
                                cmessage += "eraseImages - Очистить локальную папку изображений (Deprecated)" + Environment.NewLine;
                                cmessage += "eraseLocalFolder - Очистить локальное хранилище данных (Deprecated)" + Environment.NewLine;
                                cmessage += "eraseDataFolder - Очистить папку данных всей программы (Warning, deprecated)" + Environment.NewLine;
                                cmessage += "restartLogger - Перезагрузить логгер" + Environment.NewLine;
                                cmessage += "help - Отобразить информацию о командах" + Environment.NewLine;
                                cmessage += "Для получения более детальной информации по конкретной команде используйте \"help <command>\". Пример \"help forcePost\"";
                            }
                            break;
                        }
                    default:
                        {
                            cmessage = "Неизвестная команда \"" + cmd.GetCommand() + "\". Используйте \"help\", чтобы получить информацию о командах";
                            break;
                        }
                }

                if (string.IsNullOrEmpty(from)) EWXTelegramBot.PrintLine(cmessage);
                else EWXTelegramBot.GetController().SendMessage(from, cmessage);

                if (stop)
                {
                    _allowListen = false;
                    EWXTelegramBot.StopAll();
                }
                /*
                if (_postFlag)
                {
                    if (EWXTelegramBot.GetController().CheckUpdates()) EWXTelegramBot.GetController().SendMessage("A new version of EWX Telegram bot is available now! Use \"installUpdates\" command to start updating");
                    EWXTelegramBot.SaveAll();
                    _postFlag = false;
                }
                */
                return false;
            }
            catch (Exception e)
            {
                EWXTelegramBot.StackTrace(this.GetType().Name, "HandleCommand", e.GetType().ToString(), e.Message, e.StackTrace, ("EWXCommand>Command=" + cmd.GetCommand()));
            }
            return false;
        }
    }
}
