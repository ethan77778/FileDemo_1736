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
            string customDirectoryToWatch = @"C:\Users\user\Desktop\apple"; // 目標資料夾
            string[] filesToMonitor = { "apple.txt", "banana.txt" };

            // 確保資料夾存在，若不存在則創建
            //Directory.Exists這方法為驗證資料夾是否位置相符
            if (!Directory.Exists(customDirectoryToWatch))
            {
                Directory.CreateDirectory(customDirectoryToWatch); // 創建資料夾
                Console.WriteLine($"資料夾 '{customDirectoryToWatch}' 已創建。");
            }

            // 創建檔案並將其放入該資料夾
            foreach (var file in filesToMonitor)
            {
                //Path.Combine會自動合併兩個路徑 並自動加入\(反斜線)若有多餘則會自動刪除
                string filePath = Path.Combine(customDirectoryToWatch, file); // 路徑結合檔案名稱

                if (!File.Exists(filePath))  // 檢查檔案是否已存在
                {
                    // 創建檔案並寫入初始內容
                    File.WriteAllText(filePath, $"這是 {file} 的初始內容。");
                    Console.WriteLine($"檔案 '{file}' 已創建並放置在資料夾 '{customDirectoryToWatch}' 中。");
                }
            }
            // 使用 FileSystemWatcher 來監控檔案變動
            //FileSystemWatcher是用來監測檔案系統變化的類別
            FileSystemWatcher watcher = new FileSystemWatcher(customDirectoryToWatch);

            // 設定監控過濾條件
            //NotifyFilters.FileName當檔案名稱發生變化事件會被觸發
            //NotifyFilters.LastWrite當檔案內容被修改事件會觸發
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

            // 監控所有檔案，稍後會根據檔案名稱進行過濾
            // *.*，表示監控所有檔案類型
            watcher.Filter = "*.*";

            var saveOldContents = new Dictionary<string, string>();

            //使用迴圈先提取先前檔案內容 避免因剛開始變數初始化而導致無法記錄先前內容
            foreach (var file in filesToMonitor)
            {
                string filePath = Path.Combine(customDirectoryToWatch, file);
                //驗證檔案路徑是否正確
                if (File.Exists(filePath))
                {
                    // 讀取最初檔案內容並存入字典
                    string originalFileContents = File.ReadAllText(filePath);
                    saveOldContents[filePath] = originalFileContents;
                    Console.WriteLine($"檔案 '{file}' 的初始內容已存入字典。");
                }
            }


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
                //Path.GetFileName它的作用是從完整的檔案路徑中提取檔案名稱
                //當觸發條件時會先檢查是否為要監測的檔案
                if (Array.Exists(filesToMonitor, f => f.Equals(Path.GetFileName(e.FullPath), StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine($"檔案變動: {e.FullPath}");
                    // 讀取並打印檔案內容
                    //e.FullPath會返還完整路徑
                    PrintFileContent(e.FullPath, saveOldContents);
                }
            };

            // 會啟用檔案系統變更的監控
            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"開始監控目錄: {customDirectoryToWatch}");

            // 防止程式結束，直到使用者按下任意鍵
            Console.WriteLine("按任意鍵停止監控...");
            Console.ReadKey();

            // 停止監控
            watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// 讀取檔案內容
        /// </summary>
        /// <param name="filePath"></param>
        public static void PrintFileContent(string filePath, Dictionary<string, string> conTents)
        {
            try
            {
                // 讀取檔案內容
                //讀取路徑中的所有內容
                string currentContent = File.ReadAllText(filePath);

                // 確認是否有新增內容
                //ContainsKey方法為去查看傳進來的字典有無包含KEY與VALUE
                if (conTents.ContainsKey(filePath))
                {
                    //把傳進來的字典先前如果有資料先放到OLD裡面
                    string OldContent = conTents[filePath];
                    string newContent = GetNewContent(OldContent, currentContent);

                    // 顯示新增的內容
                    //去檢查新輸入的內容是否為空白 而.IsNullOrEmpty是看是檢查是否為空字串或是NULL
                    //IsNullOrWhiteSpace 為檢查是否為空字串或NULL或有空白字符
                    if (!string.IsNullOrWhiteSpace(newContent))
                    {
                        Console.WriteLine($"新增內容:{newContent}");
                    }
                    else
                    {
                        Console.WriteLine("沒有新增內容");
                    }
                }

                // 更新檔案的先前內容
                //把路徑跟內容存到字典中
                conTents[filePath] = currentContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"讀取檔案內容時發生錯誤: {ex.Message}");
            }
        }
        /// <summary>
        /// 顯示檔案內容
        /// </summary>
        /// <param name="originalContent"></param>
        /// <param name="updatedContent"></param>
        /// <returns></returns>
        public static string GetNewContent(string originalContent, string updatedContent)
        {
            // 假設新增內容就是從原始內容後面開始的部分
            //indexof方法作用是用來比對兩個字串中完全相符的並返回開始位置的索引值（整數）
            //ex:"Hello, World!"與"World" 加入indexof方法後，會去比對符合的部分從w開始索引為7所以會返還7
            //StringComparison.Ordinal為區分大小寫
            int indexOfOriginal = updatedContent.IndexOf(originalContent, StringComparison.Ordinal);

            //如果大於等於0代表有找到符合的字串並從第幾個索引開始
            //indexOfOriginal == 0就是代表新增的內容是在舊內容之後
            //所以就使用substring擷取舊字串長度後的內容即可
            if (indexOfOriginal >= 0)
            {
                if (indexOfOriginal == 0)
                {
                    return updatedContent.Substring(indexOfOriginal + originalContent.Length);
                }

            }
            return string.Empty;
        }
    }
}
//}
//步驟流程->先查看有無此目錄位置與檔案->設定監控路徑->設定監控觸發條件->建立事件處理器當事件觸發該處理何事