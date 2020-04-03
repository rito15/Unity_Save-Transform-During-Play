using UnityEngine;
using System.IO;

namespace Rito.Conveniences
{
    public static class JsonManager
    {
        #region Get Paths

        private static string GetRootPath([System.Runtime.CompilerServices.CallerFilePath] string path = "")
            => path.Substring(0, path.LastIndexOf('\\') + 1);

        private static string FolderPath => GetRootPath() + "Data/";
        private static string GetFullPath(CommonData data, int id)
        {
            switch (data)
            {
                case TransformData td:
                    return FolderPath + $"{id}_Transform.json";
                case SpaceData sd:
                    return FolderPath + $"{id}_Space.json";
                default:
                    return FolderPath + $"{id}.json";
            }
        }
        private static string GetFullPath<T>(int id) where T : CommonData
        {
            if(typeof(T).IsEquivalentTo(typeof(TransformData)))
                return FolderPath + $"{id}_Transform.json";

            else if(typeof(T).IsEquivalentTo(typeof(SpaceData)))
                return FolderPath + $"{id}_Space.json";

            else
                return FolderPath + $"{id}.json";
        }

        #endregion

        #region Methods - Common

        public static void SaveDataToJSON(in CommonData data, in int id)
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

        public static CommonData LoadDataFromJSON<T>(in int id) where T : CommonData
        {
            string filePath = GetFullPath<T>(id);

            // 파일 미존재 예외처리
            if (File.Exists(filePath) == false)
                return CommonData.Null;

            // 정상 로드
            string jsonStr = File.ReadAllText(filePath);

            if(typeof(T).IsEquivalentTo(typeof(TransformData)))
                return JsonUtility.FromJson<TransformData>(jsonStr);

            else if(typeof(T).IsEquivalentTo(typeof(SpaceData)))
                return JsonUtility.FromJson<SpaceData>(jsonStr);

            else
                return JsonUtility.FromJson<CommonData>(jsonStr);

        }

        #endregion
    }
}