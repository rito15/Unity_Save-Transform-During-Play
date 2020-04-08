using UnityEngine;

namespace Rito.Conveniences
{
    /// <summary> 
    /// <para/> 제작일 : 2020. 04. 01. 
    /// <para/> 제작자 : Rito
    /// <para/> 기능   : 플레이 모드에서 Transform을 변경한 경우, 플레이 모드가 종료되어도 변경사항을 적용할 수 있게 합니다.
    /// <para/> 
    /// <para/> * 사용법
    /// <para/>  - 원하는 게임오브젝트에 SaveTransformDuringPlay 컴포넌트를 추가합니다.
    /// <para/>  - 인스펙터에서 On 변수를 체크/해제함으로써 언제든 기능을 켜고 끌 수 있습니다.
    /// <para/>  - 인스펙터의 Position, Rotation, Scale Space 옵션을 설정할 수 있습니다.
    /// <para/>
    /// <para/>    1) Local Space인 경우, 플레이 모드 종료 직전의 인스펙터 값을 그대로 보존합니다.
    /// <para/>       (transform.localPosition, transform.localRotation, transform.localScale)
    /// <para/>
    /// <para/>    2) World Space인 경우, 플레이 모드 종료 직전의 실제 위치, 회전, 크기 값을 그대로 보존합니다.
    /// <para/>       (transform.position, transform.rotation, transform.lossyScale)
    /// <para/> 
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class SaveTransformDuringPlay : MonoBehaviour
    {
        #region Fields

        /// <summary> 기능 On/Off </summary>
        public bool _on = true;

        [Header("Options")]
        public Spaces _positionSpace = default;
        public Spaces _rotationSpace = default;
        public Spaces _scaleSpace    = default;

        private const int True  = 1;
        private const int False = 0;

        private int _transformID = -999999;
        private int TransformID
        {
            get
            {
                if (_transformID.Equals(-999999))
                    _transformID = transform.GetInstanceID();
                return _transformID;
            }
        }

        #endregion

        #region Unity Callbacks

        private void OnGUI()
        {
            OnGUI_EditorOnly();
        }

        private void OnApplicationQuit()
        {
            OnApplicationQuit_EditorOnly();
        }

        #endregion

        #region EditorOnly Unity Callbacks

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnGUI_EditorOnly()
        {
            if (!Application.isPlaying)
            {
                // 부모 -> 자식 순서대로 차근차근 적용(World Space 역순 적용 문제 해결)
                if (transform.parent != null)
                {
                    var parentTr = transform.parent;
                    var parentSTDP = parentTr.GetComponent<SaveTransformDuringPlay>();
                    if(parentSTDP != null && parentSTDP._on)
                    {
                        if (CheckPrefs(nameof(Prefs._OnEditor), False, parentTr))
                            return;
                    }
                }

                // OnEditorMode() 1회만 실행
                if (CheckPrefs(nameof(Prefs._OnEditor), False))
                {
                    OnEditorMode();
                    SavePrefs(nameof(Prefs._OnEditor), True);
                }
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnApplicationQuit_EditorOnly()
        {
            if (_on)
                JsonManager.SaveDataToJSON(new TransformData(transform), TransformID);

            SaveInspectorOptionsToJSON();

            SavePrefs(nameof(Prefs._OnEditor), False);
        }

        #endregion

        #region Methods

        private void OnEditorMode()
        {
            LoadInspectorOptionsFromJSON();

            if (_on)
                ApplyModifications();
        }

        private void ApplyModifications()
        {
            var savedData = (JsonManager.LoadDataFromJSON<TransformData>(TransformID)) as TransformData;

            if (savedData == null || !enabled || !_on)
                return;

            savedData.ApplyToTransform(transform, _positionSpace, _rotationSpace);

            // Scale - Global인 경우 적용
            if(_scaleSpace.Equals(Spaces.World))
            {
                Vector3 savedLossy = savedData.globalScale;
                Vector3 nowLossy = transform.lossyScale;

                (float x, float y, float z) factor
                    = (savedLossy.x / nowLossy.x, savedLossy.y / nowLossy.y, savedLossy.z / nowLossy.z);

                transform.localScale = new Vector3(
                        transform.localScale.x * factor.x,
                        transform.localScale.y * factor.y,
                        transform.localScale.z * factor.z
                    );
            }
        }

        #endregion

        #region Tiny

        private void SaveInspectorOptionsToJSON()
        {
            SpaceData sd = new SpaceData(_on, _positionSpace, _rotationSpace, _scaleSpace);
            JsonManager.SaveDataToJSON(sd, TransformID);
        }

        private void LoadInspectorOptionsFromJSON()
        {
            var savedOptions = JsonManager.LoadDataFromJSON<SpaceData>(TransformID) as SpaceData;

            if (savedOptions == null)
                return;

            _on = savedOptions.isOn;
            _positionSpace = savedOptions.positionSpace;
            _rotationSpace = savedOptions.rotationSpace;
            _scaleSpace = savedOptions.scaleSpace;
        }

        private void SavePrefs(in string prefName, in int answer)
        {
            PlayerPrefs.SetInt(TransformID + prefName, answer);
        }

        private bool CheckPrefs(in string prefName, in int answer)
        {
            return PlayerPrefs.GetInt(TransformID + prefName).Equals(answer);
        }

        private bool CheckPrefs(in string prefName, in int answer, in Transform targetTr)
        {
            return PlayerPrefs.GetInt(targetTr.GetInstanceID() + prefName).Equals(answer);
        }

        private enum Prefs
        {
            /// <summary> 플레이 모드 종료 후, 에디터 모드 실행됨 </summary>
            _OnEditor
        }

        #endregion
    }
}