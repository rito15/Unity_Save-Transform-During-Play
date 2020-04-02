using UnityEngine;
using System.IO;

namespace Rito.Conveniences
{
    public static class JsonTransformDataManager
    {
        #region Get Paths

        private static string GetRootPath([System.Runtime.CompilerServices.CallerFilePath] string path = "")
            => path.Substring(0, path.LastIndexOf('\\') + 1);

        private static string FolderPath => GetRootPath() + "Data/";
        private static string GetFullPath(int id) => FolderPath + $"{id}.json";

        #endregion

        #region Methods

        public static void SaveTransformDataToJSON(in TransformData data, int id)
        {
            DirectoryInfo di = new DirectoryInfo(FolderPath);
            if (di.Exists == false) di.Create();

            File.WriteAllText(GetFullPath(id), JsonUtility.ToJson(data));
        }

        public static TransformData LoadTransformDataFromJSON(int id)
        {
            string filePath = GetFullPath(id);

            // 파일 미존재 예외처리
            if (File.Exists(filePath) == false)
                return TransformData.Null;

            // 정상 로드
            string jsonStr = File.ReadAllText(filePath);
            TransformData data = JsonUtility.FromJson<TransformData>(jsonStr);
            return data;
        }

        #endregion
    }
}