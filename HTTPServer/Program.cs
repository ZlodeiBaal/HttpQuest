using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Mail;

namespace HTTPServer
{
    // Класс-обработчик клиента
    class Client
    {
        // Отправка страницы с ошибкой
        private void SendError(TcpClient Client, int Code)
        {
            // Получаем строку вида "200 OK"
            // HttpStatusCode хранит в себе все статус-коды HTTP/1.1
            string CodeStr = Code.ToString() + " " + ((HttpStatusCode)Code).ToString();
            // Код простой HTML-странички
            string Html = "<html><body><h1>" + CodeStr + "</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = "HTTP/1.1 " + CodeStr + "\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);
            // Отправим его клиенту
            Client.GetStream().Write(Buffer, 0, Buffer.Length);
            // Закроем соединение
            Client.Close();
        }
        Random Rand = new Random();
        public void FormImgQuest(string fadress, StreamWriter Str, string qn)
        {
            //Считаем базу возможных квестов
            string[] files = File.ReadAllLines(fadress + "\\Base.txt");

            //Срандомим квест
            int Element = Rand.Next(0, files.Length);
            
            string[] Bites = files[Element].Split(':');
            //Создадим квест из изображения
            MakeQuest(fadress,  Str, Bites[0], Element,qn);
        }
        public void RememberImgQuest(string fadress, StreamWriter Str, int Element, string qn)
        {
            //Считаем базу возможных квестов
            string[] files = File.ReadAllLines(fadress + "\\Base.txt");
            //Срандомим квест
            string[] Bites = files[Element].Split(':');
            //Создадим квест из изображения
            MakeQuest(fadress, Str, Bites[0], Element, qn);
        }
        /// <summary>
        /// Делаем квест из изображения
        /// </summary>
        /// <param name="fadress"></param>
        /// <param name="Str"></param>
        /// <param name="name"></param>
        /// <param name="Element"></param>
        /// <param name="qn"></param>
        public void MakeQuest(string fadress, StreamWriter Str, string name, int Element, string qn)
        {
            WriteStringInStream( Str, @"<html><body><h1>Задание!</h1><br/>");
            WriteStringInStream( Str, @"<i>Что за человек на фотографии? Не знаешь? Он где-то в зале!</i> <br/>");
            WriteStringInStream(Str, @"<i>Найди и попробуй узнать как его зовут!!</i> <br/>");
            WriteStringInStream(Str, @"<img src=" + fadress + "\\" + name + @" alt=""подсказывать не буду"" width=""600"">");
            WriteStringInStream( Str, @"<form method=post enctype=multipart/form-data action=/answ:"+qn+":"+Element.ToString()+">");
            WriteStringInStream( Str, @"<input type=""text"" name=""textfield"" /><br />");
            WriteStringInStream( Str, @"<input type=submit value=Продолжить file/>");
            WriteStringInStream( Str, @"</form>");
        }
        public void WriteStringInStream(StreamWriter Str, string content)
        {
           // byte[] HeadersBuffer = Encoding.Unicode.GetBytes(content);//GetBytes(content);//
           // Str.Write(HeadersBuffer, 0, HeadersBuffer.Length);
            Str.WriteLine(content);
        }
        public void FormTextQuest(string fadress, StreamWriter Str, string qn)
        {
            //Прочитаем базу квестов
            string[] files = File.ReadAllLines(fadress + "\\Base.txt");
            //Срандомим квест
            int Element = Rand.Next(0, files.Length);
            string[] Bites = files[Element].Split(':');
            //Прочитаем файл текстовой задачи
            string s = File.ReadAllText(fadress + "\\" + Bites[0]);
            //Напишем текстовый квест
            MakeTextQuest(fadress, Str, s, Element, qn);
        }
        public void RememberTextQuest(string fadress, StreamWriter Str, int Element, string qn)
        {
            //Прочитаем базу квестов
            string[] files = File.ReadAllLines(fadress + "\\Base.txt");
            string[] Bites = files[Element].Split(':');
            //Прочитаем файл текстовой задачи
            string s = File.ReadAllText(fadress + "\\" + Bites[0]);
            //Напишем текстовый квест
            MakeTextQuest(fadress, Str, s, Element, qn);
        }
        /// <summary>
        /// Текстовый квест формулируем
        /// </summary>
        /// <param name="fadress"></param>
        /// <param name="Str"></param>
        /// <param name="name"></param>
        /// <param name="Element"></param>
        /// <param name="qn"></param>
        public void MakeTextQuest(string fadress, StreamWriter Str, string name, int Element, string qn)
        {
            WriteStringInStream(Str, @"<html><body><h1>Задание!</h1><br/>");
            WriteStringInStream(Str, @"<i>"+name+"</i> <br/>");
            WriteStringInStream(Str, @"<form method=post enctype=multipart/form-data action=/answ:" + qn + ":" + Element.ToString() + ">");
            WriteStringInStream(Str, @"<input type=""text"" name=""textfield"" /><br />");
            WriteStringInStream(Str, @"<input type=submit value=Продолжить file/>");
            WriteStringInStream(Str, @"</form>");
        }
        /// <summary>
        /// Проверка того чо изображение угадано правильно
        /// </summary>
        /// <param name="f">адрес папки</param>
        /// <param name="ans">данный ответ</param>
        /// <param name="num">номер квеста</param>
        /// <returns></returns>
        public bool CheckPic(string f, string ans, int num)
        {
            string[] files = File.ReadAllLines(f + "\\Base.txt");
            string[] Bites = files[num].Split(':');
            //Проверяем все возможные ответы
            for (int i=0;i<Bites.Length;i++)
                if (Bites[i] == ans)
                    return true;
            return false;
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public Client(TcpClient Client)
        {
            // Объявим строку, в которой будет хранится запрос клиента

            string Request = "";
            // Буфер для хранения принятых от клиента данных
            byte[] Buffer = new byte[1024];
            // Переменная для хранения количества байт, принятых от клиента
            int Count;
            // Читаем из потока клиента до тех пор, пока от него поступают данные
            while ((Count = Client.GetStream().Read(Buffer, 0, Buffer.Length)) > 0)
            {
                // Преобразуем эти данные в строку и добавим ее к переменной Request
                Request += Encoding.UTF8.GetString(Buffer, 0, Count);
                // Запрос должен обрываться последовательностью \r\n\r\n
                // Либо обрываем прием данных сами, если длина строки Request превышает 4 килобайта
                // Нам не нужно получать данные из POST-запроса (и т. п.), а обычный запрос
                // по идее не должен быть больше 4 килобайт
                if (Request.IndexOf("\r\n\r\n") >= 0 || Request.Length > 4096)
                {
                    break;
                }
            }

            // Парсим строку запроса с использованием регулярных выражений
            // При этом отсекаем все переменные GET-запроса
            Match ReqMatch = Regex.Match(Request, @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");

            // Если запрос не удался
            if (ReqMatch == Match.Empty)
            {
                // Передаем клиенту ошибку 400 - неверный запрос
                SendError(Client, 400);
                return;
            }

            // Получаем строку запроса
            string RequestUri = ReqMatch.Groups[1].Value;

            // Приводим ее к изначальному виду, преобразуя экранированные символы
            // Например, "%20" -> " "
            RequestUri = Uri.UnescapeDataString(RequestUri);


            // Если строка запроса оканчивается на "/", то добавим к ней index.html
            bool FirstTime = true;
            int NumofFirstTime = 0;
            try
            {
                //Если пришла стартовая ссылка
                if (RequestUri.EndsWith("/"))
                {
                    //Тут у нас лежит приветственный инвайт
                    RequestUri = "/www/index2.html";
                }
                else
                {
                    //Проверяем, есть ли в запросе поле "ответ"
                    int its = Request.IndexOf("textfield");
                    if (its != -1)
                    {
                        //Выредем нужные нам куски
                        string s = RequestUri.Substring(1);
                        string[] Bites = s.Split(':');
                        string ss = Request.Substring(its + 14);
                        int t = ss.IndexOf("\r\n");
                        string Text = ss.Substring(0, t);
                        switch (Bites[1])
                        {
                            //Проверяем ответ на первый вопрос
                            case "pic1":
                                if (CheckPic("Anna", Text, int.Parse(Bites[2])))
                                    //Если правильно
                                    RequestUri = "/2";
                                else
                                {
                                    //Если нет повторим тот же вопрос
                                    RequestUri = "/1";
                                    FirstTime = false;
                                    NumofFirstTime = int.Parse(Bites[2]);
                                }
                                break;
                                //Аналогично проверяем ответ на второй вопрос
                            case "pic2":
                                if (CheckPic("Anton", Text, int.Parse(Bites[2])))
                                    RequestUri = "/3";
                                else
                                {
                                    RequestUri = "/2";
                                    FirstTime = false;
                                    NumofFirstTime = int.Parse(Bites[2]);
                                }
                                break;
                            //Аналогично проверяем ответ на третий вопрос
                            case "quest1":
                                if (CheckPic("q1", Text, int.Parse(Bites[2])))
                                    RequestUri = "/4";
                                else
                                {
                                    RequestUri = "/3";
                                    FirstTime = false;
                                    NumofFirstTime = int.Parse(Bites[2]);
                                }
                                break;
                            //Аналогично проверяем ответ на четвёртый вопрос
                            case "quest2":
                                if (CheckPic("q2", Text, int.Parse(Bites[2])))
                                    RequestUri = "/www/second_one.html";
                                else
                                {
                                    RequestUri = "/4";
                                    FirstTime = false;
                                    NumofFirstTime = int.Parse(Bites[2]);
                                }
                                break;
                            case "win":
                                //Если мы на странице с победой - отошлём имя победителя двум людям
                                RequestUri = "/www/second_one2.html";
                                String smtpHost = "smtp.yandex.ru";
                                //Порт SMTP-сервера
                                int smtpPort = 25;
                                //Логин
                                String smtpUserName = "TestForHabr";
                                //Пароль
                                String smtpUserPass = "Habrahabr";
                                //Создание подключения
                                SmtpClient client = new SmtpClient(smtpHost, smtpPort);
                                client.EnableSsl = true;
                                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                                client.UseDefaultCredentials = false;
                                client.Credentials = new NetworkCredential(smtpUserName, smtpUserPass);
                                string[] adress = new string[2];
                                adress[0] = "TestForHabr@yandex.ru";
                                adress[1] = "TestForHabr@yandex.ru";
                                for (int i = 0; i < 2; i++)
                                {
                                    //Адрес для поля "От"
                                    String msgFrom = "TestForHabr@yandex.ru";
                                    //Адрес для поля "Кому" (адрес получателя)
                                    String msgTo = adress[i];
                                    //Тема письма
                                    String msgSubject = "Есть победитель!";
                                    //Текст письма
                                    String msgBody = Text;
                                    //Вложение для письма
                                    //Если нужно больше вложений, для каждого вложения создаем свой объект Attachment с нужным путем к файлу
                                    //Attachment attachData = new Attachment(options.SaveAdress);
                                    client.EnableSsl = true;
                                    //Создание сообщения
                                    MailMessage message = new MailMessage(msgFrom, msgTo, msgSubject, msgBody);
                                    //Крепим к сообщению подготовленное заранее вложение
                                    //message.Attachments.Add(attachData);

                                    try
                                    {
                                        //Отсылаем сообщение
                                        client.Send(message);
                                    }
                                    catch (SmtpException ex)
                                    {
                                        //В случае ошибки при отсылке сообщения можем увидеть, в чем проблема

                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (RequestUri.Length > 0)
                            if (!File.Exists(RequestUri.Substring(1)))
                                RequestUri = "/1";
                    }

                }
            }
            catch
            {
                
            }


            string FilePath = "";
            if (RequestUri.Length>0)
                FilePath = RequestUri.Substring(1);
            string ContentType = "text/html";
            
            // Открываем файл, страхуясь на случай ошибки
            FileStream FS = null;
            //Если у нас текущий индентификатор - адрес файла - откроем его
            if (File.Exists(FilePath))
                try
                {

                    FS = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (Exception)
                {
                    // Если случилась ошибка, посылаем клиенту ошибку 500
                    SendError(Client, 500);
                    return;
                }

            // Посылаем заголовки
            string Headers = "HTTP/1.1 200 OK\nContent-Type: " + ContentType + "\nConnection:close" + "\n\n";// "\nContent-Length: " + FS.Length + "\n\n";
            byte[] HeadersBuffer = Encoding.ASCII.GetBytes(Headers);
            Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);

            // Пока не достигнут конец файла
            //Если есть файл - открываем его
            if (FS != null)
            {
                while (FS.Position < FS.Length)
                {
                    // Читаем данные из файла
                    Count = FS.Read(Buffer, 0, Buffer.Length);
                    // И передаем их клиенту
                    Client.GetStream().Write(Buffer, 0, Count);
                }

                    FS.Close();
                Client.Close();
            }
            else
            {
                //Если нет файла - фантазируем сами и пишем напрямую в стрим
                StreamWriter sw = new StreamWriter(new BufferedStream(Client.GetStream()), Encoding.Unicode);

                switch (RequestUri)
                {
                        //Выберем квест для первой задачи
                    case "/1":
                        //Если задача поставлена впервые
                        if (FirstTime)
                            FormImgQuest("Anna", sw,"pic1");
                        else
                            //Если задача поставлена не впервые
                            RememberImgQuest("Anna", sw, NumofFirstTime, "pic1");
                        break;
                    case "/2":
                        if (FirstTime)
                            FormImgQuest("Anton", sw, "pic2");
                        else
                            RememberImgQuest("Anton", sw, NumofFirstTime, "pic2");
                        break;
                    case "/3":
                        if (FirstTime)
                            FormTextQuest("q1", sw, "quest1");
                        else
                            RememberTextQuest("q1", sw, NumofFirstTime, "quest1");
                        break;
                    case "/4":
                        if (FirstTime)
                            FormTextQuest("q2", sw, "quest2");
                        else
                            RememberTextQuest("q2", sw, NumofFirstTime, "quest2");
                        break;
                }
                try
                {
                    sw.Flush();
                    sw.Close();
                }
                catch
                {

                }
            }

            // Закроем файл и соединение
            
        }
    }


    

    class Server
    {
        TcpListener Listener; // Объект, принимающий TCP-клиентов

        // Запуск сервера
        public Server(int Port)
        {
            Listener = new TcpListener(IPAddress.Any, Port); // Создаем "слушателя" для указанного порта
            Listener.Start(); // Запускаем его

            // В бесконечном цикле
            while (true)
            {
                // Принимаем новых клиентов. После того, как клиент был принят, он передается в новый поток (ClientThread)
                // с использованием пула потоков.
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), Listener.AcceptTcpClient());

                /*
                // Принимаем нового клиента
                TcpClient Client = Listener.AcceptTcpClient();
                // Создаем поток
                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                // И запускаем этот поток, передавая ему принятого клиента
                Thread.Start(Client);
                */
            }
        }

        static void ClientThread(Object StateInfo)
        {
            // Просто создаем новый экземпляр класса Client и передаем ему приведенный к классу TcpClient объект StateInfo
            new Client((TcpClient)StateInfo);
        }

        // Остановка сервера
        ~Server()
        {
            // Если "слушатель" был создан
            if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }
        }

        static void Main(string[] args)
        {
            // Определим нужное максимальное количество потоков
            // Пусть будет по 4 на каждый процессор
            int MaxThreadsCount = Environment.ProcessorCount * 4;
            // Установим максимальное количество рабочих потоков
            ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            // Установим минимальное количество рабочих потоков
            ThreadPool.SetMinThreads(2, 2);
            // Создадим новый сервер на порту 80
            new Server(777);
        }
    }
}
