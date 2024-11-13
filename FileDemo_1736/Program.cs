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

        //此變數用來儲存檔案最後修改內容
        /// <summary>
        /// 最後內容
        /// </summary>
        public Dictionary<string,string>LastWriteContent= new Dictionary<string,string>();

        // 靜態計時器變數
        public Timer timer;

        static void Main(string[] args)
        {
         
            var program = new Program();
            program.StartMonitoring();

            Console.WriteLine("按任意鍵停止監控...");
            //為按下任意鍵後停止監測
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

            // 創建並啟動定時器
            //(第二個參數)null 就是看有沒有要傳遞甚麼資訊給 回調方法中(第一個參數)
            //TimeSpan.Zero為延遲時間(立即觸發)
            //TimeSpan.FromSeconds(5)間隔時間;定時器每次觸發回調方法之間的時間間隔
            //每隔五秒觸發CheckFileChanges方法檢查檔案變更
            timer = new Timer(CheckFileChanges, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));

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
        //剛開始在建立環境時先記錄過一次初始內容
        private void InitializeFiles()
        {
            foreach (var file in FilesToMonitor)
            {
                //結合路徑
                string filePath = Path.Combine(CustomDirectoryToWatch, file);

                // Directory.Exists為檢查路徑是否存在，如果存在回傳true
                if (!Directory.Exists(CustomDirectoryToWatch))
                {
                    //創建一個新的資料夾
                    Directory.CreateDirectory(CustomDirectoryToWatch);
                    Console.WriteLine($"資料夾 '{CustomDirectoryToWatch}' 已創建。");
                }

                // 確保檔案存在，若不存在則創建
                if (!File.Exists(filePath))
                {
                    //這個方法File.WriteAllText是將指定內容寫到檔案中
                    File.WriteAllText(filePath, $"這是 {file} 的初始內容。");
                    Console.WriteLine($"檔案 '{file}' 已創建並放置在資料夾 '{CustomDirectoryToWatch}' 中。");
                }

                // 將檔案的最後修改時間存到LastWriteTimes與內容存到LastWriteContent
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
        private async void CheckFileChanges(object state)
        {

            foreach (var file in FilesToMonitor)
            {
                string filePath = Path.Combine(CustomDirectoryToWatch, file);

                //取得檔案最後修改時間
                //File.GetLastWriteTime為同步方法會娶的最後修改時間，並返還一個datetime
                //使用Task.Run啟動一個新的背景工作執行緒或任務，並異步執行可提高效能
                DateTime lastWriteTime = await Task.Run( ()=>File.GetLastWriteTime(filePath));

                // 如果檔案的修改時間有變動，則顯示變更訊息
                //LastWriteTimes.ContainsKey(filePath)為查看字典檔裡有無此key(路徑)
                //接著比對時間有無跟 lastWriteTime 有相同，若不同表示檔案有修改
                if (LastWriteTimes.ContainsKey(filePath) && LastWriteTimes[filePath] != lastWriteTime)
                {
                    Console.WriteLine($"檔案 '{file}' 已修改。");
                    Console.WriteLine($"最後修改時間: {lastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    await PrintFileContent(filePath);

                    // 更新檔案的最後修改時間
                    LastWriteTimes[filePath] = lastWriteTime;
                }
            
            }
        }

        // 顯示檔案內容的功能
        /// <summary>
        /// 顯示檔案內容
        /// </summary>
        /// <param name="filePath"></param>
        // Task 類型的主要方法可以幫助你啟動、等待、取消異步操作，並捕獲錯誤。
        private async Task PrintFileContent(string filePath)
        {
            try
            {  //File.ReadAllText讀取檔案當前內容
                string currentcontent = await Task.Run(()=>File.ReadAllText(filePath));
                //檢查字典檔裡有無相同key
                if (LastWriteContent.ContainsKey(filePath))
                {
                    //把上次檔案初始化紀錄的內容放到OldContent中
                    string OldContent =LastWriteContent[filePath];
                    //接著使用GetNewContent去比對新舊內容所以得到的NewContent是比對完的差異結果
                    string NewContent = GetNewContent(OldContent, currentcontent);
                    //IsNullOrWhiteSpace用來檢查字串是否為空或是空白字符或空字串
                    if (!string.IsNullOrWhiteSpace(NewContent))
                    {
                        Console.WriteLine($"新增內容:{NewContent}");
                    }
                    else
                    {
                        Console.WriteLine("沒有新增內容");
                    }
                }
                //再把目前新增的內容存到最後修改內容的變數中
                LastWriteContent[filePath] = currentcontent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"讀取檔案內容時發生錯誤: {ex.Message}");
            }
        }
        /// <summary>
        /// 取得新內容
        /// </summary>
        /// <param name="originalContent"></param>
        /// <param name="updateContent"></param>
        /// <returns></returns>
        public string GetNewContent(string originalContent,string updateContent)
        {
            int IndexofContent=updateContent.IndexOf(originalContent,StringComparison.Ordinal);
            if (IndexofContent >= 0)
            {
                if ( IndexofContent == 0)
                {
                    return updateContent.Substring(IndexofContent + originalContent.Length);
                }

            }
            //返回一個空字串
            return string.Empty;

        }
    }
}