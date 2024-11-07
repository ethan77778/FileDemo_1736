namespace FileDemo_1736
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //@讓後面字串變為逐字字串,就不須特別處理反斜線(\)，若沒加必須使用兩個反斜線 \\ 來表示一個反斜線
            //string DirectoryToWatch = @"C:\Users\user\Desktop\FileDemo_1736\FileDemo_1736"; // 監控的應該是目錄
            //string[] FilesToMonitor = { "apple.txt", "banana.txt" };


            // 設定要監控的目錄和檔案的自定義路徑
            // 設定要監控的資料夾路徑
            string CustomDirectoryToWatch = @"C:\Users\user\Desktop\apple"; // 目標資料夾
            string[] FilesToMonitor = { "apple.txt", "banana.txt" };

            // 確保資料夾存在，若不存在則創建
            //Directory.Exists這方法為驗證資料夾是否位置相符
            if (!Directory.Exists(CustomDirectoryToWatch))
            {
                Directory.CreateDirectory(CustomDirectoryToWatch); // 創建資料夾
                Console.WriteLine($"資料夾 '{CustomDirectoryToWatch}' 已創建。");
            }

            // 創建檔案並將其放入該資料夾
            foreach (var file in FilesToMonitor)
            {
                //Path.Combine會自動合併兩個路徑 並自動加入\(反斜線)若有多餘則會自動刪除
                string filePath = Path.Combine(CustomDirectoryToWatch, file); // 路徑結合檔案名稱

                if (!File.Exists(filePath))  // 檢查檔案是否已存在
                {
                    // 創建檔案並寫入初始內容
                    File.WriteAllText(filePath, $"這是 {file} 的初始內容。");
                    Console.WriteLine($"檔案 '{file}' 已創建並放置在資料夾 '{CustomDirectoryToWatch}' 中。");
                }
            }
            // 使用 FileSystemWatcher 來監控檔案變動
            //FileSystemWatcher是用來監測檔案系統變化的類別
            FileSystemWatcher watcher = new FileSystemWatcher(CustomDirectoryToWatch);

            // 設定監控過濾條件
            //NotifyFilters.FileName當檔案名稱發生變化事件會被觸發
            //NotifyFilters.LastWrite當檔案內容被修改事件會觸發
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

            // 監控所有檔案，稍後會根據檔案名稱進行過濾
            // *.*，表示監控所有檔案類型
            watcher.Filter = "*.*";  

            // 
            //為 Changed 事件註冊了一個事件處理器。當監控目錄中的檔案發生變動時，這個事件處理器會被觸發
            //sender讓我們能夠知道是哪個 FileSystemWatcher 物件觸發了 Changed 事件
            //ex:當檔案經歷這些 NotifyFilters.FileName | NotifyFilters.LastWrite變動就會觸發sender傳遞給事件處理器
            //而e代表事件處理器的參數當觸發事件 e裡面包含檔案的更種資訊等 如位置
            watcher.Changed += (sender, e) =>
            {
                // 檢查檔案是否是我們要監控的檔案
                //Array.Exists是一個陣列方法她會檢查陣列中是否有符合的條件的元素，並返回true或false
                //Equals這方法用來檢查兩個字串是否相等(檢查陣列中檔案名稱與檔案名稱)
                //StringComparison.OrdinalIgnoreCase忽略大小寫的意思
                if (Array.Exists(FilesToMonitor, f => f.Equals(Path.GetFileName(e.FullPath), StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine($"檔案變動: {e.FullPath}");
                    // 讀取並打印檔案內容
                    PrintFileContent(e.FullPath);
                }
            };

            // 開始監控
            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"開始監控目錄: {CustomDirectoryToWatch}");

            // 防止程式結束，直到使用者按下任意鍵
            Console.WriteLine("按任意鍵停止監控...");
            Console.ReadKey();

            // 停止監控
            watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// 打印檔案內容
        /// </summary>
        /// <param name="filePath"></param>
        static void PrintFileContent(string filePath)
        {
            try
            {
                //File.ReadAllText 方法為從指定的檔案路徑（filePath）讀取整個檔案的內容，並把內容!!存在變數中
                string content = File.ReadAllText(filePath);
                Console.WriteLine("變動內容:");
                Console.WriteLine(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"讀取檔案內容時發生錯誤: {ex.Message}");
            }
        }
    }
}
//步驟流程->先查看有無此目錄位置與檔案->設定監控路徑->設定監控觸發條件->建立事件處理器當事件觸發該處理何事