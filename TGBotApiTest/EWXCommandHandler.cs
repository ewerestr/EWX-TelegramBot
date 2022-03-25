using me.ewerestr.ewxtelegrambot.Utils;
using System;

namespace me.ewerestr.ewxtelegrambot.Components
{
    class EWXCommandHandler
    {
        private bool _allowListen = false;
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
                    EWXCommand cmd = new EWXCommand(line);
                    HandleCommand(cmd);
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
                    /*
                    case "setposttime": // WILL BE DELETED
                        {

                            if (cmd.HasArguments())
                            {
                                if (cmd.GetArgumentsArray()[0].Contains(":"))
                                {
                                    string[] lStringArr = cmd.GetArgumentsArray()[0].Split(':');
                                    byte[] time = new byte[2];
                                    if (byte.TryParse(lStringArr[0], out time[0]) && byte.TryParse(lStringArr[1], out time[1]))
                                    {
                                        EWXTelegramBot.GetController().SetTimeToPost(time);
                                        cmessage = "Успех! Новое время публикации установлено!";
                                    }
                                    else cmessage = "Некорректные аргументы команды. Пример: \"setPostTime 9:15\"";
                                }
                                else cmessage = "Некорректные аргументы команды. Пример: \"setPostTime 9:15\"";
                            }
                            else cmessage = "Программа должна иметь аргументы. Пример: \"setPostTime 9:15\"";
                            break;
                        }
                    */
                    case "setdeviation":
                        {
                            if (cmd.HasArguments())
                            {
                                int lInt;
                                if (int.TryParse(cmd.GetArgumentsArray()[0], out lInt))
                                {
                                    EWXTelegramBot.GetController().SetDeviation(lInt);
                                    cmessage = "Успех! Новый интервал погрешности установлен!";
                                }
                                else cmessage = "Некорректные аргументы команды. Пример: \"setDeviation 10\"";
                            }
                            else cmessage = "Команда должна иметь аргументы. Пример: \"setDeviation 10\"";
                            break;
                        }
                    case "setrefreshcooldown":
                        {
                            if (cmd.HasArguments())
                            {
                                int lInt;
                                if (int.TryParse(cmd.GetArgumentsArray()[0], out lInt))
                                {
                                    EWXTelegramBot.GetController().SetRefreshCooldown(lInt);
                                    cmessage = "Успех! Новый интервал тактов главного модуля установлен!";
                                }
                                else cmessage = "Некорректные аргументы команды. Пример: \"setRefreshCooldown 60\"";
                            }
                            else cmessage = "Команда должна иметь аргументы. Пример: \"setRefreshCooldown 60\"";
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
                                int[] dt = new int[] {DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second};
                                if (cmd.GetArgumentsCount() > 1)
                                {
                                    string starttime = cmd.GetArgumentsArray()[1];
                                    if (starttime.Contains(":"))
                                    {
                                        string[] q = starttime.Split(':');
                                        if (int.TryParse(q[0], out dt[3]) && int.TryParse(q[1], out dt[4]))
                                        {
                                            int a = 0;
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
                                    cmessage = "Успех! Интервал публикаций изменен";
                                    //EWXTelegramBot.GetController()._postInterval = nd; // WILL BE DELETED
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
                    case "setinvites":
                        {
                            if (cmd.HasArguments())
                            {
                                int lInt;
                                if (int.TryParse(cmd.GetArgumentsArray()[0], out lInt))
                                {
                                    EWXTelegramBot.GetController().SetInvitesCount(lInt);
                                    cmessage = "Количество допустимых приглашений установлено на " + lInt;
                                }
                                else cmessage = "Некорректные аргументы команды. Аргумент должен быть числом. Пример: \"setInvites 2\"";
                            }
                            else cmessage = "Команда должна иметь аргументы. Пример: \"setInvites 2\"";
                            break;
                        }
                    case "forcepost": // deprecated
                        {
                            if (EWXTelegramBot.GetController().GetStatus().Equals(EWXComponentStatus.Working))
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
                        /*
                    case "reloadconfig":
                        {
                            EWXTelegramBot.GetController().LoadConfig(true);
                            cmessage = "Configuration has been reloaded!";
                            break;
                        }
                        */
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
                                cmessage = "Упс... Эта возможность для команды \"help\" временно недоступна, так как еще не до конца реализована";
                                break;
                            }
                            cmessage = "Список доступных команд:" + Environment.NewLine;
                            cmessage += "start - Запустить главный вычислительный модуль бота, если он не запустился автоматически" + Environment.NewLine;
                            cmessage += "stop - Остановить выполнение программы сервера, данные сохранятся перед завершением работы" + Environment.NewLine;
                            cmessage += "saveAll - Сохранить текущую конфигурацию" + Environment.NewLine;
                            cmessage += "setPostTime - Установить время публикации" + Environment.NewLine;
                            cmessage += "setDeviation - Установить максимальную погрешность времени публикации" + Environment.NewLine;
                            cmessage += "setRefreshCooldown - Установить задержку между тактами главного цикла" + Environment.NewLine;
                            cmessage += "setLongpollTimeout - Установить интервал опроса LongPoll сервера Telegram" + Environment.NewLine;
                            cmessage += "setSecretLength - Установить длину секретного кода" + Environment.NewLine;
                            cmessage += "generateSecret - Сгенерировать секретный код добавления администратора" + Environment.NewLine;
                            cmessage += "generateChannelSecret - Сгенерировать секретный код добавления канала" + Environment.NewLine;
                            cmessage += "setInvites - Установить количество приглашений" + Environment.NewLine;
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
                            cmessage += "test - Сервисная команда. Если в ответ сервер вернет \"OK!\", значит что программа активна" + Environment.NewLine;
                            cmessage += "help - Отобразить информацию о командах" + Environment.NewLine;
                            cmessage += "Для получения более детальной информации по конкретной команде используйте \"help <command>\". Пример \"help forcePost\"";
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
