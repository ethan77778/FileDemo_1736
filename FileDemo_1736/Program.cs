namespace FileDemo_1736
{
    internal class Program
    {
        // 設定要監控的資料夾路徑和檔案列表
        /// <summary>
        /// 檔案位置
        /// </summary>
        public string CustomDirectoryToWatch = @"C:\Users\user\Desktop\apple";
        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string[] FilesToMonitor = { "apple.txt", "banana.txt" };

        // 此變數用來儲存檔案的最後修改時間
        /// <summary>
        /// 檔案的最後修改時間
        /// </summary>
        public Dictionary<string, DateTime> LastWriteTimes = new Dictionary<string, DateTime>();

        //此變數用來儲存檔案內容
        /// <summary>
        /// 最後內容
        /// </summary>
        public Dictionary<string,string>LastWriteContent= new Dictionary<string,string>();

        // 靜態計時器變數
        public Timer timer;

        static void Main(string[] args)
        {
            // 創建 Program 類的實例Program 類的主要目的是進行檔案監控
            var program = new Program();
            program.StartMonitoring();

            Console.WriteLine("按任意鍵停止監控...");
            Console.ReadKey();

            // 停止計時器
            program.StopMonitoring();
        }

        // 初始化檔案並設置監控
        /// <summary>
        /// 設定監控時間
        /// </summary>
        public void StartMonitoring()
        {
            // 初始化檔案資料
            InitializeFiles();

            // 設置計時器，每 5 秒檢查一次檔案變化
            //TimeSpan.Zero為延遲時間
            //TimeSpan.FromSeconds(5)間隔時間
            timer = new Timer(CheckFileChanges, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            Console.WriteLine($"開始監控目錄: {CustomDirectoryToWatch}");
        }

        // 停止計時器
       /// <summary>
       /// 停止監控
       /// </summary>
        public void StopMonitoring()
        {
            //會安全地停止定時器並釋放資源
            timer?.Dispose();
        }

        /// <summary>
        /// 檢查資料夾檔案是否存在
        /// </summary>
        private void InitializeFiles()
        {
            foreach (var file in FilesToMonitor)
            {
                string filePath = Path.Combine(CustomDirectoryToWatch, file);

                // 確保資料夾存在
                if (!Directory.Exists(CustomDirectoryToWatch))
                {
                    Directory.CreateDirectory(CustomDirectoryToWatch);
                    Console.WriteLine($"資料夾 '{CustomDirectoryToWatch}' 已創建。");
                }

                // 確保檔案存在，若不存在則創建
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, $"這是 {file} 的初始內容。");
                    Console.WriteLine($"檔案 '{file}' 已創建並放置在資料夾 '{CustomDirectoryToWatch}' 中。");
                }

                // 記錄檔案的最後修改時間
                //GetLastWriteTime可以獲取檔案最後修改時間
                LastWriteTimes[filePath] = File.GetLastWriteTime(filePath);
                LastWriteContent[filePath]=File.ReadAllText(filePath);
            }
        }

        // 計時器的回調方法，用來定期檢查檔案是否有變動
        /// <summary>
        /// 檢查檔案是否有變更的方法
        /// </summary>
        /// <param name="state"></param>
        private void CheckFileChanges(object state)
        {
            foreach (var file in FilesToMonitor)
            {
                string filePath = Path.Combine(CustomDirectoryToWatch, file);

               
                DateTime lastWriteTime = File.GetLastWriteTime(filePath);

                // 如果檔案的修改時間有變動，則顯示變更訊息
                if (LastWriteTimes.ContainsKey(filePath) && LastWriteTimes[filePath] != lastWriteTime)
                {
                    Console.WriteLine($"檔案 '{file}' 已修改。");
                    Console.WriteLine($"最後修改時間: {lastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    PrintFileContent(filePath);

                    // 更新檔案的最後修改時間
                    LastWriteTimes[filePath] = lastWriteTime;
                }
            
            }
        }

        // 顯示檔案內容的功能
        private void PrintFileContent(string filePath)
        {
            try
            {
                string Currentcontent = File.ReadAllText(filePath);
                if (LastWriteContent.ContainsKey(filePath))
                {
                    string OldContent=LastWriteContent[filePath];
                    string NewContent = GetNewContent(OldContent, Currentcontent);
                    if (!string.IsNullOrWhiteSpace(NewContent))
                    {
                        Console.WriteLine($"新增內容:{NewContent}");
                    }
                    else
                    {
                        Console.WriteLine("沒有新增內容");
                    }
                }
                LastWriteContent[filePath] = Currentcontent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"讀取檔案內容時發生錯誤: {ex.Message}");
            }
        }
        public string GetNewContent(string OriginalContent,string UpdateContent)
        {
            int IndexofContent=UpdateContent.IndexOf(OriginalContent,StringComparison.Ordinal);
            if (IndexofContent >= 0)
            {
                if ( IndexofContent == 0)
                {
                    return UpdateContent.Substring(IndexofContent + OriginalContent.Length);
                }

            }
            return string.Empty;

        }
    }
}