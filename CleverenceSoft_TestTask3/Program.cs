using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CleverenceSoft_TestTask3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string inputFile = "input.log";
            string outputFile = "output.log";
            string problemsFile = "problems.log";

            // Создаем тестовый input.log
            try
            {
                using (StreamWriter sw = new StreamWriter(inputFile, false, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine("10.03.2025 15:14:49.523 INFORMATION  Версия программы: '3.4.0.48729'");
                    sw.WriteLine("2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'");
                    sw.WriteLine("Невалидная строка");
                    sw.WriteLine("11.03.2025 16:22:11.777 WARNING Что-то пошло не так");
                    sw.WriteLine("2025-03-11 17:33:44.8888| DEBUG|123|AnotherClass.DoSomething| Сообщение от AnotherClass");
                    sw.WriteLine("12.03.2025 18:44:55.999 ERROR  Критическая ошибка");
                    sw.WriteLine("2025-03-12 19:55:00.1111| INFO|456|YetAnotherClass.ProcessData| Данные успешно обработаны");
                    sw.WriteLine("13.03.2025 20:06:11.222 INFORMATION  Запуск процесса");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при создании тестового файла: {e.Message}");
                return; // Завершаем программу, если не удалось создать тестовый файл.
            }


            try
            {
                ProcessLogFile(inputFile, outputFile, problemsFile);

                Console.WriteLine("\nСодержимое output.log:");
                if (File.Exists(outputFile))
                {
                    Console.WriteLine(File.ReadAllText(outputFile));
                }
                else
                {
                    Console.WriteLine($"Файл {outputFile} не найден.");
                }

                Console.WriteLine("\nСодержимое problems.log:");
                if (File.Exists(problemsFile))
                {
                    Console.WriteLine(File.ReadAllText(problemsFile));
                }
                else
                {
                    Console.WriteLine($"Файл {problemsFile} не найден.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла общая ошибка: {e.Message}");
            }
        }

        static void ProcessLogFile(string inputFile, string outputFile, string problemsFile)
        {
            try
            {
                using (StreamReader reader = new StreamReader(inputFile))
                using (StreamWriter writer = new StreamWriter(outputFile, false))
                using (StreamWriter problemsWriter = new StreamWriter(problemsFile, false))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string standardizedLine = StandardizeLogEntry(line.Trim());
                        if (standardizedLine != null)
                        {
                            writer.WriteLine(standardizedLine);
                        }
                        else
                        {
                            problemsWriter.WriteLine(line);
                        }
                    }
                }
                Console.WriteLine($"Файл {inputFile} успешно обработан.");
                Console.WriteLine($"Результат сохранен в {outputFile}.");
                Console.WriteLine($"Невалидные записи сохранены в {problemsFile}.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Ошибка: Файл {inputFile} не найден.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка при обработке файла: {e.Message}");
            }
        }

        static string StandardizeLogEntry(string logEntry)
        {
            // Формат 1
            Match match1 = Regex.Match(logEntry, @"(\d{2}\.\d{2}\.\d{4})\s+(\d{2}:\d{2}:\d{2}\.\d{3})\s+([A-Z]+)\s+(.*)");
            if (match1.Success)
            {
                string date = match1.Groups[1].Value;
                string time = match1.Groups[2].Value;
                string logLevel = match1.Groups[3].Value;
                string message = match1.Groups[4].Value.Trim();
                string callingMethod = "DEFAULT";

                // Преобразование даты
                string[] dateParts = date.Split('.');
                date = $"{dateParts[0]}-{dateParts[1]}-{dateParts[2]}";

                // Преобразование уровня логирования
                logLevel = GetStandardizedLogLevel(logLevel);

                return $"{date}\t{time}\t{logLevel}\t{callingMethod}\t{message}";
            }

            // Формат 2
            Match match2 = Regex.Match(logEntry, @"(\d{4}-\d{2}-\d{2})\s+(\d{2}:\d{2}:\d{2}\.\d+)\| ([A-Z]+)\|(\d+)\|([a-zA-Z0-9\.]+)\| (.*)");
            if (match2.Success)
            {
                string date = match2.Groups[1].Value;
                string time = match2.Groups[2].Value;
                string logLevel = match2.Groups[3].Value;
                string callingMethod = match2.Groups[5].Value;
                string message = match2.Groups[6].Value.Trim();

                // Преобразование даты
                string[] dateParts = date.Split('-');
                date = $"{dateParts[2]}-{dateParts[1]}-{dateParts[0]}";

                // Преобразование уровня логирования
                logLevel = GetStandardizedLogLevel(logLevel);

                return $"{date}\t{time}\t{logLevel}\t{callingMethod}\t{message}";
            }

            // Невалидная запись
            return null;
        }

        static string GetStandardizedLogLevel(string logLevel)
        {
            logLevel = logLevel.ToUpper();
            switch (logLevel)
            {
                case "INFORMATION": return "INFO";
                case "WARNING": return "WARN";
                case "ERROR": return "ERROR";
                case "DEBUG": return "DEBUG";
                case "INFO": return "INFO";
                case "WARN": return "WARN";
                default: return logLevel; // Возвращаем исходное значение, если не удалось преобразовать
            }
        }
    }
}