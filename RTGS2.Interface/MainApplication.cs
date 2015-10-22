using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RTGS2.BL;
using RTGS2.BL.Repository;
using System.Configuration;
using Code.Common;

namespace RTGS2.Interface
{
    public class MainApplication
    {
        private int countdownTimer;
        private int timerInterval;
        public string ftpServerRtgs;
        public string ftpUser;
        public string ftpPassword;
        private bool isProcessingTransaction;

        private ConsoleLogLibrary log = new ConsoleLogLibrary("log");

        public MainApplication()
        {
            InitializeConfiguration();

            log.Write("Application Start");
            Console.WriteLine();

            StartTimer();
        }

        private AppConfiguration InitializeConfiguration()
        {
            string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\config.json";
            var config = new AppConfiguration();
            config.SetConfiguration(configFilePath);

            timerInterval = config.Interval;

            return config;
        }

        private void StartTimer()
        {
            countdownTimer = timerInterval;
            Timer timer = new Timer(TimerCallback, null, 0, 1000);

            Console.ReadLine();
        }

        private void TimerCallback(object state)
        {
            // Restart countdown timer jika countdownTimer < 0
            if (countdownTimer < 0) countdownTimer = timerInterval;

            if (!isProcessingTransaction)
            {
                WriteCountdownTimer();

                if (countdownTimer == timerInterval)
                {
                    isProcessingTransaction = true;

                    var config = InitializeConfiguration();
                    var transactionController = new TransactionController(_config: config, _log: log);
                    isProcessingTransaction = transactionController.ProcessAllTransactions();

                }

                countdownTimer -= 1;
            }
        }

        private void WriteCountdownTimer()
        {
            int currentCursorY = Console.CursorTop;
            string message = "Next batch starts in " + countdownTimer.ToString();
            Console.WriteLine(message + new string(' ', Console.BufferWidth - message.Length));
            Console.SetCursorPosition(0, currentCursorY);
        }


    }
}
