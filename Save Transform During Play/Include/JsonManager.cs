using UnityEngine;
using System.IO;

namespace Rito.Conveniences
{
    public static class JsonManager
    {
        #region Get Paths

        //private static string GetRootPath([System.Runtime.CompilerServices.CallerFilePath] string path = "")
        //    => path.Substring(0, path.LastIndexOf('\\') + 1);
        //private static string FolderPath => GetRootPath() + "Data/";

        private static string FolderPath => $"{Application.dataPath}_TempData/Save Transform During Play/";

        private static string GetFullPath<T>(in int id) where T : IJsonData
        {
            string fileName = typeof(T).ToString();
            fileName = fileName.Substring(fileName.LastIndexOf('.') + 1);

            return $"{FolderPath}{id}_{fileName}.json";
        }

        #endregion

        #region Methods - Common

        public static void SaveDataToJSON(in IJsonData data, in int id)
        {
            DirectoryInfo di = new DirectoryInfo(FolderPath);
            if (di.Exists == false) di.Create();

            switch(data)
            {
                case TransformData td:
                    File.WriteAllText(GetFullPath<TransformData>(id), JsonUtility.ToJson(td));
                    break;

                case SpaceData sd:
                    File.WriteAllText(GetFullPath<SpaceData>(id), JsonUtility.ToJson(sd));
                    break;
            }
        }

        public static IJsonData LoadDataFromJSON<T>(in int id) where T : IJsonData
        {
            string filePath = GetFullPath<T>(id);

            // 파일 미존재 예외처리
            if (File.Exists(filePath) == false)
                return null;

            // 정상 로드
            string jsonStr = File.ReadAllText(filePath);
            return JsonUtility.FromJson<T>(jsonStr);
        }

        #endregion
    }
}