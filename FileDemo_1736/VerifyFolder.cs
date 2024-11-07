//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FileDemo_1736
//{
//    /// <summary>
//    /// 驗證資料夾
//    /// </summary>
//    public class VerifyFolder
//    {
//        /// <summary>
//        /// 創建資料夾與檔案
//        /// </summary>
//        // folderPath路徑 fileName檔案名稱
//        public static void Createfolder(string folderPath, string[] fileName)
//        {
//            // 取得檔案所在的資料夾路徑
//            string DirectoryPath = Path.GetDirectoryName(folderPath);

//            // 檢查資料夾是否存在，不存在則創建資料夾
//            //Directory.Exists檢查資料夾是否存在
//            if (!Directory.Exists(DirectoryPath))
//            {
//                //Directory.CreateDirectory這方法是創建資料夾
//                Directory.CreateDirectory(DirectoryPath);
//                Console.WriteLine($"資料夾已創建: {DirectoryPath}");
//            }
          
//            foreach(var FileName in fileName) 
//            {
//                string filePath = Path.Combine(DirectoryPath, FileName);
//                // 檢查檔案是否存在，不存在則創建檔案
//                if (!File.Exists(folderPath))
//                {
//                    File.Create(folderPath).Close(); // 創建檔案並關閉文件流
//                    Console.WriteLine($"檔案已創建: {folderPath}");
//                }
//            }          
//        }
//    }
//}
