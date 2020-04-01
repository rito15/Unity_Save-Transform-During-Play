using UnityEngine;
using System.IO;
using System;

namespace Rito.Conveniences
{
    public static class JsonTransformDataManager
    {
        private static string GetRootPath([System.Runtime.CompilerServices.CallerFilePath] string path = "")
            => path.Substring(0, path.LastIndexOf('\\') + 1);

        private static string FolderPath => GetRootPath() + "Data/";
        private static string GetFullPath(int id) => FolderPath + $"{id}.json";

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
    }

    [Serializable]
    public struct TransformData
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
        public bool isApplied;

        // 생성자
        public TransformData(in Transform transform)
        {
            localPosition = transform.localPosition;
            localRotation = transform.localRotation;
            localScale = transform.localScale;

            isApplied = false;
        }

        /// <summary> Struct -> Transform </summary>
        public void Load(in Transform transform)
        {
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            transform.localScale = localScale;
        }

        // null 패턴
        private static TransformData _null
            = new TransformData
            {
                localPosition = Vector3.zero,
                localRotation = Quaternion.identity,
                localScale = Vector3.one,
                isApplied = false
            };
        public static TransformData Null => _null;
    }
}